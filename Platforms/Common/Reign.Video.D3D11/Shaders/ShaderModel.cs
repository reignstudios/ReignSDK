using System;
using Reign.Core;
using Reign.Video;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public abstract class ShaderModel : DisposableResource
	{
		#region Properties
		#if WINRT || WP8
		protected Shader shader;
		#endif
		internal ShaderModelCom com;
		#endregion

		#region Constructors
		#if WIN32
		public ShaderModel(Shader shader, string code, ShaderTypes shaderType, ShaderVersions shaderVersion)
		#else
		public ShaderModel(Shader shader, byte[] code, ShaderTypes shaderType)
		#endif
		: base(shader)
		{
			try
			{
				#if WINRT || WP8
				this.shader = shader;
				#endif
				var video = shader.FindParentOrSelfWithException<Video>();
				com = new ShaderModelCom();

				#if WIN32
				string shaderLvl = "";
				switch (shaderVersion)
				{
					case ShaderVersions.HLSL_2_0: shaderLvl = "_4_0_level_9_1"; break;
					case ShaderVersions.HLSL_2_a: shaderLvl = "_4_0_level_9_2"; break;
					case ShaderVersions.HLSL_3_0: shaderLvl = "_4_0_level_9_3"; break;
					case ShaderVersions.HLSL_4_0: shaderLvl = "_4_0"; break;
					case ShaderVersions.HLSL_4_1: shaderLvl = "_4_1"; break;
					case ShaderVersions.HLSL_5_0: shaderLvl = "_5_0"; break;
					default: Debug.ThrowError("ShaderModel", "Unsuported ShaderVersion: " + shaderVersion); break;
				}

				string errorText;
				var error = com.Init(video.com, code, code.Length, shaderType.ToString().ToLower() + shaderLvl, out errorText);
				#else
				var error = com.Init
				(
					video.com, code, code.Length,
					shaderType == ShaderTypes.VS ? shader.vsVariableBufferSize : shader.psVariableBufferSize,
					shaderType == ShaderTypes.VS ? shader.vsResourceCount : shader.psResourceCount
				);
				#endif

				switch (error)
				{
					#if WIN32
					case ShaderModelErrors.CompileCode: Debug.ThrowError("ShaderModel", "Shader compiler error: " + errorText); break;
					#endif
					case ShaderModelErrors.VariableBuffer: Debug.ThrowError("ShaderModel", "Failed to create VariableBuffer"); break;
					case ShaderModelErrors.Reflect: Debug.ThrowError("ShaderModel", "Failed to Reflect the shader"); break;
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null) com.Dispose();
			base.Dispose();
		}
		#endregion

		#region Methods
		public virtual void Apply()
		{
			com.Apply();
		}

		#if WIN32
		public int Variable(string name)
		{
			return com.Variable(name);
		}

		public int Resource(string name)
		{
			return com.Resource(name);
		}
		#else
		public abstract int Variable(string name);
		public abstract int Resource(string name);
		#endif
		#endregion
	}
}
