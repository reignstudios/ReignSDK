using Reign.Core;

namespace Reign.Physics
{
	public class Plane
	{
		public Vector3 Location, Normal;
		public Material Material;

		public Plane(Vector3 location, Vector3 normal, Material material)
		{
			Location = location;
			Normal = normal.Normalize();
			Material = material;
		}
	}
}
