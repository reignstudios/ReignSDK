using Reign.Core;

namespace Reign.Physics
{
	public static class Physics
	{
		public static Enviroment Enviroment;

		static Physics()
		{
			Enviroment = new Enviroment();
		}

		public static void SimulateAsParticles(Body[] bodies, int simulateBodyCount, Time time)
		{
			float delta = time.Delta;
			for (int i = 0; i != simulateBodyCount; ++i)
			{
				var body = bodies[i];
				var transform = body.Transform;
				transform.Force += Enviroment.Gravity;
				transform.Velocity += transform.Force;
				transform.Velocity *= 1 - (body.Material.Resistance * Enviroment.Resistance);
				transform.Location += transform.Velocity * delta;
				transform.Force = Vector3.Zero;
			}
		}

		public static void SimulateAsRigidBodies(Body[] bodies, int simulateBodyCount, Time time)
		{
			float delta = time.Delta;
			for (int i = 0; i != simulateBodyCount; ++i)
			{
				var body = bodies[i];
				var transform = (RigidTransform)body.Transform;
				body.Collider.ApplyRigidRotation(transform.RotationMatrix);
				// normal
				transform.Force += Enviroment.Gravity;
				transform.Velocity += transform.Force;
				transform.Velocity *= 1 - (body.Material.Resistance * Enviroment.Resistance);
				transform.Location += transform.Velocity * delta;
				transform.Force = Vector3.Zero;
				// rigid
				transform.AngularVelocity += transform.Torque;
				transform.AngularVelocity *= 1 - (body.Material.EnvironmentalAngularResistance * Enviroment.AngularResistance);
				float length;
				var normal = transform.AngularVelocity.NormalizeSafe(out length);
				if (length >= 0.0001f) transform.RotationMatrix = transform.RotationMatrix.RotateAround(ref normal, length * delta);
				transform.Torque = Vector3.Zero;
			}
		}
	}
}
