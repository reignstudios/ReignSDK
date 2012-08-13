using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Line3
	{
		#region Properties
		public Vector3 P1, P2;
		#endregion

		#region Constructors
		public Line3(Vector3 p1, Vector3 p2)
		{
			P1 = p1;
			P2 = p2;
		}
		#endregion

		#region Methods
		public Vector3 InersectPlane(Vector3 planeNormal, Vector3 planeLocation)
		{
			float dot = (-(planeNormal.X*planeLocation.X) - (planeNormal.Y*planeLocation.Y) - (planeNormal.Z*planeLocation.Z));
			float dot3 = (planeNormal.X*(P2.X-P1.X)) + (planeNormal.Y*(P2.Y-P1.Y)) + (planeNormal.Z*(P2.Z-P1.Z));
			float dot2 = -((dot + (planeNormal.X*P1.X) + (planeNormal.Y*P1.Y) + (planeNormal.Z*P1.Z)) / dot3);
			return (P1 + (dot2*(P2-P1)));
		}

		//public bool InersectTriangle(out Vector3f pInersectPoint, Vector3f pPolygonPoint1, Vector3f pPolygonPoint2, Vector3f pPolygonPoint3, Vector3f pPolygonNormal, Bound3D pPolygonBoundingBox, Line3f pLine)
		//{
		//    pInersectPoint = Inersect(pPolygonNormal, pPolygonPoint1, pLine);
		//    if (pInersectPoint.WithinTriangle(pPolygonBoundingBox) == false) return false;
		//    return Within(pPolygonPoint1, pPolygonPoint2, pPolygonPoint3);
		//}

		public Line3 Inersect(Line3 line)
		{
		   Vector3 Vector1 = (P1 - line.P1), vector2 = (line.P2 - line.P1), vector3 = (P2 - P1);
		   float Dot1 = Vector1.Dot(vector2);
		   float Dot2 = vector2.Dot(vector3);
		   float Dot3 = Vector1.Dot(vector3);
		   float Dot4 = vector2.Dot();
		   float Dot5 = vector3.Dot();
		   float Mul1 = (((Dot1 * Dot2) - (Dot3 * Dot4)) / ((Dot5 * Dot4) - (Dot2 * Dot2)));
		   float Mul2 = (Dot1 + (Dot2 * Mul1)) / Dot4;
		   return new Line3((P1 + (Mul1 * vector3)), (line.P1 + (Mul2 * vector2)));
		}
		#endregion
	}
}