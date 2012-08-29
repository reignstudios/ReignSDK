using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	public class Model : Disposable, ModelI
	{
		#region Properties
		public MeshI[] Meshes {get; private set;}
		public MaterialI[] Materials {get; private set;}
		#endregion

		#region Constructors
		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, string contentDirectory, List<Type> materialTypes)
		: base(parent)
		{
			try
			{
				Materials = new MaterialI[softwareModel.Materials.Count];
				for (int i = 0; i != Materials.Length; ++i)
				{
					bool pass = false;
					foreach (var materialType in materialTypes)
					{
						if (materialType != typeof(MaterialI)) Debug.ThrowError("Model", "Invalid material type: " + materialType.ToString());

						int diffuseColors = 0, specularColors = 0, emissionColors = 0;
						int diffuseTextures = 0, specularTextures = 0, emissionTextures = 0;
						int shininessValues = 0, indexOfRefractionValues = 0;
						foreach (var field in materialType.GetFields())
						{
							var ats = (MaterialField[])field.GetCustomAttributes(typeof(MaterialField), false);
							foreach (var a in ats)
							{
								if (a.Type == MaterialFieldTypes.Diffuse)
								{
									if (field.FieldType == typeof(Vector4)) ++diffuseColors;
									else if (field.FieldType == typeof(Texture2DI)) ++diffuseTextures;
									else Debug.ThrowError("Model", "Invalid material diffuse type: " + field.FieldType.ToString());
								}
								else if (a.Type == MaterialFieldTypes.Specular)
								{
									if (field.FieldType == typeof(Vector4)) ++specularColors;
									else if (field.FieldType == typeof(Texture2DI)) ++specularTextures;
									else Debug.ThrowError("Model", "Invalide material specular type: " + field.FieldType.ToString());
								}
								else if (a.Type == MaterialFieldTypes.Emission)
								{
									if (field.FieldType == typeof(Vector4)) ++emissionColors;
									else if (field.FieldType == typeof(Texture2DI)) ++emissionTextures;
									else Debug.ThrowError("Model", "Invalide material emission type: " + field.FieldType.ToString());
								}
								else if (a.Type == MaterialFieldTypes.Shininess)
								{
									if (field.FieldType == typeof(float)) ++shininessValues;
									else Debug.ThrowError("Model", "Invalide material shininess type: " + field.FieldType.ToString());
								}
								else if (a.Type == MaterialFieldTypes.IndexOfRefraction)
								{
									if (field.FieldType == typeof(float)) ++indexOfRefractionValues;
									else Debug.ThrowError("Model", "Invalide material index of refraction type: " + field.FieldType.ToString());
								}
							}
						}

						var mat = softwareModel.Materials[i];
						if (diffuseColors == mat.DiffuseColors.Length && specularColors == mat.SpecularColors.Length && emissionColors == mat.EmissionColors.Length &&
							diffuseTextures == mat.DiffuseTextures.Length && specularTextures == mat.SpecularTextures.Length && emissionTextures == mat.EmissionTextures.Length &&
							shininessValues == mat.ShininessValues.Length && indexOfRefractionValues == mat.IndexOfRefractionValues.Length)
						{
							Materials[i] = (MaterialI)Activator.CreateInstance(materialType);
							pass = true;
							break;
						}
					}

					if (!pass) Debug.ThrowError("Model", "Failed to find a valid material type for: " + softwareModel.Materials[i].Name);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}

			init(parent, softwareModel, positionSize, contentDirectory);
		}

		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, string contentDirectory, Dictionary<string,Type> materialTypes)
		: base(parent)
		{
			try
			{
				Materials = new MaterialI[softwareModel.Materials.Count];
				for (int i = 0; i != Materials.Length; ++i)
				{
					bool pass = false;
					foreach (var materialType in materialTypes)
					{
						if (materialType.Key == softwareModel.Materials[i].Name)
						{
							Materials[i] = (MaterialI)Activator.CreateInstance(materialType.Value);
							pass = true;
							break;
						}
					}

					if (!pass) Debug.ThrowError("Model", "Failed to find a valid material type for: " + softwareModel.Materials[i].Name);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}

			init(parent, softwareModel, positionSize, contentDirectory);
		}

		private void init(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, string contentDirectory)
		{
			try
			{
				Meshes = new Mesh[softwareModel.Meshes.Count];
				for (int i = 0; i != Meshes.Length; ++i)
				{
					Meshes[i] = new Mesh(this, softwareModel, softwareModel.Meshes[i], positionSize);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public void Render()
		{
			foreach (var mesh in Meshes)
			{
				mesh.Render();
			}
		}
		#endregion
	}
}