using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Triangle
	{
		#region Properties
		public Vector3 P1, P2, P3;
		#endregion

		#region Constructors
		public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			P1 = p1;
			P2 = p2;
			P3 = p3;
		}
		#endregion

		#region Methods
		public Vector3 Normal()
		{
			Vector3 vector1 = (P1-P2), vector2 = (P3-P1);
			return new Vector3(-((vector1.Y*vector2.Z) - (vector1.Z*vector2.Y)), -((vector1.Z*vector2.X) - (vector1.X*vector2.Z)), -((vector1.X*vector2.Y) - (vector1.Y*vector2.X))).Normalize();
		}
		#endregion
	}
}