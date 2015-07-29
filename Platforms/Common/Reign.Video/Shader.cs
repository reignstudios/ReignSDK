using Reign.Core;
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections.Generic;

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
		#if WIN32 || WINRT || WP8
		HLSL_2_0,
		HLSL_2_a,
		HLSL_3_0,
		HLSL_4_0,
		HLSL_4_1,
		HLSL_5_0,
		#endif
		#if WIN32 || OSX || LINUX
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
		#if iOS || ANDROID || NaCl || RPI
		GLSL_1_00
		#endif
	}
	
	public enum ShaderFloatingPointQuality
	{
		Max,
		High,
		Med,
		Low
	}

	public interface IShaderVariable
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
		void Set(Vector2[] values);
		void Set(Vector3[] values);
		void Set(Vector4[] values);
		void Set(Matrix2[] values);
		void Set(Matrix3[] values);
		void Set(Matrix4[] values);
		void Set(float[] values, int count);
		void Set(Vector2[] values, int count);
		void Set(Vector3[] values, int count);
		void Set(Vector4[] values, int count);
		void Set(Matrix2[] values, int count);
		void Set(Matrix3[] values, int count);
		void Set(Matrix4[] values, int count);
		void Set(float[] values, int offset, int count);
		void Set(Vector2[] values, int offset, int count);
		void Set(Vector3[] values, int offset, int count);
		void Set(Vector4[] values, int offset, int count);
		void Set(Matrix2[] values, int offset, int count);
		void Set(Matrix3[] values, int offset, int count);
		void Set(Matrix4[] values, int offset, int count);
	}

	public interface IShaderResource
	{
		void Set(ITexture2D resource);
		void Set(ITexture3D resource);
	}

	public abstract class IShader : DisposableResource, ILoadable
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}
		#endregion

		#region Constructors
		public IShader(IDisposableResource parent)
		: base(parent)
		{
			
		}

		public bool UpdateLoad()
		{
			return Loaded;
		}
		#endregion

		#region Methods
		#if WINRT || WP8 || SILVERLIGHT
		protected byte[][] getShaders(Stream stream)
		{
			var code = new byte[2][];
			using (var reader = new BinaryReader(stream))
			{
				int vsSize = reader.ReadInt32();
				int psSize = reader.ReadInt32();
				code[0] = new byte[vsSize];
				code[1] = new byte[psSize];
				stream.Read(code[0], 0, vsSize);
				stream.Read(code[1], 0, psSize);
			}

			return code;
		}
		#else
		protected string[] getShaders(Stream stream)
		{
			string code = null;
			using (var reader = new StreamReader(stream))
			{
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
		#endif

		public abstract void Apply();
		public abstract IShaderVariable Variable(string name);
		public abstract IShaderResource Resource(string name);
		#endregion
	}
}
