using Reign.Core;

namespace Reign.Physics
{
	public static class Physics
	{
		public static void SimulateAsParticles(Body[] bodies, int simulateBodyCount, Time time)
		{
			float delta = time.Delta;
			for (int i = 0; i != simulateBodyCount; ++i)
			{
				var transform = bodies[i].Transform;
				transform.Velocity += transform.Force;
				transform.Location += transform.Velocity * delta;
				transform.Force = Vector3.Zero;
			}
		}
	}
}
