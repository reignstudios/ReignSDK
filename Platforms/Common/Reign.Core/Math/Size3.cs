using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Size3
	{
		#region Properties
		public int Width, Height, Depth;
		#endregion

		#region Constructors
		public Size3(int width, int height, int depth)
		{
			Width = width;
			Height = height;
			Depth = depth;
		}
		#endregion

		#region Operators
		// +
		public static Size3 operator+(Size3 p1, Size3 p2)
		{
			p1.Width += p2.Width;
			p1.Height += p2.Height;
			p1.Depth += p2.Depth;
			return p1;
		}

		public static Size3 operator+(Size3 p1, int p2)
		{
			p1.Width += p2;
			p1.Height += p2;
			p1.Depth += p2;
			return p1;
		}

		public static Size3 operator+(int p1, Size3 p2)
		{
			p2.Width = p1 + p2.Width;
			p2.Height = p1 + p2.Height;
			p2.Depth = p1 + p2.Depth;
			return p2;
		}

		// -
		public static Size3 operator-(Size3 p1, Size3 p2)
		{
			p1.Width -= p2.Width;
			p1.Height -= p2.Height;
			p1.Depth -= p2.Depth;
			return p1;
		}

		public static Size3 operator-(Size3 p1, int p2)
		{
			p1.Width -= p2;
			p1.Height -= p2;
			p1.Depth -= p2;
			return p1;
		}

		public static Size3 operator-(int p1, Size3 p2)
		{
			p2.Width = p1 - p2.Width;
			p2.Height = p1 - p2.Height;
			p2.Depth = p1 - p2.Depth;
			return p2;
		}

		public static Size3 operator-(Size3 p2)
		{
			p2.Width = -p2.Width;
			p2.Height = -p2.Height;
			p2.Depth = -p2.Depth;
			return p2;
		}

		// *
		public static Size3 operator*(Size3 p1, Size3 p2)
		{
			p1.Width *= p2.Width;
			p1.Height *= p2.Height;
			p1.Depth *= p2.Depth;
			return p1;
		}

		public static Size3 operator*(Size3 p1, int p2)
		{
			p1.Width *= p2;
			p1.Height *= p2;
			p1.Depth *= p2;
			return p1;
		}

		public static Size3 operator*(int p1, Size3 p2)
		{
			p2.Width = p1 * p2.Width;
			p2.Height = p1 * p2.Height;
			p2.Depth = p1 * p2.Depth;
			return p2;
		}

		// /
		public static Size3 operator/(Size3 p1, Size3 p2)
		{
			p1.Width /= p2.Width;
			p1.Height /= p2.Height;
			p1.Depth /= p2.Depth;
			return p1;
		}

		public static Size3 operator/(Size3 p1, int p2)
		{
			p1.Width /= p2;
			p1.Height /= p2;
			p1.Depth /= p2;
			return p1;
		}

		public static Size3 operator/(int p1, Size3 p2)
		{
			p2.Width = p1 / p2.Width;
			p2.Height = p1 / p2.Height;
			p2.Depth = p1 / p2.Depth;
			return p2;
		}

		// ==
		public static bool operator==(Size3 p1, Size3 p2) {return (p1.Width==p2.Width && p1.Height==p2.Height && p1.Depth==p2.Depth);}
		public static bool operator!=(Size3 p1, Size3 p2) {return (p1.Width!=p2.Width || p1.Height!=p2.Height || p1.Depth!=p2.Depth);}
		#endregion
		
		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Size3)obj == this;
		}
		
		public override string ToString()
		{
			return string.Format("<{0}, {1}, {2}>", Width, Height, Depth);
		}
		
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		#endregion
	}
}