using System;
using Reign.Core;

namespace Reign.Video.D3D11
{
	public class Material : Disposable, MaterialI
	{
		#region Properties
		public Texture2DI[] DiffuseTextures {get; private set;}
		#endregion

		#region Constructors
		public Material(Model model, SoftwareMaterial material, string contentDirectory)
		: base(model)
		{
			try
			{
				if (material.DiffuseTextures != null)
				{
					DiffuseTextures = new Texture2DI[material.DiffuseTextures.Length];
					for (int i = 0; i != DiffuseTextures.Length; ++i)
					{
						string fileName = material.DiffuseTextures[i];
						string path;
						if (fileName.Length >= 2 && fileName[1] != ':') path = contentDirectory + fileName;
						else path = fileName;
						DiffuseTextures[i] = new Texture2D(this, path);
					}
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion
	}
}