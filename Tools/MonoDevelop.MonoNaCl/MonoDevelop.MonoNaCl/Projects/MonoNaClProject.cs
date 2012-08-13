using System;
using System.Xml;
using MonoDevelop.Core;
using MonoDevelop.Core.Serialization;
using MonoDevelop.Projects;
using System.IO;
using System.Collections.Generic;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.Core.Execution;
using MonoDevelop.Ide;
using System.Reflection;
using System.Diagnostics;
using MonoDevelop.CSharp.Project;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MonoDevelop.MonoNaCl
{
	[DataContract]
	class Manifest
	{
		[DataContract]
		public class Icon
		{
			[DataMember(Name = "16", EmitDefaultValue = false)]
			public string _16;
			
			[DataMember(Name = "48", EmitDefaultValue = false)]
			public string _48;
			
			[DataMember(Name = "128", EmitDefaultValue = false)]
			public string _128;
		}
		
		[DataContract]
		public class Requirements
		{
			[DataContract]
			public class _3D
			{
				[DataMember(EmitDefaultValue = false)]
				public string[] features;
			}
			
			[DataMember(Name = "3D", EmitDefaultValue = false)]
			public _3D _3d;
		}
		
		[DataContract]
		public class App
		{
			[DataContract]
			public class Launch
			{
				[DataMember(EmitDefaultValue = false)]
				public string local_path;
			}
			
			[DataMember(EmitDefaultValue = false)]
			public Launch launch;
		}
	
		[DataMember(EmitDefaultValue = false)]
		public string name;
		
		[DataMember(EmitDefaultValue = false)]
		public string description;
		
		[DataMember(EmitDefaultValue = false)]
		public string version;
		
		[DataMember(EmitDefaultValue = false)]
		public Icon icons;
		
		[DataMember(EmitDefaultValue = false)]
		public Requirements requirements;
		
		[DataMember(EmitDefaultValue = false)]
		public App app;
	}

	public class MonoNaClProject : DotNetAssemblyProject, ICustomDataItem
	{
		#region Project Settings
		[ItemProperty("RequiresGLES")]
		public bool RequiresGLES = true;
		
		[ItemProperty("CopyAllJsonObjects")]
		public bool CopyAllJsonObjects = true;
		
		[ItemProperty("CopyAllHtmlObjects")]
		public bool CopyAllHtmlObjects = true;
		
		[ItemProperty("GenerateManifest")]
		public bool GenerateManifest = true;
		
		[ItemProperty("AppName")]
		public string AppName = "My MonoNaCl Project";
		
		[ItemProperty("AppDescription")]
		public string AppDescription = "This is a MonoNaCl project";
		
		[ItemProperty("AppVersion")]
		public string AppVersion = "1.0.0";
		
		[ItemProperty("AppLaunchHTML")]
		public string AppLaunchHTML;
		
		public void Deserialize (ITypeSerializer handler, DataCollection data)
		{
			handler.Deserialize (this, data);
		}

		public DataCollection Serialize (ITypeSerializer handler)
		{
			return handler.Serialize (this);
		}
		#endregion
	
		public override string ProjectType {get{return "MonoNaCl";}}
		private string executeProgramData;
		private MonoNaClProjectConfiguration buildingConfig;
		private BuildResult buildingResult;
		private IProgressMonitor progressMonitor;

		public MonoNaClProject ()
		{
			
		}

		public MonoNaClProject (string languageName)
		: base (languageName)
		{
			
		}

		public MonoNaClProject (string languageName, ProjectCreateInformation info, XmlElement projectOptions)
		: base (languageName, info, projectOptions)
		{
			
		}

		public override bool SupportsFormat (FileFormat format)
		{
			return format.Id == "MSBuild10";
		}

		public override SolutionItemConfiguration CreateConfiguration (string name)
		{
			var conf = new MonoNaClProjectConfiguration (name);
			conf.CopyFrom (base.CreateConfiguration (name));
			return conf;
		}

		protected override void OnExecute (IProgressMonitor monitor, ExecutionContext context, ConfigurationSelector configSel)
		{
			MessageService.ShowError (GettextCatalog.GetString ("You must build and execute the NaCl application through Chrome."));
		}
		
		protected override BuildResult OnBuild (IProgressMonitor monitor, ConfigurationSelector configuration)
		{
			// Handle pending events to ensure that files get saved right before the project is built
			DispatchService.RunPendingEvents ();
			
			progressMonitor = monitor;
			buildingResult = new BuildResult();
			var config = GetConfiguration(configuration) as MonoNaClProjectConfiguration;
			buildingConfig = config;
			
			if (string.IsNullOrEmpty(AppLaunchHTML))
			{
				AppLaunchHTML = config.AppName + ".html";
			}
			
			var dirInfo = new DirectoryInfo(config.OutputDirectory);
			if (!dirInfo.Exists) Directory.CreateDirectory(config.OutputDirectory);
			
			if (config.CompileTarget == MonoDevelop.Projects.CompileTarget.Exe)
			{
				buildNaCl(buildingResult, config, configuration);
			}
			buildCS(buildingResult, config, configuration);
			
			return buildingResult;
		}
		
		private string executeProgram(string exe, string args)
		{
			var process = new Process ();
			process.StartInfo.FileName = exe;
			process.StartInfo.Arguments = args;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.WorkingDirectory = buildingConfig.OutputDirectory;
			
			process.OutputDataReceived += new DataReceivedEventHandler(executeProgramCallback);
			process.ErrorDataReceived += new DataReceivedEventHandler(executeProgramErrorCallback);
			
			executeProgramData = null;
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit ();
			
			return executeProgramData;
		}
		
		private void executeProgramCallback(object sender, DataReceivedEventArgs e)
		{
			lock (this)
			{
				if (!String.IsNullOrEmpty(e.Data))
				{
					progressMonitor.ReportSuccess(e.Data);
					executeProgramData = e.Data;
				}
			}
		}
		
		private void executeProgramErrorCallback(object sender, DataReceivedEventArgs e)
		{
			lock (this)
			{
				if (!String.IsNullOrEmpty(e.Data)) 
				{
					if (Regex.Match(e.Data.ToLower(), ".*error:|error", RegexOptions.Singleline).Success)
					{
						buildingResult.AddError(e.Data);
					}
					else
					{
						buildingResult.AddWarning(e.Data);
					}
				}
			}
		}
		
		private void buildNaCl(BuildResult result, MonoNaClProjectConfiguration config, ConfigurationSelector configSel)
		{
			string NACL_SDK_ROOT = MonoNaClSettingsService.Instance.NACL_SDK_ROOT;
			string NACL_MONO_ROOT = MonoNaClSettingsService.Instance.NACL_MONO_ROOT;
		
			// Set compiler flags
			string WARNINGS = "-Wno-long-long -Wall";//"-w";
			string CFLAGS = string.Format(@"-O0 -g {0} -I {1}/include/mono-2.0", WARNINGS, NACL_MONO_ROOT);
			string flags = "";
			if (RequiresGLES) flags = "-lppapi_gles2 ";
			string LDFLAGS = string.Format(@"{0}-lppapi -lmono-2.0 -L {1}/lib32 -Wl,--export-dynamic -ldl", flags, NACL_MONO_ROOT);
			string LDFLAGS64 = string.Format(@"{0}-lppapi -lmono-2.0 -L {1}/lib -Wl,--export-dynamic -ldl", flags, NACL_MONO_ROOT);
			
			// Get output name
			string PROJECT = "main";//config.AppName;
			
			// Get C and CS sources
			string CC_SOURCES = "";
			foreach (var file in Files)
			{
				string fullPath = '"'+file.FilePath.FullPath+'"' + ' ';
				switch (file.FilePath.Extension.ToLower())
				{
					case (".c"): CC_SOURCES += fullPath; break;
				}
			}
			if (CC_SOURCES == "") return;
			
			// Get .dll sources
			string CS_DLL_SOURCES = "";
			foreach (var reference in References)
			{
				var names = reference.GetReferencedFileNames(configSel);
				foreach (var name in names)
				{
					CS_DLL_SOURCES += "-r:" + '"'+name+'"';
				}
			}
			
			// Get OS name
			string OSNAME = executeProgram("python", string.Format(@"{0}/tools/getos.py", NACL_SDK_ROOT));
			
			// If libmono was compiled in a posix environment (NaCl) use forward slashes so Windows can cope with the paths.
			if (OSNAME == "win")
			{
				NACL_SDK_ROOT = NACL_SDK_ROOT.Replace(@"\", @"/");
				NACL_MONO_ROOT = NACL_MONO_ROOT.Replace(@"\", @"/");
			}
			
			// Get System architecture type
			//string ARCH = executeProgram("python", string.Format(@"{0}/tools/getos.py --arch", NACL_SDK_ROOT));
			
			// Get NaCl Tool Chain path
			string TC_PATH = string.Format(@"{0}/toolchain/{1}_x86_glibc", NACL_SDK_ROOT, OSNAME);
			
			// Get NaCl gcc compiler
			string CC = string.Format(@"{0}/bin/i686-nacl-gcc", TC_PATH);
			
			// Compile .nexe's
			executeProgram(CC, string.Format(@"-o {0} {1} -m32 -O0 -g {2} {3}", PROJECT+"_x86_32.nexe", CC_SOURCES, CFLAGS, LDFLAGS));
			executeProgram(CC, string.Format(@"-o {0} {1} -m64 -O0 -g {2} {3}", PROJECT+"_x86_64.nexe", CC_SOURCES, CFLAGS, LDFLAGS64));
			
			// Generate manifiest(.nmf) file
			string NMF = string.Format(@"{0}/tools/create_nmf.py", NACL_SDK_ROOT);
			string NMF_ARGS = string.Format(@"-D {0}/x86_64-nacl/bin/objdump", TC_PATH);
			string NMF_PATHS = string.Format(@"-L {0}/x86_64-nacl/lib32 -L {0}/x86_64-nacl/lib -L {1}/lib32 -L {1}/lib", TC_PATH, NACL_MONO_ROOT);
			executeProgram("python", string.Format(@"{0} {1} -s . -o {2}.nmf {2}_x86_64.nexe {2}_x86_32.nexe {3}", NMF, NMF_ARGS, PROJECT, NMF_PATHS));
			
			// Copy and create other nacl files
			if (config.CompileTarget == MonoDevelop.Projects.CompileTarget.Exe)
			{
				// Create manifest.json
				if (GenerateManifest)
				{
					var manifest = new Manifest();
					manifest.name = AppName;
					manifest.description = AppDescription;
					manifest.version = AppVersion;
					
					manifest.app = new Manifest.App();
					manifest.app.launch = new Manifest.App.Launch();
					manifest.app.launch.local_path = AppLaunchHTML;
					
					if (RequiresGLES)
					{
						manifest.requirements = new Manifest.Requirements();
						manifest.requirements._3d = new Manifest.Requirements._3D();
						manifest.requirements._3d.features = new string[2];
						manifest.requirements._3d.features[0] = "css3d";
						manifest.requirements._3d.features[1] = "webgl";
					}
					
					var json = new DataContractJsonSerializer(typeof(Manifest));
					var memoryStream = new MemoryStream();
					json.WriteObject(memoryStream, manifest);
					
					memoryStream.Position = 0;
					var reader = new StreamReader(memoryStream);
					var value = reader.ReadToEnd();
					value = Regex.Replace(value, @"\,"+'"' + @"\w*" + '"' + @"\:null", "", RegexOptions.Singleline);
					value = Regex.Replace(value, '"' + @"\w*" + '"' + @"\:null", "", RegexOptions.Singleline);
					
					using (var stream = new FileStream(config.OutputDirectory+"/manifest.json", FileMode.Create, FileAccess.Write))
					using (var writer = new StreamWriter(stream))
					{
						writer.Write(value);
					}
				}
				
				// Copy Json and Html files
				if (CopyAllJsonObjects || CopyAllHtmlObjects)
				{
					foreach (var file in Files)
					{
						var srcFileInfo = new FileInfo(file.FilePath.FullPath);
						string dstDir = config.OutputDirectory+"/"+srcFileInfo.Name;
						var dstFileInfo = new FileInfo(dstDir);
						if (dstFileInfo.LastWriteTimeUtc != srcFileInfo.LastWriteTimeUtc)
						{
							string ext = file.FilePath.Extension.ToLower();
							if (CopyAllJsonObjects && ext == ".json")
							{
								srcFileInfo.CopyTo(dstDir, true);
							}
							
							if (CopyAllJsonObjects && ext == ".html")
							{
								srcFileInfo.CopyTo(dstDir, true);
							}
						}
					}
				}
			}
		}
		
		private void buildCS(BuildResult result, MonoNaClProjectConfiguration config, ConfigurationSelector configSel)
		{
			string NACL_MONO_ROOT = MonoNaClSettingsService.Instance.NACL_MONO_ROOT;
			var projProperties = ((CSharpCompilerParameters)config.CompilationParameters);
		
			// Get output name
			string OUTPUT_TARGET = config.CompileTarget == MonoDevelop.Projects.CompileTarget.Exe ? "exe" : "library";
			string CS_OUTPUT = config.AppFullName;
			string PROJECT = "main";//config.AppName;
			
			// Get C and CS sources
			string CS_SOURCES = "";
			foreach (var file in Files)
			{
				string fullPath = '"'+file.FilePath.FullPath+'"' + ' ';
				switch (file.FilePath.Extension.ToLower())
				{
					case (".cs"): CS_SOURCES += fullPath; break;
				}
				
				// Copy dependency files
				if (file.CopyToOutputDirectory == FileCopyMode.Always || file.CopyToOutputDirectory == FileCopyMode.PreserveNewest)
				{
					string virtualFileName = buildingConfig.OutputDirectory + file.ProjectVirtualPath.FullPath;
					var dstFileInfo = new FileInfo(virtualFileName);
					if (!dstFileInfo.Exists)
					{
						var srcFileInfo = new FileInfo(file.FilePath.FullPath);
						srcFileInfo.CopyTo(virtualFileName);
					}
					else
					{
						if (file.CopyToOutputDirectory == FileCopyMode.Always)
						{
							var srcFileInfo = new FileInfo(file.FilePath.FullPath);
							srcFileInfo.CopyTo(virtualFileName, true);
						}
						else
						{
							var srcFileInfo = new FileInfo(file.FilePath.FullPath);
							if (srcFileInfo.LastWriteTimeUtc != dstFileInfo.LastWriteTimeUtc)
							{
								srcFileInfo.CopyTo(virtualFileName, true);
							}
						}
					}
				} 
			}
			if (CS_SOURCES == "") return;
			
			// Get .dll sources
			string CS_DLL_SOURCES = "";
			var dlls = new List<string>();
			foreach (var reference in References)
			{
				var names = reference.GetReferencedFileNames(configSel);
				foreach (var name in names)
				{
					var srcLibFileInfo = new FileInfo(name);
					string filePath = name;
					if (reference.ReferenceType == ReferenceType.Package && reference.Package.Name == "mono")
					{
						string dstDirectory = string.Format("{0}/lib/mono/4.0/", NACL_MONO_ROOT);
						filePath = dstDirectory + srcLibFileInfo.Name;
					}
					
					CS_DLL_SOURCES += "-r:" + '"'+filePath+'"' + " ";
					dlls.Add(filePath);
					
					if (config.CompileTarget == MonoDevelop.Projects.CompileTarget.Exe)
					{
						string dstName = buildingConfig.OutputDirectory + "/" + srcLibFileInfo.Name;
						var dstLibFileInfo = new FileInfo(dstName);
						if (!dstLibFileInfo.Exists || srcLibFileInfo.LastWriteTimeUtc != dstLibFileInfo.LastWriteTimeUtc)
						{
							srcLibFileInfo.CopyTo(dstName, true);
						}
					}
				}
			}
			
			if (dlls.Count != 0 && config.CompileTarget == MonoDevelop.Projects.CompileTarget.Exe)
			{
				using (var stream = new FileStream(string.Format("{0}/{1}.dep", buildingConfig.OutputDirectory, PROJECT), FileMode.Create, FileAccess.Write))
				using (var writer = new StreamWriter(stream))
				{
					int i = 0;
					foreach (var dll in dlls)
					{
						var fileInfo = new FileInfo(dll);
						if (i == dlls.Count-1) writer.Write(fileInfo.Name);
						else writer.WriteLine(fileInfo.Name);
						++i;
					}
				}
			}
			
			// Get flags
			string flags = " -sdk:2";
			if (projProperties.UnsafeCode) flags += " -unsafe";
			if (projProperties.Optimize) flags += " -optimize";
			
			// Get compiler directives
			var symbols = projProperties.DefineSymbols;
			var symbolList = symbols.Split(';', ' ');
			symbols = "";
			foreach (var symbol in symbolList)
			{
				if (!string.IsNullOrEmpty(symbol)) symbols += "-define:" + symbol + " ";
			}
			
			// Compile dll's and .exe
			string GMCS = string.Format(@"{0}/lib/mono/4.0/mcs.exe", NACL_MONO_ROOT);
			executeProgram(GMCS, string.Format(@"{0}{1} -out:{2} {3}-target:{4}{5}", symbols, CS_SOURCES, CS_OUTPUT, CS_DLL_SOURCES, OUTPUT_TARGET, flags));//-pkg:gtk-sharp
			
			// Copy over mscorlib.dll
			if (config.CompileTarget == MonoDevelop.Projects.CompileTarget.Exe)
			{
				string mscorlibPath = buildingConfig.OutputDirectory + "/mscorlib.dll";
				var dstCoreFileInfo = new FileInfo(mscorlibPath);
				if (!dstCoreFileInfo.Exists)
				{
					var srcCoreFileInfo = new FileInfo(string.Format("{0}/lib/mono/4.0/mscorlib.dll", NACL_MONO_ROOT));
					srcCoreFileInfo.CopyTo(mscorlibPath);
				}
			}
		}

		public override bool SupportsFramework (MonoDevelop.Core.Assemblies.TargetFramework framework)
		{
			if (!framework.IsCompatibleWithFramework (MonoDevelop.Core.Assemblies.TargetFrameworkMoniker.NET_4_0)) return false;
			return base.SupportsFramework (framework);
		}

		protected override IList<string> GetCommonBuildActions ()
		{
			return new string[]
			{
				BuildAction.None,
				BuildAction.Compile,
				BuildAction.Content,
				BuildAction.EmbeddedResource,
				BuildAction.InterfaceDefinition,
			};
		}
	}
}