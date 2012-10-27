using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct BoundingSphere2
	{
		#region Properties
		public Vector2 Center;
		public float Radius;
		#endregion

		#region Constructors
		public BoundingSphere2(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
		#endregion

		#region Methods
		public bool Intersects(BoundingBox2 boundingBox)
        {
		   Vector2 clampedLocation;
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

            return clampedLocation.DistanceSquared(Center) <= (Radius * Radius);
        }

		public static void Intersects(ref BoundingSphere2 boundingSphere, ref BoundingBox2 boundingBox, out bool result)
        {
		   Vector2 clampedLocation;
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
		#endregion
	}
}