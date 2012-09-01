using System;
using Reign.Core;
using Reign.Video;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class PixelShader : ShaderModel
	{
		#region Properties
		internal PixelShaderCom pixelShaderCom;
		#endregion

		#region Constructors
		#if WINDOWS
		public PixelShader(Shader shader, string code, ShaderVersions shaderVersion)
		: base(shader, code, ShaderTypes.PS, shaderVersion)
		#else
		public PixelShader(Shader shader, byte[] code)
		: base(shader, code, ShaderTypes.PS)
		#endif
		{
			try
			{
				var video = shader.FindParentOrSelfWithException<Video>();
				pixelShaderCom = new PixelShaderCom();
				var error = pixelShaderCom.Init(video.com, com);

				if (error == PixelShaderErrors.PixelShader) Debug.ThrowError("PixelShader", "Failed to create PixelShader");
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
			if (pixelShaderCom != null)
			{
				pixelShaderCom.Dispose();
				pixelShaderCom = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			base.Apply();
			pixelShaderCom.Apply();
		}
		#endregion
	}
}
