using System.Collections.Generic;
using Reign.Core;
using System.Xml.Serialization;
using System.IO;
using System;

namespace Reign.Video
{
	public class SoftwareModel : LoadableI
	{
		#region Properties
		public bool Loaded {get; private set;}
		public bool FailedToLoad {get; protected set;}

		public float FrameStart, FrameEnd, FPS;

		public List<SoftwareMaterial> Materials;
		public List<SoftwareMesh> Meshes;
		public List<SoftwareAction> Actions;
		public List<SoftwareArmature> Armatures;
		public List<SoftwareObject> Objects;
		#endregion

		#region Constructors
		public SoftwareModel(string fileName, Loader.LoadedCallbackMethod loadedCallback)
		{
			string fileType = Streams.GetFileExt(fileName).ToLower();
			if (fileType != ".rmx") Debug.ThrowError("SoftwareModel", "Unsuported file type: " + fileType);

			new StreamLoader(fileName,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(((StreamLoader)sender).LoadedStream, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		private void init(Stream stream, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				var xml = new XmlSerializer(typeof(RMXModel), "");
				var rmx = (RMXModel)xml.Deserialize(stream);
				rmx.Init();

				// check file version
				if (rmx.Version != 1.0f) Debug.ThrowError("SoftwareModel", "Unsuported file version, must be 1.0");

				// frames
				FrameStart = rmx.FrameStart;
				FrameEnd = rmx.FrameEnd;
				FPS = rmx.FPS;

				// materials
				Materials = new List<SoftwareMaterial>();
				if (rmx.Materials.Materials != null)
				{
					foreach (var material in rmx.Materials.Materials)
					{
						Materials.Add(new SoftwareMaterial(material));
					}
				}

				// meshes
				Meshes = new List<SoftwareMesh>();
				if (rmx.Meshes.Meshes != null)
				{
					foreach (var mesh in rmx.Meshes.Meshes)
					{
						Meshes.Add(new SoftwareMesh(this, mesh));
					}
				}

				// actions
				Actions = new List<SoftwareAction>();
				if (rmx.Actions.Actions != null)
				{
					foreach (var action in rmx.Actions.Actions)
					{
						Actions.Add(new SoftwareAction(action));
					}
				}

				// armatures
				Armatures = new List<SoftwareArmature>();
				if (rmx.Armatures.Armatures != null)
				{
					foreach (var armature in rmx.Armatures.Armatures)
					{
						Armatures.Add(new SoftwareArmature(armature));
					}
				}

				// objects
				Objects = new List<SoftwareObject>();
				if (rmx.RMXObjects.Objects != null)
				{
					foreach (var o in rmx.RMXObjects.Objects)
					{
						if (o.Type == "MESH") Objects.Add(new SoftwareObjectMesh(this, o));
						else if (o.Type == "ARMATURE") Objects.Add(new SoftwareObjectArmature(this, o));
					}
				
					int i = 0;
					foreach (var o in Objects)
					{
						o.linkObjects(rmx.RMXObjects.Objects[i]);
						++i;
					}
				}
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				if (loadedCallback != null) loadedCallback(this, false);
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}

		public bool UpdateLoad()
		{
			return Loaded;
		}
		#endregion

		#region Methods
		public void Rotate(float x, float y, float z)
		{
			foreach (var o in Objects) o.Rotate(x, y, z);
		}

		public void RotateGeometry(float x, float y, float z)
		{
			foreach (var o in Objects) o.RotateGeometry(x, y, z);
			foreach (var mesh in Meshes) mesh.RotateGeometry(x, y, z);
		}
		#endregion
	}
}