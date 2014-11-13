using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Reign.Compiler
{
	public class CppCompilerSolution : CompilerSolution
	{
		public override void Compile(CompilerOutputDesc desc)
		{
			if (desc.Cpp_VC_OutputSolution && outputType == CompilerOutputTypes.Cpp_VC)
			{
				string root = Path.GetDirectoryName(desc.OutputDirectory);
				if (!Directory.Exists(root)) Directory.CreateDirectory(root);
				using (var stream = new FileStream(desc.OutputDirectory + Name + ".sln", FileMode.Create, FileAccess.Write, FileShare.None))
				using (var writer = new StreamWriter(stream))
				{
					writer.WriteLine(@"Microsoft Visual Studio Solution File, Format Version 12.00");
					writer.WriteLine(@"# Visual Studio 2013");
					writer.WriteLine(@"VisualStudioVersion = 12.0.30723.0");
					writer.WriteLine(@"MinimumVisualStudioVersion = 10.0.40219.1");
					foreach (var project in Projects)
					{
						var id = ((CppCompilerProject)project).ID;
						writer.WriteLine(string.Format(@"Project(""{{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}}"") = ""{0}"", ""{0}\{0}.vcxproj"", ""{{{1}}}""", project.Name, id));
						writer.WriteLine(@"EndProject");
					}
					writer.WriteLine(@"Global");
					writer.WriteLine(@"	GlobalSection(SolutionConfigurationPlatforms) = preSolution");
					writer.WriteLine(@"		Debug|Win32 = Debug|Win32");
					writer.WriteLine(@"		Release|Win32 = Release|Win32");
					writer.WriteLine(@"	EndGlobalSection");
					writer.WriteLine(@"	GlobalSection(ProjectConfigurationPlatforms) = postSolution");
					foreach (var project in Projects)
					{
						var id = ((CppCompilerProject)project).ID;
						writer.WriteLine(string.Format(@"		{{{0}}}.Debug|Win32.ActiveCfg = Debug|Win32", id));
						writer.WriteLine(string.Format(@"		{{{0}}}.Debug|Win32.Build.0 = Debug|Win32", id));
						writer.WriteLine(string.Format(@"		{{{0}}}.Release|Win32.ActiveCfg = Release|Win32", id));
						writer.WriteLine(string.Format(@"		{{{0}}}.Release|Win32.Build.0 = Release|Win32", id));
					}
					writer.WriteLine(@"	EndGlobalSection");
					writer.WriteLine(@"	GlobalSection(SolutionProperties) = preSolution");
					writer.WriteLine(@"		HideSolutionNode = FALSE");
					writer.WriteLine(@"	EndGlobalSection");
					writer.WriteLine(@"EndGlobal");
				}
			}
			else if (outputType == CompilerOutputTypes.Cpp_GCC)
			{
				// TODO: output make file
			}

			base.Compile(desc);
		}
	}
}
