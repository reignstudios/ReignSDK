using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Color4
	{
		#region Properties
		public byte R, G, B, A;

		public int Value
		{
			get
			{
				int color = R;
				color |= G << 8;
				color |= B << 16;
				color |= A << 24;
				return color;
			}
			set
			{
				R = (byte)(value & 0x000000FF);
				G = (byte)((value & 0x0000FF00) >> 8);
				B = (byte)((value & 0x00FF0000) >> 16);
				A = (byte)((value & 0xFF000000) >> 24);
			}
		}
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

		public Color4(int color)
		{
			R = (byte)(color & 0x000000FF);
			G = (byte)((color & 0x0000FF00) >> 8);
			B = (byte)((color & 0x00FF0000) >> 16);
			A = (byte)((color & 0xFF000000) >> 24);
		}
		#endregion

		#region Methods
		
		#endregion
	}
}