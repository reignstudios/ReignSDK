using System;
using Reign.Core;

namespace Reign.Video.D3D11
{
	public class Material : MaterialI
	{
		#region Properties
		
		#endregion

		#region Constructors
		public Material(Model model, SoftwareMaterial material, string contentDirectory)
		: base(model, material, contentDirectory)
		{
			
		}

		protected override Texture2DI initTexture(string fileName)
		{
			return new Texture2D(this, fileName);
		}
		#endregion
	}
}