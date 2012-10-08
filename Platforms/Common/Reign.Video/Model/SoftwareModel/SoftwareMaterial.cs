using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video
{
	public class SoftwareMaterial
	{
		#region Properties
		public string Name;
		public Dictionary<string,string> Textures;
		public Dictionary<string,float> Values1;
		public Dictionary<string,Vector2> Values2;
		public Dictionary<string,Vector3> Values3;
		public Dictionary<string,Vector4> Values4;
		#endregion

		#region Constructors
		public SoftwareMaterial(ColladaModel collada, ColladaModel_Material material)
		{
			Name = material.Name;

			var effect = collada.LibraryEffect.FindEffect(material.InstanceEffect.URL);
			if (effect == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material effect: " + material.InstanceEffect.URL);

			var phong = effect.ProfileCommon.Technique.Phong;
			if (phong == null) Debug.ThrowError("SoftwareMaterial", "Only phong materials are supported");

			// textures
			var diffuseTextures = phong.Diffuse.Textures;
			var specularTextures = phong.Specular.Textures;
			var emissionTextures = phong.Emission.Textures;
			Textures = new Dictionary<string,string>();

			if (diffuseTextures != null)
			{
				for (int i = 0; i != diffuseTextures.Length; ++i)
				{
					var image = getTexture(collada, effect, phong, diffuseTextures[i]);
					Textures.Add(image.ID, image.InitFrom);
				}
			}

			if (specularTextures != null)
			{
				for (int i = 0; i != specularTextures.Length; ++i)
				{
					var image = getTexture(collada, effect, phong, specularTextures[i]);
					Textures.Add(image.ID, image.InitFrom);
				}
			}

			if (emissionTextures != null)
			{
				for (int i = 0; i != emissionTextures.Length; ++i)
				{
					var image = getTexture(collada, effect, phong, emissionTextures[i]);
					Textures.Add(image.ID, image.InitFrom);
				}
			}

			// colors
			Values4 = new Dictionary<string,Vector4>();
			string[] ids;
			var colors = getColors(phong.Diffuse.Colors, out ids);
			if (colors != null)
			{
				for (int i = 0; i != colors.Length; ++i)
				{
					Values4.Add(ids[i], colors[i]);
				}
			}

			colors = getColors(phong.Specular.Colors, out ids);
			if (colors != null)
			{
				for (int i = 0; i != colors.Length; ++i)
				{
					Values4.Add(ids[i], colors[i]);
				}
			}

			colors = getColors(phong.Emission.Colors, out ids);
			if (colors != null)
			{
				for (int i = 0; i != colors.Length; ++i)
				{
					Values4.Add(ids[i], colors[i]);
				}
			}

			// values
			Values1 = new Dictionary<string,float>();
			var values = getValues(phong.Shininess.Floats, out ids);
			if (values != null)
			{
				for (int i = 0; i != values.Length; ++i)
				{
					Values1.Add(ids[i], values[i]);
				}
			}

			values = getValues(phong.IndexOfRefraction.Floats, out ids);
			if (values != null)
			{
				for (int i = 0; i != values.Length; ++i)
				{
					Values1.Add(ids[i], values[i]);
				}
			}

			// other
			Values2 = new Dictionary<string,Vector2>();
			Values3 = new Dictionary<string,Vector3>();
		}

		private Vector4[] getColors(ColladaModel_Color[] colors, out string[] ids)
		{
			ids = null;
			if (colors == null) return null;

			var vectorColors = new Vector4[colors.Length];
			ids = new string[colors.Length];
			for (int i = 0; i != colors.Length; ++i)
			{
				ids[i] = colors[i].SID;
				var colorValues = colors[i].Colors;
				vectorColors[i] = new Vector4(colorValues[0], colorValues[1], colorValues[2], colorValues[3]);
			}

			return vectorColors;
		}

		private float[] getValues(ColladaModel_Float[] values, out string[] ids)
		{
			ids = null;
			if (values == null) return null;

			var scalarValues = new float[values.Length];
			ids = new string[values.Length];
			for (int i = 0; i != values.Length; ++i)
			{
				ids[i] = values[i].SID;
				scalarValues[i] = values[i].Value;
			}

			return scalarValues;
		}

		private ColladaModel_Image getTexture(ColladaModel collada, ColladaModel_Effect effect, ColladaModel_Phong phong, ColladaModel_Texture texture)
		{
			var newParam = effect.ProfileCommon.FindNewParam(texture.Texture);
			if (newParam == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material effect newParam: " + texture.Texture);

			newParam = effect.ProfileCommon.FindNewParam(newParam.Sampler2D.Source);
			if (newParam == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material effect newParam: " + newParam.Sampler2D.Source);

			var image = collada.LibraryImage.FindImage(newParam.Surface.InitFrom);
			if (image == null) Debug.ThrowError("SoftwareMaterial", "Failed to find material image: " + newParam.Surface.InitFrom);

			return image;
		}
		#endregion
	}
}