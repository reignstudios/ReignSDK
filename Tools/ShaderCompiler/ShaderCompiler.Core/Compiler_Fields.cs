using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace ShaderCompiler.Core
{
	public partial class Compiler
	{
		private void compileFields(Type shader, StreamWriter stream, StreamWriter vsStream, StreamWriter psStream, StreamWriter reflectionStream)
		{
			// Get field types
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
			
			var vsInFields = new List<FieldInfo>();
			var vsInPSOutFields = new List<FieldInfo>();
			var psOutFields = new List<FieldInfo>();
			var normalGlobalFields = new List<FieldInfo>();
			var normalVSFields = new List<FieldInfo>();
			var normalPSFields = new List<FieldInfo>();
			foreach (var field in fields)
			{
				bool isField = false, isVSField = false, isPSField = false, isVSIn = false, isVSOutPSIn = false, isPSOut = false;
				var attributes = field.GetCustomAttributes(true);
				if (attributes.Length == 0) throw new Exception("All fields must have an attribute.");
				foreach (var a in attributes)
				{
					var t = a.GetType();
					if (t == typeof(ArrayType)) isField = true;
					if (t == typeof(FieldUsage))
					{
						isField = true;
						var aValue = (FieldUsage)a;
						if (aValue.Type == FieldUsageTypes.VS)
						{
							isVSField = true;
						}
						else if (aValue.Type == FieldUsageTypes.VS_PS)
						{
							isVSField = true;
							isPSField = true;
						}
						else if (aValue.Type == FieldUsageTypes.PS)
						{
							isPSField = true;
						}
					}
					if (t == typeof(VSInput)) isVSIn = true;
					if (t == typeof(VSOutputPSInput)) isVSOutPSIn = true;
					if (t == typeof(PSOutput)) isPSOut = true;
				}

				if (isField)
				{
					if (isVSField && isPSField) normalGlobalFields.Add(field);
					else if (isVSField) normalVSFields.Add(field);
					else if (isPSField) normalPSFields.Add(field);
				}
				else
				{
					if (attributes.Length >= 2)
					{
						throw new Exception("VS and PS In/Out Fields can only have one shader attribute.");
					}

					if (isVSIn)
					{
						vsInFields.Add(field);
					}
					else if (isVSOutPSIn)
					{
						vsInPSOutFields.Add(field);
					}
					else if (isPSOut)
					{
						psOutFields.Add(field);
					}
				}
			}

			// write reflection file
			if (reflectionStream != null)
			{
				int variableByteOffset = 0, resourceIndex = 0;
				writeReflectionFile(reflectionStream, normalGlobalFields, ref variableByteOffset, ref resourceIndex, null);
				int variableByteOffset2 = variableByteOffset, resourceIndex2 = resourceIndex;
				writeReflectionFile(reflectionStream, normalVSFields, ref variableByteOffset2, ref resourceIndex2, true);
				writeReflectionFile(reflectionStream, normalPSFields, ref variableByteOffset, ref resourceIndex, false);
			}

			// Process VSInput fields
			var baseType = getBaseCompilerOutput();
			if (baseType == BaseCompilerOutputs.HLSL)
			{
				vsStream.WriteLine("struct VSIn" + Environment.NewLine + '{');
				foreach (var field in vsInFields)
				{
					var a = (VSInput)field.GetCustomAttributes(true)[0];
					var fieldType = convertToBasicType(field.FieldType, true);
					string attributeType = "POSITION";
					if (a.Type == VSInputTypes.Color) attributeType = "COLOR";
					if (a.Type == VSInputTypes.UV) attributeType = "TEXCOORD";
					if (a.Type == VSInputTypes.Normal) attributeType = "NORMAL";
					if (a.Type == VSInputTypes.Index || a.Type == VSInputTypes.IndexClassic)
					{
						if (field.FieldType != typeof(uint)) throw new Exception("VS Index type must be uint.");
						if (a.Type == VSInputTypes.IndexClassic || outputType == CompilerOutputs.D3D9 || outputType == CompilerOutputs.XNA)
						{
							attributeType = "BLENDINDICES";
							fieldType = "float";
						}
						else
						{
							attributeType = "SV_InstanceID";
						}
					}
					vsStream.WriteLine(string.Format("{0} {1} : {2}{3};", fieldType, field.Name, attributeType, a.Index));
				}
				vsStream.WriteLine("};");
			}

			if (baseType == BaseCompilerOutputs.GLSL)
			{
				foreach (var field in vsInFields)
				{
					var a = (VSInput)field.GetCustomAttributes(true)[0];
					string fieldName = getGLVSInputFieldName(field);
                    string fieldType = convertToBasicType(field.FieldType, true);
					if (a.Type == VSInputTypes.Index || a.Type == VSInputTypes.IndexClassic)
					{
						if (field.FieldType != typeof(uint)) throw new Exception("VS Index type must be uint.");
						if (a.Type == VSInputTypes.IndexClassic || outputType == CompilerOutputs.GL2) fieldType = "float";
						else if (fieldName == "gl_InstanceID") continue;
					}
					vsStream.WriteLine(string.Format("attribute {0} {1};", fieldType, fieldName));
				}
			}
			vsStream.WriteLine();

			// Process VSOutPSIn fields
			if (baseType == BaseCompilerOutputs.HLSL)
			{
				stream.WriteLine("struct VSOutPSIn" + Environment.NewLine + '{');
				foreach (var field in vsInPSOutFields)
				{
					var a = (VSOutputPSInput)field.GetCustomAttributes(true)[0];
					if (a.Type == VSOutputPSInputTypes.Position && field.FieldType != typeof(Vector4))
					{
						throw new Exception("VS Position ouput must be a Vector4.");
					}
					string attributeType = "SV_POSITION";
					if (a.Type == VSOutputPSInputTypes.InOut) attributeType = "TEXCOORD";
                    stream.WriteLine(string.Format("{0} {1} : {2}{3};", convertToBasicType(field.FieldType, true), field.Name, attributeType, a.Index));
				}
				stream.WriteLine("};");
			}

			if (baseType == BaseCompilerOutputs.GLSL)
			{
				foreach (var field in vsInPSOutFields)
				{
					var a = (VSOutputPSInput)field.GetCustomAttributes(true)[0];
                    string fieldType = convertToBasicType(field.FieldType, true);
					if (a.Type == VSOutputPSInputTypes.Position && field.FieldType != typeof(Vector4))
					{
						throw new Exception("VS Position ouput must be a Vector4.");
					}
					if (outputType == CompilerOutputs.GL2 || outputType == CompilerOutputs.GLES2)
					{
						stream.WriteLine(string.Format("varying {0} {1};", fieldType, field.Name));
					}
					else
					{
						vsStream.WriteLine(string.Format("out {0} {1};", fieldType, field.Name));
						psStream.WriteLine(string.Format("in {0} {1};", fieldType, field.Name));
					}
				}
			}
			if (outputType == CompilerOutputs.GL2 || outputType == CompilerOutputs.GLES2)
			{
				stream.WriteLine();
			}
			else
			{
				vsStream.WriteLine();
				psStream.WriteLine();
			}

			// Process PSOut fields
			if (baseType == BaseCompilerOutputs.HLSL)
			{
				psStream.WriteLine("struct PSOut" + Environment.NewLine + '{');
				foreach (var field in psOutFields)
				{
					var a = (PSOutput)field.GetCustomAttributes(true)[0];
					string attributeType = "SV_TARGET";
                    psStream.WriteLine(string.Format("{0} {1} : {2}{3};", convertToBasicType(field.FieldType, true), field.Name, attributeType, a.Index));
				}
				psStream.WriteLine("};");
				psStream.WriteLine();
			}

			if (outputType == CompilerOutputs.GL3)
			{
				psStream.WriteLine(string.Format("out vec4 {0}[{1}];", "glFragColorOut", psOutFields.Count));
				psStream.WriteLine();
			}

			// Process normal fields
			processNormalFields(stream, normalGlobalFields, baseType);
			processNormalFields(vsStream, normalVSFields, baseType);
			processNormalFields(psStream, normalPSFields, baseType);
		}

		private void writeReflectionFile(StreamWriter reflectionStream, List<FieldInfo> normalFields, ref int variableByteOffset, ref int resourceIndex, bool? vsConstansts)
		{
			string label = null;
			if (vsConstansts == null) label = "g";
			else if (vsConstansts == true) label = "vs";
			else label = "ps";
			foreach (var field in normalFields)
			{
				if (field.FieldType == typeof(Texture2D))
				{
					reflectionStream.WriteLine(string.Format("{2}Res {0} {1}", field.Name, resourceIndex, label));
					++resourceIndex;
				}
				else
				{
					reflectionStream.WriteLine(string.Format("{2}Var {0} {1}", field.Name, variableByteOffset, label));
					if (field.FieldType == typeof(Vector2) || field.FieldType == typeof(Vector3) || field.FieldType == typeof(Vector4)) variableByteOffset += sizeof(float) * 4;
					if (field.FieldType == typeof(Matrix2)) variableByteOffset += sizeof(float) * 4 * 2;
					if (field.FieldType == typeof(Matrix3)) variableByteOffset += sizeof(float) * 4 * 3;
					if (field.FieldType == typeof(Matrix4)) variableByteOffset += sizeof(float) * 4 * 4;
				}
			}
		}

		private void processNormalFields(StreamWriter stream, List<FieldInfo> normalFields, BaseCompilerOutputs baseType)
		{
			if (outputType == CompilerOutputs.D3D11)
			{
				int fieldCount = 0;
				foreach (var field in normalFields)
				{
					Type filedType = field.FieldType;
					if (filedType == typeof(Texture2D))
					{
						++fieldCount;
					}
				}

				if (fieldCount != 0)
				{
					stream.WriteLine(string.Format("{0} {1}[{2}];", "sampler", "Samplers", fieldCount));
				}
			}

			int samplerFieldCount = 0;
			foreach (var field in normalFields)
			{
				string fieldType = convertToBasicType(field.FieldType, true);
				if (baseType == BaseCompilerOutputs.GLSL) stream.Write("uniform ");

				int arrayLength = -1;
				if (field.FieldType.IsArray && field.GetCustomAttributes(typeof(ArrayType), true).Length != 0)
				{
					var attributes = field.GetCustomAttributes(typeof(ArrayType), true)[0] as ArrayType;
					if (attributes != null)
					{
						arrayLength = attributes.Length;
					}
				}

				stream.WriteLine(string.Format("{0} {1};", fieldType, field.Name + ((arrayLength == -1) ? "" : "[" + arrayLength.ToString() + "]")));

				if (outputType == CompilerOutputs.XNA && field.FieldType == typeof(Texture2D))
				{
					stream.Write(string.Format("sampler2D {0}_S : register(s{1}) = sampler_state", field.Name, samplerFieldCount) + " {");
					stream.WriteLine(string.Format("Texture = <{0}>;", field.Name) + "};");
					++samplerFieldCount;
				}
			}
			stream.WriteLine();
		}

		private string getGLVSInputFieldName(FieldInfo field)
		{
			var a = (VSInput)field.GetCustomAttributes(true)[0];
			string fieldName = "ERROR";
			switch (a.Type)
			{
				case (VSInputTypes.Position):
					fieldName = "Position" + a.Index.ToString();
					break;

				case (VSInputTypes.Color):
					fieldName = "Color" + a.Index.ToString();
					break;

				case (VSInputTypes.UV):
					fieldName = "Texcoord" + a.Index.ToString();
					break;

				case (VSInputTypes.Normal):
					fieldName = "Normal" + a.Index.ToString();
					break;

				case (VSInputTypes.Index):
					if (outputType == CompilerOutputs.GL2) fieldName = "BlendIndex" + a.Index.ToString();
					else fieldName = "gl_InstanceID";
					break;

				case (VSInputTypes.IndexClassic):
					fieldName = "BlendIndex" + a.Index.ToString();
					break;
			}

			return fieldName;
		}
	}
}
