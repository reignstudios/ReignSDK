namespace Reign.Core
{
	public struct Size2
	{
		#region Properties
		public int Width, Height;
		#endregion

		#region Constructors
		public Size2(int width, int height)
		{
			Width = width;
			Height = height;
		}
		#endregion

		#region Operators
		// +
		public static Size2 operator+(Size2 p1, Size2 p2) {return new Size2(p1.Width+p2.Width, p1.Height+p2.Height);}
		public static Size2 operator+(Size2 p1, int p2) {return new Size2(p1.Width+p2, p1.Height+p2);}
		public static Size2 operator+(int p1, Size2 p2) {return new Size2(p1+p2.Width, p1+p2.Height);}
		// -
		public static Size2 operator-(Size2 p1, Size2 p2) {return new Size2(p1.Width-p2.Width, p1.Height-p2.Height);}
		public static Size2 operator-(Size2 p1, int p2) {return new Size2(p1.Width-p2, p1.Height-p2);}
		public static Size2 operator-(int p1, Size2 p2) {return new Size2(p1-p2.Width, p1-p2.Height);}
		public static Size2 operator-(Size2 p1) {return new Size2(-p1.Width, -p1.Height);}
		// *
		public static Size2 operator*(Size2 p1, Size2 p2) {return new Size2(p1.Width*p2.Width, p1.Height*p2.Height);}
		public static Size2 operator*(Size2 p1, int p2) {return new Size2(p1.Width*p2, p1.Height*p2);}
		public static Size2 operator*(int p1, Size2 p2) {return new Size2(p1*p2.Width, p1*p2.Height);}
		// /
		public static Size2 operator/(Size2 p1, Size2 p2) {return new Size2(p1.Width/p2.Width, p1.Height/p2.Height);}
		public static Size2 operator/(Size2 p1, int p2) {return new Size2(p1.Width/p2, p1.Height/p2);}
		public static Size2 operator/(int p1, Size2 p2) {return new Size2(p1/p2.Width, p1/p2.Height);}
		// ==
		public static bool operator==(Size2 p1, Size2 p2) {return (p1.Width==p2.Width && p1.Height==p2.Height);}
		public static bool operator!=(Size2 p1, Size2 p2) {return (p1.Width!=p2.Width || p1.Height!=p2.Height);}
		// convert
		public Vector2 ToVector2() {return new Vector2(Width, Height);}
		#endregion
	}
}