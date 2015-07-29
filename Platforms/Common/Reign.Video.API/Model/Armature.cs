using System.Collections.Generic;
using Reign.Core;
using System.IO;

namespace Reign.Video.Abstraction
{
	public class Bone
	{
		#region Properties
		public string Name {get; private set;}
		private string parentName;
		public Bone Parent {get; private set;}
		public bool InheritScale, InheritRotation;
		public Vector3 Position;
		public Quaternion Rotation;
		public Matrix3 RotationMatrix;
		#endregion

		#region Constructors
		public Bone(BinaryReader reader)
		{
			Name = reader.ReadString();
			parentName = reader.ReadString();

			InheritScale = reader.ReadBoolean();
			InheritRotation = reader.ReadBoolean();
			Position = reader.ReadVector3();
			RotationMatrix = reader.ReadMatrix3();
			Rotation = Quaternion.FromMatrix3(RotationMatrix);
		}

		internal void linkObjects(Bone[] bones)
		{
			foreach (var bone in bones)
			{
				if (parentName == bone.Name)
				{
					Parent = bone;
					parentName = null;
					break;
				}
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareBone softwareBone)
		{
			writer.Write(softwareBone.Name);
			writer.Write((softwareBone.Parent != null) ? softwareBone.Parent.Name : "");

			writer.Write(softwareBone.InheritScale);
			writer.Write(softwareBone.InheritRotation);
			writer.WriteVector(softwareBone.Position);
			writer.WriteMatrix(softwareBone.Rotation);
		}
		#endregion
	}

	public class Armature
	{
		#region Properties
		public string Name {get; private set;}
		public Bone[] Bones {get; private set;}
		#endregion

		#region Constructors
		public Armature(BinaryReader reader)
		{
			Name = reader.ReadString();

			Bones = new Bone[reader.ReadInt32()];
			for (int i = 0; i != Bones.Length; ++i)
			{
				Bones[i] = new Bone(reader);
			}

			// link bones
			foreach (var bone in Bones)
			{
				bone.linkObjects(Bones);
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareArmature softwareArmature)
		{
			writer.Write(softwareArmature.Name);

			writer.Write(softwareArmature.Bones.Count);
			foreach (var bone in softwareArmature.Bones)
			{
				Bone.Write(writer, bone);
			}
		}
		#endregion
	}
}