using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingSphere3
	{
		#region Properties
		public Vector3 Center;
		public float Radius;
		#endregion

		#region Constructors
		public BoundingSphere3(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
		#endregion

		#region Methods
		public bool Intersects(BoundingBox3 boundingBox)
        {
		   Vector3 clampedLocation;
		   clampedLocation.w = 0;
            if (Center.X > boundingBox.Max.X)
			{
                clampedLocation.X = boundingBox.Max.X;
			}
            else if (Center.X < boundingBox.Min.X)
			{
                clampedLocation.X = boundingBox.Min.X;
			}
            else
			{
                clampedLocation.X = Center.X;
			}

            if (Center.Y > boundingBox.Max.Y)
			{
                clampedLocation.Y = boundingBox.Max.Y;
			}
            else if (Center.Y < boundingBox.Min.Y)
			{
                clampedLocation.Y = boundingBox.Min.Y;
			}
            else
			{
                clampedLocation.Y = Center.Y;
			}

            if (Center.Z > boundingBox.Max.Z)
			{
                clampedLocation.Z = boundingBox.Max.Z;
			}
            else if (Center.Z < boundingBox.Min.Z)
			{
                clampedLocation.Z = boundingBox.Min.Z;
			}
            else
			{
                clampedLocation.Z = Center.Z;
			}

            return clampedLocation.DistanceSquared(Center) <= (Radius * Radius);
        }

		public static void Intersects(ref BoundingSphere3 boundingSphere, ref BoundingBox3 boundingBox, out bool result)
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
		#endregion
	}
}