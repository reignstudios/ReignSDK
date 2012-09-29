

namespace Reign.Physics
{
	public class Material
	{
		public float Bounciness, Resistance, EnvironmentalAngularResistance;

		public Material(float bounciness, float resistance, float environmentalAngularResistance)
		{
			Bounciness = bounciness;
			Resistance = resistance;
			EnvironmentalAngularResistance = environmentalAngularResistance;
		}
	}
}
