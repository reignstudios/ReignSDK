using Reign.Core;

namespace Reign.Physics
{
	public class Body
	{
		public Transform Transform;
		public Collider Collider;
		public float Mass;

		public Body()
		{
			Transform = new Transform();
			Collider = new Collider();
			Mass = 1;
		}

		public Body(Transform transform, Collider collider, float mass)
		{
			Transform = transform;
			Collider = collider;
			Mass = mass;
		}
	}
}
