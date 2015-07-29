using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.Abstraction
{
	public class InstanceBone
	{
		public Bone Bone {get; private set;}
		public InstanceBone Parent {get; private set;}
		public List<InstanceBone> Childeren {get; private set;}

		public Vector3 Position;
		public Quaternion Rotation;
		public Matrix3 RotationMatrix;

		public InstanceBone(Bone bone)
		{
			Bone = bone;
			Position = bone.Position;
			RotationMatrix = bone.RotationMatrix;
			Childeren = new List<InstanceBone>();
		}

		internal void linkObjects(InstanceBone[] bones)
		{
			if (Bone.Parent == null) return;

			foreach (var bone in bones)
			{
				if (Bone.Parent == bone.Bone)
				{
					Parent = bone;
					break;
				}
			}
		}

		internal void linkChilderen(InstanceBone[] bones)
		{
			foreach (var bone in bones)
			{
				if (bone.Parent == this) Childeren.Add(bone);
			}
		}

		internal void applyBoneToChilderen()
		{
			foreach (var child in Childeren)
			{
				//child.Rotation = child.Bone.Rotation.Multiply(this.Rotation);
				child.Position = (child.Bone.Position - this.Position).Transform(this.Rotation) + this.Position;

				child.applyBoneToChilderen();
			}
		}
	}

	public class InstanceObjectArmature : InstanceObject
	{
		#region Properties
		public Armature Armature {get; private set;}
		public InstanceBone[] Bones {get; private set;}
		#endregion

		#region Constructors
		public InstanceObjectArmature(ObjectArmature o)
		: base(o)
		{
			Armature = o.Armature;

			Bones = new InstanceBone[o.Armature.Bones.Length];
			for (int i = 0; i != Bones.Length; ++i)
			{
				Bones[i] = new InstanceBone(o.Armature.Bones[i]);
			}
		}
		#endregion

		#region Methods
		public override void Animate(float frame)
		{
			base.Animate(frame);

			// rotate bones by parent
			foreach (var bone in Bones)
			{
				bone.applyBoneToChilderen();

				//Quaternion.Multiply(ref bone.Rotation, ref bone.Parent.Rotation, out bone.Rotation);

				//bone.Rotation = bone.Bone.Rotation.Multiply(bone.Parent.Rotation);
				//bone.Position = bone.Bone.Position + bone.Position;
			}

			// generate final matrix
			foreach (var bone in Bones)
			{
				Matrix3.FromQuaternion(ref bone.Rotation, out bone.RotationMatrix);
			}
		}

		protected override void linkObjects(string dataPath, FCurve curve, int i)
		{
			bool pass = false;
			for (int bi = 0; bi != Armature.Bones.Length; ++bi)
			{
				if (dataPath == Armature.Bones[bi].Name)
				{
					boneIndices[i] = bi;

					if (curve.Index == 0) bindActionDatas[i] = bindBoneRotationX;
					else if (curve.Index == 1) bindActionDatas[i] = bindBoneRotationY;
					else if (curve.Index == 2) bindActionDatas[i] = bindBoneRotationZ;
					else if (curve.Index == 3) bindActionDatas[i] = bindBoneRotationW;

					pass = true;
					break;
				}
			}
			if (!pass) Debug.ThrowError("InstanceObjectArmature", "Failed to find Armature Bone: " + dataPath);

			// link bones
			foreach (var bone in Bones)
			{
				bone.linkObjects(Bones);
			}

			// link bone childeren
			foreach (var bone in Bones)
			{
				bone.linkChilderen(Bones);
			}
		}

		private void bindBoneRotationX(float value)
		{
			Bones[boneIndices[bindingIndex]].Rotation.X = value;
		}

		private void bindBoneRotationY(float value)
		{
			Bones[boneIndices[bindingIndex]].Rotation.Y = value;
		}

		private void bindBoneRotationZ(float value)
		{
			Bones[boneIndices[bindingIndex]].Rotation.Z = value;
		}

		private void bindBoneRotationW(float value)
		{
			Bones[boneIndices[bindingIndex]].Rotation.W = value;
		}
		#endregion
	}
}