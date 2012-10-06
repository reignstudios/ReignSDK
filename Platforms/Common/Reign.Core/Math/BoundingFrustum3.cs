using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Reign.Core
{
	public enum ContainmentTypes
	{
		Contains,
		Intersects,
		Disjoint
	}

	public class BoundingFrustum3
	{
		#region Properties
		public Plane3 Left, Right, Bottom, Top, Near, Far;
		public Matrix4 Matrix;
		#endregion

		#region Constructors
		public BoundingFrustum3(Matrix4 matrix)
		{
			
		}
		#endregion

		#region Methods
		public ContainmentTypes Contains(BoundingBox3 boundingBox)
		{
			throw new NotImplementedException();
		}

		public static void Contains(ref BoundingFrustum3 boundingFrustum, ref BoundingBox3 boundingBox, out ContainmentTypes result)
		{
			throw new NotImplementedException();
		}

		public ContainmentTypes Contains(BoundingSphere3 boundingSphere)
		{
			throw new NotImplementedException();
		}

		public static void Contains(ref BoundingFrustum3 boundingFrustum, ref BoundingSphere3 boundingSphere, out ContainmentTypes result)
		{
			throw new NotImplementedException();
		}

		public ContainmentTypes Contains(Vector3 point)
		{
			throw new NotImplementedException();
		}

		public static void Contains(ref BoundingFrustum3 boundingFrustum, ref Vector3 point, out ContainmentTypes result)
		{
			throw new NotImplementedException();
		}

		public ContainmentTypes Contains(BoundingFrustum3 boundingFrustum)
		{
			throw new NotImplementedException();
		}

		public static void Contains(ref BoundingFrustum3 boundingFrustum1, ref BoundingFrustum3 boundingFrustum2, out ContainmentTypes result)
		{
			throw new NotImplementedException();
		}

		public bool Intersects(BoundingBox3 boundingBox)
		{
			throw new NotImplementedException();
		}

		public static void Intersects(ref BoundingFrustum3 boundingFrustum, ref BoundingBox3 boundingBox, out bool result)
		{
			throw new NotImplementedException();
		}

		public bool Intersects(BoundingSphere3 boundingSphere)
		{
			throw new NotImplementedException();
		}

		public static void Intersects(ref BoundingFrustum3 boundingFrustum, ref BoundingSphere3 boundingSphere, out bool result)
		{
			throw new NotImplementedException();
		}

		public bool Intersects(Vector3 point)
		{
			throw new NotImplementedException();
		}

		public static void Intersects(ref BoundingFrustum3 boundingFrustum, ref Vector3 point, out bool result)
		{
			throw new NotImplementedException();
		}

		public bool Intersects(BoundingFrustum3 boundingFrustum)
		{
			throw new NotImplementedException();
		}

		public static void Intersects(ref BoundingFrustum3 boundingFrustum1, ref BoundingFrustum3 boundingFrustum2, out bool result)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}