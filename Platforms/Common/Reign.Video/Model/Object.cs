using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;

namespace Reign.Video
{
	public abstract class Object : Disposable
	{
		#region Properties
		public Model Model {get; private set;}
		public string Name;
		public Vector3 Position, Rotation, Scale;
		public Matrix3 RotationMatrix;
		#endregion

		#region Constructors
		public Object(BinaryReader reader, Model model)
		: base(model)
		{
			try
			{
				Model = model;
				Name = reader.ReadString();

				// transform
				Position = reader.ReadVector3();
				Scale = reader.ReadVector3();
				Rotation = reader.ReadVector3();
				RotationMatrix = Matrix3.FromEuler(Rotation);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareObject softwareObject)
		{
			string typeName = "";
			var type = softwareObject.GetType();
			if (softwareObject.GetType() == typeof(SoftwareObjectMesh)) typeName = "MESH";
			else Debug.ThrowError("Object", "Unsuported SoftwareObject type: " + softwareObject.GetType());
			writer.Write(typeName);

			// name
			writer.Write(softwareObject.Name);

			// transform
			writer.WriteVector(softwareObject.Position);
			writer.WriteVector(softwareObject.Scale);
			writer.WriteVector(softwareObject.Rotation);

			// types
			if (typeName == "MESH") ObjectMesh.Write(writer, (SoftwareObjectMesh)softwareObject);
		}

		public virtual void Render()
		{
			// place holder...
		}
		#endregion
	}
}