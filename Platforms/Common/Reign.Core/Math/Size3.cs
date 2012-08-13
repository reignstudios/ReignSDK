namespace Reign.Core
{
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
		public static Size3 operator+(Size3 p1, Size3 p2) {return new Size3(p1.Width+p2.Width, p1.Height+p2.Height, p1.Depth+p2.Depth);}
		public static Size3 operator+(Size3 p1, int p2) {return new Size3(p1.Width+p2, p1.Height+p2, p1.Depth+p2);}
		public static Size3 operator+(int p1, Size3 p2) {return new Size3(p1+p2.Width, p1+p2.Height, p1+p2.Depth);}
		// -
		public static Size3 operator-(Size3 p1, Size3 p2) {return new Size3(p1.Width-p2.Width, p1.Height-p2.Height, p1.Depth-p2.Depth);}
		public static Size3 operator-(Size3 p1, int p2) {return new Size3(p1.Width-p2, p1.Height-p2, p1.Depth-p2);}
		public static Size3 operator-(int p1, Size3 p2) {return new Size3(p1-p2.Width, p1-p2.Height, p1-p2.Depth);}
		public static Size3 operator-(Size3 p1) {return new Size3(-p1.Width, -p1.Height, -p1.Depth);}
		// *
		public static Size3 operator*(Size3 p1, Size3 p2) {return new Size3(p1.Width*p2.Width, p1.Height*p2.Height, p1.Depth*p2.Depth);}
		public static Size3 operator*(Size3 p1, int p2) {return new Size3(p1.Width*p2, p1.Height*p2, p1.Depth*p2);}
		public static Size3 operator*(int p1, Size3 p2) {return new Size3(p1*p2.Width, p1*p2.Height, p1*p2.Depth);}
		// /
		public static Size3 operator/(Size3 p1, Size3 p2) {return new Size3(p1.Width/p2.Width, p1.Height/p2.Height, p1.Depth/p2.Depth);}
		public static Size3 operator/(Size3 p1, int p2) {return new Size3(p1.Width/p2, p1.Height/p2, p1.Depth/p2);}
		public static Size3 operator/(int p1, Size3 p2) {return new Size3(p1/p2.Width, p1/p2.Height, p1/p2.Depth);}
		// ==
		public static bool operator==(Size3 p1, Size3 p2) {return (p1.Width==p2.Width && p1.Height==p2.Height && p1.Depth==p2.Depth);}
		public static bool operator!=(Size3 p1, Size3 p2) {return (p1.Width!=p2.Width || p1.Height!=p2.Height || p1.Depth!=p2.Depth);}
		#endregion
	}
}