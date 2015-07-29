using System;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class ShaderResource : IShaderResource
	{
		#region Properties
		public delegate void ApplyMethod();
		public ApplyMethod Apply;
		private WeakReference resource;

		public string Name {get; private set;}
		private ShaderResourceCom com;
		private Video video;
		private int vertexIndex, pixelIndex;
		#endregion

		#region Constructors
		public ShaderResource(Video video, string name, int vertexIndex, int pixelIndex)
		{
			Name = name;
			this.video = video;
			this.vertexIndex = vertexIndex;
			this.pixelIndex = pixelIndex;

			com = new ShaderResourceCom(video.com, vertexIndex, pixelIndex);
			Apply = setNothing;
			resource = new WeakReference(null);
		}
		#endregion

		#region Methods
		private void setNothing()
		{
			// Place Holder...
		}

		private void setTexture2D()
		{
			var renderTarget = resource.Target as RenderTarget;
			if (renderTarget != null) com.SetRenderTarget(renderTarget.com, renderTarget.renderTargetCom);
			else com.SetTexture2D(((Texture2D)resource.Target).com);
		}

		public void Set(ITexture2D resource)
		{
			this.resource.Target = resource;
			if (vertexIndex != -1) video.currentVertexTextures[vertexIndex] = (Texture2D)resource;
			if (pixelIndex != -1) video.currentPixelTextures[pixelIndex] = (Texture2D)resource;
			Apply = setTexture2D;
		}

		public void Set(ITexture3D resource)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
