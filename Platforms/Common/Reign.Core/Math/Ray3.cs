using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Ray3
	{
		#region Properties
		public Vector3 Origin, Direction;
		#endregion

		#region Constructors
		public Ray3(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }
		#endregion

		#region Methods
		public Vector3 InersectPlaneX(float planePosition)
		{
			if (Direction.X == 0) return Origin;
			if ((planePosition >= Origin.X && Direction.X <= Origin.X) || (planePosition <= Origin.X && Direction.X >= Origin.X)) return Origin;

			float dis = planePosition - Origin.X;
			float slopeY = Direction.Y / Direction.X;
			float slopeZ = Direction.Z / Direction.X;
			return new Vector3(planePosition, (slopeY * dis) + Origin.Y, (slopeZ * dis) + Origin.Z);
		}

		public Vector3 InersectPlaneY(float planePosition)
		{
			if (Direction.Y == 0) return Origin;
			if ((planePosition >= Origin.Y && Direction.Y <= Origin.Y) || (planePosition <= Origin.Y && Direction.Y >= Origin.Y)) return Origin;

			float dis = planePosition - Origin.Y;
			float slopeX = Direction.X / Direction.Y;
			float slopeZ = Direction.Z / Direction.Y;
			return new Vector3((slopeX * dis) + Origin.X, planePosition, (slopeZ * dis) + Origin.Z);
		}

		public Vector3 InersectPlaneZ(float planePosition)
		{
			if (Direction.Z == 0) return Origin;
			if ((planePosition >= Origin.Z && Direction.Z <= Origin.Z) || (planePosition <= Origin.Z && Direction.Z >= Origin.Z)) return Origin;

			float dis = planePosition - Origin.Z;
			float slopeX = Direction.X / Direction.Z;
			float slopeY = Direction.Y / Direction.Z;
			return new Vector3((slopeX * dis) + Origin.X, (slopeY * dis) + Origin.Y, planePosition);
		}

		public bool Intersects(ref BoundingBox3 boundingBox, out float result)
        {
			// X
            if (Math.Abs(Direction.X) < MathUtilities.Epsilon && (Origin.X < boundingBox.Min.X || Origin.X > boundingBox.Max.X))
            {
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
                return false;
            }
            float tmin = 0, tmax = float.MaxValue;
            float inverseDirection = 1 / Direction.X;
            float t1 = (boundingBox.Min.X - Origin.X) * inverseDirection;
            float t2 = (boundingBox.Max.X - Origin.X) * inverseDirection;
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }
            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
            if (tmin > tmax)
			{
				result = 0;
				return false;
			}

			// Y
            if (Math.Abs(Direction.Y) < MathUtilities.Epsilon && (Origin.Y < boundingBox.Min.Y || Origin.Y > boundingBox.Max.Y))
            {                
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
                return false;
            }
            inverseDirection = 1 / Direction.Y;
            t1 = (boundingBox.Min.Y - Origin.Y) * inverseDirection;
            t2 = (boundingBox.Max.Y - Origin.Y) * inverseDirection;
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }
            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
            if (tmin > tmax)
			{
				result = 0;
				return false;
			}

			// Z
            if (Math.Abs(Direction.Z) < MathUtilities.Epsilon && (Origin.Z < boundingBox.Min.Z || Origin.Z > boundingBox.Max.Z))
            {              
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
                return false;
            }
            inverseDirection = 1 / Direction.Z;
            t1 = (boundingBox.Min.Z - Origin.Z) * inverseDirection;
            t2 = (boundingBox.Max.Z - Origin.Z) * inverseDirection;
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }
            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
            if (tmin > tmax)
			{
				result = 0;
				return false;
			}
            result = tmin;

			return true;
        }

		public static void Intersects(ref Ray3 ray, ref BoundingBox3 boundingBox, out float? result)
        {
			// X
            if (Math.Abs(ray.Direction.X) < MathUtilities.Epsilon && (ray.Origin.X < boundingBox.Min.X || ray.Origin.X > boundingBox.Max.X))
            {
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = null;
                return;
            }
            float tmin = 0, tmax = float.MaxValue;
            float inverseDirection = 1 / ray.Direction.X;
            float t1 = (boundingBox.Min.X - ray.Origin.X) * inverseDirection;
            float t2 = (boundingBox.Max.X - ray.Origin.X) * inverseDirection;
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }
            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
            if (tmin > tmax)
			{
				result = null;
				return;
			}

			// Y
            if (Math.Abs(ray.Direction.Y) < MathUtilities.Epsilon && (ray.Origin.Y < boundingBox.Min.Y || ray.Origin.Y > boundingBox.Max.Y))
            {                
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = null;
                return;
            }
            inverseDirection = 1 / ray.Direction.Y;
            t1 = (boundingBox.Min.Y - ray.Origin.Y) * inverseDirection;
            t2 = (boundingBox.Max.Y - ray.Origin.Y) * inverseDirection;
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }
            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
            if (tmin > tmax)
			{
				result = null;
				return;
			}

			// Z
            if (Math.Abs(ray.Direction.Z) < MathUtilities.Epsilon && (ray.Origin.Z < boundingBox.Min.Z || ray.Origin.Z > boundingBox.Max.Z))
            {              
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = null;
                return;
            }
            inverseDirection = 1 / ray.Direction.Z;
            t1 = (boundingBox.Min.Z - ray.Origin.Z) * inverseDirection;
            t2 = (boundingBox.Max.Z - ray.Origin.Z) * inverseDirection;
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }
            tmin = Math.Max(tmin, t1);
            tmax = Math.Min(tmax, t2);
            if (tmin > tmax)
			{
				result = null;
				return;
			}
            result = tmin;
        }
		#endregion
	}
}