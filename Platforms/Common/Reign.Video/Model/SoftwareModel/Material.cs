using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video
{
	public class SoftwareMaterial
	{
		#region Properties
		public string Name;
		public string[] DiffuseTextures, SpecularTextures, EmissionTextures;
		public Vector4[] DiffuseColors, SpecularColors, EmissionColors;
		public float[] ShininessValues, IndexOfRefractionValues;
		#endregion

		#region Methods
		public SoftwareMaterial(ColladaModel collada, ColladaModel_Material material)
		{
			Name = material.Name;

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
			DiffuseColors = getColors(phong.Diffuse.Colors);
			SpecularColors = getColors(phong.Specular.Colors);
			EmissionColors = getColors(phong.Emission.Colors);

			// values
			ShininessValues = getValues(phong.Shininess.Floats);
			IndexOfRefractionValues = getValues(phong.IndexOfRefraction.Floats);
		}

		private Vector4[] getColors(ColladaModel_Color[] colors)
		{
			var vectorColors = new Vector4[colors.Length];
			for (int i = 0; i != colors.Length; ++i)
			{
				var colorValues = colors[i].Colors;
				vectorColors[i] = new Vector4(colorValues[0], colorValues[1], colorValues[2], colorValues[3]);
			}

			return vectorColors;
		}

		private float[] getValues(ColladaModel_Float[] values)
		{
			var scalarValues = new float[values.Length];
			for (int i = 0; i != values.Length; ++i)
			{
				scalarValues[i] = values[i].Value;
			}

			return scalarValues;
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