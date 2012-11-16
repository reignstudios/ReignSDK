using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace ShaderCompiler.Core
{
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
		#endregion
		
		#region Constuctors
		public Compiler(string projFileName)
		{
			codeFiles = new List<CodeFile>();
	
			var fileInfo = new FileInfo(projFileName);
			inDirectory = fileInfo.Directory.FullName + '/';
			inFile = fileInfo.Name;
			using (var reader = XmlReader.Create(projFileName))
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
		public void Compile(string outDirectory, CompilerOutputs outputType, bool compileMaterial, bool compileMetroShaders)
		{
			this.outDirectory = outDirectory;
			this.outputType = outputType;

			// compile shader library
			/*var exeDir = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0", "MSBuildToolsPath", "") as string;
			if (string.IsNullOrEmpty(exeDir)) throw new Exception("Cant find MSBuild for .NET 4.");

			var process = new System.Diagnostics.Process();
			process.StartInfo.FileName = exeDir + "MSBuild.exe";
			process.StartInfo.Arguments = string.Format("{0} /property:Configuration=Debug", inDirectory + inFile);
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.Start();
			process.WaitForExit();*/
			
			// make sure output directory exists
			if (!Directory.Exists(outDirectory))
			{
				Directory.CreateDirectory(outDirectory);
			}
			
			compileLibrary(inDirectory + "bin/Debug/" + inFile.Split('.')[0] + ".dll", false, compileMaterial, compileMetroShaders);
		}

		class MetroShaderCompiler : Reign.Video.ShaderI
		{
			public MetroShaderCompiler(string fileName)
			: base(null)
			{
				var code = getShaders(fileName);

				IntPtr vsBuffer;
				int vsBufferSize;
				var error = Reign_Video_D3D11_Component.ShaderModelCom.Compile(code[0], code[0].Length, "vs_4_0_level_9_3", out vsBuffer, out vsBufferSize);
				if (error != null) throw new Exception("Failed to compile Metro VS shader: " + error);

				IntPtr psBuffer;
				int psBufferSize;
				error = Reign_Video_D3D11_Component.ShaderModelCom.Compile(code[1], code[1].Length, "ps_4_0_level_9_3", out psBuffer, out psBufferSize);
				if (error != null) throw new Exception("Failed to compile Metro PS shader: " + error);

				var data = new byte[vsBufferSize + psBufferSize];
				System.Runtime.InteropServices.Marshal.Copy(vsBuffer, data, 0, vsBufferSize);
				System.Runtime.InteropServices.Marshal.Copy(psBuffer, data, vsBufferSize, psBufferSize);
				using (var file = new FileStream(Reign.Core.Streams.StripFileExt(fileName) + ".mrs", FileMode.Create, FileAccess.Write))
				using (var writer = new BinaryWriter(file))
				{
					writer.Write(vsBufferSize);
					writer.Write(psBufferSize);
					file.Write(data, 0, data.Length);
				}
			}

			public override void Apply()
			{
				throw new NotImplementedException();
			}

			public override Reign.Video.ShaderVariableI Variable(string name)
			{
				throw new NotImplementedException();
			}

			public override Reign.Video.ShaderResourceI Resource(string name)
			{
				throw new NotImplementedException();
			}
		}
		
		private string compileLibrary(string dllFileName, bool writeToMemory, bool compileMaterial, bool compileMetroShaders)
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
						using (var vsStream = new MemoryStream())
						using (var psStream = new MemoryStream())
						using (var reflectionStream = new MemoryStream())
						{
							if (outputType == CompilerOutputs.D3D11 && compileMetroShaders) compileShader(obj, stream, vsStream, psStream, reflectionStream);
							else compileShader(obj, stream, vsStream, psStream, null);
							
							string outDirectoryRelitive = outDirectory;	
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
								string relitivePath = "";
								foreach (var codeFile in codeFiles)
								{
									if (codeFile.IsFileOfShader(obj))
									{
										relitivePath = Reign.Core.Streams.GetFileDirectory(codeFile.ReletivePath);
										if (!Directory.Exists(outDirectory + relitivePath)) Directory.CreateDirectory(outDirectory + relitivePath);
										break;
									}
								}
								outDirectoryRelitive += relitivePath;

								string fileExt = ".rs";
								if (outputType == CompilerOutputs.XNA) fileExt = ".fx";
								using (var file = new FileStream(outDirectoryRelitive + FileTag + obj.Name + fileExt, FileMode.Create, FileAccess.Write))
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

							// compile metro shader bytecode
							if (outputType == CompilerOutputs.D3D11 && compileMetroShaders)
							{
								new MetroShaderCompiler(outDirectoryRelitive + FileTag + obj.Name + ".rs");
							}

							// compile materials
							if (compileMaterial)
							{
								var codeText = compileMaterialFiles(name, obj);
								using (var codeFile = new FileStream(outDirectoryRelitive + obj.Name + ".cs", FileMode.Create, FileAccess.Write))
								using (var writer = new StreamWriter(codeFile))
								{
									writer.Write(codeText);
								}
							}

							// compile metro shaders
							if (outputType == CompilerOutputs.D3D11 && compileMetroShaders)
							{
								using (var reflectionFile = new FileStream(outDirectoryRelitive + FileTag + obj.Name + ".ref", FileMode.Create, FileAccess.Write))
								{
									var data = reflectionStream.ToArray();
									reflectionFile.Write(data, 0, data.Length);
								}
							}
						}
					}
				}
			}
			
			return lastShaderOut;
		}
		
		private void compileShader(Type shader, Stream stream, Stream vsStream, Stream psStream, Stream reflectionStream)
		{
			using (var writer = new StreamWriter(stream))
			using (var vsWriter = new StreamWriter(vsStream))
			using (var psWriter = new StreamWriter(psStream))
			using (var reflectionWriter = new StreamWriter(reflectionStream == null ? new MemoryStream() : reflectionStream))
			{
				compileFields(shader, writer, vsWriter, psWriter, reflectionStream == null ? null : reflectionWriter);
				compileMethods(shader, writer, vsWriter, psWriter);
			}
		}

		private string compileMaterialFiles(string shaderLibName, Type shader)
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
			string constantProperties = null, constantTypeProperties = null, applyGlobalMethodBody = null, applyInstanceMethodBody = null, applyInstancingMethodBody = null, elementsBody = null, constantInitBody = null;
			int floatOffset = 0;
			foreach (var field in fields)
			{
				var attributes = field.GetCustomAttributes(true);
				foreach (var a in attributes)
				{
					if (a.GetType() == typeof(FieldUsage))
					{
						var usage = (FieldUsage)a;

						if (field.FieldType == typeof(double) ||
							field.FieldType == typeof(Vector2) || field.FieldType == typeof(Vector3) || field.FieldType == typeof(Vector4) ||
							field.FieldType == typeof(Matrix2) || field.FieldType == typeof(Matrix3) || field.FieldType == typeof(Matrix4) ||
							field.FieldType == typeof(Vector2[]) || field.FieldType == typeof(Vector3[]) || field.FieldType == typeof(Vector4[]) ||
							field.FieldType == typeof(Matrix2[]) || field.FieldType == typeof(Matrix3[]) || field.FieldType == typeof(Matrix4[]))
						{
							constantProperties += string.Format("public static ShaderVariableI {0}Constant {{get; private set;}}", field.Name);
							constantInitBody += string.Format(@"{0}Constant = Shader.Variable(""{0}"");", field.Name);
						}
						else if (field.FieldType == typeof(Texture2D))
						{
							constantProperties += string.Format("public static ShaderResourceI {0}Constant {{get; private set;}}", field.Name);
							constantInitBody += string.Format(@"{0}Constant = Shader.Resource(""{0}"");", field.Name);
						}
						else
						{
							throw new Exception("Unsuported field type.");
						}

						string globalFieldFormat = "[MaterialField(MaterialFieldUsages.{2})] public static {0} {1};";
						string fieldFormat = "[MaterialField(MaterialFieldUsages.{2})] public {0} {1};";
						string methodValue = "{0}Constant.Set({0});";
						if (field.FieldType == typeof(double))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "float", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						if (field.FieldType == typeof(Vector2))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Vector2", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Vector3))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Vector3", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Vector4))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Vector4", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Matrix2))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Matrix2", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Matrix3))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Matrix3", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Matrix4))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Matrix4", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Texture2D))
						{
							createConstantType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Texture2DI", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}

						globalFieldFormat = "private static WeakReference {2}; [MaterialField(MaterialFieldUsages.{3})] public static {0} {1} {{ get{{return ({0}){2}.Target;}} set{{{2} = new WeakReference(value);}} }}";
						fieldFormat = "private WeakReference {2}; [MaterialField(MaterialFieldUsages.{3})] public {0} {1} {{ get{{return ({0}){2}.Target;}} set{{{2} = new WeakReference(value);}} }}";
						if (field.FieldType == typeof(double[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "float[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						if (field.FieldType == typeof(Vector2[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Vector2[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Vector3[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Vector3[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Vector4[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Vector4[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Matrix2[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Matrix2[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Matrix3[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Matrix3[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
						else if (field.FieldType == typeof(Matrix4[]))
						{
							createConstantArrayType(usage, field, methodValue, globalFieldFormat, fieldFormat, "Matrix4[]", ref constantTypeProperties, ref applyGlobalMethodBody, ref applyInstanceMethodBody, ref applyInstancingMethodBody);
						}
					}
					else if (a.GetType() == typeof(VSInput))
					{
						var input = (VSInput)a;
						
						string usage = null;
						int streamIndex = 0;
						switch (input.Type)
						{
							case (VSInputTypes.Position): usage = "BufferLayoutElementUsages.Position"; break;
							case (VSInputTypes.UV): usage = "BufferLayoutElementUsages.UV"; break;
							case (VSInputTypes.Color): usage = "BufferLayoutElementUsages.Color"; break;
							case (VSInputTypes.Normal): usage = "BufferLayoutElementUsages.Normal"; break;
							case (VSInputTypes.Index): usage = "BufferLayoutElementUsages.Index"; streamIndex = 1; break;
							case (VSInputTypes.IndexClassic): usage = "BufferLayoutElementUsages.IndexClassic"; break;
							default: throw new Exception("Unsuported material usage type.");
						}
						
						string type = null;
						int offset = 0;
						if (field.FieldType == typeof(double) || field.FieldType == typeof(uint)) {type = "BufferLayoutElementTypes.Float"; offset = 1;}
						else if (field.FieldType == typeof(Vector4) && input.Type == VSInputTypes.Color) {type = "BufferLayoutElementTypes.RGBAx8"; offset = 1;}
						else if (field.FieldType == typeof(Vector2)) {type = "BufferLayoutElementTypes.Vector2"; offset = 2;}
						else if (field.FieldType == typeof(Vector3)) {type = "BufferLayoutElementTypes.Vector3"; offset = 3;}
						else if (field.FieldType == typeof(Vector4)) {type = "BufferLayoutElementTypes.Vector4"; offset = 4;}
						else throw new Exception("Unsuported material element type.");

						elementsBody += string.Format("elements.Add(new BufferLayoutElement({0}, {1}, {2}, {3}, {4}));", type, usage, streamIndex, input.Index, floatOffset);
						floatOffset += offset;
					}
				}
			}

			// create class objects
			string shaderFile = 
@"
using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using A = Reign.Video.API;

namespace ShaderMaterials.{0}
{{
	class {1}MaterialStreamLoader : StreamLoaderI
	{{
		private A.VideoTypes videoType;
		private DisposableI parent;
		private string contentPath;
		private string tag;
		private ShaderVersions shaderVersion;

		public {1}MaterialStreamLoader(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{{
			this.videoType = videoType;
			this.parent = parent;
			this.contentPath = contentPath;
			this.tag = tag;
			this.shaderVersion = shaderVersion;
		}}

		#if METRO
		public override async System.Threading.Tasks.Task<bool> Load() {{
		#else
		public override bool Load() {{
		#endif
			return {1}Material.load(videoType, parent, contentPath, tag, shaderVersion);
		}}
	}}

	public class {1}Material : MaterialI
	{{
		#region Static Properties
		public static bool Loaded {{get; private set;}}
		public static ShaderI Shader {{get; private set;}}
		public static BufferLayoutDescI BufferLayoutDesc {{get; private set;}}
		public static BufferLayoutI BufferLayout {{get; private set;}}
		{2}
		#endregion

		#region Instance Properties
		public string Name {{get; set;}}
		public delegate void ApplyCallbackMethod({1}Material material, MeshI mesh);
		public static ApplyCallbackMethod ApplyGlobalConstantsCallback, ApplyInstanceConstantsCallback, ApplyInstancingConstantsCallback;
		{3}
		#endregion

		#region Constructors
		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + ""{1}.rs"", shaderVersion);
			new {1}MaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			{7}
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}}

		public static void Init(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality)
		{{
			Shader = A.Shader.Create(videoType, parent, contentPath + tag + ""{1}.rs"", shaderVersion, vsQuality, psQuality);
			new {1}MaterialStreamLoader(videoType, parent, contentPath, tag, shaderVersion);
			var elements = new List<BufferLayoutElement>();
			{7}
			BufferLayoutDesc = A.BufferLayoutDesc.Create(videoType, elements);
		}}

		internal static bool load(A.VideoTypes videoType, DisposableI parent, string contentPath, string tag, ShaderVersions shaderVersion)
		{{
			if (!Shader.Loaded)
			{{
				return false;
			}}
			{8}

			BufferLayout = A.BufferLayout.Create(videoType, Shader, Shader, BufferLayoutDesc);

			Loaded = true;
			return true;
		}}
		#endregion

		#region Methods
		public void Enable()
		{{
			BufferLayout.Enable();
		}}

		public void ApplyGlobalContants(MeshI mesh)
		{{
			if (ApplyGlobalConstantsCallback != null) ApplyGlobalConstantsCallback(this, mesh);
			{4}
		}}

		public void ApplyInstanceContants(MeshI mesh)
		{{
			if (ApplyInstanceConstantsCallback != null) ApplyInstanceConstantsCallback(this, mesh);
			{5}
		}}

		public void ApplyInstancingContants(MeshI mesh)
		{{
			if (ApplyInstancingConstantsCallback != null) ApplyInstancingConstantsCallback(this, mesh);
			{6}
		}}

		public void Apply(MeshI mesh)
		{{
			ApplyGlobalContants(mesh);
			ApplyInstanceContants(mesh);
			ApplyInstancingContants(mesh);
			Shader.Apply();
		}}

		public static void ApplyGlobalContants()
		{{
			{4}
		}}

		public void ApplyInstanceContants()
		{{
			{5}
		}}

		public static void ApplyInstancingContants()
		{{
			{6}
		}}

		public void Apply()
		{{
			ApplyGlobalContants();
			ApplyInstanceContants();
			ApplyInstancingContants();
			Shader.Apply();
		}}
		#endregion
	}}
}}
";

			return string.Format(shaderFile, shaderLibName, shader.Name, constantProperties, constantTypeProperties, applyGlobalMethodBody, applyInstanceMethodBody, applyInstancingMethodBody, elementsBody, constantInitBody);
		}

		private void createConstantType(FieldUsage usage, FieldInfo field, string methodValue, string globalFieldFormat, string fieldFormat, string constantType, ref string constantTypeProperties, ref string applyGlobalMethodBody, ref string applyInstanceMethodBody, ref string applyInstancingMethodBody)
		{
			if (usage.MaterialUsages == MaterialUsages.Global)
			{
				constantTypeProperties += string.Format(globalFieldFormat, constantType, field.Name, usage.MaterialUsages);
				applyGlobalMethodBody += string.Format(methodValue, field.Name);
			}
			else if (usage.MaterialUsages == MaterialUsages.Instance)
			{
				constantTypeProperties += string.Format(fieldFormat, constantType, field.Name, usage.MaterialUsages);
				applyInstanceMethodBody += string.Format(methodValue, field.Name);
			}
			else if (usage.MaterialUsages == MaterialUsages.Instancing)
			{
				constantTypeProperties += string.Format(globalFieldFormat, constantType, field.Name, usage.MaterialUsages);
				applyInstancingMethodBody += string.Format(methodValue, field.Name);
			}
			else
			{
				throw new Exception("Unsuported MaterialUsages: " + usage.MaterialUsages.ToString());
			}
		}

		private void createConstantArrayType(FieldUsage usage, FieldInfo field, string methodValue, string globalFieldFormat, string fieldFormat, string constantType, ref string constantTypeProperties, ref string applyGlobalMethodBody, ref string applyInstanceMethodBody, ref string applyInstancingMethodBody)
		{
			if (usage.MaterialUsages == MaterialUsages.Global)
			{
				constantTypeProperties += string.Format(globalFieldFormat, constantType, field.Name, field.Name.ToLower(), usage.MaterialUsages);
				applyGlobalMethodBody += string.Format(methodValue, field.Name);
			}
			else if (usage.MaterialUsages == MaterialUsages.Instance)
			{
				constantTypeProperties += string.Format(fieldFormat, constantType, field.Name, field.Name.ToLower(), usage.MaterialUsages);
				applyInstanceMethodBody += string.Format(methodValue, field.Name);
			}
			else if (usage.MaterialUsages == MaterialUsages.Instancing)
			{
				constantTypeProperties += string.Format(globalFieldFormat, constantType, field.Name, field.Name.ToLower(), usage.MaterialUsages);
				applyInstancingMethodBody += string.Format(methodValue, field.Name);
			}
			else
			{
				throw new Exception("Unsuported MaterialUsages: " + usage.MaterialUsages.ToString());
			}
		}
		
		private string[] getCompilerIfBlockNames()
		{
			switch (outputType)
			{
				case (CompilerOutputs.D3D11): return new string[]{"D3D", "D3D11"};
				case (CompilerOutputs.D3D9): return new string[]{"D3D", "D3D9"};
				case (CompilerOutputs.XNA): return new string[]{"D3D", "XNA"};
				case (CompilerOutputs.GL3): return new string[]{"GL", "GL3"};
				case (CompilerOutputs.GL2): return new string[]{"GL", "GL2"};
				case (CompilerOutputs.GLES2): return new string[]{"GL", "GLES2"};
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
					if (c == '\n' && lastChar != '\r') formatedCode += "\r\n";
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

