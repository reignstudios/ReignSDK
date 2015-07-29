using System;
using Reign.Core;
using Reign.Video;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class ShaderResource : IShaderResource
	{
		#region Properties
		private ShaderResourceCom com;
		public string Name {get; private set;}
		#endregion

		#region Constructors
		public ShaderResource(string name, VideoCom video, ShaderModelCom vertexShader, ShaderModelCom pixelShader, int vertexIndex, int pixelIndex)
		{
			Name = name;
			com = new ShaderResourceCom(video, vertexShader, pixelShader, vertexIndex, pixelIndex);
		}
		#endregion

		#region Methods
		public void Set(ITexture2D resource)
		{
			com.Set(((Texture2D)resource).com);
		}

		public void Set(ITexture3D resource)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
