using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace Reign.Compiler
{
	public enum CppNodeTypes
	{
		None,
		Disabled,
		OpenNamespaceBracket,
		CloseNamespaceBracket,
		OpenObjectBracket,
		CloseObjectBracket
	}

	public class CppNode : Node
	{
		private CppNodeTypes markedType;
		private int endBracketCount;

		public CppNode(CompilerBase compiler, AstNode astNode)
		: base(compiler, astNode)
		{
			
		}

		public override void Compile(StreamWriter writer, int mode)
		{
			if (mode == 0) CompileHeaderFile(writer);
			else CompileCppFile(writer);

			base.Compile(writer, mode);
		}

		public void CompileHeaderFile(StreamWriter writer)
		{
			var type = astNode.GetType();
			if (type == typeof(UsingDeclaration))
			{
				var value = ((UsingDeclaration)astNode);
				writer.WriteLine(string.Format(@"using namespace {0};", value.Namespace.Replace(".", "::")));
			}
			else if (type == typeof(CSharpTokenNode))
			{
				switch (astNode.Role.ToString())
				{
					case "{":
						if (markedType == CppNodeTypes.OpenNamespaceBracket) writer.WriteLine(Environment.NewLine + "{");
						else if (markedType == CppNodeTypes.OpenObjectBracket) writer.WriteLine("{");
						break;

					case "}":
						if (markedType == CppNodeTypes.CloseNamespaceBracket)
						{
							for (int i = 0; i != endBracketCount; ++i) writer.WriteLine("}");
						}
						else if (markedType == CppNodeTypes.CloseObjectBracket)
						{
							writer.WriteLine("};");
						}
						break;
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
					writer.Write(string.Format(@"namespace {0}{1}", name, count == names.Length ? "" : "{"));
				}

				// mark brackets
				int openBrackets = 0;
				foreach (CppNode node in Children)
				{
					var t = node.astNode.GetType();
					if (t == typeof(CSharpTokenNode))
					{
						switch (node.astNode.Role.ToString())
						{
							case "{":
								if (openBrackets == 0) node.markedType = CppNodeTypes.OpenNamespaceBracket;
								openBrackets++;
								break;

							case "}":
								openBrackets--;
								if (openBrackets == 0)
								{
									node.markedType = CppNodeTypes.CloseNamespaceBracket;
									node.endBracketCount = count;
								}
								break;
						}
					}
				}
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

				writer.WriteLine(value.Name);

				// mark brackets
				int openBrackets = 0;
				foreach (CppNode node in Children)
				{
					var t = node.astNode.GetType();
					if (t == typeof(CSharpTokenNode))
					{
						switch (node.astNode.Role.ToString())
						{
							case "{":
								if (openBrackets == 0) node.markedType = CppNodeTypes.OpenObjectBracket;
								openBrackets++;
								break;

							case "}":
								openBrackets--;
								if (openBrackets == 0) node.markedType = CppNodeTypes.CloseObjectBracket;
								break;
						}
					}
				}
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
		}

		public void CompileCppFile(StreamWriter writer)
		{
			var type = astNode.GetType();
			if (type == typeof(UsingDeclaration))
			{
				var value = ((UsingDeclaration)astNode);
				writer.WriteLine(string.Format(@"#include ""{0}.h""", value.Namespace));
				foreach (CppNode child in Children)
				{
					if (child.astNode.GetType() == typeof(CSharpTokenNode) && child.astNode.Role.ToString() == ";")
					{
						child.markedType = CppNodeTypes.Disabled;
					}
				}
			}
			else if (type == typeof(CSharpTokenNode))
			{
				switch (astNode.Role.ToString())
				{
					case "{": writer.WriteLine("{"); break;

					case "}":
						if (markedType == CppNodeTypes.CloseNamespaceBracket)
						{
							for (int i = 0; i != endBracketCount; ++i) writer.WriteLine("}");
						}
						else
						{
							writer.WriteLine("}");
						}
						break;

					case ",": writer.Write(", "); break;
					case ";": if (markedType != CppNodeTypes.Disabled) writer.WriteLine(";"); break;
					//case ".": if (markedType != CppNodeTypes.Disabled) writer.Write("."); break;
					case "(": writer.Write("("); break;
					case ")": writer.Write(")"); break;
					case "=": writer.Write(" = "); break;
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
			}
			else if (type == typeof(MethodDeclaration))
			{
				var value = ((MethodDeclaration)astNode);
					
				var p = value.Parent as TypeDeclaration;
				writer.Write(string.Format("{0} {1}::{2}", value.ReturnType.ToString(), p.Name, value.Name));
			}
			else if (type == typeof(ParameterDeclaration))
			{
				var value = ((ParameterDeclaration)astNode);
				writer.Write(string.Format("{0} {1}", value.Type, value.Name));
			}
			/*else if (type == typeof(Identifier))
			{
				var value = ((Identifier)astNode);
				writer.Write(value.ToString());
			}
			else if (type == typeof(IdentifierExpression))
			{
				var value = ((IdentifierExpression)astNode);
				writer.Write(value.Identifier.ToString());
			}
			else if (type == typeof(ThisReferenceExpression))
			{
				var value = ((ThisReferenceExpression)astNode);
				writer.Write("this->");
			}*/
		}
	}
}
