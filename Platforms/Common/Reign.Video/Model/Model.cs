using Reign.Core;
using System.Collections.Generic;
using System;

namespace Reign.Video
{
	class ModelStreamLoader : StreamLoaderI
	{
		private List<SoftwareMaterial> diffuseFileNames;

		private ModelI model;
		private SoftwareModel softwareModel;
		private MeshVertexSizes positionSize;
		private DisposableI contentParent;
		private string contentDirectory;
		private object materialTypes;
		private Dictionary<Type,MaterialFieldBinder> materialFieldTypes;

		public ModelStreamLoader(ModelI model, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, object materialTypes, Dictionary<Type,MaterialFieldBinder> materialFieldTypes)
		{
			this.model = model;
			this.softwareModel = softwareModel;
			this.positionSize = positionSize;
			this.contentParent = contentParent;
			this.contentDirectory = contentDirectory;
			this.materialTypes = materialTypes;
			this.materialFieldTypes = materialFieldTypes;
		}

		public override bool Load()
		{
			if (!softwareModel.Loaded) return false;

			if (model.Materials == null)
			{
				model.load(softwareModel, positionSize, materialTypes, out diffuseFileNames);
				return false;
			}
			else if (!model.loadMaterials(diffuseFileNames, contentParent, contentDirectory, materialFieldTypes))
			{
				return false;
			}

			return true;
		}
	}

	public abstract class ModelI : Disposable
	{
		#region Properties
		public bool Loaded {get; private set;}

		public MeshI[] Meshes {get; private set;}
		public MaterialI[] Materials {get; private set;}
		public List<Texture2DI> Textures {get; private set;}
		#endregion

		#region Constructors
		public ModelI(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, Dictionary<string,Type> materialTypes, Dictionary<Type,MaterialFieldBinder> materialFieldTypes)
		: base(parent)
		{
			new ModelStreamLoader(this, softwareModel, positionSize, contentParent, contentDirectory, materialTypes, materialFieldTypes);
		}

		internal void load(SoftwareModel softwareModel, MeshVertexSizes positionSize, object materialTypes, out List<SoftwareMaterial> textureFileNames)
		{
			try
			{
				textureFileNames = new List<SoftwareMaterial>();

				// create materials from matching field types
				if (materialTypes.GetType() == typeof(Dictionary<string,Type>))
				{
					Materials = new MaterialI[softwareModel.Materials.Count];
					Textures = new List<Texture2DI>();
					for (int i = 0; i != Materials.Length; ++i)
					{
						bool pass = false;
						foreach (var materialType in (Dictionary<string,Type>)materialTypes)
						{
							if (materialType.Key == softwareModel.Materials[i].Name)
							{
								Materials[i] = (MaterialI)Activator.CreateInstance(materialType.Value);
								textureFileNames.Add(softwareModel.Materials[i]);
								pass = true;
								break;
							}
						}

						if (!pass) Debug.ThrowError("Model", "Failed to find a valid material type for: " + softwareModel.Materials[i].Name);
					}
				}
				else
				{
					Debug.ThrowError("Model", "Unsuported materialTypes");
				}

				// create meshes
				Meshes = new MeshI[softwareModel.Meshes.Count];
				for (int i = 0; i != Meshes.Length; ++i)
				{
					Meshes[i] = createMesh(this, softwareModel, softwareModel.Meshes[i], positionSize);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		internal bool loadMaterials(List<SoftwareMaterial> softwareMaterials, DisposableI contentParent, string contentDirectory, Dictionary<Type,MaterialFieldBinder> materialFieldTypes)
		{
			for (int i = 0; i != Materials.Length; ++i)
			{
				// make sure there is a field binder for the material
				var material = Materials[i];
				var materialType = material.GetType();
				if (!materialFieldTypes.ContainsKey(materialType)) Debug.ThrowError("Model", "Material field types do not contain a material type: " + materialType.ToString());

				// get texture field
				var binder = materialFieldTypes[materialType];
				var textureFileName = softwareMaterials[i].Textures[binder.ID];
				var field = materialType.GetField(binder.MaterialFieldName);
				if (field == null) Debug.ThrowError("Model", "Material field name does not exist: " + binder.MaterialFieldName);

				// load texture
				var value = field.GetValue(material);
				if (value != null)
				{
					if (!((Texture2DI)value).Loaded) return false;
				}
				else
				{
					var texture = createTexture(contentParent, contentDirectory + Streams.GetFileNameWithExt(textureFileName));
					if (!Textures.Exists(x => x == texture)) Textures.Add(texture);
					field.SetValue(material, texture);
					return false;
				}
			}

			Loaded = true;
			return true;
		}

		protected abstract MeshI createMesh(ModelI model, SoftwareModel softwareModel, SoftwareMesh softwareMesh, MeshVertexSizes positionSize);
		protected abstract Texture2DI createTexture(DisposableI parent, string fileName);

		public override void Dispose()
		{
			disposeChilderen();
			if (Textures != null)
			{
				for (int i = 0; i != Textures.Count; ++i)
				{
					Textures[i].DisposeReference();
				}
				Textures = null;
			}
			base.Dispose();
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