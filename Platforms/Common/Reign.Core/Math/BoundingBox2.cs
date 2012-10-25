using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingBox2
	{
		#region Properties
		public Vector2 Min, Max;

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

		public static readonly BoundingBox2 Zero = new BoundingBox2();
		#endregion

		#region Constructors
		public BoundingBox2(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

		public static BoundingBox2 FromPoints(IList<Vector2> points)
        {
            BoundingBox2 boundingBox;
			#if DEBUG
			if (points.Count == 0) Debug.ThrowError("BoundingBox2", "Cannot construct a bounding box from an empty list");
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
            }

            return boundingBox;
        }
		#endregion

		#region Methods
		public bool Intersects(BoundingBox2 boundingBox)
        {
            return
				!(boundingBox.Min.X > Max.X || boundingBox.Min.Y > Max.Y ||
				  Min.X > boundingBox.Max.X || Min.Y > boundingBox.Max.Y);
        }

		public static void Intersects(ref BoundingBox2 boundingBox1, ref BoundingBox2 boundingBox2, out bool result)
        {
            result =
				!(boundingBox1.Min.X > boundingBox2.Max.X || boundingBox1.Min.Y > boundingBox2.Max.Y ||
				  boundingBox2.Min.X > boundingBox1.Max.X || boundingBox2.Min.Y > boundingBox1.Max.Y);
        }

		public bool Intersects(BoundingSphere2 boundingSphere)
        {
		   Vector2 clampedLocation;
		   clampedLocation.z = 0;
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

            return clampedLocation.DistanceSquared(boundingSphere.Center) <= (boundingSphere.Radius * boundingSphere.Radius);
        }

		public static void Intersects(ref BoundingBox2 boundingBox, ref BoundingSphere2 boundingSphere, out bool result)
        {
		   Vector2 clampedLocation;
		   clampedLocation.z = 0;
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

            result = clampedLocation.DistanceSquared(boundingSphere.Center) <= (boundingSphere.Radius * boundingSphere.Radius);
        }

		public BoundingBox2 Merge(BoundingBox2 boundingBox2)
        {
			BoundingBox2 result;
            if (Min.X < boundingBox2.Min.X) result.Min.X = Min.X;
            else result.Min.X = boundingBox2.Min.X;

            if (Min.Y < boundingBox2.Min.Y) result.Min.Y = Min.Y;
            else result.Min.Y = boundingBox2.Min.Y;

            if (Max.X > boundingBox2.Max.X) result.Max.X = Max.X;
            else result.Max.X = boundingBox2.Max.X;

            if (Max.Y > boundingBox2.Max.Y) result.Max.Y = Max.Y;
            else result.Max.Y = boundingBox2.Max.Y;

			result.Min.z = 0;
			result.Max.z = 0;
			result.Min.w = 0;
			result.Max.w = 0;
			return result;
        }

		public static void Merge(ref BoundingBox2 boundingBox1, ref BoundingBox2 boundingBox2, out BoundingBox2 result)
        {
            if (boundingBox1.Min.X < boundingBox2.Min.X) result.Min.X = boundingBox1.Min.X;
            else result.Min.X = boundingBox2.Min.X;

            if (boundingBox1.Min.Y < boundingBox2.Min.Y) result.Min.Y = boundingBox1.Min.Y;
            else result.Min.Y = boundingBox2.Min.Y;

            if (boundingBox1.Max.X > boundingBox2.Max.X) result.Max.X = boundingBox1.Max.X;
            else result.Max.X = boundingBox2.Max.X;

            if (boundingBox1.Max.Y > boundingBox2.Max.Y) result.Max.Y = boundingBox1.Max.Y;
            else result.Max.Y = boundingBox2.Max.Y;

			result.Min.z = 0;
			result.Max.z = 0;
			result.Min.w = 0;
			result.Max.w = 0;
        }
		#endregion
	}
}