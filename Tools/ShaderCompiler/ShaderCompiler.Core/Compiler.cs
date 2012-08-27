using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace ShaderCompiler.Core
{
	public enum CompilerCodeSources
	{
		Memory,
		File,
		Project
	}

	public enum CompilerOutputs
	{
		D3D11,
		D3D9,
		XNA,
		GL3,
		GL2,
		GLES2
	}

	enum BaseCompilerOutputs
	{
		HLSL,
		GLSL
	}

	public partial class Compiler
	{
		#region Properties
		private List<CodeFile> codeFiles;
		private string inDirectory, inFile, outDirectory;
		private CompilerOutputs outputType;
		public string FileTag;
		private CompilerCodeSources codeSource;
		#endregion
		
		#region Constuctors
		public Compiler(string sourceCode, CompilerCodeSources codeSource)
		{
			this.codeSource = codeSource;
			codeFiles = new List<CodeFile>();
			
			if (codeSource == CompilerCodeSources.Memory)
			{
				codeFiles.Add(new CodeFile(sourceCode));
			}
			
			if (codeSource == CompilerCodeSources.File)
			{
				throw new NotImplementedException();
			}
			
			if (codeSource == CompilerCodeSources.Project)
			{
				var fileInfo = new FileInfo(sourceCode);
				inDirectory = fileInfo.Directory.FullName + '/';
				inFile = fileInfo.Name;
				initProject(sourceCode);
			}
		}
		
		private void initProject(string fileName)
		{
			using (var reader = new XmlTextReader(fileName))
			{
				while (reader.Read())
				{
					if (reader.Name == "Compile")
					{
						string fileReletivePath = reader.GetAttribute("Include");
						if (string.IsNullOrEmpty(fileReletivePath)) throw new Exception("Invalide Project cs file path.");

						using (var stream = new FileStream(inDirectory + fileReletivePath, FileMode.Open))
						{
							var streamReader = new StreamReader(stream);
							var codeBlock = new CodeFile(streamReader.ReadToEnd(), fileReletivePath);
							codeFiles.Add(codeBlock);
						}
					}
				}
			}
		}
		#endregion
		
		#region Methods
		public string CompileFromMemory(string code, CompilerOutputs outputType)
		{
			this.outputType = outputType;
			return compileFromMemory(code);
		}
		
		public void Compile(string outDirectory, CompilerOutputs outputType, bool compileMaterial)
		{
			this.outDirectory = outDirectory;
			this.outputType = outputType;
			
			if (!Directory.Exists(outDirectory))
			{
				Directory.CreateDirectory(outDirectory);
			}
			
			var materialCode = new List<string>();
			if (!compileMaterial) materialCode = null;
			compileLibrary(inDirectory + "bin/Debug/" + inFile.Split('.')[0] + ".dll", false, materialCode);
		}
		
		private string compileFromMemory(string code)
		{
			var codeProvider = new CSharpCodeProvider();
			var options = new CompilerParameters(new string[] {"System.dll", "ShaderCompiler.Core.dll"});
			options.GenerateExecutable = false;
			options.TreatWarningsAsErrors = false;
			//options.CompilerOptions = "/optimize";

			try
			{
				codeFiles[0].Code = code;
				var compilerResults = codeProvider.CompileAssemblyFromSource(options, new string[] {code});

				// Compiler Output
				foreach (var line in compilerResults.Output)
				{
				    Console.WriteLine(line);
				}

				// Compiler Errors
				string errors = null;
				foreach (var line in compilerResults.Errors)
				{
				    errors += line + Environment.NewLine;
				}
				if (compilerResults.Errors.Count != 0) throw new Exception(errors + Environment.NewLine + "CSharp Compiler Errors.");

				// Convert Reign Code
				code = compileLibrary(compilerResults.CompiledAssembly.Location, true, null);

				Console.WriteLine("Compile Success!");
			}
			catch(Exception e)
			{
				string message = "Compile Failer:" + Environment.NewLine + e.Message;
				Console.WriteLine(message);
				throw new Exception(message);
			}
			finally
			{
				codeProvider.Dispose();
				options.TempFiles.Delete();
			}
			
			return code;
		}
		
		private string compileLibrary(string dllFileName, bool writeToMemory, List<string> materialCode)
		{
string xnaEndTechniqueBlock =
@"
technique MainTechnique
{
	pass Pass0
	{
		VertexShader = compile vs_3_0 mainVS();
		PixelShader = compile ps_3_0 mainPS();
	}
}
";

			//var domain = AppDomain.CreateDomain("CompilerDomain");
			var assembly = Assembly.LoadFile(dllFileName);
			var objects = assembly.GetTypes();
			string lastShaderOut = null;
			var initLines = new List<string>();
			var names = dllFileName.Split('/', '\\', '.');
			string name = names[names.Length-2];
			foreach (var obj in objects)
			{
				var iFaces = obj.GetInterfaces();
				foreach (var iFace in iFaces)
				{
					if (iFace.Name == "ShaderI" && obj.IsSealed)
					{
						using (var stream = new MemoryStream())
						{
							using (var vsStream = new MemoryStream())
							{
								using (var psStream = new MemoryStream())
								{
									compileShader(obj, stream, vsStream, psStream, materialCode);
									if (materialCode != null)
									{
										
										string initLine;
										var codeFile = compileMaterial(name, obj, out initLine);
										materialCode.Add(codeFile);
										initLines.Add(initLine);
									}
									
									if (writeToMemory)
									{
										if (outputType != CompilerOutputs.XNA) lastShaderOut += "#GLOBAL" + Environment.NewLine;
										lastShaderOut += formatCode(stream.GetBuffer());
										if (outputType != CompilerOutputs.XNA) lastShaderOut += "#END" + Environment.NewLine;
										lastShaderOut += Environment.NewLine;
										
										if (outputType != CompilerOutputs.XNA) lastShaderOut += "#VS" + Environment.NewLine;
										lastShaderOut += formatCode(vsStream.GetBuffer());
										if (outputType != CompilerOutputs.XNA) lastShaderOut += "#END" + Environment.NewLine;
										lastShaderOut += Environment.NewLine;
										
										if (outputType != CompilerOutputs.XNA) lastShaderOut += "#PS" + Environment.NewLine;
										lastShaderOut += formatCode(psStream.GetBuffer());
										if (outputType != CompilerOutputs.XNA) lastShaderOut += "#END" + Environment.NewLine;
										
										if (outputType == CompilerOutputs.XNA)
										{
											lastShaderOut += xnaEndTechniqueBlock;
										}
									}
									else
									{
										string fileExt = ".rs";
										if (outputType == CompilerOutputs.XNA) fileExt = ".fx";
										using (var file = new FileStream(outDirectory + FileTag + obj.Name + fileExt, FileMode.Create, FileAccess.Write))
										{
											using (var writer = new StreamWriter(file))
											{
												if (outputType != CompilerOutputs.XNA) writer.WriteLine("#GLOBAL");
												writer.Write(formatCode(stream.GetBuffer()));
												if (outputType != CompilerOutputs.XNA) writer.WriteLine("#END");
												writer.WriteLine();
	
												if (outputType != CompilerOutputs.XNA) writer.WriteLine("#VS");
												writer.Write(formatCode(vsStream.GetBuffer()));
												if (outputType != CompilerOutputs.XNA) writer.WriteLine("#END");
												writer.WriteLine();
	
												if (outputType != CompilerOutputs.XNA) writer.WriteLine("#PS");
												writer.Write(formatCode(psStream.GetBuffer()));
												if (outputType != CompilerOutputs.XNA) writer.WriteLine("#END");
												if (outputType != CompilerOutputs.XNA) writer.WriteLine();
												
												if (outputType == CompilerOutputs.XNA)
												{
													writer.Write(xnaEndTechniqueBlock);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}

			if (materialCode != null)
			{
				var codeProvider = new CSharpCodeProvider();
				var options = new CompilerParameters(new string[] {"System.dll", "ShaderCompiler.Core.dll", "Reign.Core.dll", "Reign.Video.dll", "Reign.Video.API.dll"});
				options.GenerateExecutable = false;
				options.TreatWarningsAsErrors = false;
				options.OutputAssembly = outDirectory + name + ".dll";

				try
				{
					string compiledShaders =
					@"
						using System;
						using System.Collections.Generic;
						using Reign.Core;
						using Reign.Video;
						using Reign.Video.API;

						namespace ShaderMaterials.{0}
						{{
							public static class Materials
							{{
								public static List<Type> Types {{get; private set;}}

								public static void Init(VideoTypes apiType, DisposableI parent, string shaderFolerPath, string tag, ShaderVersions shaderVersion)
								{{
									// init shaders
									Types = new List<Type>();
									{1}
								}}
							}}
						}}
					";
					string initLineText = "";
					foreach (var initLine in initLines) initLineText += initLine;
					materialCode.Add(string.Format(compiledShaders, name, initLineText));
					var compilerResults = codeProvider.CompileAssemblyFromSource(options, materialCode.ToArray());

					// Compiler Output
					foreach (var line in compilerResults.Output)
					{
						Console.WriteLine(line);
					}

					// Compiler Errors
					string errors = null;
					foreach (var line in compilerResults.Errors)
					{
						errors += line + Environment.NewLine;
					}
					if (compilerResults.Errors.Count != 0) throw new Exception(errors + Environment.NewLine + "CSharp Material Compiler Errors.");

					Console.WriteLine("Compile Material Success!");
				}
				catch(Exception e)
				{
					string message = "Material Compile Failer:" + Environment.NewLine + e.Message;
					Console.WriteLine(message);
					throw new Exception(message);
				}
				finally
				{
					codeProvider.Dispose();
					options.TempFiles.Delete();
				}
			}
			
			return lastShaderOut;
		}
		
		private void compileShader(Type shader, Stream stream, Stream vsStream, Stream psStream, List<string> materialCode)
		{
			using (var writer = new StreamWriter(stream))
			{
				using (var vsWriter = new StreamWriter(vsStream))
				{
					using (var psWriter = new StreamWriter(psStream))
					{
						compileFields(shader, writer, vsWriter, psWriter);
						compileMethods(shader, writer, vsWriter, psWriter);
					}
				}
			}
		}

		private string compileMaterial(string shaderLibName, Type shader, out string initLine)
		{
			// get fields
			var fieldInfoList = new List<FieldInfo[]>();
			var baseShaderType = shader;
			while (baseShaderType.BaseType != null)
			{
				fieldInfoList.Add(baseShaderType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
				baseShaderType = baseShaderType.BaseType;
			}
			fieldInfoList.Reverse();
			var fields = new List<FieldInfo>();
			foreach (var fieldInfos in fieldInfoList)
			{
				foreach (var info in fieldInfos) fields.Add(info);
			}

			// create properties and method property body
			string materialProperties = null, materialInstanceProperties = null, applyMethodBody = null;
			foreach (var field in fields)
			{
				var attributes = field.GetCustomAttributes(true);
				foreach (var a in attributes)
				{
					if (a.GetType() == typeof(FieldUsage))
					{
						var m = (FieldUsage)a;

						if (field.FieldType == typeof(Vector2) || field.FieldType == typeof(Vector3) || field.FieldType == typeof(Vector4) ||
							field.FieldType == typeof(Matrix2) || field.FieldType == typeof(Matrix3) || field.FieldType == typeof(Matrix4) ||
							field.FieldType == typeof(Vector2[]) || field.FieldType == typeof(Vector3[]) || field.FieldType == typeof(Vector4[]) ||
							field.FieldType == typeof(Matrix2[]) || field.FieldType == typeof(Matrix3[]) || field.FieldType == typeof(Matrix4[]))
						{
							materialProperties += string.Format("public static ShaderVariableI {0};", field.Name);
						}
						else if (field.FieldType == typeof(Texture2D))
						{
							materialProperties += string.Format("public static ShaderResourceI {0};", field.Name);
						}
						else
						{
							throw new Exception("Unsuported field type.");
						}

						string fieldFormat = "[MaterialField(MaterialFieldTypes.{2})] public {0} {1};", methodValue = "gMATERIAL.{0}.Set({0});";
						if (field.FieldType == typeof(Vector2))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Vector2", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Vector3))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Vector3", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Vector4))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Vector4", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Matrix2))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Matrix2", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Matrix3))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Matrix3", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Matrix4))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Matrix4", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Texture2D))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Texture2DI", field.Name, m.MaterialType);
							applyMethodBody += string.Format(methodValue, field.Name);
						}

						fieldFormat = "private WeakReference {2}; public {0} {1} {{ get{{return {2}.Target;}} set{{{2} = new WeakReference(value);}} }}";
						if (field.FieldType == typeof(Vector2[]))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Vector2[]", field.Name, field.Name.ToLower());
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Vector3[]))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Vector3[]", field.Name, field.Name.ToLower());
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Vector4[]))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Vector4[]", field.Name, field.Name.ToLower());
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Matrix2[]))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Matrix2[]", field.Name, field.Name.ToLower());
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Matrix3[]))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Matrix3[]", field.Name, field.Name.ToLower());
							applyMethodBody += string.Format(methodValue, field.Name);
						}
						if (field.FieldType == typeof(Matrix4[]))
						{
							materialInstanceProperties += string.Format(fieldFormat, "Matrix4[]", field.Name, field.Name.ToLower());
							applyMethodBody += string.Format(methodValue, field.Name);
						}
					}
				}
			}

			// create class objects
			string shaderFile = 
			@"
				using Reign.Core;
				using Reign.Video;
				using A = Reign.Video.API;

				namespace ShaderMaterials.{0}
				{{
					public static class {1}Material
					{{
						// properties
						public static ShaderI Shader;
						{2}

						// constructors
						public static void Init(A.VideoTypes apiType, DisposableI parent, string shaderFolerPath, string tag, ShaderVersions shaderVersion)
						{{
							Shader = A.Shader.Create(apiType, parent, shaderFolerPath + tag + ""{1}.rs"", shaderVersion);
						}}
					}}

					public class {1}MaterialInstance
					{{
						// properties
						{3}

						// methods
						public void Apply()
						{{
							// set properties
							{4}
							{1}Material.Shader.Apply();
						}}
					}}
				}}
			";

			initLine = string.Format("Types.Add(typeof({0}Material)); ", shader.Name) + shader.Name + "Material.Init(apiType, parent, shaderFolerPath, tag, shaderVersion);";
			shaderFile = string.Format(shaderFile, shaderLibName, shader.Name, materialProperties, materialInstanceProperties, applyMethodBody);
			return shaderFile.Replace("gMATERIAL", shader.Name + "Material");
		}
		
		private string getCompilerIfBlockName()
		{
			switch (outputType)
			{
				case (CompilerOutputs.D3D11): return "D3D11";
				case (CompilerOutputs.D3D9): return "D3D9";
				case (CompilerOutputs.XNA): return "XNA";
				case (CompilerOutputs.GL3): return "GL3";
				case (CompilerOutputs.GL2): return "GL2";
				case (CompilerOutputs.GLES2): return "GLES2";
				default: throw new Exception("Unknown CompilerIfBlockType.");
			}
		}

		private string convertToBasicType(Type type, bool convertVectorTypes)
        {
			return convertToBasicType(type, outputType, convertVectorTypes);
        }

        internal static string convertToBasicType(Type type, CompilerOutputs outputType, bool convertVectorTypes)
		{
            return convertToBasicType(type.Name, outputType, convertVectorTypes);
		}

        private string convertToBasicType(string type, bool convertVectorTypes)
        {
			return convertToBasicType(type, outputType, convertVectorTypes);
        }

        internal static string convertToBasicType(string type, CompilerOutputs outputType, bool convertVectorTypes)
		{
			switch (type)
		    {
				case ("Void"): return "void";
		        case ("Int32"): return "int";
		        case ("UInt32"): return "uint";
				case ("Single"): return "float";
		        case ("Double"): return "float";
				case ("Boolean"): return "bool";
                case ("Vector2"): return convertVectorTypes ? Vector2.Output(outputType) : type;
				case ("Vector3"): return convertVectorTypes ? Vector3.Output(outputType) : type;
				case ("Vector4"): return convertVectorTypes ? Vector4.Output(outputType) : type;
				case ("Matrix3"): return convertVectorTypes ? Matrix3.Output(outputType) : type;
				case ("Matrix4"): return convertVectorTypes ? Matrix4.Output(outputType) : type;
				
				case ("Void[]"): return "void";
		        case ("Int32[]"): return "int";
		        case ("UInt32[]"): return "uint";
				case ("Single[]"): return "float";
		        case ("Double[]"): return "float";
				case ("Boolean[]"): return "bool";
				case ("Vector2[]"): return convertVectorTypes ? Vector2.Output(outputType) : type;
				case ("Vector3[]"): return convertVectorTypes ? Vector3.Output(outputType) : type;
				case ("Vector4[]"): return convertVectorTypes ? Vector4.Output(outputType) : type;
				case ("Matrix3[]"): return convertVectorTypes ? Matrix3.Output(outputType) : type;
				case ("Matrix4[]"): return convertVectorTypes ? Matrix4.Output(outputType) : type;
				case ("Texture2D"): return convertVectorTypes ? Texture2D.Output(outputType) : type;
		        default: return type;
		    }
		}
		
		private BaseCompilerOutputs getBaseCompilerOutput()
		{
			return getBaseCompilerOutput(outputType);
		}

		internal static BaseCompilerOutputs getBaseCompilerOutput(CompilerOutputs outputType)
		{
			switch (outputType)
			{
				case (CompilerOutputs.D3D11): return BaseCompilerOutputs.HLSL;
				case (CompilerOutputs.D3D9): return BaseCompilerOutputs.HLSL;
				case (CompilerOutputs.XNA): return BaseCompilerOutputs.HLSL;
				case (CompilerOutputs.GL3): return BaseCompilerOutputs.GLSL;
				case (CompilerOutputs.GL2): return BaseCompilerOutputs.GLSL;
				case (CompilerOutputs.GLES2): return BaseCompilerOutputs.GLSL;
				default: throw new Exception("Unknown BaseCompilerType.");
			}
		}
		
		private string formatCode(byte[] code)
		{
			// Remove tabs and \r
			string formatedCode = "";
			char lastChar = ' ';
			for (int i = 0; i != code.Length; ++i)
			{
				char c = (char)code[i];
				if (c != '\t')
				{
					if (c == '\n' && lastChar != '\r')
					{
						if (codeSource != CompilerCodeSources.Memory) formatedCode += "\r\n";
						else formatedCode += "\r";
					}
					else formatedCode += c;
				}
				
				lastChar = c;
			}
			
			// Remove spaces
			string formatedCode2 = "";
			bool copy = false;
			foreach (char c in formatedCode)
			{
				if (c != ' ') copy = true;
				if (c == '\n' || c == '\r')
				{
					formatedCode2 += c;
					copy = false;
				}

				if (copy) formatedCode2 += c;
			}
			
			// Add tabs between '{}' brackets
			formatedCode = "";
			int tabCount = 0;
			lastChar = ' ';
			foreach (char c in formatedCode2)
			{
				//if (c == '{') ++tabCount;
				if (c == '}') --tabCount;
				if (tabCount < 0) throw new Exception("Failed to format code.");

				bool copyTabs = false;
				if ((lastChar == '\n' || lastChar == '\r') && (c != '\n' && c != '\r'))// && c != '{')
				{
					copyTabs = true;
				}

				if (copyTabs && tabCount != 0)
				{
					string tabs = "";
					for (int i = 0; i != tabCount; ++i)
					{
						tabs += '\t';
					}
					formatedCode += tabs;
				}

				formatedCode += c;
				lastChar = c;
				
				if (c == '{') ++tabCount;
			}

			// Remove data bleed
			formatedCode2 = "";
			for (int i = formatedCode.Length-1; i != -1; --i)
			{
				char c = formatedCode[i];
				if (c == '}' || c == ';')
				{
					formatedCode2 = formatedCode.Substring(0, i+1);
					formatedCode2 += Environment.NewLine;
					break;
				}
			}
			
			return formatedCode2;
		}
		#endregion
	}
}

