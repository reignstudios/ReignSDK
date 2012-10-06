using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct RigidTransform3
	{
		#region Properties
		public Quaternion Orientation;
		public Vector3 Position;
		#endregion

		#region Constructors
		public RigidTransform3(Quaternion orienation, Vector3 position)
        {
            Orientation = orienation;
			Position = position;
        }

		public RigidTransform3(Vector3 position)
        {
            Position = position;
            Orientation = Quaternion.Identity;
        }

		public RigidTransform3(Quaternion orienation)
        {
            Position = new Vector3();
            Orientation = orienation;
        }

		public static readonly RigidTransform3 Identity = new RigidTransform3(Quaternion.Identity, new Vector3());
		#endregion

		#region Methods
		public static void Invert(ref RigidTransform3 transform, out RigidTransform3 result)
        {
            Quaternion.Conjugate(ref transform.Orientation, out result.Orientation);
            Vector3.Transform(ref transform.Position, ref result.Orientation, out result.Position);
			result.Position = -result.Position;
        }

		public static void Transform(ref RigidTransform3 transform1, ref RigidTransform3 transform2, out RigidTransform3 result)
        {
			Vector3.Transform(ref transform1.Position, ref transform2.Orientation, out result.Position);
			result.Position += transform2.Position;
            Quaternion.Concatenate(ref transform1.Orientation, ref transform2.Orientation, out result.Orientation);
        }
		#endregion
	}
}