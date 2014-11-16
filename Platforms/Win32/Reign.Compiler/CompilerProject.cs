using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using System.IO;

namespace Reign.Compiler
{
	public enum CompilerProjectTypes
	{
		EXE,
		DLL
	}

	public abstract class CompilerProject
	{
		public string Name;
		public CompilerProjectTypes CompilerProjectType;
		protected CompilerSolution compiler;
		protected Project project;
		public IReadOnlyList<CodeFile> CodeFiles;

		public static async Task<CompilerProject> New(CompilerSolution compiler, Project project)
		{
			switch (compiler.baseOutputType)
			{
				case CompilerBaseOutputTypes.Cpp:
					var cppProject = new CppCompilerProject();
					await cppProject.init(compiler, project);
					return cppProject;

				default: throw new Exception("Unsuported Project base type: " + compiler.baseOutputType);
			}
		}

		private async Task init(CompilerSolution compiler, Project project)
		{
			this.compiler = compiler;
			this.project = project;
			Name = Path.GetFileNameWithoutExtension(project.Name);
			if (project.CompilationOptions.OutputKind == OutputKind.ConsoleApplication) CompilerProjectType = CompilerProjectTypes.EXE;
			else if (project.CompilationOptions.OutputKind == OutputKind.DynamicallyLinkedLibrary) CompilerProjectType = CompilerProjectTypes.DLL;
			else throw new Exception("Unsuported project type: " + project.CompilationOptions.OutputKind);

			// cs files
			var codeFiles = new List<CodeFile>();
			foreach (var document in project.Documents)
			{
				codeFiles.Add(await CodeFile.New(compiler, document));
			}
			CodeFiles = codeFiles;
		}

		public void Compile()
		{
			foreach (var file in CodeFiles)
			{
				file.Compile();
			}
		}

		public virtual void Compile(CompilerOutputDesc desc)
		{
			foreach (var file in CodeFiles)
			{
				file.Compile(desc.OutputDirectory + project.Name + "/");
			}
		}
	}
}
