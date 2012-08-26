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
		internal static CompilerOutputs outputType;
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
				//var parser = new CSharpParser();
				while (reader.Read())
				{
					if (reader.Name == "Compile")
					{
						string fileReletivePath = reader.GetAttribute("Include");
						if (string.IsNullOrEmpty(fileReletivePath)) throw new Exception("Invalide Project cs file path.");

						using (var stream = new FileStream(inDirectory + fileReletivePath, FileMode.Open))
						{
							var streamReader = new StreamReader(stream);
							var codeBlock = new CodeFile(streamReader.ReadToEnd(), fileReletivePath);//, parser);
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
			Compiler.outputType = outputType;
			return compileFromMemory(code);
		}
		
		public void Compile(string outDirectory, CompilerOutputs outputType)
		{
			this.outDirectory = outDirectory;
			Compiler.outputType = outputType;
			
			if (!Directory.Exists(outDirectory))
			{
				Directory.CreateDirectory(outDirectory);
			}
			
			compileLibrary(inDirectory + "bin/Debug/" + extractFileString(inFile, '.') + ".dll", false);
		}
		
		private static string extractFileString(string input, params char[] delimiter)
		{
			int foundIndex = 0;
			for (int i = input.Length-1; i != -1; --i)
			{
				bool pass = false;
				foreach (char c in delimiter)
				{
					if (input[i] == c)
					{
						pass = true;
						break;
					}
				}

				if (pass)
				{
					foundIndex = i;
					break;
				}
			}

			string output = "";
			for (int i = 0; i != foundIndex; ++i)
			{
				output += input[i];
			}

			return output;
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
				code = compileLibrary(compilerResults.CompiledAssembly.Location, true);

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
		
		private string compileLibrary(string dllFileName, bool writeToMemory)
		{
string xnaEndCode =
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
									compileShader(obj, stream, vsStream, psStream);
									
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
											lastShaderOut += xnaEndCode;
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
													writer.Write(xnaEndCode);
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
			
			return lastShaderOut;
		}
		
		private void compileShader(Type shader, Stream stream, Stream vsStream, Stream psStream)
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
			//return System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Convert(System.Text.Encoding.ASCII, System.Text.Encoding.ASCII, code));

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

