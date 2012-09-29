using Reign.Core;

namespace Reign.Physics
{
	public static class Collision
	{
		public static void CollideAsParticles_IgnoreMass(Body[] particles)
		{
			int loop1 = particles.Length-1, loop2 = particles.Length;
			for (int i = 0; i != loop1; ++i)
			{
				var body = particles[i];
				var transform = body.Transform;
				var collider = body.Collider;
				for (int i2 = i+1; i2 != loop2; ++i2)
				{
					var body2 = particles[i2];
					var transform2 = body2.Transform;
					var collider2 = body2.Collider;

					var normal = transform2.Location - transform.Location;
					float dis;
					normal = normal.Normalize(out dis);
					float totalRadius = collider.Radius + collider2.Radius;
					if (dis < totalRadius)
					{
						var force = normal * (totalRadius - dis) * 8;
						transform.Force -= force;
						transform2.Force += force;
					}
				}
			}
		}

		public static void CollideAsSpheres(Body[] spheres)
		{
			int loop1 = spheres.Length-1, loop2 = spheres.Length;
			for (int i = 0; i != loop1; ++i)
			{
				var sphere1 = spheres[i];
				var transform = sphere1.Transform;
				var collider = sphere1.Collider;
				for (int i2 = i+1; i2 != loop2; ++i2)
				{
					var sphere2 = spheres[i2];
					var transform2 = sphere2.Transform;
					var collider2 = sphere2.Collider;

					var normal = transform2.Location - transform.Location;
					float dis;
					normal = normal.Normalize(out dis);
					float totalRadius = collider.Radius + collider2.Radius;
					if (dis < totalRadius)
					{
						applyFrictionalForces(sphere1, sphere2, normal);

						var center = sphere1.Transform.Location + (normal * sphere1.Collider.Radius);
						center += sphere2.Transform.Location - (normal * sphere2.Collider.Radius);
						center *= .5f;
						sphere1.Transform.Location = center - (normal * sphere1.Collider.Radius);
						sphere2.Transform.Location = center + (normal * sphere2.Collider.Radius);
					}
				}
			}
		}

		/*public static void CollideCylindersWithSpheres(Body[] cylinders, Body[] spheres)
		{
			
		}*/

		public static void CollideSpheresWithPlane(Body[] spheres, Plane plane)
		{
			foreach (var sphere in spheres)
			{
				var transform = sphere.Transform;
				var collider = sphere.Collider;

				var collisionPoint = transform.Location.InersectPlane(plane.Normal, plane.Location);
				var normal = transform.Location - collisionPoint;
				float dis;
				normal = normal.Normalize(out dis);
				if (dis < collider.Radius)
				{
					applyFrictionalForces(sphere, plane.Material, plane.Normal);
					transform.Location = collisionPoint + (plane.Normal * collider.Radius);
				}
			}
		}

		private static void applyFrictionalForces(Body body, Material staticBodyMaterial, Vector3 normal)
		{
			var transform = (RigidTransform)body.Transform;
			var material = body.Material;

			// get velocities
			var slidingVelocity = transform.Velocity.InersectPlane(normal);
			var reflectingVelocity = slidingVelocity - transform.Velocity;

			// reflect object
			transform.Force += reflectingVelocity + (reflectingVelocity * (material.Bounciness + staticBodyMaterial.Bounciness) * .5f);

			// add frictional force and torque
			float resistance = material.Resistance * staticBodyMaterial.Resistance;
			var angularResistanceForce = transform.AngularVelocity * resistance;
			var torque = (normal.Cross(slidingVelocity) * resistance * Math.PiQuarterDelta) / body.Collider.Radius;// calculate torque from sliding objects friction
			transform.Torque += torque;// add torque from sliding objects friction
			transform.Torque -= angularResistanceForce;// damp torque from rotational friction
			transform.Force -= normal.Cross(angularResistanceForce - torque) * Math.PiDelta * body.Collider.Radius;// transfer rotational friction into directional force
		}

		private static void applyFrictionalForces(Body body1, Body body2, Vector3 normal)
		{
			var transform1 = (RigidTransform)body1.Transform;
			var transform2 = (RigidTransform)body2.Transform;
			var material1 = body1.Material;
			var material2 = body2.Material;

			// get velocities
			var slidingVelocity1 = transform1.Velocity.InersectPlane(normal);
			var reflectingVelocity1 = slidingVelocity1 - transform1.Velocity;
			var slidingVelocity2 = transform2.Velocity.InersectPlane(normal);
			var reflectingVelocity2 = slidingVelocity2 - transform2.Velocity;

			// get mass ratios
			float totalMass = body1.Mass + body2.Mass;
			float massRatio1 = body1.Mass / totalMass;
			float massRatio2 = body2.Mass / totalMass;

			// add frictional force and torque
			float resistance = material1.Resistance * material2.Resistance;

			var angularResistanceForce1 = transform1.AngularVelocity * resistance;
			var angularResistanceForce2 = transform2.AngularVelocity * resistance;
			var torque1 = normal.Cross(slidingVelocity1) * resistance * Math.PiQuarterDelta;// calculate torque from sliding objects friction
			var torque2 = normal.Cross(slidingVelocity2) * resistance * Math.PiQuarterDelta;// calculate torque from sliding objects friction

			// reflect velocities
			if (transform1.Velocity.Dot(normal) >= 0)
			{
				var force = reflectingVelocity1;
				transform1.Force += force * massRatio2;
				transform2.Force -= force * massRatio1;
			}
			
			if (transform2.Velocity.Dot(normal) <= 0)
			{
				var force = reflectingVelocity2;
				transform2.Force += force * massRatio1;
				transform1.Force -= force * massRatio2;
			}

			var torque = torque1 + torque2;
			var angularResistanceForce = -angularResistanceForce1 - angularResistanceForce2;
			transform1.Torque += torque * massRatio2;// add torque from sliding objects friction
			transform1.Torque += angularResistanceForce * massRatio2;// damp torque from rotational friction
			transform2.Torque += torque * massRatio1;// add torque from sliding objects friction
			transform2.Torque += angularResistanceForce * massRatio1;// damp torque from rotational friction

			var directionalForce = normal.Cross(angularResistanceForce1 - torque1) * Math.PiDelta * body1.Collider.Radius * massRatio2;
			transform1.Force -= directionalForce;// transfer rotational friction into directional force

			directionalForce = normal.Cross(angularResistanceForce2 - torque2) * Math.PiDelta * body1.Collider.Radius * massRatio1;
			transform2.Force -= directionalForce;// transfer rotational friction into directional force
		}
	}
}
