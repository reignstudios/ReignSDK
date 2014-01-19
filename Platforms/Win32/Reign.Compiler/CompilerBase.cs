using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;

namespace Reign.Compiler
{
	public enum CompilerInputTypes
	{
		CsCode,
		CsFile,
		CsProject,
		CsSolution
	}

	public enum CompilerOutputTypes
	{
		Cpp_VC,
		Cpp_GCC,
		D3D11_HLSL,
		D3D9_HLSL,
		XNA_HLSL,
		Silverlight_HLSL,
		GL4_GLSL,
		GL3_GLSL,
		GL2_GLSL,
		GLES3_GLSL,
		GLES2_GLSL,
		Vita_CG
	}

	public enum CompilerBaseOutputTypes
	{
		Cpp,
		HLSL,
		GLSL,
		CG
	}

    public abstract class CompilerBase
    {
		internal CSharpParser parser;
		internal CompilerInputTypes inputType;
		internal CompilerOutputTypes outputType;
		internal CompilerBaseOutputTypes baseOutputType;
		private List<CodeFile> codeFiles;

		protected CompilerBase(string input, CompilerInputTypes inputType, CompilerOutputTypes outputType)
		{
			this.inputType = inputType;
			this.outputType = outputType;

			// get base type
			switch (outputType)
			{
				case CompilerOutputTypes.Cpp_VC: baseOutputType = CompilerBaseOutputTypes.Cpp; break;
				case CompilerOutputTypes.Cpp_GCC: baseOutputType = CompilerBaseOutputTypes.Cpp; break;

				case CompilerOutputTypes.D3D11_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;
				case CompilerOutputTypes.D3D9_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;
				case CompilerOutputTypes.XNA_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;
				case CompilerOutputTypes.Silverlight_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;

				case CompilerOutputTypes.GL4_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				case CompilerOutputTypes.GL3_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				case CompilerOutputTypes.GL2_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				case CompilerOutputTypes.GLES3_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				case CompilerOutputTypes.GLES2_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;

				case CompilerOutputTypes.Vita_CG: baseOutputType = CompilerBaseOutputTypes.CG; break;

				default: throw new Exception("Unknown compiler base type: " + outputType);
			}

			// create objects
			parser = new CSharpParser();
			codeFiles = new List<CodeFile>();
			switch (inputType)
			{
				case CompilerInputTypes.CsCode:
					codeFiles.Add(CodeFile.New(this, input, "Main.cs"));
					break;

				default: throw new Exception("InputType not supported yet: " + inputType);
			}
		}
		
		public void Compile(string outputDirectory)
		{
			foreach (var file in codeFiles)
			{
				file.Compile(outputDirectory);
			}
		}
    }
}
