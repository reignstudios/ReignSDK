using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	class ModelStreamLoader : StreamLoaderI
	{
		private List<string[]> diffuseFileNames;

		private Model model;
		private SoftwareModel softwareModel;
		private MeshVertexSizes positionSize;
		private DisposableI contentParent;
		private string contentDirectory;
		private object materialTypes;

		public ModelStreamLoader(Model model, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, object materialTypes)
		{
			this.model = model;
			this.softwareModel = softwareModel;
			this.positionSize = positionSize;
			this.contentParent = contentParent;
			this.contentDirectory = contentDirectory;
			this.materialTypes = materialTypes;
		}

		public override bool Load()
		{
			if (!softwareModel.Loaded) return false;

			if (model.Materials == null)
			{
				model.load(softwareModel, positionSize, materialTypes, out diffuseFileNames);
				return false;
			}
			else if (!model.loadMaterials(diffuseFileNames, contentParent, contentDirectory))
			{
				return false;
			}

			return true;
		}
	}

	public class Model : Disposable, ModelI
	{
		#region Properties
		public bool Loaded {get; private set;}

		public MeshI[] Meshes {get; private set;}
		public MaterialI[] Materials {get; private set;}
		#endregion

		#region Constructors
		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, List<Type> materialTypes)
		: base(parent)
		{
			new ModelStreamLoader(this, softwareModel, positionSize, contentParent, contentDirectory, materialTypes);
		}

		public Model(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, Dictionary<string,Type> materialTypes)
		: base(parent)
		{
			new ModelStreamLoader(this, softwareModel, positionSize, contentParent, contentDirectory, materialTypes);
		}

		internal void load(SoftwareModel softwareModel, MeshVertexSizes positionSize, object materialTypes, out List<string[]> diffuseFileNames)
		{
			try
			{
				diffuseFileNames = new List<string[]>();

				// create materials from matching field types
				if (materialTypes.GetType() == typeof(List<Type>))
				{
					Materials = new MaterialI[softwareModel.Materials.Count];
					for (int i = 0; i != Materials.Length; ++i)
					{
						bool pass = false;
						foreach (var materialType in (List<Type>)materialTypes)
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
								diffuseFileNames.Add(softwareModel.Materials[i].DiffuseTextures);
								pass = true;
								break;
							}
						}

						if (!pass) Debug.ThrowError("Model", "Failed to find a valid material type for: " + softwareModel.Materials[i].Name);
					}
				}
				else if (materialTypes.GetType() == typeof(Dictionary<string,Type>))
				{
					Materials = new MaterialI[softwareModel.Materials.Count];
					for (int i = 0; i != Materials.Length; ++i)
					{
						bool pass = false;
						foreach (var materialType in (Dictionary<string,Type>)materialTypes)
						{
							if (materialType.Key == softwareModel.Materials[i].Name)
							{
								Materials[i] = (MaterialI)Activator.CreateInstance(materialType.Value);
								diffuseFileNames.Add(softwareModel.Materials[i].DiffuseTextures);
								pass = true;
								break;
							}
						}

						if (!pass) Debug.ThrowError("Model", "Failed to find a valid material type for: " + softwareModel.Materials[i].Name);
					}
				}

				// create meshes
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

		internal bool loadMaterials(List<string[]> diffuseFileNames, DisposableI contentParent, string contentDirectory)
		{
			bool pass = true;
			for (int i = 0; i != Materials.Length; ++i)
			{
				var material = Materials[i];
				var diffuseTextures = material.DiffuseTextures;
				var diffuseTextureList = new List<Texture2DI>();
				for (int i2 = 0; i2 != diffuseTextures.Length; ++i2)
				{
					if (diffuseTextures[i2] == null)
					{
						diffuseTextureList.Add(new Texture2D(contentParent, contentDirectory + Streams.GetFileNameWithExt(diffuseFileNames[i][i2])));
						pass = false;
					}
					else
					{
						if (!diffuseTextures[i2].Loaded) pass = false;
					}
				}

				if (diffuseTextureList.Count != 0) material.DiffuseTextures = diffuseTextureList.ToArray();
			}
			if (!pass) return false;

			Loaded = true;
			return true;
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