using Reign.Core;
using System.Collections.Generic;
using System;

#if METRO
using System.Reflection;
#endif

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
		private Dictionary<string,Type> materialTypes;
		private List<MaterialFieldBinder> materialFieldTypes;
		private Dictionary<string,string> fileExtOverrides;

		public ModelStreamLoader(ModelI model, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> materialFieldTypes, Dictionary<string,string> fileExtOverrides)
		{
			this.model = model;
			this.softwareModel = softwareModel;
			this.positionSize = positionSize;
			this.contentParent = contentParent;
			this.contentDirectory = contentDirectory;
			this.materialTypes = materialTypes;
			this.materialFieldTypes = materialFieldTypes;
			this.fileExtOverrides = fileExtOverrides;
		}

		public override bool Load()
		{
			if (!softwareModel.Loaded) return false;

			if (model.Materials == null)
			{
				model.load(softwareModel, positionSize, materialTypes, out diffuseFileNames);
				return false;
			}
			else if (!model.loadMaterials(diffuseFileNames, contentParent, contentDirectory, materialFieldTypes, fileExtOverrides))
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
		public ModelI(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, DisposableI contentParent, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> materialFieldTypes, Dictionary<string,string> fileExtOverrides)
		: base(parent)
		{
			new ModelStreamLoader(this, softwareModel, positionSize, contentParent, contentDirectory, materialTypes, materialFieldTypes, fileExtOverrides);
		}

		internal void load(SoftwareModel softwareModel, MeshVertexSizes positionSize, Dictionary<string,Type> materialTypes, out List<SoftwareMaterial> textureFileNames)
		{
			try
			{
				textureFileNames = new List<SoftwareMaterial>();

				// create materials from matching field types
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

		internal bool loadMaterials(List<SoftwareMaterial> softwareMaterials, DisposableI contentParent, string contentDirectory, List<MaterialFieldBinder> materialFieldTypes, Dictionary<string,string> fileExtOverrides)
		{
			for (int i = 0; i != Materials.Length; ++i)
			{
				// make sure there is a field binder for the material
				var material = Materials[i];
				var materialType = material.GetType();
				MaterialFieldBinder binder = null;
				foreach (var materialFieldType in materialFieldTypes)
				{
					if (materialFieldType.MaterialName == softwareMaterials[i].Name)
					{
						binder = materialFieldType;
						break;
					}
				}
				if (binder == null) Debug.ThrowError("Model", "Material field types do not contain a material type: " + materialType.ToString());
				
				// get texture field
				var textureFileName = softwareMaterials[i].Textures[binder.TextureID];
				#if METRO
				var field = materialType.GetTypeInfo().GetDeclaredField(binder.ShaderMaterialFieldName);
				#else
				var field = materialType.GetField(binder.ShaderMaterialFieldName);
				#endif
				if (field == null) Debug.ThrowError("Model", "Material field name does not exist: " + binder.ShaderMaterialFieldName);

				// load texture
				var value = field.GetValue(material);
				if (value != null)
				{
					if (!((Texture2DI)value).Loaded) return false;
				}
				else
				{
					if (fileExtOverrides != null)
					{
						string ext = Streams.GetFileExt(textureFileName);
						if (fileExtOverrides.ContainsKey(ext)) textureFileName = Streams.GetFileNameWithoutExt(textureFileName) + fileExtOverrides[ext];
						else textureFileName = Streams.GetFileNameWithExt(textureFileName);
					}
					else
					{
						textureFileName = Streams.GetFileNameWithExt(textureFileName);
					}
					var texture = createTexture(contentParent, contentDirectory + textureFileName);
					if (!Textures.Contains(texture)) Textures.Add(texture);
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