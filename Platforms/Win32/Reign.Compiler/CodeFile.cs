using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace Reign.Compiler
{
	public abstract class CodeFile
	{
		public string Code, FilePath, FileName, Name;
		protected CompilerSolution compiler;
		protected Node rootNode;
		private Document Document;
		private CSharpSyntaxTree syntaxTree;
		private SemanticModel semanticModel;

		public static async Task<CodeFile> New(CompilerSolution compiler, Document document)
		{
			switch (compiler.baseOutputType)
			{
				case CompilerBaseOutputTypes.Cpp:
					var file = new CppCodeFile();
					await file.init(compiler, document);
					return file;

				default: throw new Exception("Unsuported CodeFile base type: " + compiler.baseOutputType);
			}
		}

		private async Task init(CompilerSolution compiler, Document document)
		{
			this.compiler = compiler;
			this.Document = document;
			this.FilePath = document.FilePath;
			Name = Path.GetFileNameWithoutExtension(document.Name);
			FileName = Path.GetFileName(FilePath);
			syntaxTree = (CSharpSyntaxTree)document.GetSyntaxTreeAsync().Result;
			semanticModel = await document.GetSemanticModelAsync();
			rootNode = Node.New(compiler, syntaxTree.GetRoot(), semanticModel);
		}

		public abstract void Compile();
		public abstract void Compile(string outputDirectory);
		public virtual void Compile(StreamWriter writer, int mode)
		{
			rootNode.Compile(writer, mode);
		}

		protected Stream compileToStream(string outputDirectory, string fileExt)
		{
			string filePath = Path.GetFileNameWithoutExtension(FilePath) + fileExt;
			string path = outputDirectory + filePath;
			string root = Path.GetDirectoryName(path);
			if (!Directory.Exists(root)) Directory.CreateDirectory(root);
			return new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
		}
	}
}
