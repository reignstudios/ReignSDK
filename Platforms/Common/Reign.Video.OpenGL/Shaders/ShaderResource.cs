using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class ShaderResource : IShaderResource
	{
		#region Properties
		internal delegate void ApplyFunc();
		internal ApplyFunc Apply;
		private uint resourceIndex;
		private WeakReference texture;

		private Video video;
		private int location, index;
		public string Name {get; private set;}

		internal static uint[] textureMaps =
		{
			GL.TEXTURE0,
			GL.TEXTURE1,
			GL.TEXTURE2,
			GL.TEXTURE3,
			GL.TEXTURE4,
			GL.TEXTURE5,
			GL.TEXTURE6,
			GL.TEXTURE7
		};
		#endregion

		#region Constructors
		public ShaderResource(IVideo video, int location, int index, string name)
		{
			this.video = (Video)video;
			this.location = location;
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
			GL.ActiveTexture(textureMaps[index]);
			GL.BindTexture(GL.TEXTURE_2D, resourceIndex);
			GL.Uniform1i(location, index);

			#if DEBUG
			Video.checkForError();
			#endif

			var state = video.currentSamplerStates[index];
			if (state != null) state.enable((Texture2D)texture.Target);
		}

		public void Set(ITexture2D resource)
		{
			var texture = (Texture2D)resource;
			video.currentTextures[index] = texture;
			resourceIndex = texture.Texture;
			this.texture = new WeakReference(texture);
			Apply = setTexture2D;
		}

		public void Set(ITexture3D resource)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}