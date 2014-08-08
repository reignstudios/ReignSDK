﻿using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace Reign.Compiler
{
	public abstract class CodeFile
	{
		public readonly string Code, FilePath, FileName;
		protected CompilerBase compiler;
		protected Node rootNode;
		private Document Document;
		private CSharpSyntaxTree syntaxTree;

		public static CodeFile New(CompilerBase compiler, Document document)
		{
			switch (compiler.baseOutputType)
			{
				case CompilerBaseOutputTypes.Cpp: return new CppCodeFile(compiler, document);
				default: throw new Exception("Unsuported CodeFile base type: " + compiler.baseOutputType);
			}
		}

		protected CodeFile(CompilerBase compiler, Document document)
		{
			this.compiler = compiler;
			this.Document = document;
			this.FilePath = document.FilePath;
			FileName = Path.GetFileName(FilePath);
			syntaxTree = (CSharpSyntaxTree)document.GetSyntaxTreeAsync().Result;
			rootNode = Node.New(compiler, syntaxTree.GetRoot());
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
			return new FileStream(path, FileMode.Create, FileAccess.Write);
		}
	}
}