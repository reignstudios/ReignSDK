using System;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;

namespace Reign.Compiler
{
	public class CppCodeFile : CodeFile
	{
		public string CppCode, HeaderCode;

		public CppCodeFile(CompilerBase compiler, Document document)
		: base(compiler, document)
		{
			
		}

		public override void Compile()
		{
			// write h file
			using (var stream = new MemoryStream())
			{
				var writer = new StreamWriter(stream);
				var reader = new StreamReader(stream);
				Compile(writer, 0);

				writer.Flush();
				stream.Position = 0;
				HeaderCode = reader.ReadToEnd();
			}

			// write cpp file
			using (var stream = new MemoryStream())
			{
				var writer = new StreamWriter(stream);
				var reader = new StreamReader(stream);
				writer.WriteLine(string.Format(@"#include ""{0}.h""", Path.GetFileNameWithoutExtension(FileName)));
				Compile(writer, 1);

				writer.Flush();
				stream.Position = 0;
				CppCode = reader.ReadToEnd();
			}
		}

		public override void Compile(string outputDirectory)
		{
			// write h file
			HeaderCode = FilePath + " - HEADER";
			using (var stream = compileToStream(outputDirectory, ".h"))
			using (var writer = new StreamWriter(stream))
			{
				Compile(writer, 0);
			}

			// write cpp file
			CppCode = FilePath + " - CPP";
			using (var stream = compileToStream(outputDirectory, ".cpp"))
			using (var writer = new StreamWriter(stream))
			{
				writer.WriteLine(string.Format(@"#include ""{0}.h""", Path.GetFileNameWithoutExtension(FileName)));
				Compile(writer, 1);
			}
		}

		public override void Compile(StreamWriter writer, int mode)
		{
			// pre write includes before using statments
			if (mode == 0)
			{
				// write header files
				foreach (var node in rootNode.Children)
				{
					var type = node.syntaxNode.GetType();
					if (type == typeof(UsingDirectiveSyntax))
					{
						var n = (UsingDirectiveSyntax)node.syntaxNode;
						string value = n.Name.ToFullString();
						writer.WriteLine(string.Format(@"#include ""{0}.h""", value));
					}
				}

				// write using statments
				foreach (var node in rootNode.Children)
				{
					var type = node.syntaxNode.GetType();
					if (type == typeof(UsingDirectiveSyntax))
					{
						var n = (UsingDirectiveSyntax)node.syntaxNode;
						string value = n.Name.ToFullString();
						writer.WriteLine(string.Format(@"using namespace {0};", value));
					}
				}
			}

			base.Compile(writer, mode);
		}
	}
}
