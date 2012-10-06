using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Plane3
	{
		#region Properties
		public Vector3 Normal;
		public float Distance;
		#endregion

		#region Constructors
		public Plane3(Vector3 normal, float distance)
        {
            Normal = normal;
            Distance = distance;
        }
		#endregion

		#region Methods
		public float DotCoordinate(Vector3 vector)
        {
            return (Normal.X * vector.X) + (Normal.Y * vector.Y) + (Normal.Z * vector.Z) + Distance;
        }

		public static void DotCoordinate(ref Plane3 plane, ref Vector3 vector, out float result)
        {
            result = (plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y) + (plane.Normal.Z * vector.Z) + plane.Distance;
        }
		#endregion
	}
}