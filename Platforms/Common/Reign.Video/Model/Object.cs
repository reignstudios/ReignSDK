using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;

namespace Reign.Video
{
	public class ObjectBoneGroup
	{
		public string Name {get; private set;}
		public int Index {get; private set;}
		public Bone Bone {get; private set;}

		public ObjectBoneGroup(BinaryReader reader)
		{
			Name = reader.ReadString();
			Index = reader.ReadInt32();
		}

		internal void linkObjects(Object o)
		{
			if (o.ArmatureObject == null) Debug.ThrowError("ObjectBoneGroup", "Object must reference a ArmatureObject");
			foreach (var bone in o.ArmatureObject.Armature.Bones)
			{
				if (Name == bone.Name)
				{
					Bone = bone;
					break;
				}
			}
			
			if (Bone == null) Debug.ThrowError("ObjectBoneGroup", "Failed to find bone");
		}

		public static void Write(BinaryWriter writer, SoftwareObjectBoneGroup softwareBoneGroup)
		{
			writer.Write(softwareBoneGroup.Name);
			writer.Write(softwareBoneGroup.Index);
		}
	}

	public abstract class Object
	{
		#region Properties
		public Model Model {get; private set;}
		public string Name {get; private set;}
		private string parentName, armatureObjectName;
		public Object Parent {get; private set;}

		public Vector3 Position, Scale;

		private Vector3 rotation;
		public Vector3 Rotation
		{
			get {return rotation;}
			set
			{
				rotation = value;
				Matrix3.FromEuler(ref value, out rotationMatrix);
			}
		}

		private Matrix3 rotationMatrix;
		public Matrix3 RotationMatrix
		{
			get {return rotationMatrix;}
			set
			{
				rotationMatrix = value;
				Matrix3.Euler(ref value, out rotation);
			}
		}

		public ObjectArmature ArmatureObject {get; private set;}
		public Action Action;
		public ObjectBoneGroup[] BoneGroups;
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

			// bone groups
			BoneGroups = new ObjectBoneGroup[reader.ReadInt32()];
			for (int i = 0; i != BoneGroups.Length; ++i)
			{
				BoneGroups[i] = new ObjectBoneGroup(reader);
			}
		}

		internal void linkObjects(Object[] objects)
		{
			// link parent
			if (!string.IsNullOrEmpty(parentName))
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

				if (Parent == null) Debug.ThrowError("Object", "Failed to find Parent");
			}

			// link object armature
			if (!string.IsNullOrEmpty(armatureObjectName))
			{
				foreach (var o in objects)
				{
					if (armatureObjectName == o.Name)
					{
						ArmatureObject = (ObjectArmature)o;
						armatureObjectName = null;
						break;
					}
				}

				if (ArmatureObject == null) Debug.ThrowError("Object", "Failed to find ObjectArmature");
			}

			// link bones to bone groups
			foreach (var bonegroup in BoneGroups)
			{
				bonegroup.linkObjects(this);
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

			// bone groups
			writer.Write(softwareObject.BoneGroups.Count);
			foreach (var bonegroup in softwareObject.BoneGroups)
			{
				ObjectBoneGroup.Write(writer, bonegroup);
			}

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