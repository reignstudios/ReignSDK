using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace Reign.Compiler
{
	public class CppNode : Node
	{
		private static bool enableTokens, enableTokens2, convertNextPointerType, disableNextIdentifier;

		public CppNode(CompilerBase compiler, AstNode astNode)
		: base(compiler, astNode)
		{
			
		}

		public override void Compile(StreamWriter writer, int mode)
		{
			if (mode == 0) CompileHeaderFile(writer);
			else CompileCppFile(writer);
		}

		public void CompileHeaderFile(StreamWriter writer)
		{
			var type = astNode.GetType();
			if (type == typeof(SyntaxTree))
			{
				base.Compile(writer, 0);
			}
			else if (type == typeof(UsingDeclaration))
			{
				var value = ((UsingDeclaration)astNode);
				writer.WriteLine(string.Format(@"#include ""{0}.h""", value.Namespace));
				writer.WriteLine(string.Format(@"using namespace {0};", value.Namespace.Replace(".", "::")));
			}
			else if (type == typeof(NamespaceDeclaration))
			{
				var value = ((NamespaceDeclaration)astNode);
				var names = value.Name.Split('.');
				int count = 0;
				foreach (var name in names)
				{
					++count;
					writer.Write(string.Format(@"namespace {0}{1}", name, count == names.Length ? "" : "{"));
				}

				writer.WriteLine(Environment.NewLine + "{");
				base.Compile(writer, 0);
				for (int i = 0; i != count; ++i) writer.WriteLine("}");
			}
			else if (type == typeof(TypeDeclaration))
			{
				var value = ((TypeDeclaration)astNode);
					
				//if ((value.Modifiers & Modifiers.Public) != 0) writer.Write("public ");
				//if ((value.Modifiers & Modifiers.Static) != 0) writer.Write("static ");

				switch (value.ClassType)
				{
					case ClassType.Class: writer.Write("class "); break;
					case ClassType.Struct: writer.Write("struct "); break;
					case ClassType.Interface: writer.Write("class "); break;
					case ClassType.Enum: writer.Write("enum "); break;
				}

				writer.Write(value.Name);
				foreach (var b in value.BaseTypes)
				{
					if (b == value.BaseTypes.FirstOrNullObject()) writer.Write(" : ");
					else writer.Write(", ");
					writer.Write("public " + b.ToString());
				}
				writer.WriteLine(Environment.NewLine + "{");
				base.Compile(writer, 0);
				writer.WriteLine("};");
			}
			else if (type == typeof(MethodDeclaration))
			{
				var value = ((MethodDeclaration)astNode);

				writeModifier(writer, value.Modifiers);
				writer.Write(string.Format("{0} {1}(", value.ReturnType.ToString(), value.Name));
				int count = 0;
				foreach (var p in value.Parameters)
				{
					++count;
					writer.Write(string.Format("{0} {1}{2}", p.Type.ToString(), p.Name, count != value.Parameters.Count ? ", " : ""));
				}

				writer.WriteLine(");");
				base.Compile(writer, 0);
			}
			else if (type == typeof(FieldDeclaration))
			{
				var value = ((FieldDeclaration)astNode);

				writeModifier(writer, value.Modifiers);
				writer.Write(value.ReturnType.ToString().Replace("{", "").Replace("}", "") + " ");
				foreach (CppNode child in Children)
				{
					var t = child.astNode.GetType();
					if (t == typeof(VariableInitializer))
					{
						var v = ((VariableInitializer)child.astNode);
						writer.Write(v.Name);
					}
					else if (t == typeof(CSharpTokenNode))
					{
						switch (child.astNode.Role.ToString())
						{
							case ",": writer.Write(", "); break;
							case ";": writer.WriteLine(";"); break;
						}
					}
				}
			}
			else if (type == typeof(PropertyDeclaration))
			{
				// TODO
			}
		}

		private static void writeModifier(StreamWriter writer, Modifiers modifier)
		{
			if ((modifier & Modifiers.Private) != 0) writer.Write("private: ");
			if ((modifier & Modifiers.Protected) != 0) writer.Write("protected: ");
			if ((modifier & Modifiers.Public) != 0) writer.Write("public: ");
			if ((modifier & Modifiers.Static) != 0) writer.Write("static ");
			if ((modifier & Modifiers.Virtual) != 0) writer.Write("virtual ");
			if ((modifier & Modifiers.Abstract) != 0) writer.Write("virtual ");
		}

		public void CompileCppFile(StreamWriter writer)
		{
			var type = astNode.GetType();
			if (type == typeof(SyntaxTree))
			{
				base.Compile(writer, 1);
			}
			else if ((enableTokens || enableTokens2) && type == typeof(CSharpTokenNode))
			{
				switch (astNode.Role.ToString())
				{
					case "{": writer.WriteLine("{"); break;
					case "}": writer.WriteLine("}"); break;
					case ",": writer.Write(", "); break;
					case ";": writer.WriteLine(";"); break;
					case ".": writer.Write(convertNextPointerType ? "->" : "."); convertNextPointerType = false; break;
					case "(": writer.Write("("); break;
					case ")": writer.Write(")"); break;
					case "[": writer.Write("["); break;
					case "]": writer.Write("]"); break;

					case "=": writer.Write(" = "); break;
					case "+": writer.Write(" + "); break;
					case "-": writer.Write(" - "); break;
					case "*": writer.Write(" * "); break;
					case "/": writer.Write(" / "); break;

					case "+=": writer.Write(" += "); break;
					case "-=": writer.Write(" -= "); break;
					case "*=": writer.Write(" *= "); break;
					case "/=": writer.Write(" /= "); break;

					case "==": writer.Write(" == "); break;
					case "!=": writer.Write(" != "); break;

					case "++": writer.Write("++"); break;
					case "--": writer.Write("--"); break;

					case "for": writer.Write("for "); break;
				}
			}
			else if (type == typeof(NamespaceDeclaration))
			{
				var value = ((NamespaceDeclaration)astNode);
				var names = value.Name.Split('.');
				int count = 0;
				foreach (var name in names)
				{
					++count;
					writer.WriteLine(string.Format(@"namespace {0}{1}", name, count == names.Length ? "" : "{"));
				}

				writer.WriteLine("{");
				base.Compile(writer, 1);
				for (int i = 0; i != count; ++i) writer.WriteLine("}");
			}
			else if (type == typeof(TypeDeclaration))
			{
				disableNextIdentifier = true;
				base.Compile(writer, 1);
			}
			else if (type == typeof(MethodDeclaration))
			{
				var value = ((MethodDeclaration)astNode);
					
				var t = value.Parent as TypeDeclaration;
				writer.Write(string.Format("{0} {1}::{2}(", value.ReturnType.ToString(), t.Name, value.Name));
				disableNextIdentifier = true;
				
				int count = 0;
				foreach (var p in value.Parameters)
				{
					++count;
					writer.Write(string.Format("{0} {1}{2}", p.Type.ToString(), p.Name, count != value.Parameters.Count ? ", " : ""));
				}
				
				if ((value.Modifiers & Modifiers.Abstract) != 0)
				{
					writer.WriteLine(") {/* Abstract Method */}");
				}
				else
				{
					writer.WriteLine(")");
					base.Compile(writer, 1);
				}
			}
			else if (type == typeof(BlockStatement))
			{
				writer.WriteLine("{");
				base.Compile(writer, 1);
				writer.WriteLine("}");
			}
			else if (type == typeof(VariableDeclarationStatement))
			{
				var value = ((VariableDeclarationStatement)astNode);
				string t = value.Type.ToString();
				if (t == "var") writer.Write("auto ");
				else writer.Write(t + " ");
				enableTokens = true;
				base.Compile(writer, 1);
				enableTokens = false;
			}
			else if (type == typeof(ExpressionStatement))
			{
				enableTokens = true;
				base.Compile(writer, 1);
				enableTokens = false;
			}
			else if (type == typeof(AssignmentExpression))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(UnaryOperatorExpression))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(VariableInitializer))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(PrimitiveExpression))
			{
				var value = ((PrimitiveExpression)astNode);
				writer.Write(value.LiteralValue);
			}
			else if (type == typeof(IdentifierExpression))
			{
				var value = ((IdentifierExpression)astNode);
				writer.Write(value.Identifier.ToString());
			}
			else if (type == typeof(BinaryOperatorExpression))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(ParenthesizedExpression))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(MemberReferenceExpression))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(ThisReferenceExpression))
			{
				convertNextPointerType = true;
				writer.Write("this");
			}
			else if (type == typeof(IndexerExpression))
			{
				base.Compile(writer, 1);
			}
			else if (type == typeof(Identifier))
			{
				if (disableNextIdentifier)
				{
					disableNextIdentifier = false;
				}
				else
				{
					var value = ((Identifier)astNode);
					writer.Write(value.Name);
				}
			}
			else if (type == typeof(ForStatement))
			{
				enableTokens2 = true;
				base.Compile(writer, 1);
				enableTokens2 = false;
			}
		}
	}
}
