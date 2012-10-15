using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Color4
	{
		#region Properties
		public byte R, G, B, A;
		#endregion

		#region Operators
		// convert
		public Vector4 ToVector4()
		{
			return new Vector4(R/255f, G/255f, B/255f, A/255f);
		}

		public static void ToVector4(ref Color4 color, out Vector4 vector)
		{
			vector.X = color.R / 255f;
			vector.Y = color.G / 255f;
			vector.Z = color.B / 255f;
			vector.W = color.A / 255f;
		}
		#endregion

		#region Constructors
		public Color4(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
		#endregion

		#region Methods
		
		#endregion
	}
}