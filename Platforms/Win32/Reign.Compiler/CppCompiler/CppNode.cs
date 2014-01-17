using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace Reign.Compiler
{
	public class CppNode : Node
	{
		public CppNode(CompilerBase compiler, AstNode astNode)
		: base(compiler, astNode)
		{
			
		}

		public override void Compile(StreamWriter writer, int mode)
		{
			if (mode == 0) CompileCppFile(writer);
			else CompileHeaderFile(writer);

			base.Compile(writer, mode);
		}

		public void CompileCppFile(StreamWriter writer)
		{
			var type = astNode.GetType();
			if (type == typeof(UsingDeclaration))
			{
				var value = ((UsingDeclaration)astNode);
				writer.WriteLine(string.Format(@"#include ""{0}.h""", value.Namespace) + Environment.NewLine);
			}
			else if (type == typeof(CSharpTokenNode))
			{
				switch (astNode.Role.ToString())
				{
					case "{": writer.WriteLine("{"); break;
					case "}": writer.WriteLine("}"); break;
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
			else if (type == typeof(TypeDeclaration))
			{
				var value = ((TypeDeclaration)astNode);
					
				if ((value.Modifiers & Modifiers.Public) != 0) writer.Write("public ");
				if ((value.Modifiers & Modifiers.Static) != 0) writer.Write("static ");

				switch (value.ClassType)
				{
					case ClassType.Class: writer.Write("class "); break;
					case ClassType.Struct: writer.Write("struct "); break;
				}

				writer.WriteLine(value.Name);
			}
			else if (type == typeof(MethodDeclaration))
			{
				var value = ((MethodDeclaration)astNode);
					
				if ((value.Modifiers & Modifiers.Private) != 0) writer.Write("private: ");
				if ((value.Modifiers & Modifiers.Public) != 0) writer.Write("public: ");
				if ((value.Modifiers & Modifiers.Static) != 0) writer.Write("static ");

				writer.Write(value.Name + "(");
				foreach (var p in value.Parameters)
				{
					writer.Write(p.Type.ToString() + " " + p.Name);
				}

				writer.WriteLine(")");
			}
		}

		public void CompileHeaderFile(StreamWriter writer)
		{
			
		}
	}
}
