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
		#if WINDOWS || METRO || WP8
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

	public interface ShaderResourceI
	{
		void Set(Texture2DI resource);
		void Set(Texture3DI resource);
	}

	public abstract class ShaderI : Disposable, LoadableI
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}
		#endregion

		#region Constructors
		public ShaderI(DisposableI parent)
		: base(parent)
		{
			
		}

		public bool UpdateLoad()
		{
			return Loaded;
		}
		#endregion

		#region Methods
		#if METRO || WP8 || SILVERLIGHT
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
		public abstract ShaderVariableI Variable(string name);
		public abstract ShaderResourceI Resource(string name);
		#endregion
	}

	public static class ShaderAPI
	{
		public static void Init(NewPtrMethod1 newPtr1, NewPtrMethod2 newPtr2)
		{
			ShaderAPI.newPtr1 = newPtr1;
			ShaderAPI.newPtr2 = newPtr2;
		}

		public delegate ShaderI NewPtrMethod1(DisposableI parent, string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback);
		private static NewPtrMethod1 newPtr1;
		public static ShaderI New(DisposableI parent, string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			return newPtr1(parent, fileName, shaderVersion, loadedCallback);
		}

		public delegate ShaderI NewPtrMethod2(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback);
		private static NewPtrMethod2 newPtr2;
		public static ShaderI New(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			return newPtr2(parent, fileName, shaderVersion, vsQuality, psQuality, loadedCallback);
		}
	}
}
