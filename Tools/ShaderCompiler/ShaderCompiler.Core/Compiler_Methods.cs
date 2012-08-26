using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;

namespace ShaderCompiler.Core
{
	public partial class Compiler
	{
		enum MethodTypes
		{
			Normal,
			VS,
			PS
		}
		
		private MethodTypes getMethodType(MethodInfo method)
		{
			var attributes = method.GetCustomAttributes(typeof(ShaderMethod), true);
			if (attributes.Length == 1)
			{
				var attribute = (ShaderMethod)attributes[0];
				switch (attribute.Type)
				{
				    case (ShaderMethodTypes.VS): return MethodTypes.VS;
				    case (ShaderMethodTypes.PS): return MethodTypes.PS;
				}
			}
			
			return MethodTypes.Normal;
		}
	
		private void compileMethods(Type shader, StreamWriter stream, StreamWriter vsStream, StreamWriter psStream)
		{
			var methods = shader.GetMethods();
			
			// Seperate VS-PS and Normal method types
			var mainMethods = new List<MethodInfo>();
			var normalMethods = new List<MethodInfo>();
			foreach (var method in methods)
			{
				var methodType = getMethodType(method);
				if (methodType == MethodTypes.Normal)
				{
					normalMethods.Add(method);
				}
				else
				{
					if (method.GetParameters().Length != 0) throw new Exception("VS and PS methods cannot have parameters.");
					if (method.ReturnType != typeof(void)) throw new Exception("VS and PS methods must return void.");
					mainMethods.Add(method);
				}
			}
			
			// Object base method type
			Func<string, bool> isBaseMethodName = delegate(string name)
			{
				return (name == "ToString" || name == "Equals" || name == "GetHashCode" || name == "GetType");
			};
			
			// Process Normal methods
			foreach (var method in normalMethods)
			{
				if (isBaseMethodName(method.Name)) continue;
				compileMethodExt(shader, stream, method, MethodTypes.Normal);
			}
			
			// Process VS-PS methods
			foreach (var method in mainMethods)
			{
				if (isBaseMethodName(method.Name)) continue;
				var methodType = getMethodType(method);
				compileMethodExt(shader, methodType == MethodTypes.VS ? vsStream : psStream, method, methodType);
			}
		}
		
		private void compileMethodExt(Type shader, StreamWriter stream, MethodInfo method, MethodTypes methodType)
		{
			var parameters = method.GetParameters();
			var parameterTypes = new Type[parameters.Length];
			for (int i = 0; i != parameters.Length; ++i)
			{
				parameterTypes[i] = parameters[i].ParameterType;
			}
			
			var baseType = getBaseCompilerOutput();
			bool foundMethod = false;
			foreach (var codeFile in codeFiles)
			{
				string codeBlock = codeFile.FindMethodBlock(shader, method);

				if (codeBlock != null)
				{
					if (methodType == MethodTypes.Normal)
					{
						string methodParameters = null;
						for (int i = 0; i != parameters.Length; ++i)
						{
                            methodParameters += convertToBasicType(parameterTypes[i], true) + ' ' + parameters[i].Name;
							if (i != parameters.Length-1) methodParameters += ", ";
						}

                        stream.WriteLine(string.Format("{0} {1}({2})", convertToBasicType(method.ReturnType, true), method.Name, methodParameters));
					}
					else
					{
						if (baseType == BaseCompilerOutputs.GLSL)
						{
							stream.WriteLine("void main()");
						}
						
						if (baseType == BaseCompilerOutputs.HLSL)
						{
							if (methodType == MethodTypes.VS)
							{
								string mainName = "main";
								if (outputType == CompilerOutputs.XNA) mainName = "mainVS";
								stream.WriteLine(string.Format("VSOutPSIn {0}(VSIn In)", mainName));
								codeBlock = insertCodeAtMethodStart(ref codeBlock, "VSOutPSIn Out;");
								codeBlock = insertCodeAtMethodEnd(ref codeBlock, "return Out;");
							}
							
							if (methodType == MethodTypes.PS)
							{
								string mainName = "main";
								if (outputType == CompilerOutputs.XNA) mainName = "mainPS";
								stream.WriteLine(string.Format("PSOut {0}(VSOutPSIn In)", mainName));
								codeBlock = insertCodeAtMethodStart(ref codeBlock, "PSOut Out;");
								codeBlock = insertCodeAtMethodEnd(ref codeBlock, "return Out;");
							}
						}
					}

					// Method Block
					stream.WriteLine(compileMethodBlock(shader, codeBlock, methodType));
					stream.WriteLine();

					foundMethod = true;
					break;
				}
			}

			if (!foundMethod)
			{
				throw new Exception(string.Format("Shader {0} Method {1} was not found in any cs file.", shader.Name, method.Name));
			}
		}
		
		private string insertCodeAtMethodStart(ref string code, string codeToInsert)
		{
			int index = 0;
			for (int i = 0; i != code.Length; ++i)
			{
				if (code[i] == '{')
				{
					index = i + 1;
					break;
				}
			}
			
			return code.Insert(index, Environment.NewLine + codeToInsert + Environment.NewLine);
		}
		
		private string insertCodeAtMethodEnd(ref string code, string codeToInsert)
		{
			int index = 0;
			for (int i = code.Length-1; i != -1; --i)
			{
				if (code[i] == '}')
				{
					index = i;
					break;
				}
			}
			
			return code.Insert(index, Environment.NewLine + codeToInsert + Environment.NewLine);
		}
		
		private string compileMethodBlock(Type shader, string methodBlock, MethodTypes methodType)
		{
			var baseType = getBaseCompilerOutput();
			
			// Remove compiler #if blocks
			{
				string blockName = getCompilerIfBlockName();
				methodBlock = Regex.Replace(methodBlock, @"#if.*\b"+blockName+@"\b.*", "");
				
				string[] names = new string[]
				{
					"D3D10",
					"D3D9",
					"XNA",
					"GL3",
					"GL2",
					"GLES2",
				};
				foreach (var name in names)
				{
					if (name != blockName)
					{
						methodBlock = Regex.Replace(methodBlock, @"#if.*\b"+name+@"\b"+@".*?#endif", "", RegexOptions.Singleline);
					}
				}
				
				methodBlock = Regex.Replace(methodBlock, @"#endif", "");
			}
			
			// Convert basics
			{
				string find = @"\s*=\s*new.*\[(\w*)]\s*;";
				var match = Regex.Match(methodBlock, find);
				methodBlock = Regex.Replace(methodBlock, find, string.Format("[{0}];", match.Groups[1].Value));
			}
			
			if (baseType == BaseCompilerOutputs.GLSL)
			{
				string regStart = @"\s*=\s*new\s*";
				string regEnd = @"\s*\(\s*\)\s*;";
				var findTypes = new string[]
				{
					"Vector2", "(0, 0);",
					"Vector3", "(0, 0, 0);",
					"Vector4", "(0, 0, 0, 0);",
					"Matrix3", "(0);",
					"Matrix4", "(0);",
				};
				
				for (int i = 0; i != findTypes.Length; i += 2)
				{
					string find = regStart + findTypes[i] + regEnd;
					var match = Regex.Match(methodBlock, find);
					methodBlock = Regex.Replace(methodBlock, find, " = " + findTypes[i] + findTypes[i+1]);
				}
			}
			else
			{
				methodBlock = Regex.Replace(methodBlock, @"\s*=\s*new.*\(\s*\)\s*;", " = 0;");
			}
			
			methodBlock = Regex.Replace(methodBlock, @"\[\s*\]", "");
			methodBlock = Utility.RemoveKeyword(methodBlock, "new");
			methodBlock = Utility.ReplaceKeyword(methodBlock, "double", "float");
		
			// Convert vector types
			methodBlock = Regex.Replace(methodBlock, @"\s*Vector2\(\s*\)", @"Vector2(0, 0)");
			methodBlock = Regex.Replace(methodBlock, @"\s*Vector3\(\s*\)", @"Vector3(0, 0, 0)");
			methodBlock = Regex.Replace(methodBlock, @"\s*Vector4\(\s*\)", @"Vector4(0, 0, 0, 0)");
            methodBlock = Utility.ReplaceKeyword(methodBlock, "Vector2", convertToBasicType("Vector2", true));
            methodBlock = Utility.ReplaceKeyword(methodBlock, "Vector3", convertToBasicType("Vector3", true));
            methodBlock = Utility.ReplaceKeyword(methodBlock, "Vector4", convertToBasicType("Vector4", true));
            methodBlock = Utility.ReplaceKeyword(methodBlock, "Matrix3", convertToBasicType("Matrix3", true));
            methodBlock = Utility.ReplaceKeyword(methodBlock, "Matrix4", convertToBasicType("Matrix4", true));
			
			// Convert SL types
			methodBlock = Utility.ReplaceKeyword(methodBlock, "SL.Distance", "distance");
			methodBlock = Utility.ReplaceKeyword(methodBlock, "SL.Normalize", "normalize");
			methodBlock = Utility.ReplaceKeyword(methodBlock, "SL.Cross", "cross");
			methodBlock = Utility.ReplaceKeyword(methodBlock, "SL.Sin", "sin");
			methodBlock = Utility.ReplaceKeyword(methodBlock, "SL.Cos", "cos");
			
			if (baseType == BaseCompilerOutputs.HLSL)
			{
				methodBlock = Utility.ReplaceKeyword(methodBlock, "SL.Clip", "clip");
			}
			
			if (baseType == BaseCompilerOutputs.GLSL)
			{
				string find = @"SL.Clip\s*\((.*?;)";
				var match = Regex.Match(methodBlock, find, RegexOptions.Singleline);
				if (match.Success)
				{
					string newVal = match.Groups[1].Value;
					newVal = Regex.Replace(newVal, @"\)\s*;", "", RegexOptions.Singleline);
					methodBlock = Regex.Replace(methodBlock, find, string.Format("if ({0} < 0.0) discard;", newVal));
				}
			}

			// Convert matrix multiply types
			bool replaceMatrix = true;
			while (replaceMatrix)
			{
				replaceMatrix = replaceMatrixMultiplyTypes(ref methodBlock);
			}
			
			// Convert field usage types
			int textureIndex = 0;
			var fields = shader.GetFields();
			foreach (var field in fields)
			{
				// Convert sampler types
				if (field.FieldType == typeof(Texture2D))
				{
					if (outputType == CompilerOutputs.D3D11)
					{
						string find = field.Name + @"\.Sample\s*\(";
						string replace = string.Format("{0}.Sample(Samplers[{1}], ", field.Name, textureIndex);
						methodBlock = Regex.Replace(methodBlock, find, replace);
						++textureIndex;
					}
					else
					{
						string samplerName = "tex2D";
						if (baseType == BaseCompilerOutputs.GLSL) samplerName = "texture2D";
						
						string find = field.Name + @"\.Sample\s*\(";
						string replace = string.Format("{0}({1}, ", samplerName, field.Name + (outputType == CompilerOutputs.XNA ? "_S" : ""));
						methodBlock = Regex.Replace(methodBlock, find, replace);
					}
				}
				
				// Convert InOut types
				if (methodType != MethodTypes.Normal)
				{
					// VS In
					if (methodType == MethodTypes.VS)
					{
						var a = field.GetCustomAttributes(typeof(VSInput), true);
						if (a.Length != 0)
						{
							if (baseType == BaseCompilerOutputs.HLSL)
							{
								methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, "In." + field.Name);
							}

							if (baseType == BaseCompilerOutputs.GLSL)
							{
								string fieldType = getGLVSInputFieldName(field);
								if ((((VSInput)a[0]).Type == VSInputTypes.IndexClassic || outputType == CompilerOutputs.GL2 || outputType == CompilerOutputs.GLES2) && (field.FieldType == typeof(uint) || field.FieldType == typeof(int)))
								{
									fieldType = string.Format("int({0})", fieldType);
								}
								methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, fieldType);
							}
						}
					}
					
					// VS In PS Out
					if (field.GetCustomAttributes(typeof(VSOutputPSInput), true).Length != 0)
					{
						if (methodType == MethodTypes.VS)
						{
							if (baseType == BaseCompilerOutputs.HLSL)
							{
								methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, "Out." + field.Name);
							}
							
							if (baseType == BaseCompilerOutputs.GLSL)
							{
								var a = (VSOutputPSInput)field.GetCustomAttributes(true)[0];
								if (a.Type == VSOutputPSInputTypes.Position)
								{
									methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, "gl_Position = " + field.Name);
								}
							}
						}
						
						if (methodType == MethodTypes.PS)
						{
							if (baseType == BaseCompilerOutputs.HLSL)
							{
								methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, "In." + field.Name);
							}
						}
					}
					
					// PS Out
					if (methodType == MethodTypes.PS)
					{
						if (baseType == BaseCompilerOutputs.HLSL && field.GetCustomAttributes(typeof(PSOutput), true).Length != 0)
						{
							methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, "Out." + field.Name);
						}
					
						if (baseType == BaseCompilerOutputs.GLSL)
						{
							foreach (var a in field.GetCustomAttributes(typeof(PSOutput), true))
							{
								var psOut = ((PSOutput)a);
								if (psOut.Type == PSOutputTypes.Color)
								{
									if (outputType == CompilerOutputs.GL3)
									{
										methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, string.Format("{0}[{1}]", "glFragColorOut", psOut.Index));
									}
									else if (outputType == CompilerOutputs.GL2 || outputType == CompilerOutputs.GLES2)
									{
										methodBlock = Utility.ReplaceKeyword(methodBlock, field.Name, string.Format("gl_FragData[{0}]", psOut.Index));
									}
								}
							}
						}
					}
				}
			}

			return methodBlock;
		}

		private bool replaceMatrixMultiplyTypes(ref string methodBlock)
		{
			var baseType = getBaseCompilerOutput();
		
			// Find matrix multiply block
			string findParameters = @".*?;";
			var match = Regex.Match(methodBlock, @"=\s*([\w\[\]]*)\.Multiply" + findParameters, RegexOptions.Singleline);
			
			// If matrix multiply block exists, replace it
			var matrixGroups = match.Groups;
			if (match.Success && matrixGroups.Count == 2)
			{
				// Find vector parameter
				match = Regex.Match(matrixGroups[0].Value, @"\((.*)\)", RegexOptions.Singleline);

				// Replace matrix multiply block
				var vectorGroups = match.Groups;
				if (match.Success && vectorGroups.Count == 2)
				{
					string matrixName = matrixGroups[1].Value;
					string vectorName = vectorGroups[1].Value;
					string formatString = "= mul({0}, {1});";
					if (outputType == CompilerOutputs.D3D9 || outputType == CompilerOutputs.XNA)
					{
						formatString = "= mul({1}, {0});";
					}
					else if (baseType == BaseCompilerOutputs.GLSL)
					{
						formatString = @"= {0} * {1};";
					}
					string replace = string.Format(formatString, vectorName, matrixName);
					matrixName = matrixName.Replace("[", @"\[");
					matrixName = matrixName.Replace("]", @"\]");
					methodBlock = Regex.Replace(methodBlock, @"=\s*" + matrixName + @"\.Multiply" + findParameters, replace, RegexOptions.Singleline);

					return true;
				}
			}

			return false;
		}
	}
}

