using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.OpenGL
{
	public abstract class ShaderModel : Disposable
	{
		#region Properties
		public uint Shader {get; private set;}
		#endregion

		#region Constructors
		public unsafe ShaderModel(ShaderI shader, string code, ShaderVersions shaderVersion, ShaderTypes shaderType)
		: base(shader)
		{
			try
			{
				Shader = GL.CreateShader((shaderType == ShaderTypes.VS) ? GL.VERTEX_SHADER : GL.FRAGMENT_SHADER);
				if (Shader == 0) Debug.ThrowError("ShaderModel", "Failed to create shader");

				#if iOS || ANDROID || NaCl
				if (shaderType == ShaderTypes.VS) code = "precision highp float;" + Environment.NewLine + code;
				else code = "precision lowp float;" + Environment.NewLine + code;
				#endif
				
				string shaderLvl = "";
				switch (shaderVersion)
				{
					#if iOS || ANDROID || NaCl
					case (ShaderVersions.GLSL_1_00): shaderLvl = "100"; break;
					#else
					case (ShaderVersions.GLSL_1_10): shaderLvl = "110"; break;
					case (ShaderVersions.GLSL_1_20): shaderLvl = "120"; break;
					case (ShaderVersions.GLSL_1_30): shaderLvl = "130"; break;
					case (ShaderVersions.GLSL_1_40): shaderLvl = "140"; break;
					case (ShaderVersions.GLSL_1_50): shaderLvl = "150"; break;
					case (ShaderVersions.GLSL_3_30): shaderLvl = "330"; break;
					#endif
				}
				code = "#version " + shaderLvl + Environment.NewLine + code;
				
				int codeLength = code.Length;
				fixed (byte* codeData = code.CastToBytes())
				{
					byte* codeData2 = codeData;
					GL.ShaderSource(Shader, 1, &codeData2, &codeLength);
					GL.CompileShader(Shader);
				}

				int result = 0;
				GL.GetShaderiv(Shader, GL.COMPILE_STATUS, &result);
				if (result == 0)
				{
					int logLength = 0;
					GL.GetShaderiv(Shader, GL.INFO_LOG_LENGTH, &logLength);
					byte* logPtr = stackalloc byte[logLength];
					GL.GetShaderInfoLog(Shader, logLength, &result, logPtr);
					byte[] log = new byte[logLength];
					System.Runtime.InteropServices.Marshal.Copy(new IntPtr(logPtr), log, 0, logLength);

					Debug.ThrowError("ShaderModel", string.Format("{0} Shader compile error: {1}", shaderType, System.Text.ASCIIEncoding.ASCII.GetString(log)));
				}

				Video.checkForError();
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (Shader != 0)
			{
				if (!OS.AutoDisposedGL) GL.DeleteShader(Shader);
				Shader = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
			}
			base.Dispose();
		}
		#endregion
	}
}