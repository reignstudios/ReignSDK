namespace Reign.Core
{
	public class Random
	{
		#region Properties
		private System.Random random;
		#endregion

		#region Constructors
		public Random()
		{
			random = new System.Random();
		}
		#endregion

		#region Methods
		public double RangeDouble(double min, double max)
		{
			double dis = max - min;
			return (random.NextDouble() * dis) + min;
		}

		public float RangeFloat(float min, float max)
		{
			float dis = max - min;
			return (float)(random.NextDouble() * dis) + min;
		}

		public int RangeInt(int min, int max)
		{
			int dis = max - min;
			return (int)(random.NextDouble() * dis) + min;
		}

		public double NextDouble()
		{
			return random.NextDouble();
		}

		public float NextFloat()
		{
			return (float)random.NextDouble();
		}

		public int NextInt(int range)
		{
			return (int)(random.NextDouble() * range);
		}
		#endregion
	}
}