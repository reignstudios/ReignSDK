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

		public Point2(int value)
		{
			X = value;
			Y = value;
		}

		public static readonly Point2 One = new Point2(1);
		public static readonly Point2 MinusOne = new Point2(-1);
		public static readonly Point2 Zero = new Point2(0);
		public static readonly Point2 Right = new Point2(1, 0);
		public static readonly Point2 Left = new Point2(-1, 0);
		public static readonly Point2 Up = new Point2(0, 1);
		public static readonly Point2 Down = new Point2(0, -1);
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
		public override bool Equals(object obj)
		{
			return obj != null && (Point2)obj == this;
		}
		
		public override string ToString()
		{
			return string.Format("<{0}, {1}>", X, Y);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public bool Intersects(Rect2 rect)
		{
			return X >= rect.Left && X <= rect.Right && Y >= rect.Bottom && Y <= rect.Top;
		}

		public void Intersects(Rect2 rect, out bool result)
		{
			result = X >= rect.Left && X <= rect.Right && Y >= rect.Bottom && Y <= rect.Top;
		}

		public bool Intersects(BoundingBox2 boundingBox)
		{
			return X >= boundingBox.Left && X <= boundingBox.Right && Y >= boundingBox.Bottom && Y <= boundingBox.Top;
		}

		public void Intersects(BoundingBox2 boundingBox, out bool result)
		{
			result = X >= boundingBox.Left && X <= boundingBox.Right && Y >= boundingBox.Bottom && Y <= boundingBox.Top;
		}
		#endregion
	}
}