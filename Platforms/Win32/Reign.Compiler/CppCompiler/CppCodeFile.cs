using System;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace Reign.Compiler
{
	public class CppCodeFile : CodeFile
	{
		public CppCodeFile(CompilerBase compiler, string code, string fileName)
		: base(compiler, code, fileName)
		{
			
		}

		public override void Compile(string outputDirectory)
		{
			// write cpp file
			using (var stream = compileToStream(outputDirectory, ".cpp"))
			using (var writer = new StreamWriter(stream))
			{
				Compile(writer, 0);
			}

			// write h file
			/*using (var stream = compileToStream(outputDirectory, ".h"))
			using (var writer = new StreamWriter(stream))
			{
				Compile(writer, 1);
			}*/
		}
	}
}
