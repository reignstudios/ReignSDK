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

		public List<SoftwareMesh> Meshes;
		public List<SoftwareMaterial> Materials;
		#endregion

		#region Constructors
		public SoftwareModel(string fileName, Loader.LoadedCallbackMethod loadedCallback)
		{
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
				var xml = new XmlSerializer(typeof(ColladaModel), "http://www.collada.org/2005/11/COLLADASchema");
				var collada = (ColladaModel)xml.Deserialize(stream);
				collada.Init();

				// check file version
				var versions = collada.Version.Split('.');
				bool pass = true;
				if (versions.Length >= 1 && int.Parse(versions[0]) != 1) pass = false;
				if (versions.Length >= 2 && int.Parse(versions[1]) != 4) pass = false;
				if (versions.Length >= 3 && int.Parse(versions[2]) < 1) pass = false;
				if (!pass) Debug.ThrowError("SoftwareModel", "Unsuported file version, must be 1.4.(1+)");

				// meshes
				Meshes = new List<SoftwareMesh>();
				var meshIDHash = new Dictionary<string, SoftwareMesh>();
				foreach (var geometry in collada.LibraryGeometry.Geometies)
				{
					var mesh = new SoftwareMesh(this, geometry);
					Meshes.Add(mesh);
					meshIDHash.Add(geometry.ID, mesh);
				}

				// materials
				Materials = new List<SoftwareMaterial>();
				var materialIDHash = new Dictionary<string, SoftwareMaterial>();
				foreach (var material in collada.LibraryMaterial.Materials)
				{
					var newMaterial = new SoftwareMaterial(collada, material);
					Materials.Add(newMaterial);
					materialIDHash.Add(material.ID, newMaterial);
				}

				// apply scenes (translate, rotate, scale and bind materials)
				var scene = collada.LibraryVisualScene.FindVisualScene(collada.Scene.InstanceVisualScene.URL);
				if (scene == null) Debug.ThrowError("SoftwareModel", "Failed to find visual scene: " + collada.Scene.InstanceVisualScene.URL);
				foreach (var node in scene.Nodes)
				{
					if (node.InstanceGeometry == null) continue;

					var id = node.InstanceGeometry.URL.Replace("#", "");
					var mesh = meshIDHash[id];

					// translate, rotate and scale
					var values = node.Translate.Values;
					mesh.Position = new Vector3(values[0], values[1], values[2]);

					values = node.Scale.Values;
					mesh.Scale = new Vector3(values[0], values[1], values[2]);

					var rotX = node.FindRotation("rotationX");
					var rotY = node.FindRotation("rotationY");
					var rotZ = node.FindRotation("rotationZ");
					mesh.Rotation = new Vector3(rotX.Values[3], rotY.Values[3], rotZ.Values[3]).DegToRad();

					// bind material
					id = node.InstanceGeometry.BindMaterial.TechniqueCommon.InstanceMatrial.Target.Replace("#", "");
					mesh.Material = materialIDHash[id];
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
	}
}