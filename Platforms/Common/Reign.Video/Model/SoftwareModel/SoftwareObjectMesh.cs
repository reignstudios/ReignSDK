using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public class SoftwareObjectMesh : SoftwareObject
	{
		#region Properties
		public SoftwareMesh Mesh;
		#endregion

		#region Constructors
		public SoftwareObjectMesh(SoftwareModel model, RMX_Object o)
		: base(model, o)
		{
			if (o.Mesh != null)
			{
				foreach (var mesh in model.Meshes)
				{
					if (o.Mesh.Name == mesh.Name)
					{
						Mesh = mesh;
						break;
					}
				}

				if (Mesh == null) Debug.ThrowError("SoftwareObjectMesh", "Failed to find mesh");
			}
			else
			{
				Debug.ThrowError("SoftwareObjectMesh", "RMX_Object be linked to a Mesh object");
			}
		}
		#endregion
	}
}