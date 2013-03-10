using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;

namespace Reign.Video
{
	public class ObjectMesh : Object
	{
		#region Properties
		public Mesh Mesh {get; private set;}
		#endregion

		#region Constructors
		public ObjectMesh(BinaryReader reader, Model model)
		: base(reader, model)
		{
			try
			{
				string meshName = reader.ReadString();
				foreach (var mesh in model.Meshes)
				{
					if (meshName == mesh.Name)
					{
						Mesh = mesh;
						break;
					}
				}

				if (Mesh == null) Debug.ThrowError("ObjectMesh", "Failed to find Mesh");
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareObjectMesh softwareObjectMesh)
		{
			writer.Write(softwareObjectMesh.Mesh.Name);
		}

		public override void Render()
		{
			Mesh.Enable(this);
			Mesh.Draw();
		}
		#endregion
	}
}