using Reign.Core;

namespace Reign.Physics
{
	public class Collider
	{
		public float Radius;
	}

	public class CylinderCollider : Collider
	{
		public Line3 Line;
		public float Diameter;
	}
}
