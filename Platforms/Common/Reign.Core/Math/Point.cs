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
		public static Point operator+(Point p1, Point p2) {return new Point(p1.X+p2.X, p1.Y+p2.Y);}
		public static Point operator+(Point p1, int p2) {return new Point(p1.X+p2, p1.Y+p2);}
		public static Point operator+(int p1, Point p2) {return new Point(p1+p2.X, p1+p2.Y);}
		// -
		public static Point operator-(Point p1, Point p2) {return new Point(p1.X-p2.X, p1.Y-p2.Y);}
		public static Point operator-(Point p1, int p2) {return new Point(p1.X-p2, p1.Y-p2);}
		public static Point operator-(int p1, Point p2) {return new Point(p1-p2.X, p1-p2.Y);}
		public static Point operator-(Point p1) {return new Point(-p1.X, -p1.Y);}
		// *
		public static Point operator*(Point p1, Point p2) {return new Point(p1.X*p2.X, p1.Y*p2.Y);}
		public static Point operator*(Point p1, int p2) {return new Point(p1.X*p2, p1.Y*p2);}
		public static Point operator*(int p1, Point p2) {return new Point(p1*p2.X, p1*p2.Y);}
		// /
		public static Point operator/(Point p1, Point p2) {return new Point(p1.X/p2.X, p1.Y/p2.Y);}
		public static Point operator/(Point p1, int p2) {return new Point(p1.X/p2, p1.Y/p2);}
		public static Point operator/(int p1, Point p2) {return new Point(p1/p2.X, p1/p2.Y);}
		// ==
		public static bool operator==(Point p1, Point p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Point p1, Point p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}
		#endregion
	}
}