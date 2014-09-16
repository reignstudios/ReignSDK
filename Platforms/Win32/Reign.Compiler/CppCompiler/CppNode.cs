using System;
using System.IO;
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

		private string comileBlockStatment(string line, CSharpSyntaxNode baseNode)
		{
			var type = baseNode.GetType();
			if (type == typeof(LocalDeclarationStatementSyntax))
			{
				var node = (LocalDeclarationStatementSyntax)baseNode;
				if (node.Declaration.Type.IsVar)
				{
					var symbolInfo = semanticModel.GetSymbolInfo(node.Declaration.Type);
					line = Regex.Replace(line, @"var\s*", symbolInfo.Symbol.ToString() + " ", RegexOptions.Singleline);
					return line;
				}
			}
			else if (type == typeof(ExpressionStatementSyntax))
			{
				var node = (ExpressionStatementSyntax)baseNode;
				var e = node.Expression;
				string fullName = "";
				while (e != null)
				{
					if (e.GetType() == typeof(InvocationExpressionSyntax))
					{
						var eNode = (InvocationExpressionSyntax)e;
						e = eNode.Expression;
					}
					else if (e.GetType() == typeof(MemberAccessExpressionSyntax))
					{
						var eNode = (MemberAccessExpressionSyntax)e;
						fullName += eNode.Name + ".";
						e = eNode.Expression;
					}
					else if (e.GetType() == typeof(IdentifierNameSyntax))
					{
						var eNode = (IdentifierNameSyntax)e;
						fullName += eNode.Identifier;
						e = null;
					}
					else
					{
						e = null;
					}
				}
				//var e = (InvocationExpressionSyntax)node.Expression;
				//var e2 = (MemberAccessExpressionSyntax)e.Expression;
				//var i = (IdentifierNameSyntax)e2.Expression;

				var values = fullName.Split('.');


				return line;
			}

			// search deeper
			foreach (var childNode in baseNode.ChildNodes())
			{
				line = comileBlockStatment(line, (CSharpSyntaxNode)childNode);
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
					line = comileBlockStatment(line, statement);
					writer.WriteLine(line);
					writer.WriteLine(statement.GetType());
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
