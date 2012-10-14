using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingBox3
	{
		#region Properties
		public Vector3 Min, Max;

		public float Left
		{
			get {return Min.X;}
			set {Min.X = value;}
		}

		public float Bottom
		{
			get {return Min.Y;}
			set {Min.Y = value;}
		}

		public float Back
		{
			get {return Min.Z;}
			set {Min.Z = value;}
		}

		public float Right
		{
			get {return Max.X;}
			set {Max.X = value;	}
		}

		public float Top
		{
			get {return Max.Y;}
			set {Max.Y = value;}
		}

		public float Front
		{
			get {return Max.Z;}
			set {Max.Z = value;}
		}
		#endregion

		#region Constructors
		public BoundingBox3(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

		public static BoundingBox3 FromPoints(IList<Vector3> points)
        {
            BoundingBox3 boundingBox;
			#if DEBUG
			if (points.Count == 0) Debug.ThrowError("BoundingBox3", "Cannot construct a bounding box from an empty list");
			#endif
            boundingBox.Min = points[0];
            boundingBox.Max = boundingBox.Min;
			foreach (var point in points)
            {
                if (point.X < boundingBox.Min.X)
				{
                    boundingBox.Min.X = point.X;
				}
                else if (point.X > boundingBox.Max.X)
				{
                    boundingBox.Max.X = point.X;
				}

                if (point.Y < boundingBox.Min.Y)
				{
                    boundingBox.Min.Y = point.Y;
				}
                else if (point.Y > boundingBox.Max.Y)
				{
                    boundingBox.Max.Y = point.Y;
				}

                if (point.Z < boundingBox.Min.Z)
				{
                    boundingBox.Min.Z = point.Z;
				}
                else if (point.Z > boundingBox.Max.Z)
				{
                    boundingBox.Max.Z = point.Z;
				}
            }

            return boundingBox;
        }
		#endregion

		#region Methods
		public bool Intersects(BoundingBox3 boundingBox)
        {
            return
				!(boundingBox.Min.X > Max.X || boundingBox.Min.Y > Max.Y || boundingBox.Min.Z > Max.Z ||
				  Min.X > boundingBox.Max.X || Min.Y > boundingBox.Max.Y || Min.Z > boundingBox.Max.Z);
        }

		public static void Intersects(ref BoundingBox3 boundingBox1, ref BoundingBox3 boundingBox2, out bool result)
        {
            result =
				!(boundingBox1.Min.X > boundingBox2.Max.X || boundingBox1.Min.Y > boundingBox2.Max.Y || boundingBox1.Min.Z > boundingBox2.Max.Z ||
				  boundingBox2.Min.X > boundingBox1.Max.X || boundingBox2.Min.Y > boundingBox1.Max.Y || boundingBox2.Min.Z > boundingBox1.Max.Z);
        }

		public bool Intersects(BoundingSphere3 boundingSphere)
        {
		   Vector3 clampedLocation;
		   clampedLocation.w = 0;
            if (boundingSphere.Center.X > Max.X)
			{
                clampedLocation.X = Max.X;
			}
            else if (boundingSphere.Center.X < Min.X)
			{
                clampedLocation.X = Min.X;
			}
            else
			{
                clampedLocation.X = boundingSphere.Center.X;
			}

            if (boundingSphere.Center.Y > Max.Y)
			{
                clampedLocation.Y = Max.Y;
			}
            else if (boundingSphere.Center.Y < Min.Y)
			{
                clampedLocation.Y = Min.Y;
			}
            else
			{
                clampedLocation.Y = boundingSphere.Center.Y;
			}

            if (boundingSphere.Center.Z > Max.Z)
			{
                clampedLocation.Z = Max.Z;
			}
            else if (boundingSphere.Center.Z < Min.Z)
			{
                clampedLocation.Z = Min.Z;
			}
            else
			{
                clampedLocation.Z = boundingSphere.Center.Z;
			}

            return clampedLocation.DistanceSquared(boundingSphere.Center) <= (boundingSphere.Radius * boundingSphere.Radius);
        }

		public static void Intersects(ref BoundingBox3 boundingBox, ref BoundingSphere3 boundingSphere, out bool result)
        {
		   Vector3 clampedLocation;
		   clampedLocation.w = 0;
            if (boundingSphere.Center.X > boundingBox.Max.X)
			{
                clampedLocation.X = boundingBox.Max.X;
			}
            else if (boundingSphere.Center.X < boundingBox.Min.X)
			{
                clampedLocation.X = boundingBox.Min.X;
			}
            else
			{
                clampedLocation.X = boundingSphere.Center.X;
			}

            if (boundingSphere.Center.Y > boundingBox.Max.Y)
			{
                clampedLocation.Y = boundingBox.Max.Y;
			}
            else if (boundingSphere.Center.Y < boundingBox.Min.Y)
			{
                clampedLocation.Y = boundingBox.Min.Y;
			}
            else
			{
                clampedLocation.Y = boundingSphere.Center.Y;
			}

            if (boundingSphere.Center.Z > boundingBox.Max.Z)
			{
                clampedLocation.Z = boundingBox.Max.Z;
			}
            else if (boundingSphere.Center.Z < boundingBox.Min.Z)
			{
                clampedLocation.Z = boundingBox.Min.Z;
			}
            else
			{
                clampedLocation.Z = boundingSphere.Center.Z;
			}

            result = clampedLocation.DistanceSquared(boundingSphere.Center) <= (boundingSphere.Radius * boundingSphere.Radius);
        }

		public BoundingBox3 Merge(BoundingBox3 boundingBox2)
        {
			BoundingBox3 result;
            if (Min.X < boundingBox2.Min.X) result.Min.X = Min.X;
            else result.Min.X = boundingBox2.Min.X;

            if (Min.Y < boundingBox2.Min.Y) result.Min.Y = Min.Y;
            else result.Min.Y = boundingBox2.Min.Y;

            if (Min.Z < boundingBox2.Min.Z) result.Min.Z = Min.Z;
            else result.Min.Z = boundingBox2.Min.Z;

            if (Max.X > boundingBox2.Max.X) result.Max.X = Max.X;
            else result.Max.X = boundingBox2.Max.X;

            if (Max.Y > boundingBox2.Max.Y) result.Max.Y = Max.Y;
            else result.Max.Y = boundingBox2.Max.Y;

            if (Max.Z > boundingBox2.Max.Z) result.Max.Z = Max.Z;
            else result.Max.Z = boundingBox2.Max.Z;

			result.Min.w = 0;
			result.Max.w = 0;
			return result;
        }

		public static void Merge(ref BoundingBox3 boundingBox1, ref BoundingBox3 boundingBox2, out BoundingBox3 result)
        {
            if (boundingBox1.Min.X < boundingBox2.Min.X) result.Min.X = boundingBox1.Min.X;
            else result.Min.X = boundingBox2.Min.X;

            if (boundingBox1.Min.Y < boundingBox2.Min.Y) result.Min.Y = boundingBox1.Min.Y;
            else result.Min.Y = boundingBox2.Min.Y;

            if (boundingBox1.Min.Z < boundingBox2.Min.Z) result.Min.Z = boundingBox1.Min.Z;
            else result.Min.Z = boundingBox2.Min.Z;

            if (boundingBox1.Max.X > boundingBox2.Max.X) result.Max.X = boundingBox1.Max.X;
            else result.Max.X = boundingBox2.Max.X;

            if (boundingBox1.Max.Y > boundingBox2.Max.Y) result.Max.Y = boundingBox1.Max.Y;
            else result.Max.Y = boundingBox2.Max.Y;

            if (boundingBox1.Max.Z > boundingBox2.Max.Z) result.Max.Z = boundingBox1.Max.Z;
            else result.Max.Z = boundingBox2.Max.Z;

			result.Min.w = 0;
			result.Max.w = 0;
        }
		#endregion
	}
}