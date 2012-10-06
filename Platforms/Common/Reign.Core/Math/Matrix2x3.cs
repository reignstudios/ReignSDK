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
		public static void Add(ref Matrix2x3 value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
		}

		public static void Add(ref Matrix2x3 value1, float value2, out Matrix2x3 result)
		{
			result.X = value1.X + value2;
			result.Y = value1.Y + value2;
		}

		public static void Add(float value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1 + value2.X;
			result.Y = value1 + value2.Y;
		}

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
		public static void Sub(ref Matrix2x3 value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
		}

		public static void Sub(ref Matrix2x3 value1, float value2, out Matrix2x3 result)
		{
			result.X = value1.X - value2;
			result.Y = value1.Y - value2;
		}

		public static void Sub(float value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1 - value2.X;
			result.Y = value1 - value2.Y;
		}

		public static void Neg(ref Matrix2x3 value, out Matrix2x3 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
		}

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
		public static void Mul(ref Matrix2x3 value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
		}

		public static void Mul(ref Matrix2x3 value1, float value2, out Matrix2x3 result)
		{
			result.X = value1.X * value2;
			result.Y = value1.Y * value2;
		}

		public static void Mul(float value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1 * value2.X;
			result.Y = value1 * value2.Y;
		}

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
		public static void Div(ref Matrix2x3 value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
		}

		public static void Div(ref Matrix2x3 value1, float value2, out Matrix2x3 result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
		}

		public static void Div(float value1, ref Matrix2x3 value2, out Matrix2x3 result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
		}

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
			result.X.w = 0;

            result.Y.X = (matrix1.Y.X * matrix2.X.X) + (matrix1.Y.Y * matrix2.Y.X) + (matrix1.Y.Z * matrix2.Z.X);
            result.Y.Y = (matrix1.Y.X * matrix2.X.Y) + (matrix1.Y.Y * matrix2.Y.Y) + (matrix1.Y.Z * matrix2.Z.Y);
            result.Y.Z = (matrix1.Y.X * matrix2.X.Z) + (matrix1.Y.Y * matrix2.Y.Z) + (matrix1.Y.Z * matrix2.Z.Z);
			result.Y.w = 0;
        }

		public static void Multiply(ref Matrix2x3 matrix1, ref Matrix3x2 matrix2, out Matrix2 result)
        {
            result.X.X = (matrix1.X.X * matrix2.X.X) + (matrix1.X.Y * matrix2.Y.X) + (matrix1.X.Z * matrix2.Z.X);
            result.X.Y = (matrix1.X.X * matrix2.X.Y) + (matrix1.X.Y * matrix2.Y.Y) + (matrix1.X.Z * matrix2.Z.Y);
			result.X.z = 0;
			result.X.w = 0;

            result.Y.X = (matrix1.Y.X * matrix2.X.X) + (matrix1.Y.Y * matrix2.Y.X) + (matrix1.Y.Z * matrix2.Z.X);
            result.Y.Y = (matrix1.Y.X * matrix2.X.Y) + (matrix1.Y.Y * matrix2.Y.Y) + (matrix1.Y.Z * matrix2.Z.Y);
			result.Y.z = 0;
			result.Y.w = 0;

			result.z = Vector2.Zero;
			result.w = Vector2.Zero;
        }

		public static void Transpose(ref Matrix2x3 matrix, out Matrix3x2 result)
        {
            result.X.X = matrix.X.X;
            result.X.Y = matrix.Y.X;
			result.X.z = 0;
			result.X.w = 0;

            result.Y.X = matrix.X.Y;
            result.Y.Y = matrix.Y.Y;
			result.Y.z = 0;
			result.Y.w = 0;

            result.Z.X = matrix.X.Z;
            result.Z.Y = matrix.Y.Z;
			result.Z.z = 0;
			result.Z.w = 0;
        }
		#endregion
	}
}