using System;
using Reign.Core;
using Reign.Video;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class VertexShader : ShaderModel
	{
		#region Properties
		internal VertexShaderCom vertexShaderCom;
		#endregion

		#region Constructors
		public VertexShader(Shader shader, string code, ShaderVersions shaderVersion)
		: base(shader, code, ShaderTypes.VS, shaderVersion)
		{
			try
			{
				var video = shader.FindParentOrSelfWithException<Video>();
				vertexShaderCom = new VertexShaderCom();
				var error = vertexShaderCom.Init(video.com, com);

				if (error == VertexShaderErrors.VertexShader) Debug.ThrowError("VertexShader", "Failed to create VertexShader");
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
		public override void Apply()
		{
			base.Apply();
			vertexShaderCom.Apply();
		}
		#endregion
	}
}
