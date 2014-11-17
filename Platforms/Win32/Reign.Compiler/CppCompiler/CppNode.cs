using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Reign.Compiler
{
	public class CppNode : Node
	{
		private static TypeDeclarationSyntax currentTypeDecl;

		public override void Compile(StreamWriter writer, int mode)
		{
			if (mode == 0) compileHeaderFile(writer, mode);
			else compileCppFile(writer, mode);
		}

		private void writeModifiers(StreamWriter writer, SyntaxTokenList modifiers)
		{
			// write public modifier
			foreach (var m in modifiers)
			{
				if (m.ValueText == "public") writer.Write(m.ValueText + ": ");
			}

			// write others
			foreach (var m in modifiers)
			{
				switch (m.ValueText)
				{
					case "static": writer.Write("static "); break;
					case "virtual": writer.Write("virtual "); break;
				}
			}
		}

		private string formatNamedType(string name, ref SymbolInfo symbolInfo, INamedTypeSymbol namedType, bool removeArrayAndPointerSymbols)
		{
			if (namedType != null)
			{
				switch (namedType.SpecialType)// TODO: add more types
				{
					case SpecialType.System_Int32: name = name.Replace(name, "Int32"); break;
				}

				if (!namedType.IsValueType && namedType.SpecialType != SpecialType.System_String) name += "*";
			}
			else if (symbolInfo.Symbol as IArrayTypeSymbol != null)
			{
				name = name.Replace("[]", "*");
			}

			if (removeArrayAndPointerSymbols) name = name.Replace("[]", "").Replace("*", "");
			return name;
		}

		private void writeTypeInfoHeader(StreamWriter writer, TypeDeclarationSyntax node)
		{
			writer.WriteLine(string.Format("class TYPE_{0} : public Type", node.Identifier));
			writer.WriteLine("{");
			foreach (var childNode in node.ChildNodes())
			{
				var type = childNode.GetType();
				if (type == typeof(FieldDeclarationSyntax))
				{
					var n = (FieldDeclarationSyntax)childNode;
					string name = n.Declaration.Type.ToString();
					var symbolInfo = semanticModel.GetSymbolInfo(n.Declaration.Type);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					name = formatNamedType(name, ref symbolInfo, namedType, true);

					foreach (var childNode2 in childNode.ChildNodes())
					{
						if (childNode2.GetType() == typeof(VariableDeclarationSyntax))
						{
							var n2 = (VariableDeclarationSyntax)childNode2;
							foreach (var v in n2.Variables)
							{
								writer.WriteLine(string.Format("public: TYPE_{0}* {1};", name, v.Identifier.Value));
							}
						}
					}
				}
			}
			writer.WriteLine(string.Format("public: TYPE_{0}();", node.Identifier));
			writer.WriteLine("};");
			writer.WriteLine(string.Format("TYPE_{0}* TYPEOBJ_{0} = new TYPE_{0}();", node.Identifier));
		}

		private void compileHeaderFile(StreamWriter writer, int mode)
		{
			var nodeType = syntaxNode.GetType();

			// namespace
			if (nodeType == typeof(NamespaceDeclarationSyntax))
			{
				var node = (NamespaceDeclarationSyntax)syntaxNode;
				string value = node.Name.ToString();
				var values = value.Split('.');
				foreach (var name in values) writer.Write(string.Format(@"namespace {0}{{ ", name));
				writer.WriteLine();
				// write object forward declares
				foreach (CSharpSyntaxNode node2 in syntaxNode.ChildNodes())
				{
					var type = node2.GetType();
					if (type == typeof(ClassDeclarationSyntax))
					{
						var n = (ClassDeclarationSyntax)node2;
						writer.WriteLine(string.Format("class {0};", n.Identifier));
					}
					else if (type == typeof(StructDeclarationSyntax))
					{
						var n = (StructDeclarationSyntax)node2;
						writer.WriteLine(string.Format("struct {0};", n.Identifier));
					}
				}
				base.Compile(writer, mode);
				for (int i = 0; i != values.Length; ++i) writer.Write("}");
			}
			// class
			else if (nodeType == typeof(ClassDeclarationSyntax))
			{
				var node = (ClassDeclarationSyntax)syntaxNode;
				writer.Write(string.Format("class {0} : public ", node.Identifier));
				if (node.BaseList != null)
				{
					var last = node.BaseList.Types.Last();
					foreach (var type in node.BaseList.Types)
					{
						writer.Write(type);
						if (type != last) writer.Write(", ");
					}
				}
				else
				{
					writer.WriteLine("object");
				}
				writer.WriteLine("{");
				writer.WriteLine("public: void* operator new(size_t size);");// write GC new override
				base.Compile(writer, mode);
				writer.WriteLine("};");
				writeTypeInfoHeader(writer, node);
			}
			// struct
			else if (nodeType == typeof(StructDeclarationSyntax))
			{
				var node = (StructDeclarationSyntax)syntaxNode;
				writer.Write(string.Format("struct {0} : public ", node.Identifier));
				if (node.BaseList != null)
				{
					var last = node.BaseList.Types.Last();
					foreach (var type in node.BaseList.Types)
					{
						writer.Write(type);
						if (type != last) writer.Write(", ");
					}
				}
				else
				{
					writer.WriteLine("object");
				}
				writer.WriteLine("{");
				base.Compile(writer, mode);
				writer.WriteLine("};");
				writeTypeInfoHeader(writer, node);
			}
			// fields
			else if (nodeType == typeof(FieldDeclarationSyntax))
			{
				var node = (FieldDeclarationSyntax)syntaxNode;

				// convert type name
				string name = node.Declaration.Type.ToString();
				var symbolInfo = semanticModel.GetSymbolInfo(node.Declaration.Type);
				var namedType = symbolInfo.Symbol as INamedTypeSymbol;
				name = formatNamedType(name, ref symbolInfo, namedType, false);

				foreach (var childNode in node.ChildNodes())
				{
					if (childNode.GetType() == typeof(VariableDeclarationSyntax))
					{
						var n = (VariableDeclarationSyntax)childNode;
						foreach (var v in n.Variables)
						{
							writeModifiers(writer, node.Modifiers);
							writer.WriteLine(string.Format("{0} {1};", name, v.Identifier.Value));
						}
					}
				}
			}
			// methods
			else if (nodeType == typeof(MethodDeclarationSyntax))
			{
				var node = (MethodDeclarationSyntax)syntaxNode;

				// convert return name
				string name = node.ReturnType.ToString();
				var symbolInfo = semanticModel.GetSymbolInfo(node.ReturnType);
				var namedType = symbolInfo.Symbol as INamedTypeSymbol;
				name = formatNamedType(name, ref symbolInfo, namedType, false);

				writeModifiers(writer, node.Modifiers);
				writer.Write(name + " ");
				writer.Write(node.Identifier.ToString() + "(");
				foreach (var p in node.ParameterList.Parameters)
				{
					// convert parameter name
					symbolInfo = semanticModel.GetSymbolInfo(p.Type);
					namedType = symbolInfo.Symbol as INamedTypeSymbol;
					name = p.Type.ToString();
					name = formatNamedType(name, ref symbolInfo, namedType, false);

					writer.Write(string.Format("{0} {1}", name, p.Identifier));
					if (p != node.ParameterList.Parameters.Last()) writer.Write(", ");
				}
				writer.WriteLine(");");
			}
			else
			{
				base.Compile(writer, mode);
			}
		}

		private string formatBlockStatment(string line, CSharpSyntaxNode baseNode)
		{
			var type = baseNode.GetType();
			if (type == typeof(LocalDeclarationStatementSyntax))
			{
				var node = (LocalDeclarationStatementSyntax)baseNode;
				var symbolInfo = semanticModel.GetSymbolInfo(node.Declaration.Type);
				bool isValueType = true;
				foreach (var r in symbolInfo.Symbol.DeclaringSyntaxReferences)
				{
					var root = (CSharpSyntaxNode)r.GetSyntax();
					var symbol = semanticModel.GetDeclaredSymbol(root) as INamedTypeSymbol;
					if (symbol != null) isValueType = symbol.IsValueType;
				}

				// convert var type
				if (node.Declaration.Type.IsVar)
				{
					line = Regex.Replace(line, @"var\s*", symbolInfo.Symbol.ToString().Replace(".", "::") + (isValueType?" ":"* "));
				}

				// convert special types
				foreach (var r in node.Declaration.Variables)
				{
					var rootType = (VariableDeclaratorSyntax)r;
					var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbol(rootType);
					var namedType = symbol.Type;
					switch (namedType.SpecialType)
					{
						case SpecialType.System_Int32:
							line = Regex.Replace(line, symbolInfo.Symbol+@"\s", "Int32 ");
							line = Regex.Replace(line, symbolInfo.Symbol+@"::", "Int32::");
							break;
						// TODO: Add more types
					}
				}
				
				// remove new for value types
				if (isValueType)
				{
					foreach (var variable in node.Declaration.Variables)
					{
						line = Regex.Replace(line, variable.Identifier+@"\s*=\s*new", variable.Identifier+" = ");
					}
				}
			}
			else if (type == typeof(LiteralExpressionSyntax))
			{
				var node = (LiteralExpressionSyntax)baseNode;
				if (node.IsKind(SyntaxKind.StringLiteralExpression))
				{
					line = line.Replace(node.ToString(), "string(L"+node.ToString()+")");
				}
			}
			else if (type == typeof(ExpressionStatementSyntax))
			{
				var node = (ExpressionStatementSyntax)baseNode;
				if (node.Expression.GetType() == typeof(BinaryExpressionSyntax))
				{
					var e = (BinaryExpressionSyntax)node.Expression;

					// remove new for value types
					if (e.Left.GetType() == typeof(IdentifierNameSyntax))
					{
						var t = (IdentifierNameSyntax)e.Left;
						var symbolInfo = semanticModel.GetSymbolInfo(t);
						bool isValueType = false;
						foreach (var r in symbolInfo.Symbol.DeclaringSyntaxReferences)
						{
							var root = (CSharpSyntaxNode)r.GetSyntax();
							if (root.GetType() == typeof(VariableDeclaratorSyntax))
							{
								var rootType = (VariableDeclaratorSyntax)root;
								var symbol = semanticModel.GetDeclaredSymbol(rootType) as IFieldSymbol;
								var namedType = symbol.Type;
								isValueType = namedType.IsValueType;
							}
							else if (root.GetType() == typeof(ParameterSyntax))
							{
								var rootType = (ParameterSyntax)root;
								var symbol = (IParameterSymbol)semanticModel.GetDeclaredSymbol(rootType);
								var namedType = symbol.Type;
								isValueType = namedType.IsValueType;
							}
						}
						
						if (isValueType) line = Regex.Replace(line, t.Identifier+@"\s*=\s*new", t.Identifier+" = ");
					}

					// add roots to GC
					if (e.Left.GetType() == typeof(IdentifierNameSyntax) && e.Right.GetType() == typeof(ObjectCreationExpressionSyntax))
					{
						bool isStatic = false;
						var t = (IdentifierNameSyntax)e.Left;
						var symbolInfo = semanticModel.GetSymbolInfo(t);
						foreach (var r in symbolInfo.Symbol.DeclaringSyntaxReferences)
						{
							var root = (CSharpSyntaxNode)r.GetSyntax();
							if (root.GetType() == typeof(VariableDeclaratorSyntax))
							{
								var rootType = (VariableDeclaratorSyntax)root;
								var symbol = semanticModel.GetDeclaredSymbol(rootType);
								isStatic = symbol.IsStatic;
							}
							else if (root.GetType() == typeof(ParameterSyntax))
							{
								var rootType = (ParameterSyntax)root;
								var symbol = semanticModel.GetDeclaredSymbol(rootType);
								var namedType = symbol.Type;
								isStatic = symbol.IsStatic;
							}
						}

						if (isStatic) line += string.Format("GC::AddRootObjectPtrToLastHeapNode((void**)&{0});", t.Identifier) + Environment.NewLine;
					}
				}
			}
			else if (type == typeof(InvocationExpressionSyntax))
			{
				var node = (InvocationExpressionSyntax)baseNode;
				string fullName = "", identifier = null;
				var e = node.Expression;
				bool isInstanceBlock = false, isValueType = false;
				while (e != null)
				{
					if (e.GetType() == typeof(MemberAccessExpressionSyntax))
					{
						var eNode = (MemberAccessExpressionSyntax)e;
						
						// convert ext method
						bool isExt = false;
						var symbolInfo = semanticModel.GetSymbolInfo(eNode);
						foreach (var r in symbolInfo.Symbol.DeclaringSyntaxReferences)
						{
							var root = (CSharpSyntaxNode)r.GetSyntax();
							var symbol = semanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)root);
							if (symbol.IsExtensionMethod)
							{
								isExt = true;
								var iNode = (IdentifierNameSyntax)eNode.Expression;
								string fullNameExt = ((ClassDeclarationSyntax)root.Parent).Identifier.ToString() + "::" + eNode.Name;
								line = Regex.Replace(line, iNode.Identifier.ToString() + @"\." + eNode.Name + @".*?\(", fullNameExt + "(" + iNode.Identifier + (symbol.Parameters.Length >= 2 ? ", " : ""), RegexOptions.Singleline);
								e = eNode.Expression;
							}
						}
						
						// otherwise continue normaly
						if (!isExt)
						{
							fullName += eNode.Name + ".";
						}

						e = eNode.Expression;
					}
					else if (e.GetType() == typeof(IdentifierNameSyntax))
					{
						var eNode = (IdentifierNameSyntax)e;

						// check if were a value type or if were an instance block
						var symbolInfo = semanticModel.GetSymbolInfo(eNode);
						foreach (var r in symbolInfo.Symbol.DeclaringSyntaxReferences)
						{
							var root = (CSharpSyntaxNode)r.GetSyntax();
							if (root.GetType() == typeof(VariableDeclaratorSyntax))
							{
								var rootType = (VariableDeclaratorSyntax)root;
								var symbol = (ILocalSymbol)semanticModel.GetDeclaredSymbol(rootType);
								var namedType = symbol.Type;
								if (namedType.IsValueType) isValueType = true;
								if (!symbol.IsStatic) isInstanceBlock = true;
							}
							else if (root.GetType() == typeof(ParameterSyntax))
							{
								var rootType = (ParameterSyntax)root;
								var symbol = (IParameterSymbol)semanticModel.GetDeclaredSymbol(rootType);
								var namedType = symbol.Type;
								if (namedType.IsValueType) isValueType = true;
								if (!symbol.IsStatic) isInstanceBlock = true;
							}
						}

						fullName += eNode.Identifier;
						identifier = eNode.Identifier.ToString();
						if (!isValueType && isInstanceBlock) fullName = fullName.Replace("."+eNode.Identifier, "->"+eNode.Identifier);
						e = null;
					}
					else if (e.GetType() == typeof(ThisExpressionSyntax))
					{
						var eNode = (ThisExpressionSyntax)e;
						isInstanceBlock = true;
						fullName += "this";
						fullName = fullName.Replace(".this", "->this");
						e = null;
					}
					else if (e.GetType() == typeof(PredefinedTypeSyntax))
					{
						var eNode = (PredefinedTypeSyntax)e;

						var symbolInfo = semanticModel.GetSymbolInfo(eNode);
						var namedType = (INamedTypeSymbol)symbolInfo.Symbol;
						switch (namedType.SpecialType)// convert special type static methods
						{
							case SpecialType.System_Int32: line = line.Replace("int.", "Int32."); break;
							// TODO: Add more base types
						}
						e = null;
					}
					else
					{
						e = null;
					}
				}

				// replace C# path with CPP path
				if (!isInstanceBlock)
				{
					// flip path
					var values = fullName.Split('.');
					fullName = "";
					for (int i = values.Length-1; i != -1; --i)
					{
						fullName += values[i];
						if (i != 0) fullName += "::";
					}

					line = Regex.Replace(line, fullName.Replace("::", "."), fullName);
				}
				else
				{
					if (!isValueType)
					{
						// flip path
						var values = Regex.Split(fullName, @"(\.|->)");
						string newFullName = "";
						fullName = "";
						for (int i = values.Length-1; i > -1; i -= 2)
						{
							newFullName += values[i];
							fullName += values[i];
							if (i != 0)
							{
								newFullName += values[i-1];
								fullName += values[i-1];
							}
						}

						line = Regex.Replace(line, fullName.Replace("->", "."), newFullName);
					}
				}
			}

			// search deeper
			foreach (var childNode in baseNode.ChildNodes())
			{
				line = formatBlockStatment(line, (CSharpSyntaxNode)childNode);
			}

			return line;
		}

		private void writeTypeInfoCpp(StreamWriter writer, TypeDeclarationSyntax node)
		{
			// get full name
			string fullname = node.Identifier.ToString();
			var parent = node.Parent;
			while (parent != null)
			{
				if (parent.GetType() == typeof(TypeDeclarationSyntax)) fullname = ((TypeDeclarationSyntax)parent).Identifier + "." + fullname;
				else if (parent.GetType() == typeof(NamespaceDeclarationSyntax)) fullname = ((NamespaceDeclarationSyntax)parent).Name + "." + fullname;
				parent = parent.Parent;
			}

			// check if value type
			bool isValueType = node.GetType() == typeof(StructDeclarationSyntax);

			writer.WriteLine(string.Format(@"TYPE_{0}::TYPE_{0}() : Type(string(L""{1}""), {2})", node.Identifier, fullname, isValueType?"true":"false"));
			writer.WriteLine("{");
			foreach (var childNode in node.ChildNodes())
			{
				var type = childNode.GetType();
				if (type == typeof(FieldDeclarationSyntax))
				{
					var n = (FieldDeclarationSyntax)childNode;
					string name = n.Declaration.Type.ToString();
					var symbolInfo = semanticModel.GetSymbolInfo(n.Declaration.Type);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					name = formatNamedType(name, ref symbolInfo, namedType, true);

					foreach (var childNode2 in childNode.ChildNodes())
					{
						if (childNode2.GetType() == typeof(VariableDeclarationSyntax))
						{
							var n2 = (VariableDeclarationSyntax)childNode2;
							writer.WriteLine(string.Format("TypeInfosCount = {0};", n2.Variables.Count));
							writer.WriteLine("TypeInfoOffsets = new int[TypeInfosCount];");
							writer.WriteLine("TypeInfos = new Type*[TypeInfosCount];");
							writer.WriteLine("int sizeOffset = 0;");
							int i = 0;
							foreach (var v in n2.Variables)
							{
								writer.WriteLine(string.Format("{0} = TYPEOBJ_{1};", v.Identifier.Value, name));
								writer.WriteLine(string.Format("TypeInfos[{0}] = {1}; TypeInfoOffsets[{0}] = sizeOffset;", i, v.Identifier.Value));
								writer.WriteLine(string.Format("sizeOffset += sizeof({0});", v.Identifier.Value));
								++i;
							}
						}
					}
				}
			}
			writer.WriteLine("}");
		}

		private void compileCppFile(StreamWriter writer, int mode)
		{
			var nodeType = syntaxNode.GetType();

			// namespace
			if (nodeType == typeof(NamespaceDeclarationSyntax))
			{
				var node = (NamespaceDeclarationSyntax)syntaxNode;
				string value = node.Name.ToString();
				var values = value.Split('.');
				foreach (var name in values) writer.Write(string.Format(@"namespace {0}{{ ", name));
				writer.WriteLine();
				base.Compile(writer, mode);
				for (int i = 0; i != values.Length; ++i) writer.Write("}");
			}
			// class
			else if (nodeType == typeof(ClassDeclarationSyntax))
			{
				var node = (ClassDeclarationSyntax)syntaxNode;
				currentTypeDecl = node;
				bool isStatic = false;
				foreach (var m in node.Modifiers)
				{
					switch (m.ValueText)
					{
						case "static": isStatic = true; break;
					}
				}

				// write type info
				writeTypeInfoCpp(writer, node);

				// write GC new override
				if (!isStatic)
				{
					writer.WriteLine(string.Format("void* {0}::operator new(size_t size)", currentTypeDecl.Identifier));
					writer.WriteLine("{");
					writer.WriteLine("	void* data = malloc(size);");
					writer.WriteLine(@"	if(data == NULL) throw ""Allocation Failed: No free memory"";");
					writer.WriteLine("	memset(data, 0, size);");
					writer.WriteLine(string.Format("	GC::AddHeapObject(TYPEOBJ_{0}, data);", currentTypeDecl.Identifier));
					writer.WriteLine("	return data;");
					writer.WriteLine("}");
				}
				base.Compile(writer, mode);
			}
			// struct
			else if (nodeType == typeof(StructDeclarationSyntax))
			{
				var node = (StructDeclarationSyntax)syntaxNode;
				currentTypeDecl = node;
				writeTypeInfoCpp(writer, node);// write type info
				base.Compile(writer, mode);
			}
			// methods
			else if (nodeType == typeof(MethodDeclarationSyntax))
			{
				var node = (MethodDeclarationSyntax)syntaxNode;

				// convert return name
				string name = node.ReturnType.ToString();
				var symbolInfo = semanticModel.GetSymbolInfo(node.ReturnType);
				var namedType = symbolInfo.Symbol as INamedTypeSymbol;
				name = formatNamedType(name, ref symbolInfo, namedType, false);

				writer.Write(string.Format("{0} {1}::", name, currentTypeDecl.Identifier));
				writer.Write(node.Identifier.ToString() + "(");
				foreach (var p in node.ParameterList.Parameters)
				{
					// convert parameter name
					symbolInfo = semanticModel.GetSymbolInfo(p.Type);
					namedType = symbolInfo.Symbol as INamedTypeSymbol;
					name = p.Type.ToString();
					name = formatNamedType(name, ref symbolInfo, namedType, false);

					writer.Write(string.Format("{0} {1}", name, p.Identifier));
					if (p != node.ParameterList.Parameters.Last()) writer.Write(", ");
				}
				writer.WriteLine(")");
				base.Compile(writer, mode);
			}
			else if (nodeType == typeof(BlockSyntax))
			{
				var node = (BlockSyntax)syntaxNode;
				writer.WriteLine("{");
				foreach (var statement in node.Statements)
				{
					string line = statement.GetText().ToString().Replace("\t", " ");
					line = formatBlockStatment(line, statement);
					writer.Write(line);
				}
				writer.WriteLine("}");
			}
			else
			{
				base.Compile(writer, mode);
			}
		}
	}
}
