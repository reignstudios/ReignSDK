using Reign.Core;

namespace Reign.Physics
{
	public class Transform
	{
		public Vector3 Location, Velocity, Force;
	}

	public class RigidTransform : Transform
	{
		public Vector3 Rotation, AngularVelocity, Torque;
	}
}
