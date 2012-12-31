using Microsoft.Xna.Framework.Graphics;
using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class ShaderResource : ShaderResourceI
	{
		#region Properties
		#if SILVERLIGHT
		private Video video;
		private int resourceIndex;
		#else
		private EffectParameter parameter;
		#endif
		public string Name { get; private set; }
		#endregion

		#region Constructors
		#if SILVERLIGHT
		public ShaderResource(Video video, int resourceIndex, string name)
		{
			this.video = video;
			this.resourceIndex = resourceIndex;
			this.Name = name;
		}
		#else
		public ShaderResource(EffectParameter parameter, string name)
		{
			this.parameter = parameter;
			this.Name = name;
		}
		#endif
		#endregion

		#region Methods
		public void Set(Texture2DI resource)
		{
			var texture = (Texture2D)resource;
			#if SILVERLIGHT
			video.Device.Textures[resourceIndex] = texture.texture;
			#else
			parameter.SetValue(texture.texture);
			#endif
		}

		public void Set(Texture3DI resource)
		{
			//var texture = (Texture2D)resource;
			//parameter.SetValue(texture.texture);
		}
		#endregion
	}
}
