using Reign.Core;

namespace Reign.Physics
{
	public class Body
	{
		public Transform Transform;
		public Collider Collider;
		public Material Material;
		public float Mass;

		public Body()
		{
			Transform = new Transform();
			Collider = new Collider(1);
			Material = new Material(.5f, .5f, .5f);
			Mass = 1;
		}

		public Body(Transform transform, Collider collider, Material material, float mass)
		{
			Transform = transform;
			Collider = collider;
			Material = material;
			Mass = mass;
		}
	}
}
