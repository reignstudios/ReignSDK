using Reign.Core;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace Reign.Video
{
	public enum ShaderTypes
	{
		VS,
		PS
	}

	public enum ShaderVersions
	{
		Unknown,
		Max,
		#if WINDOWS || METRO
		HLSL_2_0,
		HLSL_2_a,
		HLSL_3_0,
		HLSL_4_0,
		HLSL_4_1,
		HLSL_5_0,
		#endif
		#if WINDOWS || OSX || LINUX
		GLSL_1_10,
		GLSL_1_20,
		GLSL_1_30,
		GLSL_1_40,
		GLSL_1_50,
		GLSL_3_30,
		GLSL_4_00,
		GLSL_4_10,
		GLSL_4_20,
		#endif
		#if iOS || ANDROID || NaCl
		GLSL_1_00
		#endif
	}

	public interface ShaderVariableI
	{
		void Set(float value);
		void Set(float x, float y);
		void Set(float x, float y, float z);
		void Set(float x, float y, float z, float w);
		void Set(Vector2 value);
		void Set(Vector3 value);
		void Set(Vector4 value);
		void Set(Matrix2 value);
		void Set(Matrix3 value);
		void Set(Matrix4 value);
		void Set(float[] values);
		void Set(Vector4[] values);
		void Set(Matrix4[] values);
		void Set(float[] values, int count);
		void Set(Vector4[] values, int count);
		void Set(Matrix4[] values, int count);
		void Set(float[] values, int offset, int count);
		void Set(Vector4[] values, int offset, int count);
		void Set(Matrix4[] values, int offset, int count);
	}

	public interface ShaderResourceI
	{
		void Set(Texture2DI resource);
		void Set(Texture3DI resource);
	}

	public abstract class ShaderI : Disposable
	{
		#region Properties
		public static string FileTag;
		#if WP7
		public Microsoft.Xna.Framework.Graphics.Effect FX {get; protected set;}
		#endif
		#endregion

		#region Constructors
		public ShaderI(DisposableI parent)
		: base(parent)
		{
			
		}
		#endregion

		#region Methods
		protected string[] getShaders(string fileName)
		{
			string code = null;
			using (var file = Streams.OpenFile(fileName))
			{
				var reader = new StreamReader(file);
				code = reader.ReadToEnd();
			}

			var match = Regex.Match(code, "#GLOBAL.*?#END", RegexOptions.Singleline);
			string globalCode = Regex.Replace(match.Value, "#.*", "");

			match = Regex.Match(code, "#VS.*?#END", RegexOptions.Singleline);
			string vertexCode = Regex.Replace(match.Value, "#.*", "");

			match = Regex.Match(code, "#PS.*?#END", RegexOptions.Singleline);
			string pixelCode = Regex.Replace(match.Value, "#.*", "");

			return new string[] {globalCode + vertexCode, globalCode + pixelCode};
		}

		public abstract void Apply();
		public abstract ShaderVariableI Variable(string name);
		public abstract ShaderResourceI Resource(string name);
		#endregion
	}
}
