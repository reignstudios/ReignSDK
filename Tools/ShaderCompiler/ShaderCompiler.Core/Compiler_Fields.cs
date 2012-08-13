using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace ShaderCompiler.Core
{
	public partial class Compiler
	{
		private void compileFields(Type shader, StreamWriter stream, StreamWriter vsStream, StreamWriter psStream)
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
			var fieldArray = new List<FieldInfo>();
			foreach (var fieldInfos in fieldInfoList)
			{
				foreach (var info in fieldInfos) fieldArray.Add(info);
			}
			var fields = fieldArray.ToArray();
			
			var vsInFields = new List<FieldInfo>();
			var vsInPSOutFields = new List<FieldInfo>();
			var psOutFields = new List<FieldInfo>();
			var normalFields = new List<FieldInfo>();
			foreach (var field in fields)
			{
				var attributes = field.GetCustomAttributes(true);
				var arrayAttributes = field.GetCustomAttributes(typeof(ArrayType), true);
				if (attributes.Length == 0 || arrayAttributes.Length != 0)
				{
					normalFields.Add(field);
				}
				else
				{
					if (attributes.Length >= 2)
					{
						throw new Exception("Fields can only have one shader attribute.");
					}

					var a = attributes[0].GetType();
					if (a == typeof(VSInput))
					{
						vsInFields.Add(field);
					}
					else if (a == typeof(VSOutputPSInput))
					{
						vsInPSOutFields.Add(field);
					}
					else if (a == typeof(PSOutput))
					{
						psOutFields.Add(field);
					}
				}
			}

			// Process VSInput fields
			var baseType = getBaseCompilerOutput();
			if (baseType == BaseCompilerOutputs.HLSL)
			{
				vsStream.WriteLine("struct VSIn" + Environment.NewLine + '{');
				foreach (var field in vsInFields)
				{
					var a = (VSInput)field.GetCustomAttributes(true)[0];
					var fieldType = convertToBasicType(field.FieldType);
					string attributeType = "POSITION";
					if (a.Type == VSInputTypes.Color) attributeType = "COLOR";
					if (a.Type == VSInputTypes.UV) attributeType = "TEXCOORD";
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
					string fieldType = convertToBasicType(field.FieldType);
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
					stream.WriteLine(string.Format("{0} {1} : {2}{3};", convertToBasicType(field.FieldType), field.Name, attributeType, a.Index));
				}
				stream.WriteLine("};");
			}

			if (baseType == BaseCompilerOutputs.GLSL)
			{
				foreach (var field in vsInPSOutFields)
				{
					var a = (VSOutputPSInput)field.GetCustomAttributes(true)[0];
					string fieldType = convertToBasicType(field.FieldType);
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
					psStream.WriteLine(string.Format("{0} {1} : {2}{3};", convertToBasicType(field.FieldType), field.Name, attributeType, a.Index));
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
				string fieldType = convertToBasicType(field.FieldType);
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
				
				stream.WriteLine(string.Format("{0} {1};", fieldType, field.Name + ((arrayLength == -1) ? "" : "["+arrayLength.ToString()+"]")));
				
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
