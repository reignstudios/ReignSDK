using Reign.Core;
using System;

namespace Reign.Video
{
	public enum MaterialFieldTypes
	{
		None,
		Diffuse,
		Specular,
		Emission,
		Shininess,
		IndexOfRefraction
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class MaterialField : Attribute
	{
		public MaterialFieldTypes Type;

		public MaterialField(MaterialFieldTypes type)
		{
			Type = type;
		}
	}

	public abstract class MaterialI : Disposable
	{
		#region Properties
		public Texture2DI[] DiffuseTextures;
		public Texture2DI[] SpecularTextures;
		public Texture2DI[] EmissionTextures;
		public Vector4 Diffuse, Specular, Emission;
		public float Shininess, IndexOfRefraction;
		#endregion

		#region Constructors
		public MaterialI(ModelI model, SoftwareMaterial material, string contentDirectory)
		: base(model)
		{
			try
			{
				// textures
				if (material.DiffuseTextures != null)
				{
					DiffuseTextures = new Texture2DI[material.DiffuseTextures.Length];
					for (int i = 0; i != DiffuseTextures.Length; ++i)
					{
						string fileName = material.DiffuseTextures[i];
						if (!Streams.IsAbsolutePath(fileName)) fileName = contentDirectory + fileName;
						DiffuseTextures[i] = initTexture(fileName);
					}
				}

				if (material.SpecularTextures != null)
				{
					SpecularTextures = new Texture2DI[material.SpecularTextures.Length];
					for (int i = 0; i != SpecularTextures.Length; ++i)
					{
						string fileName = material.SpecularTextures[i];
						if (!Streams.IsAbsolutePath(fileName)) fileName = contentDirectory + fileName;
						SpecularTextures[i] = initTexture(fileName);
					}
				}

				if (material.EmissionTextures != null)
				{
					EmissionTextures = new Texture2DI[material.EmissionTextures.Length];
					for (int i = 0; i != EmissionTextures.Length; ++i)
					{
						string fileName = material.EmissionTextures[i];
						if (!Streams.IsAbsolutePath(fileName)) fileName = contentDirectory + fileName;
						EmissionTextures[i] = initTexture(fileName);
					}
				}

				// colors
				Diffuse = material.Diffuse;
				Specular = material.Specular;
				Emission = material.Emission;
				Shininess = material.Shininess;
				IndexOfRefraction = material.IndexOfRefraction;
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		protected abstract Texture2DI initTexture(string fileName);
		#endregion
	}
}