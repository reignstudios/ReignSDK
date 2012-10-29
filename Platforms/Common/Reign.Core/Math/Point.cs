namespace Reign.Core
{
	public struct Point
	{
		#region Properties
		public int X, Y;
		#endregion

		#region Constructors
		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}
		#endregion

		#region Operators
		// +
		public static Point operator+(Point p1, Point p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			return p1;
		}

		public static Point operator+(Point p1, int p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Point operator+(int p1, Point p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		// -
		public static Point operator-(Point p1, Point p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			return p1;
		}

		public static Point operator-(Point p1, int p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Point operator-(int p1, Point p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Point operator-(Point p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			return p2;
		}

		// *
		public static Point operator*(Point p1, Point p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			return p1;
		}

		public static Point operator*(Point p1, int p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Point operator*(int p1, Point p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		// /
		public static Point operator/(Point p1, Point p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			return p1;
		}

		public static Point operator/(Point p1, int p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Point operator/(int p1, Point p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		// ==
		public static bool operator==(Point p1, Point p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Point p1, Point p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}

		// convert
		public Vector2 ToVector2()
		{
			return new Vector2(X, Y);
		}

		public static void ToVector2(ref Point point, out Vector2 result)
		{
			result.X = point.X;
			result.Y = point.Y;
		}
		#endregion
	}
}