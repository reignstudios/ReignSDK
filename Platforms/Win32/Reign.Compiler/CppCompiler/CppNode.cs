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
		private static ClassDeclarationSyntax currentClass;

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

		private string formatSpecialType(SpecialType specialType, string typeName)
		{
			switch (specialType)// TODO: add more types
			{
				case SpecialType.System_Int32: return typeName.Replace(typeName, "Int32");
				default: return typeName;
			}
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
				base.Compile(writer, mode);
				for (int i = 0; i != values.Length; ++i) writer.Write("}");
			}
			// class
			else if (nodeType == typeof(ClassDeclarationSyntax))
			{
				var node = (ClassDeclarationSyntax)syntaxNode;
				writer.WriteLine(string.Format("class {0}", node.Identifier));
				writer.WriteLine("{");
				base.Compile(writer, mode);
				writer.WriteLine("};");
			}
			// fields
			else if (nodeType == typeof(FieldDeclarationSyntax))
			{
				var node = (FieldDeclarationSyntax)syntaxNode;

				// convert type name
				string name = node.Declaration.Type.ToString();
				if (node.Declaration.Type.GetType() == typeof(PredefinedTypeSyntax))
				{
					var eNode = (PredefinedTypeSyntax)node.Declaration.Type;

					var symbolInfo = semanticModel.GetSymbolInfo(eNode);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					if (namedType != null) name = formatSpecialType(namedType.SpecialType, name);
				}

				writeModifiers(writer, node.Modifiers);
				writer.Write(name + " ");
				base.Compile(writer, mode);
			}
			else if (nodeType == typeof(VariableDeclarationSyntax))
			{
				var node = (VariableDeclarationSyntax)syntaxNode;
				foreach (var v in node.Variables)
				{
					writer.Write(string.Format("{0} = {1}", v.Identifier.Value, v.Initializer.Value));
					if (v != node.Variables.Last()) writer.Write(", ");
				}
				writer.WriteLine(";");
			}
			// methods
			else if (nodeType == typeof(MethodDeclarationSyntax))
			{
				var node = (MethodDeclarationSyntax)syntaxNode;

				// convert return name
				string name = node.ReturnType.ToString();
				if (node.ReturnType.GetType() == typeof(PredefinedTypeSyntax))
				{
					var eNode = (PredefinedTypeSyntax)node.ReturnType;

					var symbolInfo = semanticModel.GetSymbolInfo(eNode);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					if (namedType != null) name = formatSpecialType(namedType.SpecialType, name);
				}

				writeModifiers(writer, node.Modifiers);
				writer.Write(name + " ");
				writer.Write(node.Identifier.ToString() + "(");
				foreach (var p in node.ParameterList.Parameters)
				{
					// convert parameter name
					var symbolInfo = semanticModel.GetSymbolInfo(p.Type);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					name = p.Type.ToString();
					if (namedType != null) name = formatSpecialType(namedType.SpecialType, name);

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
				// do nothing...
			}
			else if (type == typeof(InvocationExpressionSyntax))
			{
				var node = (InvocationExpressionSyntax)baseNode;
				string fullName = "", identifier = null;
				var e = node.Expression;
				bool isInstanceBlock = false, isValueType = false;//, isSpecialType = false;
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

						//Func<SpecialType,bool> handleSpecial = delegate(SpecialType specialType)
						//{
						//	switch (specialType)// TODO: add more types
						//	{
						//		case SpecialType.System_Int32: return true;
						//		default: return false;
						//	}
						//};

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
								//isSpecialType = handleSpecial(namedType.SpecialType);
							}
							else if (root.GetType() == typeof(ParameterSyntax))
							{
								var rootType = (ParameterSyntax)root;
								var symbol = (IParameterSymbol)semanticModel.GetDeclaredSymbol(rootType);
								var namedType = symbol.Type;
								if (namedType.IsValueType) isValueType = true;
								if (!symbol.IsStatic) isInstanceBlock = true;
								//isSpecialType = handleSpecial(namedType.SpecialType);
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
					//else if (isSpecialType)
					//{
					//	line = Regex.Replace(line, identifier+@"\.ToString\(\)", string.Format("STRING_EXT::ToString({0})", identifier));
					//}
				}
			}

			// search deeper
			foreach (var childNode in baseNode.ChildNodes())
			{
				line = formatBlockStatment(line, (CSharpSyntaxNode)childNode);
			}

			return line;
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
				currentClass = (ClassDeclarationSyntax)syntaxNode;
				base.Compile(writer, mode);
			}
			// methods
			else if (nodeType == typeof(MethodDeclarationSyntax))
			{
				var node = (MethodDeclarationSyntax)syntaxNode;

				//Func<SpecialType,string,string> formatSpecialType = delegate(SpecialType specialType, string typeName)
				//{
				//	switch (specialType)// TODO: add more types
				//	{
				//		case SpecialType.System_Int32: return typeName.Replace(typeName, "Int32");
				//		default: return typeName;
				//	}
				//};

				// convert return name
				string name = node.ReturnType.ToString();
				if (node.ReturnType.GetType() == typeof(PredefinedTypeSyntax))
				{
					var eNode = (PredefinedTypeSyntax)node.ReturnType;

					var symbolInfo = semanticModel.GetSymbolInfo(eNode);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					if (namedType != null) name = formatSpecialType(namedType.SpecialType, name);
				}

				writer.Write(string.Format("{0} {1}::", name, currentClass.Identifier));
				writer.Write(node.Identifier.ToString() + "(");
				foreach (var p in node.ParameterList.Parameters)
				{
					// convert parameter name
					var symbolInfo = semanticModel.GetSymbolInfo(p.Type);
					var namedType = symbolInfo.Symbol as INamedTypeSymbol;
					name = p.Type.ToString();
					if (namedType != null) name = formatSpecialType(namedType.SpecialType, name);

					writer.Write(string.Format("{0} {1}", name, p.Identifier));
					if (p != node.ParameterList.Parameters.Last()) writer.Write(", ");
				}
				writer.WriteLine(")");
				writer.WriteLine("{");
				base.Compile(writer, mode);
				writer.WriteLine("}");
			}
			else if (nodeType == typeof(BlockSyntax))
			{
				var node = (BlockSyntax)syntaxNode;
				foreach (var statement in node.Statements)
				{
					string line = statement.GetText().ToString().Replace("\n", "").Replace("\r", "").Replace("\t", "");
					line = formatBlockStatment(line, statement);
					writer.WriteLine(line);
				}

				base.Compile(writer, mode);
			}
			else
			{
				base.Compile(writer, mode);
			}
		}
	}
}
