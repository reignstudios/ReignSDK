using System;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class ShaderResource : ShaderResourceI
	{
		#region Properties
		public delegate void ApplyMethod();
		public ApplyMethod Apply;
		private WeakReference resource;

		public string Name {get; private set;}
		private ShaderResourceCom com;
		#endregion

		#region Constructors
		public ShaderResource(VideoCom video, string name, int vertexIndex, int pixelIndex)
		{
			Name = name;
			com = new ShaderResourceCom(video, vertexIndex, pixelIndex);
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
			com.SetTexture2D(((Texture2D)resource.Target).com);
		}

		public void Set(Texture2DI resource)
		{
			this.resource.Target = resource;
			Apply = setTexture2D;
		}

		public void Set(Texture3DI resource)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
