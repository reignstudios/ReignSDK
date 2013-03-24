using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;

namespace Reign.Video
{
	public abstract class Object
	{
		#region Properties
		public Model Model {get; private set;}
		public string Name {get; private set;}
		private string parentName, armatureObjectName;
		public Object Parent {get; private set;}
		public Vector3 Position, Rotation, Scale;
		public Matrix3 RotationMatrix;
		public ObjectArmature ArmatureObject {get; private set;}
		public Action Action;
		#endregion

		#region Constructors
		public Object(BinaryReader reader, Model model)
		{
			Model = model;
			Name = reader.ReadString();
			parentName = reader.ReadString();

			// transform
			Position = reader.ReadVector3();
			Scale = reader.ReadVector3();
			Rotation = reader.ReadVector3();
			RotationMatrix = Matrix3.FromEuler(Rotation);

			// animation
			armatureObjectName = reader.ReadString();
			string actionName = reader.ReadString();
			foreach (var action in model.Actions)
			{
				if (actionName == action.Name)
				{
					Action = action;
					break;
				}
			}
		}

		internal void linkObjects(Object[] objects)
		{
			foreach (var o in objects)
			{
				if (parentName == o.Name)
				{
					Parent = o;
					parentName = null;
					break;
				}
			}

			foreach (var o in objects)
			{
				if (armatureObjectName == o.Name)
				{
					ArmatureObject = (ObjectArmature)o;
					armatureObjectName = null;
					break;
				}
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareObject softwareObject)
		{
			string typeName = "";
			var type = softwareObject.GetType();
			if (softwareObject.GetType() == typeof(SoftwareObjectMesh)) typeName = "MESH";
			else if (softwareObject.GetType() == typeof(SoftwareObjectArmature)) typeName = "ARMATURE";
			else Debug.ThrowError("Object", "Unsuported SoftwareObject type: " + softwareObject.GetType());
			writer.Write(typeName);

			// name
			writer.Write(softwareObject.Name);
			writer.Write((softwareObject.Parent != null) ? softwareObject.Parent.Name : "");

			// transform
			writer.WriteVector(softwareObject.Position);
			writer.WriteVector(softwareObject.Scale);
			writer.WriteVector(softwareObject.Rotation);

			// animation
			writer.Write((softwareObject.ArmatureObject != null) ? softwareObject.ArmatureObject.Name : "");
			writer.Write((softwareObject.DefaultAction != null) ? softwareObject.DefaultAction.Name : "");

			// types
			if (typeName == "MESH") ObjectMesh.Write(writer, (SoftwareObjectMesh)softwareObject);
			else if (typeName == "ARMATURE") ObjectArmature.Write(writer, (SoftwareObjectArmature)softwareObject);
		}

		public virtual void Render()
		{
			// place holder...
		}
		#endregion
	}
}