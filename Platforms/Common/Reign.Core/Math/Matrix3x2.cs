using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix3x2
	{
		#region Properties
		public Vector2 X, Y, Z;
		#endregion

		#region Operators
		// +
		public static Matrix3x2 operator+(Matrix3x2 p1, Matrix3x2 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			p1.Z += p2.Z;
			return p1;
		}

		public static Matrix3x2 operator+(Matrix3x2 p1, Vector2 p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			return p1;
		}

		public static Matrix3x2 operator+(Vector2 p1, Matrix3x2 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			return p2;
		}

		public static Matrix3x2 operator+(Matrix3x2 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			return p1;
		}

		public static Matrix3x2 operator+(float p1, Matrix3x2 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			return p2;
		}

		// -
		public static Matrix3x2 operator-(Matrix3x2 p1, Matrix3x2 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			p1.Z -= p2.Z;
			return p1;
		}

		public static Matrix3x2 operator-(Matrix3x2 p1, Vector2 p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			return p1;
		}

		public static Matrix3x2 operator-(Vector2 p1, Matrix3x2 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			return p2;
		}

		public static Matrix3x2 operator-(Matrix3x2 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			return p1;
		}

		public static Matrix3x2 operator-(float p1, Matrix3x2 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			return p2;
		}

		public static Matrix3x2 operator-(Matrix3x2 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			p2.Z = -p2.Z;
			return p2;
		}

		// *
		public static Matrix3x2 operator*(Matrix3x2 p1, Matrix3x2 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			p1.Z *= p2.Z;
			return p1;
		}

		public static Matrix3x2 operator*(Matrix3x2 p1, Vector2 p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			return p1;
		}

		public static Matrix3x2 operator*(Vector2 p1, Matrix3x2 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			return p2;
		}

		public static Matrix3x2 operator*(Matrix3x2 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			return p1;
		}

		public static Matrix3x2 operator*(float p1, Matrix3x2 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			return p2;
		}

		// /
		public static Matrix3x2 operator/(Matrix3x2 p1, Matrix3x2 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			p1.Z /= p2.Z;
			return p1;
		}

		public static Matrix3x2 operator/(Matrix3x2 p1, Vector2 p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			return p1;
		}

		public static Matrix3x2 operator/(Vector2 p1, Matrix3x2 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			return p2;
		}

		public static Matrix3x2 operator/(Matrix3x2 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			return p1;
		}

		public static Matrix3x2 operator/(float p1, Matrix3x2 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			return p2;
		}

		// ==
		public static bool operator==(Matrix3x2 p1, Matrix3x2 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z);}
		public static bool operator!=(Matrix3x2 p1, Matrix3x2 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z);}
		#endregion

		#region Methods
		public static void Transpose(Matrix3x2 matrix, out Matrix2x3 result)
        {
            result.X.X = matrix.X.X;
            result.X.Y = matrix.Y.X;
            result.X.Z = matrix.Z.X;

            result.Y.X = matrix.X.Y;
            result.Y.Y = matrix.Y.Y;
            result.Y.Z = matrix.Z.Y;
        }
		#endregion
	}
}