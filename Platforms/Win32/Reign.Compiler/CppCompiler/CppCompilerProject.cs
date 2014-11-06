using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Reign.Compiler
{
	namespace XML
	{
		public class ClInclude
		{
			[XmlAttribute("Include")] public string Include;
		}
		
		public class ClCompile
		{
			[XmlAttribute("Include")] public string Include;
			[XmlElement("PrecompiledHeader")] public string PrecompiledHeader;
			[XmlElement("WarningLevel")] public string WarningLevel;
			[XmlElement("Optimization")] public string Optimization;
			[XmlElement("PreprocessorDefinitions")] public string PreprocessorDefinitions;
			[XmlElement("FunctionLevelLinking")] public string FunctionLevelLinking;
			[XmlElement("IntrinsicFunctions")] public string IntrinsicFunctions;
		}

		public class Link
		{
			[XmlElement("SubSystem")] public string SubSystem;
			[XmlElement("GenerateDebugInformation")] public string GenerateDebugInformation;
			[XmlElement("EnableCOMDATFolding")] public string EnableCOMDATFolding;
			[XmlElement("OptimizeReferences")] public string OptimizeReferences;
		}

		public class ItemDefinitionGroup
		{
			[XmlAttribute("Condition")] public string Condition;
			[XmlElement("ClCompile")] public ClCompile ClCompile;
			[XmlElement("Link")] public Link Link;
		}

		public class Import
		{
			[XmlAttribute("Project")] public string Project;
			[XmlAttribute("Condition")] public string Condition;
			[XmlAttribute("Label")] public string Label;
		}

		public class PropertyGroup
		{
			[XmlAttribute("Label")] public string Label;
			[XmlAttribute("Condition")] public string Condition;
			[XmlElement("ProjectGuid")] public string ProjectGuid;
			[XmlElement("Keyword")] public string Keyword;
			[XmlElement("RootNamespace")] public string RootNamespace;
			[XmlElement("LinkIncremental")] public string LinkIncremental;
			[XmlElement("OutDir")] public string OutDir;
			[XmlElement("IntDir")] public string IntDir;
			[XmlElement("TargetName")] public string TargetName;
			[XmlElement("TargetExt")] public string TargetExt;
			[XmlElement("ExtensionsToDeleteOnClean")] public string ExtensionsToDeleteOnClean;
			[XmlElement("EnableManagedIncrementalBuild")] public string EnableManagedIncrementalBuild;
			[XmlElement("IncludePath")] public string IncludePath;
			[XmlElement("ConfigurationType")] public string ConfigurationType;
			[XmlElement("UseDebugLibraries")] public string UseDebugLibraries;
			[XmlElement("PlatformToolset")] public string PlatformToolset;
			[XmlElement("CharacterSet")] public string CharacterSet;
			[XmlElement("UseOfMfc")] public string UseOfMfc;
			[XmlElement("CLRSupport")] public string CLRSupport;
			[XmlElement("WholeProgramOptimization")] public string WholeProgramOptimization;
			[XmlElement("WindowsAppContainer")] public string WindowsAppContainer;
		}

		public class ProjectConfiguration
		{
			[XmlAttribute("Include")] public string Include;
			[XmlElement("Configuration")] public string Configuration;
			[XmlElement("Platform")] public string Platform;
		}

		public class Text
		{
			[XmlAttribute("Include")] public string Include;
		}

		public class ItemGroup
		{
			[XmlAttribute("Label")] public string Label;
			[XmlAttribute("Condition")] public string Condition;
			[XmlElement("ProjectConfiguration")] public List<ProjectConfiguration> ProjectConfigurations;
			[XmlElement("Text")] public Text Text;
			[XmlElement("ClInclude")] public List<ClInclude> ClIncludes;
			[XmlElement("ClCompile")] public List<ClCompile> ClCompiles;
		}

		public class ImportGroup
		{
			[XmlAttribute("Label")] public string Label;
			[XmlAttribute("Condition")] public string Condition;
			[XmlElement("Import")] public Import Import;
		}

		[XmlRoot("Project")]
		public class Project
		{
			// root
			[XmlAttribute("DefaultTargets")] public string DefaultTargets = "Build";
			[XmlAttribute("ToolsVersion")] public string ToolsVersion = "12.0";

			// ProjectConfigurations
			[XmlElement("ProjectConfigurations_TEMP")] public ItemGroup ProjectConfigurations;// ItemGroup

			// Globals
			[XmlElement("Globals_TEMP")] public PropertyGroup Globals;// PropertyGroup

			// Configuration
			[XmlElement("ConfigurationsImport_TEMP")] public Import ConfigurationsImport;// Import
			[XmlElement("Configurations_TEMP")] public List<PropertyGroup> Configurations;// PropertyGroup

			// ExtensionSettings
			[XmlElement("ExtensionSettingsImport_TEMP")] public Import ExtensionSettingsImport;// Import
			[XmlElement("ExtensionSettings_TEMP")] public ImportGroup ExtensionSettings;// ImportGroup

			// PropertySheets
			[XmlElement("PropertySheets_TEMP")] public List<ImportGroup> PropertySheets;// ImportGroup

			// UserMacros
			[XmlElement("UserMacros_TEMP")] public PropertyGroup UserMacros;// PropertyGroup

			// PropertyGroup
			[XmlElement("PropertyGroups_TEMP")] public List<PropertyGroup> PropertyGroups;// PropertyGroup

			// ItemDefinitionGroup
			[XmlElement("ItemDefinitionGroups_TEMP")] public List<ItemDefinitionGroup> ItemDefinitionGroups;// ItemDefinitionGroup

			// ItemGroup
			[XmlElement("ItemGroups_TEMP")] public List<ItemGroup> ItemGroups;// ItemGroup

			// ExtensionTargets
			[XmlElement("ExtensionTargetsImport_TEMP")] public Import ExtensionTargetsImport;// Import
			[XmlElement("ExtensionTargets_TEMP")] public ImportGroup ExtensionTargets;// ImportGroup
		}
	}

	public class CppCompilerProject : CompilerProject
	{
		public Guid ID;

		public CppCompilerProject()
		{
			ID = Guid.NewGuid();
		}

		public override void Compile(string outputDirectory)
		{
			if (!compiler.outputProjectFiles)
			{
				// do nothing...
			}
			else if (compiler.outputType == CompilerOutputTypes.Cpp_VC)
			{
				// generate xml >>>
				var projXML = new XML.Project();
				
				// <<< ProjectConfigurations
				projXML.ProjectConfigurations = new XML.ItemGroup()
				{
					Label = "ProjectConfigurations",
					ProjectConfigurations = new List<XML.ProjectConfiguration>()
					{
						new XML.ProjectConfiguration()
						{
							Include = "Debug|Win32",
							Configuration = "Debug",
							Platform = "Win32"
						},
						new XML.ProjectConfiguration()
						{
							Include = "Release|Win32",
							Configuration = "Release",
							Platform = "Win32"
						}
					}
				};

				// <<< globals
				projXML.Globals = new XML.PropertyGroup()
				{
					Label = "Globals",
					ProjectGuid = "{"+ID+"}",
					Keyword = "Win32Proj",
					RootNamespace = "Reign"
				};

				// <<< Configurations
				projXML.ConfigurationsImport = new XML.Import() {Project = @"$(VCTargetsPath)\Microsoft.Cpp.Default.props"};
				projXML.Configurations = new List<XML.PropertyGroup>();
				projXML.Configurations.Add
				(
					new XML.PropertyGroup()
					{
						Label = "Configuration",
						Condition = @"'$(Configuration)|$(Platform)'=='Debug|Win32'",
						ConfigurationType = CompilerProjectType == CompilerProjectTypes.EXE ? "Application" : "StaticLibrary",//"DynamicLibrary",
						UseDebugLibraries = "true",
						PlatformToolset = "v120",
						CharacterSet = "Unicode",
						//UseOfMfc = "false",
						CLRSupport = "false",
						WholeProgramOptimization = "false",
						WindowsAppContainer = "false"
					}
				);

				projXML.Configurations.Add
				(
					new XML.PropertyGroup()
					{
						Label = "Configuration",
						Condition = @"'$(Configuration)|$(Platform)'=='Release|Win32'",
						ConfigurationType = CompilerProjectType == CompilerProjectTypes.EXE ? "Application" : "StaticLibrary",//"DynamicLibrary",
						UseDebugLibraries = "false",
						PlatformToolset = "v120",
						CharacterSet = "Unicode",
						//UseOfMfc = "false",
						CLRSupport = "false",
						WholeProgramOptimization = "true",
						WindowsAppContainer = "false"
					}
				);

				// <<< ExtensionSettings
				projXML.ExtensionSettingsImport = new XML.Import() {Project = @"$(VCTargetsPath)\Microsoft.Cpp.props"};
				projXML.ExtensionSettings = new XML.ImportGroup() {Label = "ExtensionSettings"};

				// <<< PropertySheets
				projXML.PropertySheets = new List<XML.ImportGroup>()
				{
					new XML.ImportGroup()
					{
						Label = "PropertySheets",
						Condition = @"'$(Configuration)|$(Platform)'=='Debug|Win32'",
						Import = new XML.Import()
						{
							Project = @"$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props",
							Condition = @"exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')",
							Label = "LocalAppDataPlatform"
						}
					},
					new XML.ImportGroup()
					{
						Label = "PropertySheets",
						Condition = @"'$(Configuration)|$(Platform)'=='Release|Win32'",
						Import = new XML.Import()
						{
							Project = @"$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props",
							Condition = @"exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')",
							Label = "LocalAppDataPlatform"
						}
					}
				};

				// <<< UserMacros
				projXML.UserMacros = new XML.PropertyGroup() {Label = "UserMacros"};

				// <<< PropertyGroups
				projXML.PropertyGroups = new List<XML.PropertyGroup>()
				{
					new XML.PropertyGroup()
					{
						Condition = @"'$(Configuration)|$(Platform)'=='Debug|Win32'",
						LinkIncremental = "true",
						//OutDir = @"$(SolutionDir)$(Configuration)\",
						//IntDir = @"$(Configuration)\",
						//TargetName = @"$(ProjectName)",
						TargetExt = CompilerProjectType == CompilerProjectTypes.EXE ? ".exe" : ".lib",
						//ExtensionsToDeleteOnClean = "*.cdf;*.cache;*.obj;*.ilk;*.resources;*.tlb;*.tli;*.tlh;*.tmp;*.rsp;*.pgc;*.pgd;*.meta;*.tlog;*.manifest;*.res;*.pch;*.exp;*.idb;*.rep;*.xdc;*.pdb;*_manifest.rc;*.bsc;*.sbr;*.xml;*.metagen;*.bi",
						EnableManagedIncrementalBuild = "false",
						IncludePath = compiler.reignCppSorces + ";$(IncludePath)"
					},
					new XML.PropertyGroup()
					{
						Condition = @"'$(Configuration)|$(Platform)'=='Release|Win32'",
						LinkIncremental = "false",
						//OutDir = @"$(SolutionDir)$(Configuration)\",
						//IntDir = @"$(Configuration)\",
						//TargetName = @"$(ProjectName)",
						TargetExt = CompilerProjectType == CompilerProjectTypes.EXE ? ".exe" : ".lib",
						//ExtensionsToDeleteOnClean = "*.cdf;*.cache;*.obj;*.ilk;*.resources;*.tlb;*.tli;*.tlh;*.tmp;*.rsp;*.pgc;*.pgd;*.meta;*.tlog;*.manifest;*.res;*.pch;*.exp;*.idb;*.rep;*.xdc;*.pdb;*_manifest.rc;*.bsc;*.sbr;*.xml;*.metagen;*.bi",
						EnableManagedIncrementalBuild = "false",
						IncludePath = compiler.reignCppSorces + ";$(IncludePath)"
					}
				};

				// <<< ItemDefinitionGroup
				projXML.ItemDefinitionGroups = new List<XML.ItemDefinitionGroup>()
				{
					new XML.ItemDefinitionGroup()
					{
						Condition = @"'$(Configuration)|$(Platform)'=='Debug|Win32'",
						ClCompile = new XML.ClCompile()
						{
							WarningLevel = "Level3",
							Optimization = "Disabled",
							PreprocessorDefinitions = CompilerProjectType == CompilerProjectTypes.EXE ? "WIN32;_DEBUG;_CONSOLE;_LIB;%(PreprocessorDefinitions)" : "WIN32;_DEBUG;_WINDOWS;_USRDLL;TESTCPPDLL_EXPORTS;%(PreprocessorDefinitions)"
						},
						Link = new XML.Link()
						{
							SubSystem = CompilerProjectType == CompilerProjectTypes.EXE ? "Console" : "Windows",
							GenerateDebugInformation = "true"
						}
					},
					new XML.ItemDefinitionGroup()
					{
						Condition = @"'$(Configuration)|$(Platform)'=='Release|Win32'",
						ClCompile = new XML.ClCompile()
						{
							WarningLevel = "Level3",
							Optimization = "MaxSpeed",
							PreprocessorDefinitions = CompilerProjectType == CompilerProjectTypes.EXE ? "WIN32;NDEBUG;_CONSOLE;_LIB;%(PreprocessorDefinitions)" : "WIN32;NDEBUG;_WINDOWS;_USRDLL;TESTCPPDLL_EXPORTS;%(PreprocessorDefinitions)",
							FunctionLevelLinking = "true",
							IntrinsicFunctions = "true"
						},
						Link = new XML.Link()
						{
							SubSystem = CompilerProjectType == CompilerProjectTypes.EXE ? "Console" : "Windows",
							GenerateDebugInformation = "true",
							EnableCOMDATFolding = "true",
							OptimizeReferences = "true"
						}
					}
				};

				// <<< ItemGroups (aka file references)
				projXML.ItemGroups = new List<XML.ItemGroup>();
				var group = new XML.ItemGroup();
				group.ClIncludes = new List<XML.ClInclude>();
				foreach (var file in CodeFiles)
				{
					group.ClIncludes.Add
					(
						new XML.ClInclude()
						{
							Include = string.Format("{0}.h", file.Name)
						}
					);
				}
				projXML.ItemGroups.Add(group);

				group = new XML.ItemGroup();
				group.ClIncludes = new List<XML.ClInclude>();
				foreach (var file in CodeFiles)
				{
					group.ClIncludes.Add
					(
						new XML.ClInclude()
						{
							Include = string.Format("{0}.cpp", file.Name)
						}
					);
				}
				projXML.ItemGroups.Add(group);

				// <<< reign cpp sources
				group = new XML.ItemGroup();
				group.ClIncludes = new List<XML.ClInclude>();
				foreach (var file in ReignCppSourceFiles)
				{
					group.ClIncludes.Add
					(
						new XML.ClInclude()
						{
							Include = compiler.reignCppSorces + string.Format("/{0}.h", file)
						}
					);
				}
				projXML.ItemGroups.Add(group);

				group = new XML.ItemGroup();
				group.ClCompiles = new List<XML.ClCompile>();
				foreach (var file in ReignCppSourceFiles)
				{
					group.ClCompiles.Add
					(
						new XML.ClCompile()
						{
							Include = compiler.reignCppSorces + string.Format("/{0}.cpp", file)
						}
					);
				}
				projXML.ItemGroups.Add(group);

				// <<< ExtensionTargets
				projXML.ExtensionTargetsImport = new XML.Import() {Project = @"$(VCTargetsPath)\Microsoft.Cpp.targets"};
				projXML.ExtensionTargets = new XML.ImportGroup() {Label = "ExtensionTargets"};

				// save proj file
				string root = outputDirectory + project.Name;
				if (!Directory.Exists(root)) Directory.CreateDirectory(root);
				using (var stream = new FileStream(outputDirectory + string.Format("{0}/{0}.vcxproj", Name), FileMode.Create, FileAccess.Write, FileShare.None))
				using (var writer = new StreamWriter(stream))
				{
					using (var unformatedStream = new MemoryStream())
					using (var reader = new StreamReader(unformatedStream))
					{
						var serializer = new XmlSerializer(typeof(XML.Project), "http://schemas.microsoft.com/developer/msbuild/2003");
						serializer.Serialize(unformatedStream, projXML);
						unformatedStream.Position = 0;
						string xml = reader.ReadToEnd();
						xml = xml.Replace("ProjectConfigurations_TEMP", "ItemGroup");
						xml = xml.Replace("Globals_TEMP", "PropertyGroup");

						xml = xml.Replace("ConfigurationsImport_TEMP", "Import");
						xml = xml.Replace("Configurations_TEMP", "PropertyGroup");

						xml = xml.Replace("ExtensionSettingsImport_TEMP", "Import");
						xml = xml.Replace("ExtensionSettings_TEMP", "ImportGroup");

						xml = xml.Replace("PropertySheets_TEMP", "ImportGroup");

						xml = xml.Replace("UserMacros_TEMP", "PropertyGroup");

						xml = xml.Replace("PropertyGroups_TEMP", "PropertyGroup");

						xml = xml.Replace("ItemDefinitionGroups_TEMP", "ItemDefinitionGroup");

						xml = xml.Replace("ItemGroups_TEMP", "ItemGroup");

						xml = xml.Replace("ExtensionTargetsImport_TEMP", "Import");
						xml = xml.Replace("ExtensionTargets_TEMP", "ImportGroup");
						writer.Write(xml);
					}
				}
			}
			else if (compiler.outputType == CompilerOutputTypes.Cpp_GCC)
			{
				// TODO: output make file
			}

			base.Compile(outputDirectory);
		}
	}
}
