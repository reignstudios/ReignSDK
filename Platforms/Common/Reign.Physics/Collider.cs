using Reign.Core;

namespace Reign.Physics
{
	public class Collider
	{
		public float Radius;

		public Collider(float radius)
		{
			Radius = radius;
		}
	}

	public class CylinderCollider : Collider
	{
		public Line3 Line;
		public float Diameter;

		public CylinderCollider(float radius, Line3 line, float diameter)
		: base(radius)
		{
			Line = line;
			Diameter = diameter;
		}
	}
}
