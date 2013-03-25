using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public class SoftwareObjectBoneGroup
	{
		public string Name;
		public int Index;

		public SoftwareObjectBoneGroup(RMX_ObjectBoneGroup boneGroup)
		{
			Name = boneGroup.Name;
			Index = boneGroup.Index;
		}
	}

	public abstract class SoftwareObject
	{
		#region Properties
		public SoftwareModel Model;
		public SoftwareObject Parent;
		public string Name;
		public Vector3 Position, Rotation, Scale;
		public SoftwareObjectArmature ArmatureObject;
		public SoftwareAction DefaultAction;
		public List<SoftwareObjectBoneGroup> BoneGroups;
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

			// find action
			if (o.DefaultAction != null)
			{
				foreach (var action in model.Actions)
				{
					if (o.DefaultAction.Name == action.Name)
					{
						DefaultAction = action;
						break;
					}
				}
				if (DefaultAction == null) Debug.ThrowError("SoftwareObjectArmature", "Failed to find Action: " + o.DefaultAction.Name);
			}

			// bone groups
			BoneGroups = new List<SoftwareObjectBoneGroup>();
			if (o.BoneGroups != null)
			{
				foreach (var bonegroup in o.BoneGroups.BoneGroups)
				{
					BoneGroups.Add(new SoftwareObjectBoneGroup(bonegroup));
				}
			}
		}

		internal void linkObjects(RMX_Object o)
		{
			// find parent
			if (!string.IsNullOrEmpty(o.Parent))
			{
				foreach (var parent in Model.Objects)
				{
					if (o.Parent == parent.Name)
					{
						Parent = parent;
						break;
					}
				}
				if (Parent == null) Debug.ThrowError("SoftwareObject", "Failed to find Parent: " + o.Parent);
			}

			// find armature object
			if (o.ArmatureObject != null)
			{
				foreach (var action in Model.Objects)
				{
					if (o.ArmatureObject.Name == action.Name)
					{
						ArmatureObject = (SoftwareObjectArmature)action;
						break;
					}
				}
				if (ArmatureObject == null) Debug.ThrowError("SoftwareObject", "Failed to find ArmatureObject: " + o.ArmatureObject.Name);
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