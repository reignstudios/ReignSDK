using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;
using System.Reflection;

#if METRO
using System.Reflection;
#endif

namespace Reign.Video
{
	class ModelStreamLoader : StreamLoaderI
	{
		private Dictionary<string,string>[] textures;
		#if METRO
		private System.Threading.Tasks.Task<Stream> fileStream;
		#endif

		private ModelI model;
		private string fileName;
		private Stream stream;
		private string contentDirectory;
		private Dictionary<string,Type> materialTypes;
		private List<MaterialFieldBinder> value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes;
		private Dictionary<string,string> fileExtOverrides;

		public ModelStreamLoader(ModelI model, string fileName, Stream stream, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides)
		{
			this.model = model;
			this.fileName = fileName;
			this.stream = stream;
			this.contentDirectory = contentDirectory;
			this.materialTypes = materialTypes;
			this.value1BinderTypes = value1BinderTypes;
			this.value2BinderTypes = value2BinderTypes;
			this.value3BinderTypes = value3BinderTypes;
			this.value4BinderTypes = value4BinderTypes;
			this.textureBinderTypes = textureBinderTypes;
			this.fileExtOverrides = fileExtOverrides;
		}

		public override bool Load()
		{
			if (stream != null)
			{
				return model.load(fileName, stream, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, ref textures);
			}
			else
			{
				if (fileName != null)
				{
					#if METRO
					if (fileStream == null)
					{
						fileStream = Streams.OpenFile(fileName);
						return false;
					}
					else
					{
						if (!fileStream.IsCompleted) return false;
						bool pass = model.load(fileName, fileStream.Result, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, ref textures);
						fileName = null;
						return pass;
					}
					#else
					using (var file = Streams.OpenFile(fileName))
					{
						bool pass = model.load(fileName, file, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, ref textures);
						fileName = null;
						return pass;
					}
					#endif
				}
				else
				{
					return model.load(null, null, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, ref textures);
				}
			}
		}

		public override void Dispose()
		{
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}

			#if METRO
			if (fileStream != null && fileStream.Result != null)
			{
				fileStream.Result.Dispose();
				fileStream = null;
			}
			#endif
		}
	}

	class ModelSoftwareStreamLoader : StreamLoaderI
	{
		private ModelI model;
		private SoftwareModel softwareModel;
		private MeshVertexSizes positionSize;
		private bool loadColors, loadUVs, loadNormals;
		private string contentDirectory;
		private Dictionary<string,Type> materialTypes;
		private List<MaterialFieldBinder> value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes;
		private Dictionary<string,string> fileExtOverrides;

		public ModelSoftwareStreamLoader(ModelI model, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides)
		{
			this.model = model;
			this.softwareModel = softwareModel;
			this.positionSize = positionSize;
			this.loadColors = loadColors;
			this.loadUVs = loadUVs;
			this.loadNormals = loadNormals;
			this.contentDirectory = contentDirectory;
			this.materialTypes = materialTypes;
			this.value1BinderTypes = value1BinderTypes;
			this.value2BinderTypes = value2BinderTypes;
			this.value3BinderTypes = value3BinderTypes;
			this.value4BinderTypes = value4BinderTypes;
			this.textureBinderTypes = textureBinderTypes;
			this.fileExtOverrides = fileExtOverrides;
		}

		public override bool Load()
		{
			if (!softwareModel.Loaded) return false;
			var stream = new MemoryStream();
			ModelI.Save(stream, false, softwareModel, positionSize, loadColors, loadUVs, loadNormals);
			stream.Position = 0;
			new ModelStreamLoader(model, null, stream, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides);
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
		public ModelI(DisposableI parent, string fileName, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides)
		: base(parent)
		{
			new ModelStreamLoader(this, fileName, null, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides);
		}

		public ModelI(DisposableI parent, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides)
		: base(parent)
		{
			new ModelSoftwareStreamLoader(this, softwareModel, positionSize, loadColors, loadUVs, loadNormals, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides);
		}

		internal bool load(string fileName, Stream stream, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides, ref Dictionary<string,string>[] textures)
		{
			try
			{
				if (Materials == null)
				{
					var reader = new BinaryReader(stream);
					// meta data
					if (reader.ReadInt32() != Streams.MakeFourCC('R', 'M', 'F', 'T')) Debug.ThrowError("Error", "Not a ReignModel file: " + fileName);
					float version = reader.ReadSingle();
					if (version != 1.0f) Debug.ThrowError("Error", "Unsuported model version: " + version.ToString());
					bool compressed = reader.ReadBoolean();

					// materials
					int materialCount = reader.ReadInt32();
					Materials = new MaterialI[materialCount];
					textures = new Dictionary<string,string>[materialCount];
					Textures = new List<Texture2DI>();
					for (int i = 0; i != materialCount; ++i)
					{
						string name = reader.ReadString();

						// create material
						bool pass = false;
						foreach (var materialType in (Dictionary<string,Type>)materialTypes)
						{
							if (materialType.Key == name)
							{
								Materials[i] = (MaterialI)Activator.CreateInstance(materialType.Value);
								Materials[i].Name = name;
								pass = true;
								break;
							}
						}
						if (!pass) Debug.ThrowError("Model", "Failed to find a valid material type for: " + name);

						// values1
						var values1 = new Dictionary<string,float>();
						int valueCount = reader.ReadInt32();
						for (int i2 = 0; i2 != valueCount; ++i2)
						{
							values1.Add(reader.ReadString(), reader.ReadSingle());
						}
						MaterialFieldBinder binder;
						FieldInfo field;
						if (value1BinderTypes != null && value1BinderTypes.Count != 0)
						{
							field = getMaterialFieldInfo(Materials[i], values1, value1BinderTypes, out binder);
							field.SetValue(Materials[i], values1[binder.ID]);
						}

						// values2
						var values2 = new Dictionary<string,Vector2>();
						valueCount = reader.ReadInt32();
						for (int i2 = 0; i2 != valueCount; ++i2)
						{
							values2.Add(reader.ReadString(), reader.ReadVector2());
						}
						if (value2BinderTypes != null && value2BinderTypes.Count != 0)
						{
							field = getMaterialFieldInfo(Materials[i], values2, value2BinderTypes, out binder);
							field.SetValue(Materials[i], values2[binder.ID]);
						}

						// values3
						var values3 = new Dictionary<string,Vector3>();
						valueCount = reader.ReadInt32();
						for (int i2 = 0; i2 != valueCount; ++i2)
						{
							values3.Add(reader.ReadString(), reader.ReadVector3());
						}
						if (value3BinderTypes != null && value3BinderTypes.Count != 0)
						{
							field = getMaterialFieldInfo(Materials[i], values3, value3BinderTypes, out binder);
							field.SetValue(Materials[i], values3[binder.ID]);
						}

						// values4
						var values4 = new Dictionary<string,Vector4>();
						valueCount = reader.ReadInt32();
						for (int i2 = 0; i2 != valueCount; ++i2)
						{
							values4.Add(reader.ReadString(), reader.ReadVector4());
						}
						if (value4BinderTypes != null && value4BinderTypes.Count != 0)
						{
							field = getMaterialFieldInfo(Materials[i], values4, value4BinderTypes, out binder);
							field.SetValue(Materials[i], values4[binder.ID]);
						}

						// textures
						textures[i] = new Dictionary<string,string>();
						int textureCount = reader.ReadInt32();
						for (int i2 = 0; i2 != textureCount; ++i2)
						{
							textures[i].Add(reader.ReadString(), reader.ReadString());
						}
					}

					// meshes
					int meshCount = reader.ReadInt32();
					Meshes = new MeshI[meshCount];
					for (int i = 0; i != meshCount; ++i)
					{
						Meshes[i] = createMesh(reader, this);
					}

					return false;
				}
				else
				{
					for (int i = 0; i != Materials.Length; ++i)
					{
						if (textureBinderTypes == null || textureBinderTypes.Count == 0) continue;

						var material = Materials[i];
						var materialType = material.GetType();
						var materialTextures = textures[i];
						
						// load texture
						MaterialFieldBinder binder;
						var field = getMaterialFieldInfo(Materials[i], materialTextures, textureBinderTypes, out binder);
						var value = field.GetValue(material);
						if (value != null)
						{
							if (!((Texture2DI)value).Loaded) return false;
						}
						else
						{
							var textureFileName = materialTextures[binder.ID];
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

							var texture = createTexture(Parent, contentDirectory + textureFileName);
							if (!Textures.Contains(texture)) Textures.Add(texture);
							field.SetValue(material, texture);
							return false;
						}
					}
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}

			Loaded = true;
			return true;
		}

		private FieldInfo getMaterialFieldInfo(MaterialI material, IDictionary values, List<MaterialFieldBinder> binderTypes, out MaterialFieldBinder binder)
		{
			string materialName = material.Name;
			Type materialType = material.GetType();

			binder = null;
			foreach (var binderType in binderTypes)
			{
				if (binderType.MaterialName == materialName && values.Contains(binderType.ID))
				{
					binder = binderType;
					break;
				}
			}
			if (binder == null) Debug.ThrowError("Model", "Failed to find a binder for: " + materialType.ToString());

			#if METRO
			var field = materialType.GetTypeInfo().GetDeclaredField(binder.ShaderMaterialFieldName);
			#else
			var field = materialType.GetField(binder.ShaderMaterialFieldName);
			#endif
			if (field == null) Debug.ThrowError("Model", "Shader material field name does not exist: " + binder.ShaderMaterialFieldName);

			return field;
		}

		protected abstract MeshI createMesh(BinaryReader reader, ModelI model);
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
		public static void Save(string fileName, bool compress, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals)
		{
			using (var file = Streams.SaveFile(fileName))
			{
				Save(file, compress, softwareModel, positionSize, loadColors, loadUVs, loadNormals);
			}
		}

		public static void Save(Stream stream, bool compress, SoftwareModel softwareModel, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals)
		{
			var writer = new BinaryWriter(stream);

			// meta data
			writer.Write(Streams.MakeFourCC('R', 'M', 'F', 'T'));// tag
			writer.Write(1.0f);// version
			writer.Write(compress);

			// materials
			writer.Write(softwareModel.Materials.Count);
			foreach (var material in softwareModel.Materials)
			{
				writer.Write(material.Name);

				// values1
				writer.Write(material.Values1.Count);
				foreach (var value in material.Values1)
				{
					writer.Write(value.Key);
					writer.Write(value.Value);
				}

				// values2
				writer.Write(material.Values2.Count);
				foreach (var value in material.Values2)
				{
					writer.Write(value.Key);
					writer.WriteVector(value.Value);
				}

				// values3
				writer.Write(material.Values3.Count);
				foreach (var value in material.Values3)
				{
					writer.Write(value.Key);
					writer.WriteVector(value.Value);
				}

				// values4
				writer.Write(material.Values4.Count);
				foreach (var value in material.Values4)
				{
					writer.Write(value.Key);
					writer.WriteVector(value.Value);
				}

				// textures
				writer.Write(material.Textures.Count);
				foreach (var texture in material.Textures)
				{
					writer.Write(texture.Key);
					writer.Write(texture.Value);
				}
			}

			// meshes
			writer.Write(softwareModel.Meshes.Count);
			foreach (var mesh in softwareModel.Meshes)
			{
				MeshI.Write(writer, softwareModel, mesh, positionSize, loadColors, loadUVs, loadNormals);
			}
		}

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