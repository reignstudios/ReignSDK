using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Ray3
	{
		#region Properties
		public Vector3 Position, Direction;
		#endregion

		#region Constructors
		public Ray3(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }
		#endregion

		#region Methods
		public bool Intersects(ref BoundingBox3 boundingBox, out float result)
        {
			// X
            if (Math.Abs(Direction.X) < MathUtilities.Epsilon && (Position.X < boundingBox.Min.X || Position.X > boundingBox.Max.X))
            {
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
                return false;
            }
            float tmin = 0, tmax = float.MaxValue;
            float inverseDirection = 1 / Direction.X;
            float t1 = (boundingBox.Min.X - Position.X) * inverseDirection;
            float t2 = (boundingBox.Max.X - Position.X) * inverseDirection;
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
            if (Math.Abs(Direction.Y) < MathUtilities.Epsilon && (Position.Y < boundingBox.Min.Y || Position.Y > boundingBox.Max.Y))
            {                
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
                return false;
            }
            inverseDirection = 1 / Direction.Y;
            t1 = (boundingBox.Min.Y - Position.Y) * inverseDirection;
            t2 = (boundingBox.Max.Y - Position.Y) * inverseDirection;
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
            if (Math.Abs(Direction.Z) < MathUtilities.Epsilon && (Position.Z < boundingBox.Min.Z || Position.Z > boundingBox.Max.Z))
            {              
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = 0;
                return false;
            }
            inverseDirection = 1 / Direction.Z;
            t1 = (boundingBox.Min.Z - Position.Z) * inverseDirection;
            t2 = (boundingBox.Max.Z - Position.Z) * inverseDirection;
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
            if (Math.Abs(ray.Direction.X) < MathUtilities.Epsilon && (ray.Position.X < boundingBox.Min.X || ray.Position.X > boundingBox.Max.X))
            {
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = null;
                return;
            }
            float tmin = 0, tmax = float.MaxValue;
            float inverseDirection = 1 / ray.Direction.X;
            float t1 = (boundingBox.Min.X - ray.Position.X) * inverseDirection;
            float t2 = (boundingBox.Max.X - ray.Position.X) * inverseDirection;
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
            if (Math.Abs(ray.Direction.Y) < MathUtilities.Epsilon && (ray.Position.Y < boundingBox.Min.Y || ray.Position.Y > boundingBox.Max.Y))
            {                
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = null;
                return;
            }
            inverseDirection = 1 / ray.Direction.Y;
            t1 = (boundingBox.Min.Y - ray.Position.Y) * inverseDirection;
            t2 = (boundingBox.Max.Y - ray.Position.Y) * inverseDirection;
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
            if (Math.Abs(ray.Direction.Z) < MathUtilities.Epsilon && (ray.Position.Z < boundingBox.Min.Z || ray.Position.Z > boundingBox.Max.Z))
            {              
                //If the ray isn't pointing along the axis at all, and is outside of the box's interval, then it can't be intersecting.
				result = null;
                return;
            }
            inverseDirection = 1 / ray.Direction.Z;
            t1 = (boundingBox.Min.Z - ray.Position.Z) * inverseDirection;
            t2 = (boundingBox.Max.Z - ray.Position.Z) * inverseDirection;
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