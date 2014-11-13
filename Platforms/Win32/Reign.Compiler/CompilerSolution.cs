using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace Reign.Compiler
{
	public enum CompilerInputTypes
	{
		CsProject,
		CsSolution
	}

	public enum CompilerOutputTypes
	{
		Cpp_VC,
		Cpp_GCC,
		HLSL_D3D11,
		HLSL_D3D9,
		HLSL_XNA,
		HLSL_Silverlight,
		GLSL_GL4,
		GLSL_GL3,
		GLSL_GL2,
		GLSL_GLES3,
		GLSL_GLES2,
		CG_Vita
	}

	public enum CompilerBaseOutputTypes
	{
		Cpp,
		HLSL,
		GLSL,
		CG
	}

	public class CompilerOutputDesc
	{
		// Global
		public string OutputDirectory;

		// Visual C++
		public bool Cpp_VC_OutputSolution, Cpp_VC_OutputProjects;
		public string Cpp_VC_ReignCppSources;
	}

    public abstract class CompilerSolution : IDisposable
    {
		public string Name;
		internal CompilerInputTypes inputType;
		internal CompilerOutputTypes outputType;
		internal CompilerBaseOutputTypes baseOutputType;
		public IReadOnlyList<CompilerProject> Projects;
		private MSBuildWorkspace workspace;

		public static async Task<CompilerSolution> New(string input, CompilerInputTypes inputType, CompilerOutputTypes outputType)
		{
			switch (outputType)
			{
				case CompilerOutputTypes.Cpp_VC:
				case CompilerOutputTypes.Cpp_GCC:
					var obj = new CppCompilerSolution();
					await obj.init(input, inputType, outputType);
					return obj;

				default: throw new Exception("Unsuported Solution output type: " + outputType);
			}
		}

		private async Task init(string input, CompilerInputTypes inputType, CompilerOutputTypes outputType)
		{
			this.inputType = inputType;
			this.outputType = outputType;

			// get base type
			switch (outputType)
			{
				case CompilerOutputTypes.Cpp_VC: baseOutputType = CompilerBaseOutputTypes.Cpp; break;
				//case CompilerOutputTypes.Cpp_GCC: baseOutputType = CompilerBaseOutputTypes.Cpp; break;

				//case CompilerOutputTypes.D3D11_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;
				//case CompilerOutputTypes.D3D9_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;
				//case CompilerOutputTypes.XNA_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;
				//case CompilerOutputTypes.Silverlight_HLSL: baseOutputType = CompilerBaseOutputTypes.HLSL; break;

				//case CompilerOutputTypes.GL4_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				//case CompilerOutputTypes.GL3_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				//case CompilerOutputTypes.GL2_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				//case CompilerOutputTypes.GLES3_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;
				//case CompilerOutputTypes.GLES2_GLSL: baseOutputType = CompilerBaseOutputTypes.GLSL; break;

				//case CompilerOutputTypes.Vita_CG: baseOutputType = CompilerBaseOutputTypes.CG; break;

				default: throw new Exception("Unsuported compiler base type: " + outputType);
			}

			// load workspace
			workspace = MSBuildWorkspace.Create();
			if (inputType == CompilerInputTypes.CsProject)
			{
				Name = "Reign";
				var langs = workspace.Services.SupportedLanguages;
				var csProj = await workspace.OpenProjectAsync(input);
				var projects = new List<CompilerProject>();
				projects.Add(await CompilerProject.New(this, csProj));
				Projects = projects;
			}
			else
			{
				throw new Exception("Input type not supported: " + inputType);
			}
		}

		~CompilerSolution()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (workspace != null)
			{
				workspace.Dispose();
				workspace = null;
			}
		}

		public void Compile()
		{
			foreach (var project in Projects)
			{
				project.Compile();
			}
		}
		
		public virtual void Compile(CompilerOutputDesc desc)
		{
			foreach (var project in Projects)
			{
				project.Compile(desc);
			}
		}
    }
}
