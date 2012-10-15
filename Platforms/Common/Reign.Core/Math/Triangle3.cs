using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Triangle3
	{
		#region Properties
		public Vector3 Point1, Point2, Point3;
		#endregion

		#region Constructors
		public Triangle3(Vector3 point1, Vector3 point2, Vector3 point3)
		{
			Point1 = point1;
			Point2 = point2;
			Point3 = point3;
		}
		#endregion

		#region Methods
		public Vector3 Normal()
		{
			Vector3 vector1 = (Point1-Point2), vector2 = (Point3-Point1);
			return new Vector3(-((vector1.Y*vector2.Z) - (vector1.Z*vector2.Y)), -((vector1.Z*vector2.X) - (vector1.X*vector2.Z)), -((vector1.X*vector2.Y) - (vector1.Y*vector2.X))).Normalize();
		}

		public static void Normal(ref Triangle3 triangle, out Vector3 result)
		{
			Vector3 vector1 = (triangle.Point1-triangle.Point2), vector2 = (triangle.Point3-triangle.Point1);
			result = new Vector3(-((vector1.Y*vector2.Z) - (vector1.Z*vector2.Y)), -((vector1.Z*vector2.X) - (vector1.X*vector2.Z)), -((vector1.X*vector2.Y) - (vector1.Y*vector2.X))).Normalize();
		}
		#endregion
	}
}