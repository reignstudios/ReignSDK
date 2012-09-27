using Reign.Core;

namespace Reign.Physics
{
	public class Particle : Body
	{
		public bool Dead;
		public float AliveSeconds, AliveTic;
	}

	public class ParticleSystem
	{
		private Particle[] particles;
		private int[] deadParticleIndices;
		public Particle[] Particles {get{return particles;}}
		private int activeCount;
		public int ActiveCount {get{return activeCount;}}

		public ParticleSystem(int maxParticleCount)
		{
			particles = new Particle[maxParticleCount];
			deadParticleIndices = new int[maxParticleCount];
			for (int i = 0; i != particles.Length; ++i)
			{
				particles[i] = new Particle();
			}
		}

		public void Update(Time time)
		{
			// simulate physics
			Physics.SimulateAsParticles(particles, activeCount, time);

			// update particle data
			float delta = time.Delta;
			int deadCount = 0;
			for (int i = 0; i != activeCount; ++i)
			{
				var particle = particles[i];
				if (particle.AliveTic <= 0 || particle.Dead)
				{
					deadParticleIndices[deadCount] = i;
					++deadCount;
					continue;
				}
				
				particle.AliveTic -= delta;
			}

			// remove dead particles
			for (int i = 0; i != deadCount; ++i)
			{
				int pi = deadParticleIndices[i];
				var particle = particles[pi];
				particle.Dead = true;

				--activeCount;
				particles[pi] = particles[activeCount];
				particles[activeCount] = particle;
			}
		}

		public void Emit(float minVelocity, float maxVelocity, float aliveSeconds)
		{
			if (activeCount == particles.Length) return;

			var particle = particles[activeCount];
			particle.Dead = false;
			particle.AliveSeconds = aliveSeconds;
			particle.AliveTic = 0;

			var normal = new Vector3(Random.RangeFloat(-1, 1), Random.RangeFloat(-1, 1), Random.RangeFloat(-1, 1)).Normalize();
			particle.Transform.Velocity = normal * Random.RangeFloat(minVelocity, maxVelocity);

			++activeCount;
		}
	}
}
