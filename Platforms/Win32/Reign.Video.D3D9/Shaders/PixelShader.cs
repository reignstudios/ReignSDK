using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class PixelShader : ShaderModel
	{
		#region Properties
		private PixelShaderCom pixelShaderCom;
		#endregion

		#region Constructors
		public PixelShader(DisposableI parent, string code, ShaderVersions shaderVersion)
		: base(parent, code, shaderVersion, ShaderTypes.PS)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				pixelShaderCom = new PixelShaderCom();
				var error = pixelShaderCom.Init(video.com, com);

				switch (error)
				{
					case PixelShaderErrors.PixelShader: Debug.ThrowError("PixelShader", "Failed to create pixel shader"); break;
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
			if (pixelShaderCom != null)
			{
				pixelShaderCom.Dispose();
				pixelShaderCom = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Apply()
		{
			pixelShaderCom.Apply();
		}
		#endregion
	}
}
