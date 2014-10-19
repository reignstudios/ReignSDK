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

		private void compileHeaderFile(StreamWriter writer, int mode)
		{
			//var asNode = syntaxNode.AsNode();
			//var asToken = syntaxNode.AsToken();
			//var nodeType = asNode != null ? asNode.GetType() : null;
			//var tokenType = asToken != null ? asToken.GetType() : null;
			var nodeType = syntaxNode.GetType();
			//writer.WriteLine(type); base.Compile(writer, mode); return;

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
				writeModifiers(writer, node.Modifiers);
				writer.Write(node.Declaration.Type.ToString() + " ");
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
				writeModifiers(writer, node.Modifiers);
				writer.Write(node.ReturnType.ToString() + " ");
				writer.Write(node.Identifier.ToString() + "(");
				foreach (var p in node.ParameterList.Parameters)
				{
					writer.Write(string.Format("{0} {1}", p.Type, p.Identifier));
					if (p != node.ParameterList.Parameters.Last()) writer.Write(", ");
				}
				writer.WriteLine(");");
			}
			else
			{
				//writer.WriteLine(type);
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
					var symbol = semanticModel.GetDeclaredSymbol(root);
					isValueType = Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbolInfo.GetTypeSymbel(symbolInfo.Symbol);
				}

				if (node.Declaration.Type.IsVar)
				{
					line = Regex.Replace(line, @"var\s*", symbolInfo.Symbol + (isValueType?" ":"* "), RegexOptions.Singleline);
				}
				
				if (isValueType)
				{
					foreach (var variable in node.Declaration.Variables)
					{
						line = Regex.Replace(line, variable.Identifier+@"\s*=\s*new", variable.Identifier+" = ", RegexOptions.Singleline);
					}
				}
			}
			else if (type == typeof(ExpressionStatementSyntax))
			{
				// do nothing...
			}
			else if (type == typeof(InvocationExpressionSyntax))
			{
				var node = (InvocationExpressionSyntax)baseNode;
				string fullName = "";
				var e = node.Expression;
				bool isInstanceMethod = false, isValueType = false;
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
						var symbolInfo = semanticModel.GetSymbolInfo(eNode);
						foreach (var r in symbolInfo.Symbol.DeclaringSyntaxReferences)
						{
							var root = (CSharpSyntaxNode)r.GetSyntax();
							if (root.GetType() == typeof(VariableDeclaratorSyntax))
							{
								var rootType = (VariableDeclaratorSyntax)root;
								var symbol = semanticModel.GetDeclaredSymbol(rootType);
								var namedType = Microsoft.CodeAnalysis.CSharp.Symbols.SourceLocalSymbolInfo.GetTypeSymbel((ILocalSymbol)symbol);
								if (namedType.IsValueType) isValueType = true;
								isInstanceMethod = true;
							}
						}
						fullName += eNode.Identifier;
						if (isInstanceMethod) fullName = fullName.Replace("."+eNode.Identifier, "->"+eNode.Identifier);
						e = null;
					}
					else if (e.GetType() == typeof(ThisExpressionSyntax))
					{
						var eNode = (ThisExpressionSyntax)e;
						isInstanceMethod = true;
						fullName += "this";
						fullName = fullName.Replace(".this", "->this");
						e = null;
					}
					else if (e.GetType() == typeof(PredefinedTypeSyntax))
					{
						var eNode = (PredefinedTypeSyntax)e;
						var symbolInfo = semanticModel.GetSymbolInfo(eNode);
						switch (symbolInfo.Symbol.Name)// convert base type static methods
						{
							case "Int32": line = line.Replace("int.", "Int32_EXT."); break;
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
				if (!isInstanceMethod)
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

				// format sub method calls
				//foreach (var childNode in node.ChildNodes())
				//{
				//	if (childNode.GetType() != typeof(ArgumentListSyntax)) continue;
				//	var n = (ArgumentListSyntax)childNode;
				//	foreach (var a in n.Arguments)
				//	{
				//		if (a.Expression == null) continue;
				//		var t = a.Expression.GetType();
				//		if (t == typeof(InvocationExpressionSyntax))
				//		{
				//			line = formatExpressions(line, a.Expression);
				//		}
				//		else if (t == typeof(BinaryExpressionSyntax))
				//		{
				//			var b = (BinaryExpressionSyntax)a.Expression;
				//			if (b.Left.GetType() == typeof(InvocationExpressionSyntax)) line = formatExpressions(line, b.Left);
				//			else if (b.Right.GetType() == typeof(InvocationExpressionSyntax)) line = formatExpressions(line, b.Right);
				//		}
				//	}
				//}
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
			//var asNode = syntaxNode.AsNode();
			//var asToken = syntaxNode.AsToken();
			//var nodeType = asNode != null ? asNode.GetType() : null;
			//var tokenType = asToken != null ? asToken.GetType() : null;
			var nodeType = syntaxNode.GetType();
			//writer.WriteLine(type); base.Compile(writer, mode); return;

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
			//// fields
			//else if (type == typeof(FieldDeclarationSyntax))
			//{
			//	var node = (FieldDeclarationSyntax)syntaxNode;
			//	writeModifiers(writer, node.Modifiers);
			//	writer.Write(node.Declaration.Type.ToString() + " ");
			//	base.Compile(writer, mode);
			//}
			//else if (type == typeof(VariableDeclarationSyntax))
			//{
			//	var node = (VariableDeclarationSyntax)syntaxNode;
			//	foreach (var v in node.Variables)
			//	{
			//		writer.Write(string.Format("{0} = {1}", v.Identifier.Value, v.Initializer.Value));
			//		if (v != node.Variables.Last()) writer.Write(", ");
			//	}
			//	writer.WriteLine(";");
			//}
			// methods
			else if (nodeType == typeof(MethodDeclarationSyntax))
			{
				var node = (MethodDeclarationSyntax)syntaxNode;
				var test = node.GetText();
				writer.Write(string.Format("{0} {1}::", node.ReturnType.ToString(), currentClass.Identifier));
				writer.Write(node.Identifier.ToString() + "(");
				foreach (var p in node.ParameterList.Parameters)
				{
					writer.Write(string.Format("{0} {1}", p.Type, p.Identifier));
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
					//writer.WriteLine(statement.GetType());
				}

				base.Compile(writer, mode);
			}
			/*else if (nodeType == typeof(LocalDeclarationStatementSyntax))
			{
				var node = (LocalDeclarationStatementSyntax)syntaxNode;
				var symbolInfo = semanticModel.GetSymbolInfo(node.Declaration.Type);
				writer.Write(symbolInfo.Symbol + " ");
				base.Compile(writer, mode);
			}*/
			//else if (type == typeof(VariableDeclarationSyntax))
			//{
			//	var node = (VariableDeclarationSyntax)syntaxNode;
			//	var symbolInfo = semanticModel.GetSymbolInfo(node.Type);
			//	writer.WriteLine(string.Format("{0} {1}", symbolInfo.Symbol, node.));
			//}
			/*else if (nodeType == typeof(VariableDeclaratorSyntax))
			{
				var node = (VariableDeclaratorSyntax)syntaxNode;
				writer.Write(node.Identifier);
				base.Compile(writer, mode);
			}
			else if (nodeType == typeof(EqualsValueClauseSyntax))
			{
				var node = (EqualsValueClauseSyntax)syntaxNode;
				writer.Write(" = ");
				base.Compile(writer, mode);
			}
			else if (nodeType == typeof(LiteralExpressionSyntax))
			{
				var node = (LiteralExpressionSyntax)syntaxNode;
				writer.Write(node.Token);
				base.Compile(writer, mode);
			}*/
			//else if (type == typeof(BinaryExpressionSyntax))
			//{
			//	var node = (BinaryExpressionSyntax)syntaxNode;
			//	writer.Write(string.Format("{0} {1} {2}", node.Left, node.OperatorToken, node.Right));
			//	base.Compile(writer, mode);
			//}
			//else if (tokenType != null)// && tokenType.Name == ";")
			//{
			//	writer.Write(tokenType.FullName);
			//	base.Compile(writer, mode);
			//}
			else
			{
				//if (nodeType != null) writer.WriteLine(nodeType);
				//writer.WriteLine(nodeType);
				base.Compile(writer, mode);
			}
		}
	}
}
