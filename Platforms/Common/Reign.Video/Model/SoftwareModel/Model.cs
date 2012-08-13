using System.Collections.Generic;
using Reign.Core;
using System.Xml.Serialization;

namespace Reign.Video
{
	public class SoftwareModel
	{
		#region Properties
		public List<SoftwareMesh> Meshes;
		public List<SoftwareMaterial> Materials;
		#endregion

		#region Constructors
		public SoftwareModel(string fileName)
		{
			Meshes = new List<SoftwareMesh>();
			Materials = new List<SoftwareMaterial>();

			using (var file = Streams.OpenFile(fileName))
			{
				var xml = new XmlSerializer(typeof(ColladaModel), "http://www.collada.org/2005/11/COLLADASchema");
				var collada = (ColladaModel)xml.Deserialize(file);
				collada.Init();

				// check file version
				var versions = collada.Version.Split('.');
				bool pass = true;
				if (versions.Length >= 1 && int.Parse(versions[0]) != 1) pass = false;
				if (versions.Length >= 2 && int.Parse(versions[1]) != 4) pass = false;
				if (versions.Length >= 3 && int.Parse(versions[2]) < 1) pass = false;
				if (!pass) Debug.ThrowError("SoftwareModel", "Unsuported file version, must be 1.4.(1+)");

				// meshes
				var meshIDHash = new Dictionary<string, SoftwareMesh>();
				foreach (var geometry in collada.LibraryGeometry.Geometies)
				{
					var mesh = new SoftwareMesh(geometry);
					Meshes.Add(mesh);
					meshIDHash.Add(geometry.ID, mesh);

					switch (collada.Asset.UpAxis)
					{
						case ("Z_UP"): mesh.Rotate(-Reign.Core.Math.Pi * .5f, 0, 0); break;
					}
				}

				// materials
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
					mesh.Location = new Vector3(values[0], values[1], values[2]);

					values = node.Scale.Values;
					mesh.Scale = new Vector3(values[0], values[1], values[2]);

					// bind material
					id = node.InstanceGeometry.BindMaterial.TechniqueCommon.InstanceMatrial.Target.Replace("#", "");
					mesh.Material = materialIDHash[id];
				}
			}
		}
		#endregion
	}
}