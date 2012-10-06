using Reign.Core;

namespace Reign.Physics
{
	public class Transform
	{
		public Vector3 Location, Velocity, Force;
	}

	public class RigidTransform : Transform
	{
		public Matrix3 RotationMatrix;
		public Vector3 AngularVelocity, Torque;

		public RigidTransform()
		{
			RotationMatrix = Matrix3.Identity;
		}

		public RigidTransform(Vector3 angularVelocity, Vector3 torque)
		{
			AngularVelocity = angularVelocity;
			Torque = torque;
			RotationMatrix = Matrix3.Identity;
		}

		public void AddForce(Vector3 force, Vector3 location, float radius)
		{
			var normal = (Location - location).Normalize();
			var angularForce = force.InersectPlane(normal);
			Force += angularForce - force;
			Torque += (normal.Cross(angularForce) * MathUtilities.PiQuarterDelta) / radius;
		}
	}
}
