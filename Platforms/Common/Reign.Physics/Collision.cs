using Reign.Core;

namespace Reign.Physics
{
	public static class Collision
	{
		public static void CollideAsParticles_IgnoreMass(Body[] particles1, Body[] particles2)
		{
			int loop1 = particles1.Length-1, loop2 = particles2.Length;
			for (int i = 0; i != loop1; ++i)
			{
				var body = particles1[i];
				var transform = body.Transform;
				var collider = body.Collider;
				for (int i2 = i+1; i2 != loop2; ++i2)
				{
					var body2 = particles2[i2];
					var transform2 = body2.Transform;
					var collider2 = body2.Collider;

					var vec = transform2.Location - transform.Location;
					float dis = vec.Length();
					float totalRadius = collider.Radius + collider2.Radius;
					if (dis <= totalRadius)
					{
						float force = totalRadius - dis;
						transform.Force -= force;
						transform2.Force += force;
					}
				}
			}
		}

		public static void CollideCylindersWithSpheres(Body[] cylinders, Body[] spheres)
		{
			/*int loop1 = cylinders.Length-1, loop2 = spheres.Length;
			for (int i = 0; i != loop1; ++i)
			{
				var body = cylinders[i];
				var transform = body.Transform;
				var collider = body.Collider;
				for (int i2 = i+1; i2 != loop2; ++i2)
				{
					var body2 = spheres[i2];
					var transform2 = body2.Transform;
					var collider2 = body2.Collider;

					var vec = transform2.Location - transform.Location;
					float dis = vec.Length();
					float totalRadius = collider.Radius + collider2.Radius;
					if (dis <= totalRadius)
					{
						float force = totalRadius - dis;
						transform.Force -= force;
						transform2.Force += force;
					}
				}
			}*/
		}
	}
}
