using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix2x3
	{
		#region Properties
		public Vector3 X, Y;
		#endregion

		#region Operators
		// +
		public static Matrix2x3 operator+(Matrix2x3 p1, Matrix2x3 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			return p1;
		}

		public static Matrix2x3 operator+(Matrix2x3 p1, Vector3 p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Matrix2x3 operator+(Vector3 p1, Matrix2x3 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		public static Matrix2x3 operator+(Matrix2x3 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Matrix2x3 operator+(float p1, Matrix2x3 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		// -
		public static Matrix2x3 operator-(Matrix2x3 p1, Matrix2x3 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			return p1;
		}

		public static Matrix2x3 operator-(Matrix2x3 p1, Vector3 p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Matrix2x3 operator-(Vector3 p1, Matrix2x3 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Matrix2x3 operator-(Matrix2x3 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Matrix2x3 operator-(float p1, Matrix2x3 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Matrix2x3 operator-(Matrix2x3 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			return p2;
		}

		// *
		public static Matrix2x3 operator*(Matrix2x3 p1, Matrix2x3 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			return p1;
		}

		public static Matrix2x3 operator*(Matrix2x3 p1, Vector3 p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Matrix2x3 operator*(Vector3 p1, Matrix2x3 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		public static Matrix2x3 operator*(Matrix2x3 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Matrix2x3 operator*(float p1, Matrix2x3 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		// /
		public static Matrix2x3 operator/(Matrix2x3 p1, Matrix2x3 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			return p1;
		}

		public static Matrix2x3 operator/(Matrix2x3 p1, Vector3 p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Matrix2x3 operator/(Vector3 p1, Matrix2x3 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		public static Matrix2x3 operator/(Matrix2x3 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Matrix2x3 operator/(float p1, Matrix2x3 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		// ==
		public static bool operator==(Matrix2x3 p1, Matrix2x3 p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Matrix2x3 p1, Matrix2x3 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}
		#endregion

		#region Methods
		public static void Multiply(ref Matrix2x3 matrix1, ref Matrix3 matrix2, out Matrix2x3 result)
        {
            result.X.X = (matrix1.X.X * matrix2.X.X) + (matrix1.X.Y * matrix2.Y.X) + (matrix1.X.Z * matrix2.Z.X);
            result.X.Y = (matrix1.X.X * matrix2.X.Y) + (matrix1.X.Y * matrix2.Y.Y) + (matrix1.X.Z * matrix2.Z.Y);
            result.X.Z = (matrix1.X.X * matrix2.X.Z) + (matrix1.X.Y * matrix2.Y.Z) + (matrix1.X.Z * matrix2.Z.Z);

            result.Y.X = (matrix1.Y.X * matrix2.X.X) + (matrix1.Y.Y * matrix2.Y.X) + (matrix1.Y.Z * matrix2.Z.X);
            result.Y.Y = (matrix1.Y.X * matrix2.X.Y) + (matrix1.Y.Y * matrix2.Y.Y) + (matrix1.Y.Z * matrix2.Z.Y);
            result.Y.Z = (matrix1.Y.X * matrix2.X.Z) + (matrix1.Y.Y * matrix2.Y.Z) + (matrix1.Y.Z * matrix2.Z.Z);
        }

		public static void Multiply(ref Matrix2x3 matrix1, ref Matrix3x2 matrix2, out Matrix2 result)
        {
            result.X.X = (matrix1.X.X * matrix2.X.X) + (matrix1.X.Y * matrix2.Y.X) + (matrix1.X.Z * matrix2.Z.X);
            result.X.Y = (matrix1.X.X * matrix2.X.Y) + (matrix1.X.Y * matrix2.Y.Y) + (matrix1.X.Z * matrix2.Z.Y);

            result.Y.X = (matrix1.Y.X * matrix2.X.X) + (matrix1.Y.Y * matrix2.Y.X) + (matrix1.Y.Z * matrix2.Z.X);
            result.Y.Y = (matrix1.Y.X * matrix2.X.Y) + (matrix1.Y.Y * matrix2.Y.Y) + (matrix1.Y.Z * matrix2.Z.Y);
        }

		public static void Transpose(Matrix2x3 matrix, out Matrix3x2 result)
        {
            result.X.X = matrix.X.X;
            result.X.Y = matrix.Y.X;

            result.Y.X = matrix.X.Y;
            result.Y.Y = matrix.Y.Y;

            result.Z.X = matrix.X.Z;
            result.Z.Y = matrix.Y.Z;
        }
		#endregion
	}
}