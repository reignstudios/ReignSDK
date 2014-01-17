using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace Reign.Compiler
{
	public abstract class CodeFile
	{
		public readonly string Code, FileName;
		protected CompilerBase compiler;
		protected Node rootNode;

		public static CodeFile New(CompilerBase compiler, string code, string fileName)
		{
			switch (compiler.baseOutputType)
			{
				case CompilerBaseOutputTypes.Cpp: return new CppCodeFile(compiler, code, fileName);
				default: throw new Exception("Unsuported CodeFile base type: " + compiler.baseOutputType);
			}
		}

		protected CodeFile(CompilerBase compiler, string code, string fileName)
		{
			this.compiler = compiler;
			this.Code = code;
			this.FileName = fileName;
			var syntaxTree = compiler.parser.Parse(code);
			rootNode = Node.New(compiler, syntaxTree);
		}

		public abstract void Compile(string outputDirectory);
		public void Compile(StreamWriter writer, int mode)
		{
			rootNode.Compile(writer, mode);
		}

		protected Stream compileToStream(string outputDirectory, string fileExt)
		{
			string fileName = Path.GetFileNameWithoutExtension(FileName) + fileExt;
			string path = outputDirectory + fileName;
			string root = Path.GetDirectoryName(path);
			if (!Directory.Exists(root)) Directory.CreateDirectory(root);
			return new FileStream(path, FileMode.Create, FileAccess.Write);
		}
	}
}
