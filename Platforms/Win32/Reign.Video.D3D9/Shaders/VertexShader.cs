using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class VertexShader : ShaderModel
	{
		#region Properties
		private VertexShaderCom vertexShaderCom;
		#endregion

		#region Constructors
		public VertexShader(DisposableI parent, string code, ShaderVersions shaderVersion)
		: base(parent, code, shaderVersion, ShaderTypes.VS)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				vertexShaderCom = new VertexShaderCom();
				var error = vertexShaderCom.Init(video.com, com);

				switch (error)
				{
					case VertexShaderErrors.VertexShader: Debug.ThrowError("VertexShader", "Failed to create vertex shader"); break;
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
			if (vertexShaderCom != null)
			{
				vertexShaderCom.Dispose();
				vertexShaderCom = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Apply()
		{
			vertexShaderCom.Apply();
		}
		#endregion
	}
}
