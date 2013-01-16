using System;

namespace Reign.Video.Vita
{
	public class ShaderResource : ShaderResourceI
	{
		#region Properties
		internal delegate void ApplyFunc();
		internal ApplyFunc Apply;
		private WeakReference texture;

		private Video video;
		private int index;
		public string Name {get; private set;}
		#endregion

		#region Constructors
		public ShaderResource(VideoI video, int index, string name)
		{
			this.video = (Video)video;
			this.index = index;
			this.Name = name;

			Apply = setNothing;
		}
		#endregion

		#region Methods
		private void setNothing()
		{
			// Place holder.
		}

		private void setTexture2D()
		{
			var textureRef = (Texture2D)texture.Target;
			video.context.SetTexture(index, textureRef.texture);
		}

		public void Set(Texture2DI resource)
		{
			var texture = (Texture2D)resource;
			video.currentPixelTextures[index] = texture;
			this.texture = new WeakReference(texture);
			Apply = setTexture2D;
		}

		public void Set(Texture3DI resource)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}

