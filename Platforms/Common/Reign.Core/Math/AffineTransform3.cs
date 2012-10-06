using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct AffineTransform3
	{
		#region Properties
		public Matrix3 Transform;
		public Vector3 Translation;
		#endregion

		#region Constructors
		public AffineTransform3(Matrix3 transform, Vector3 translation)
        {
            Transform = transform;
            Translation = translation;
        }

		public AffineTransform3(Vector3 translation)
        {
            Transform = Matrix3.Identity;
            Translation = translation;
        }

		public AffineTransform3(Quaternion orientation, Vector3 translation)
        {
            Transform = Matrix3.FromQuaternion(orientation);
            Translation = translation;
        }

		public AffineTransform3(Quaternion orientation, Vector3 scale, Vector3 translation)
        {
			Transform = Matrix3.FromQuaternion(orientation) * scale;
            Translation = translation;
        }

		public static void FromRigidTransform(ref RigidTransform3 transform, out AffineTransform3 result)
        {
            result.Translation = transform.Position;
            Matrix3.FromQuaternion(ref transform.Orientation, out result.Transform);
        }

		public static readonly AffineTransform3 Identity = new AffineTransform3(Matrix3.Identity, new Vector3());
		#endregion

		#region Methods
		public AffineTransform3 Invert()
        {
			AffineTransform3 result;
            Matrix3.Invert(ref Transform, out result.Transform);
			Vector3.Transform(ref Translation, ref result.Transform, out result.Translation);
			result.Translation = -result.Translation;
			return result;
        }

		public static void Invert(ref AffineTransform3 transform, out AffineTransform3 result)
        {
            Matrix3.Invert(ref transform.Transform, out result.Transform);
			Vector3.Transform(ref transform.Translation, ref result.Transform, out result.Translation);
			result.Translation = -result.Translation;
        }

		public AffineTransform3 Multiply(ref RigidTransform3 transform)
        {
			AffineTransform3 result;
            Matrix3.FromQuaternion(ref transform.Orientation, out result.Transform);
            Matrix3.Multiply(ref result.Transform, ref Transform, out result.Transform);
			Vector3.Transform(ref transform.Position, ref Transform, out result.Translation);
			result.Translation += Translation;
			return result;
        }

		public static void Multiply(ref RigidTransform3 rigidTransform, ref AffineTransform3 affineTransform, out AffineTransform3 result)
        {
            Matrix3.FromQuaternion(ref rigidTransform.Orientation, out result.Transform);
            Matrix3.Multiply(ref result.Transform, ref affineTransform.Transform, out result.Transform);
			Vector3.Transform(ref rigidTransform.Position, ref affineTransform.Transform, out result.Translation);
			result.Translation +=  affineTransform.Translation;
        }
		#endregion
	}
}