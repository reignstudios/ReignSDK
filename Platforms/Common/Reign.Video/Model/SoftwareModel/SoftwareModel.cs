﻿using System.Collections.Generic;
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

		public List<SoftwareObject> Objects;
		public List<SoftwareMesh> Meshes;
		public List<SoftwareMaterial> Materials;
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

				// materials
				Materials = new List<SoftwareMaterial>();
				foreach (var material in rmx.Materials.Materials)
				{
					Materials.Add(new SoftwareMaterial(material));
				}

				// meshes
				Meshes = new List<SoftwareMesh>();
				foreach (var mesh in rmx.Meshes.Meshes)
				{
					Meshes.Add(new SoftwareMesh(this, mesh));
				}

				// objects
				Objects = new List<SoftwareObject>();
				foreach (var o in rmx.RMXObjects.Objects)
				{
					if (o.Type == "MESH") Objects.Add(new SoftwareObjectMesh(this, o));
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