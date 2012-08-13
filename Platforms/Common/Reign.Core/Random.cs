namespace Reign.Core
{
	public static class Random
	{
		#region Properties
		private static System.Random random;
		#endregion

		#region Constructors
		static Random()
		{
			random = new System.Random();
		}
		#endregion

		#region Methods
		public static double RangeDouble(double min, double max)
		{
			double dis = max - min;
			return (random.NextDouble() * dis) + min;
		}

		public static float RangeFloat(float min, float max)
		{
			float dis = max - min;
			return (float)(random.NextDouble() * dis) + min;
		}

		public static int RangeInt(int min, int max)
		{
			int dis = max - min;
			return (int)(random.NextDouble() * dis) + min;
		}

		public static double NextDouble()
		{
			return random.NextDouble();
		}

		public static float NextFloat()
		{
			return (float)random.NextDouble();
		}

		public static int NextInt(int range)
		{
			return (int)(random.NextDouble() * range);
		}
		#endregion
	}
}