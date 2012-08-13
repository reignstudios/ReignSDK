using Microsoft.Xna.Framework.Graphics;
using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class ShaderResource : ShaderResourceI
	{
		#region Properties
		private EffectParameter parameter;
		public string Name { get; private set; }
		#endregion

		#region Constructors
		public ShaderResource(EffectParameter parameter, string name)
		{
			this.parameter = parameter;
			this.Name = name;
		}
		#endregion

		#region Methods
		public void Set(Texture2DI resource)
		{
			var texture = (Texture2D)resource;
			parameter.SetValue(texture.texture);
		}

		public void Set(Texture3DI resource)
		{
			//var texture = (Texture2D)resource;
			//parameter.SetValue(texture.texture);
		}
		#endregion
	}
}
