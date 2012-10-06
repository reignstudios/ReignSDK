using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Line3
	{
		#region Properties
		public Vector3 Point1, Point2;
		#endregion

		#region Constructors
		public Line3(Vector3 point1, Vector3 point2)
		{
			Point1 = point1;
			Point2 = point2;
		}
		#endregion

		#region Methods
		public Line3 Transform(Matrix3 matrix)
		{
			return new Line3(Point1.Transform(matrix), Point2.Transform(matrix));
		}

		public static void Transform(ref Line3 line, ref Matrix3 matrix, out Line3 result)
		{
			Vector3.Transform(ref line.Point1, ref matrix, out result.Point1);
			Vector3.Transform(ref line.Point2, ref matrix, out result.Point2);
		}

		public float Length()
		{
			return (Point1 - Point2).Length();
		}

		public static void Length(ref Line3 line, out float result)
		{
			result = (line.Point1 - line.Point2).Length();
		}

		public Vector3 InersectPlane(Vector3 planeNormal, Vector3 planeLocation)
		{
			float dot = (-(planeNormal.X*planeLocation.X) - (planeNormal.Y*planeLocation.Y) - (planeNormal.Z*planeLocation.Z));
			float dot3 = (planeNormal.X*(Point2.X-Point1.X)) + (planeNormal.Y*(Point2.Y-Point1.Y)) + (planeNormal.Z*(Point2.Z-Point1.Z));
			float dot2 = -((dot + (planeNormal.X*Point1.X) + (planeNormal.Y*Point1.Y) + (planeNormal.Z*Point1.Z)) / dot3);
			return (Point1 + (dot2*(Point2-Point1)));
		}

		public static void InersectPlane(ref Line3 line, ref Vector3 planeNormal, ref Vector3 planeLocation, out Vector3 result)
		{
			float dot = (-(planeNormal.X*planeLocation.X) - (planeNormal.Y*planeLocation.Y) - (planeNormal.Z*planeLocation.Z));
			float dot3 = (planeNormal.X*(line.Point2.X-line.Point1.X)) + (planeNormal.Y*(line.Point2.Y-line.Point1.Y)) + (planeNormal.Z*(line.Point2.Z-line.Point1.Z));
			float dot2 = -((dot + (planeNormal.X*line.Point1.X) + (planeNormal.Y*line.Point1.Y) + (planeNormal.Z*line.Point1.Z)) / dot3);
			result = (line.Point1 + (dot2*(line.Point2-line.Point1)));
		}

		//public bool InersectTriangle(out Vector3f pInersectPoint, Vector3f pPolygonPoint1, Vector3f pPolygonPoint2, Vector3f pPolygonPoint3, Vector3f pPolygonNormal, Bound3D pPolygonBoundingBox, Line3f pLine)
		//{
		//    pInersectPoint = Inersect(pPolygonNormal, pPolygonPoint1, pLine);
		//    if (pInersectPoint.WithinTriangle(pPolygonBoundingBox) == false) return false;
		//    return Within(pPolygonPoint1, pPolygonPoint2, pPolygonPoint3);
		//}

		public Line3 Inersect(Line3 line)
		{
		   Vector3 vector = (Point1 - line.Point1), vector2 = (line.Point2 - line.Point1), vector3 = (Point2 - Point1);
		   float dot1 = vector.Dot(vector2);
		   float dot2 = vector2.Dot(vector3);
		   float dot3 = vector.Dot(vector3);
		   float dot4 = vector2.Dot();
		   float dot5 = vector3.Dot();
		   float mul1 = (((dot1 * dot2) - (dot3 * dot4)) / ((dot5 * dot4) - (dot2 * dot2)));
		   float mul2 = (dot1 + (dot2 * mul1)) / dot4;
		   return new Line3((Point1 + (mul1 * vector3)), (line.Point1 + (mul2 * vector2)));
		}

		public static void Inersect(ref Line3 line1, ref Line3 line2, out Line3 result)
		{
		   Vector3 vector = (line1.Point1 - line2.Point1), vector2 = (line2.Point2 - line2.Point1), vector3 = (line1.Point2 - line1.Point1);
		   float dot1 = vector.Dot(vector2);
		   float dot2 = vector2.Dot(vector3);
		   float dot3 = vector.Dot(vector3);
		   float dot4 = vector2.Dot();
		   float dot5 = vector3.Dot();
		   float mul1 = (((dot1 * dot2) - (dot3 * dot4)) / ((dot5 * dot4) - (dot2 * dot2)));
		   float mul2 = (dot1 + (dot2 * mul1)) / dot4;
		   result = new Line3((line1.Point1 + (mul1 * vector3)), (line2.Point1 + (mul2 * vector2)));
		}
		#endregion
	}
}