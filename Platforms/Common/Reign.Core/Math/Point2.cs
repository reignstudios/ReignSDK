using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Point2
	{
		#region Properties
		public int X, Y;
		#endregion

		#region Constructors
		public Point2(int x, int y)
		{
			X = x;
			Y = y;
		}
		#endregion

		#region Operators
		// +
		public static Point2 operator+(Point2 p1, Point2 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			return p1;
		}

		public static Point2 operator+(Point2 p1, int p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Point2 operator+(int p1, Point2 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		// -
		public static Point2 operator-(Point2 p1, Point2 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			return p1;
		}

		public static Point2 operator-(Point2 p1, int p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Point2 operator-(int p1, Point2 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Point2 operator-(Point2 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			return p2;
		}

		// *
		public static Point2 operator*(Point2 p1, Point2 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			return p1;
		}

		public static Point2 operator*(Point2 p1, int p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Point2 operator*(int p1, Point2 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		// /
		public static Point2 operator/(Point2 p1, Point2 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			return p1;
		}

		public static Point2 operator/(Point2 p1, int p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Point2 operator/(int p1, Point2 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		// ==
		public static bool operator==(Point2 p1, Point2 p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Point2 p1, Point2 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}

		// convert
		public Vector2 ToVector2()
		{
			return new Vector2(X, Y);
		}

		public static void ToVector2(ref Point2 point, out Vector2 result)
		{
			result.X = point.X;
			result.Y = point.Y;
		}
		#endregion

		#region Methods
		public bool Intersects(Rect2 rect)
		{
			return X >= rect.Left && X <= rect.Right && Y >= rect.Bottom && Y <= rect.Top;
		}

		public void Intersects(Rect2 rect, out bool result)
		{
			result = X >= rect.Left && X <= rect.Right && Y >= rect.Bottom && Y <= rect.Top;
		}
		#endregion
	}
}