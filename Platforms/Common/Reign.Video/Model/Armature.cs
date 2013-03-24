using System.Collections.Generic;
using Reign.Core;
using System.IO;

namespace Reign.Video
{
	public class Bone
	{
		#region Properties
		public string Name;
		public Bone Parent;
		public bool InheritScale, InheritRotation;
		public Vector3 Position;
		public Matrix3 Orientation;
		#endregion

		#region Constructors
		public Bone()
		{
			
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareBone softwareBone)
		{
			writer.Write(softwareBone.Name);
			writer.Write(softwareBone.Parent != null);
			if (softwareBone.Parent != null) writer.Write(softwareBone.Parent.Name);

			writer.Write(softwareBone.InheritScale);
			writer.Write(softwareBone.InheritRotation);
			writer.WriteVector(softwareBone.Position);
			writer.WriteMatrix(softwareBone.Orientation);
		}
		#endregion
	}

	public class Armature
	{
		#region Properties
		public string Name;
		public Bone[] Bones;
		#endregion

		#region Constructors
		public Armature()
		{
			
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