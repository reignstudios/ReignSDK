using Reign.Core;

namespace Reign.Physics
{
	public class Enviroment
	{
		public Vector3 Gravity;
		public float Resistance, AngularResistance;

		public Enviroment()
		{
			Gravity = new Vector3(0, -.5f, 0);
			Resistance = .1f;
			AngularResistance = .1f;
		}
	}
}
