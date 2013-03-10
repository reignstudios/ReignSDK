using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public abstract class SoftwareObject
	{
		#region Properties
		public SoftwareModel Model;
		public string Name;
		public Vector3 Position, Rotation, Scale;
		#endregion

		#region Constructors
		public SoftwareObject(SoftwareModel model, RMX_Object o)
		{
			Model = model;
			Name = o.Name;

			// transform
			foreach (var input in o.Transform.Inputs)
			{
				switch (input.Type)
				{
					case "EulerRotation": Rotation = new Vector3(input.Values[0], input.Values[1], input.Values[2]); break;
					case "Scale": Scale = new Vector3(input.Values[0], input.Values[1], input.Values[2]); break;
					case "Position": Position = new Vector3(input.Values[0], input.Values[1], input.Values[2]); break;
					default: Debug.ThrowError("SoftwareMesh", "Unsuported Transform Type: " + input.Type); break;
				}
			}
		}
		#endregion

		#region Methods
		public void Rotate(float x, float y, float z)
		{
			Rotation += new Vector3(x, y, z);
		}

		public void RotateGeometry(float x, float y, float z)
		{
			var mat = Matrix3.FromEuler(x, y, z);
			Position = Position.Transform(mat);
		}
		#endregion
	}
}