using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public class SoftwareBone
	{
		public string Name;
		public SoftwareBone Parent;
		public bool InheritScale, InheritRotation;
		public Vector3 Position;
		public Matrix3 Orientation;

		public SoftwareBone(RMX_ArmatureBone bone)
		{
			Name = bone.Name;
			InheritScale = bone.InheritScale;
			InheritRotation = bone.InheritRotation;
			Position = new Vector3(bone.Position.Values[0], bone.Position.Values[1], bone.Position.Values[2]);
			Orientation = new Matrix3
			(
				new Vector3(bone.Orientation.Values[0], bone.Orientation.Values[1], bone.Orientation.Values[2]),
				new Vector3(bone.Orientation.Values[3], bone.Orientation.Values[4], bone.Orientation.Values[5]),
				new Vector3(bone.Orientation.Values[6], bone.Orientation.Values[7], bone.Orientation.Values[8])
			);
		}

		internal void linkObjects(RMX_ArmatureBone bone, List<SoftwareBone> bones)
		{
			if (!string.IsNullOrEmpty(bone.Parent))
			{
				foreach (var parent in bones)
				{
					if (bone.Parent == parent.Name)
					{
						Parent = parent;
						break;
					}
				}
				if (Parent == null) Debug.ThrowError("SoftwareObject", "Failed to find Parent: " + bone.Parent);
			}
		}
	}

	public class SoftwareArmature
	{
		#region Properties
		public string Name;
		public List<SoftwareBone> Bones;
		#endregion

		#region Constructors
		public SoftwareArmature(RMX_Armature armature)
		{
			Name = armature.Name;

			Bones = new List<SoftwareBone>();
			foreach (var bone in armature.Bones.Bones)
			{
				Bones.Add(new SoftwareBone(bone));
			}

			int i = 0;
			foreach (var bone in Bones)
			{
				bone.linkObjects(armature.Bones.Bones[i], Bones);
				++i;
			}
		}
		#endregion
	}
}