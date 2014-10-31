using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reign.Compiler
{
	public class CppCompiler : CompilerBase
	{
		public static async Task<CppCompiler> New(string input, CompilerInputTypes inputType, CompilerOutputTypes outputType)
		{
			var obj = new CppCompiler();
			await obj.init(input, inputType, outputType);
			return obj;
		}

		public override void Compile(string outputDirectory)
		{
			if (outputType == CompilerOutputTypes.Cpp_VC)
			{
				// TODO: output VS proj file
			}
			else if (outputType == CompilerOutputTypes.Cpp_GCC)
			{
				// TODO: output make file
			}

			base.Compile(outputDirectory);
		}
	}
}
