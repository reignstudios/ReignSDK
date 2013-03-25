using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;
using System.Reflection;

namespace Reign.Video
{
	public class Model : Disposable, LoadableI
	{
		#region Properties
		public bool Loaded {get; private set;}
		public bool FailedToLoad {get; private set;}

		public float FrameStart {get; private set;}
		public float FrameEnd {get; private set;}
		public float FrameCount {get; private set;}
		public float FPS {get; private set;}

		public MaterialI[] Materials {get; private set;}
		public Mesh[] Meshes {get; private set;}
		public Action[] Actions {get; private set;}
		public Armature[] Armatures {get; private set;}
		public Object[] Objects {get; private set;}
		
		public List<Texture2DI> Textures {get; private set;}
		#endregion

		#region Constructors
		public Model(DisposableI parent, string fileName, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides, int classicInstanceCount, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			Loader.AddLoadable(this);
			new StreamLoader(fileName,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(fileName, ((StreamLoader)sender).LoadedStream, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, classicInstanceCount, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		public Model(DisposableI parent, SoftwareModel softwareModel, bool loadColors, bool loadUVs, bool loadNormals, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides, int classicInstanceCount, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			Loader.AddLoadable(this);
			new LoadWaiter(new LoadableI[]{softwareModel},
			delegate(object sender, bool succeeeded)
			{
				if (succeeeded)
				{
					using (var stream = new MemoryStream())
					{
						try
						{
							Model.Save(stream, false, softwareModel, loadColors, loadUVs, loadNormals);
							stream.Position = 0;
						}
						catch (Exception e)
						{
							FailedToLoad = true;
							Dispose();
							if (loadedCallback != null) loadedCallback(this, false);
						}

						if (!FailedToLoad) init(null, stream, contentDirectory, materialTypes, value1BinderTypes, value2BinderTypes, value3BinderTypes, value4BinderTypes, textureBinderTypes, fileExtOverrides, classicInstanceCount, loadedCallback);
					}
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		private void init(string fileName, Stream stream, string contentDirectory, Dictionary<string,Type> materialTypes, List<MaterialFieldBinder> value1BinderTypes, List<MaterialFieldBinder> value2BinderTypes, List<MaterialFieldBinder> value3BinderTypes, List<MaterialFieldBinder> value4BinderTypes, List<MaterialFieldBinder> textureBinderTypes, Dictionary<string,string> fileExtOverrides, int classicInstanceCount, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				var reader = new BinaryReader(stream);

				// meta data
				if (reader.ReadInt32() != Streams.MakeFourCC('R', 'M', 'F', 'T')) Debug.ThrowError("Error", "Not a ReignModel file: " + fileName);
				float version = reader.ReadSingle();
				if (version != 1.0f) Debug.ThrowError("Error", "Unsuported model version: " + version.ToString());
				bool compressed = reader.ReadBoolean();

				// frames
				FrameStart = reader.ReadSingle();
				FrameEnd = reader.ReadSingle();
				FrameCount = FrameEnd - FrameStart;
				FPS = reader.ReadSingle();

				// materials
				int materialCount = reader.ReadInt32();
				Materials = new MaterialI[materialCount];
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
					var material = Materials[i];

					// values1
					var values1 = new Dictionary<string,float>();
					int valueCount = reader.ReadInt32();
					for (int i2 = 0; i2 != valueCount; ++i2) values1.Add(reader.ReadString(), reader.ReadSingle());
					bindTypes(material, values1, value1BinderTypes, contentDirectory, fileExtOverrides, handleFoundValueBinder);

					// values2
					var values2 = new Dictionary<string,Vector2>();
					valueCount = reader.ReadInt32();
					for (int i2 = 0; i2 != valueCount; ++i2) values2.Add(reader.ReadString(), reader.ReadVector2());
					bindTypes(material, values2, value2BinderTypes, contentDirectory, fileExtOverrides, handleFoundValueBinder);

					// values3
					var values3 = new Dictionary<string,Vector3>();
					valueCount = reader.ReadInt32();
					for (int i2 = 0; i2 != valueCount; ++i2) values3.Add(reader.ReadString(), reader.ReadVector3());
					bindTypes(material, values3, value3BinderTypes, contentDirectory, fileExtOverrides, handleFoundValueBinder);

					// values4
					var values4 = new Dictionary<string,Vector4>();
					valueCount = reader.ReadInt32();
					for (int i2 = 0; i2 != valueCount; ++i2) values4.Add(reader.ReadString(), reader.ReadVector4());
					bindTypes(material, values4, value4BinderTypes, contentDirectory, fileExtOverrides, handleFoundValueBinder);

					// textures
					var textures = new Dictionary<string,string>();
					int textureCount = reader.ReadInt32();
					for (int i2 = 0; i2 != textureCount; ++i2) textures.Add(reader.ReadString(), reader.ReadString());
					bindTypes(material, textures, textureBinderTypes, contentDirectory, fileExtOverrides, handleFoundTextureBinder);
				}

				// meshes
				int meshCount = reader.ReadInt32();
				Meshes = new Mesh[meshCount];
				for (int i = 0; i != meshCount; ++i)
				{
					Meshes[i] = new Mesh(reader, this, classicInstanceCount);
				}

				// actions
				int actionCount = reader.ReadInt32();
				Actions = new Action[actionCount];
				for (int i = 0; i != actionCount; ++i)
				{
					Actions[i] = new Action(reader);
				}

				// armatures
				int armatureCount = reader.ReadInt32();
				Armatures = new Armature[armatureCount];
				for (int i = 0; i != armatureCount; ++i)
				{
					Armatures[i] = new Armature(reader);
				}

				// objects
				int objectCount = reader.ReadInt32();
				Objects = new Object[objectCount];
				for (int i = 0; i != objectCount; ++i)
				{
					string type = reader.ReadString();
					if (type == "MESH") Objects[i] = new ObjectMesh(reader, this);
					else if (type == "ARMATURE") Objects[i] = new ObjectArmature(reader, this);
					else Debug.ThrowError("Mesh", "Unsuported Object type: " + type);
				}

				// link objects
				foreach (var o in Objects)
				{
					o.linkObjects(Objects);
				}
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return;
			}

			if (Textures.Count == 0)
			{
				Loaded = true;
				if (loadedCallback != null) loadedCallback(this, true);
			}
			else
			{
				new LoadWaiter(Textures.ToArray(),
				delegate(object sender, bool succeeded)
				{
					if (succeeded)
					{
						Loaded = true;
						if (loadedCallback != null) loadedCallback(this, true);
					}
					else
					{
						FailedToLoad = true;
						Dispose();
						if (loadedCallback != null) loadedCallback(this, false);
					}
				});
			}
		}

		private delegate void FoundBinderMethod(MaterialI material, FieldInfo materialField, IDictionary values, MaterialFieldBinder binder, string contentDirectory, Dictionary<string,string> fileExtOverrides);
		private void bindTypes(MaterialI material, IDictionary values, List<MaterialFieldBinder> binders, string contentDirectory, Dictionary<string,string> fileExtOverrides, FoundBinderMethod handleFoundBinder)
		{
			if (binders == null) return;

			var materialType = material.GetType();
			foreach (var binder in binders)
			{
				if (binder.MaterialName == material.Name && values.Contains(binder.InputID))
				{
					#if WINRT
					var materialField = materialType.GetTypeInfo().GetDeclaredField(binder.ShaderMaterialFieldName);
					#else
					var materialField = materialType.GetField(binder.ShaderMaterialFieldName);
					#endif
					if (materialField == null) Debug.ThrowError("Model", "Shader material field name does not exist: " + binder.ShaderMaterialFieldName);

					if (handleFoundBinder != null) handleFoundBinder(material, materialField, values, binder, contentDirectory, fileExtOverrides);
				}
			}
		}

		private void handleFoundValueBinder(MaterialI material, FieldInfo materialField, IDictionary values, MaterialFieldBinder binder, string contentDirectory, Dictionary<string,string> fileExtOverrides)
		{
			var value = values[binder.InputID];
			var valueType = value.GetType();
			var materialType = materialField.FieldType;
			if (materialType == valueType)
			{
				materialField.SetValue(material, values[binder.InputID]);
			}
			else if (materialType == typeof(Vector2))
			{
				if (valueType == typeof(Vector3))
				{
					var vector = (Vector3)value;
					materialField.SetValue(material, new Vector2(vector.X, vector.Y));
				}
				else if (valueType == typeof(Vector4))
				{
					var vector = (Vector4)value;
					materialField.SetValue(material, new Vector2(vector.X, vector.Y));
				}
			}
			else if (materialType == typeof(Vector3))
			{
				if (valueType == typeof(Vector2))
				{
					var vector = (Vector2)value;
					materialField.SetValue(material, new Vector3(vector.X, vector.Y, 0));
				}
				else if (valueType == typeof(Vector4))
				{
					var vector = (Vector4)value;
					materialField.SetValue(material, new Vector3(vector.X, vector.Y, vector.Z));
				}
			}
			else if (materialType == typeof(Vector4))
			{
				if (valueType == typeof(Vector2))
				{
					var vector = (Vector2)value;
					materialField.SetValue(material, new Vector4(vector.X, vector.Y, 0, 0));
				}
				else if (valueType == typeof(Vector3))
				{
					var vector = (Vector3)value;
					materialField.SetValue(material, new Vector4(vector.X, vector.Y, vector.Z, 0));
				}
			}
		}

		private void handleFoundTextureBinder(MaterialI material, FieldInfo materialField, IDictionary values, MaterialFieldBinder binder, string contentDirectory, Dictionary<string,string> fileExtOverrides)
		{
			var textureFileName = ((Dictionary<string,string>)values)[binder.InputID];
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

			var texture = Texture2DAPI.NewReference(Parent, contentDirectory + textureFileName, null);
			if (!Textures.Contains(texture)) Textures.Add(texture);
			materialField.SetValue(material, texture);
		}

		public bool UpdateLoad()
		{
			return Loaded;
		}

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
		#if WINRT || WP8
		public static async void Save(string fileName, bool compress, SoftwareModel softwareModel, bool loadColors, bool loadUVs, bool loadNormals)
		{
			using (var file = await Streams.SaveFile(fileName, FolderLocations.Unknown))
		#else
		public static void Save(string fileName, bool compress, SoftwareModel softwareModel, bool loadColors, bool loadUVs, bool loadNormals)
		{
			using (var file = Streams.SaveFile(fileName, FolderLocations.Unknown))
		#endif
			{
				Save(file, compress, softwareModel, loadColors, loadUVs, loadNormals);
			}
		}

		public static void Save(Stream stream, bool compress, SoftwareModel softwareModel, bool loadColors, bool loadUVs, bool loadNormals)
		{
			var writer = new BinaryWriter(stream);
			
			// meta data
			writer.Write(Streams.MakeFourCC('R', 'M', 'F', 'T'));// tag
			writer.Write(1.0f);// version
			writer.Write(false);//compress);// TODO: add zip compression

			// frames
			writer.Write(softwareModel.FrameStart);
			writer.Write(softwareModel.FrameEnd);
			writer.Write(softwareModel.FPS);

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
				Mesh.Write(writer, softwareModel, mesh, loadColors, loadUVs, loadNormals);
			}

			// actions
			writer.Write(softwareModel.Actions.Count);
			foreach (var action in softwareModel.Actions)
			{
				Action.Write(writer, action);
			}

			// armatures
			writer.Write(softwareModel.Armatures.Count);
			foreach (var armature in softwareModel.Armatures)
			{
				Armature.Write(writer, armature);
			}

			// objects
			writer.Write(softwareModel.Objects.Count);
			foreach (var o in softwareModel.Objects)
			{
				Object.Write(writer, o);
			}
		}

		public void Render()
		{
			foreach (var o in Objects)
			{
				o.Render();
			}
		}
		#endregion
	}
}