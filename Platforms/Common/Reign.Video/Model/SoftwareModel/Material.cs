using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video
{
	public class SoftwareMaterial
	{
		#region Properties
		public string[] DiffuseTextures, SpecularTextures, EmissionTextures;
		public Vector4 Diffuse, Specular, Emission;
		public float Shininess, IndexOfRefraction;
		#endregion

		#region Methods
		public SoftwareMaterial(ColladaModel collada, ColladaModel_Material material)
		{
			var effect = collada.LibraryEffect.FindEffect(material.InstanceEffect.URL);
			if (effect == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material effect: " + material.InstanceEffect.URL);

			var phong = effect.ProfileCommon.Technique.Phong;
			if (phong == null) Debug.ThrowError("SoftwareMaterial", "Only phong materials are supported");

			// textures
			var textures = phong.Diffuse.Textures;
			if (textures != null)
			{
				DiffuseTextures = new string[textures.Length];
				for (int i = 0; i != textures.Length; ++i)
				{
					DiffuseTextures[i] = getTexture(collada, effect, phong, textures[i]);
				}
			}

			textures = phong.Specular.Textures;
			if (textures != null)
			{
				SpecularTextures = new string[textures.Length];
				for (int i = 0; i != textures.Length; ++i)
				{
					SpecularTextures[i] = getTexture(collada, effect, phong, textures[i]);
				}
			}

			textures = phong.Emission.Textures;
			if (textures != null)
			{
				EmissionTextures = new string[textures.Length];
				for (int i = 0; i != textures.Length; ++i)
				{
					EmissionTextures[i] = getTexture(collada, effect, phong, textures[i]);
				}
			}

			// colors
			float[] colors;
			if (phong.Diffuse.Color != null)
			{
				colors = phong.Diffuse.Color.Colors;
				Diffuse = new Vector4(colors[0], colors[1], colors[2], colors[3]);
			}
			else
			{
				Diffuse = new Vector4(1);
			}

			if (phong.Specular.Color != null)
			{
				colors = phong.Specular.Color.Colors;
				if (colors != null) Specular = new Vector4(colors[0], colors[1], colors[2], colors[3]);
			}
			else
			{
				Specular = new Vector4(1);
			}

			colors = phong.Emission.Color.Colors;
			if (colors != null) Emission = new Vector4(colors[0], colors[1], colors[2], colors[3]);

			Shininess = phong.Shininess.Float.Value;
			IndexOfRefraction = phong.IndexOfRefraction.Float.Value;
		}

		private string getTexture(ColladaModel collada, ColladaModel_Effect effect, ColladaModel_Phong phong, ColladaModel_Texture texture)
		{
			var newParam = effect.ProfileCommon.FindNewParam(texture.Texture);
			if (newParam == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material effect newParam: " + texture.Texture);

			newParam = effect.ProfileCommon.FindNewParam(newParam.Sampler2D.Source);
			if (newParam == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material effect newParam: " + newParam.Sampler2D.Source);

			var image = collada.LibraryImage.FindImage(newParam.Surface.InitFrom);
			if (image == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material image: " + newParam.Surface.InitFrom);

			return image.InitFrom;
		}
		#endregion
	}
}