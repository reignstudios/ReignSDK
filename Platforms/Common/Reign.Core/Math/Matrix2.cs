﻿using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix2
	{
		#region Properties
		public Vector2 X, Y;

		public Vector2 Right {get{return X;}}
		public Vector2 Left {get{return -X;}}
		public Vector2 Up {get{return Y;}}
		public Vector2 Down {get{return -Y;}}
		#endregion

		#region Constructors
		public Matrix2(float value)
		{
			X = new Vector2(value);
			Y = new Vector2(value);
		}

		public Matrix2(Vector2 x, Vector2 y)
		{
			X = x;
			Y = y;
		}

		public static Matrix2 FromCross(Vector2 xVector)
		{
			return new Matrix2(xVector, new Vector2(-xVector.Y, xVector.X));
		}

		public static void FromCross(ref Vector2 xVector, out Matrix2 result)
		{
			result = new Matrix2(xVector, new Vector2(-xVector.Y, xVector.X));
		}

		public static readonly Matrix2 Identity = new Matrix2
		(
		    new Vector2(1, 0),
		    new Vector2(0, 1)
		);
		#endregion

		#region Operators
		// +
		public static Matrix2 operator+(Matrix2 p1, Matrix2 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			return p1;
		}

		public static Matrix2 operator+(Matrix2 p1, Vector2 p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Matrix2 operator+(Vector2 p1, Matrix2 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		public static Matrix2 operator+(Matrix2 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Matrix2 operator+(float p1, Matrix2 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		// -
		public static Matrix2 operator-(Matrix2 p1, Matrix2 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			return p1;
		}

		public static Matrix2 operator-(Matrix2 p1, Vector2 p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Matrix2 operator-(Vector2 p1, Matrix2 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Matrix2 operator-(Matrix2 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Matrix2 operator-(float p1, Matrix2 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Matrix2 operator-(Matrix2 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			return p2;
		}

		// *
		public static Matrix2 operator*(Matrix2 p1, Matrix2 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			return p1;
		}

		public static Matrix2 operator*(Matrix2 p1, Vector2 p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Matrix2 operator*(Vector2 p1, Matrix2 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		public static Matrix2 operator*(Matrix2 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Matrix2 operator*(float p1, Matrix2 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		// /
		public static Matrix2 operator/(Matrix2 p1, Matrix2 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			return p1;
		}

		public static Matrix2 operator/(Matrix2 p1, Vector2 p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Matrix2 operator/(Vector2 p1, Matrix2 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		public static Matrix2 operator/(Matrix2 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Matrix2 operator/(float p1, Matrix2 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		// ==
		public static bool operator==(Matrix2 p1, Matrix2 p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Matrix2 p1, Matrix2 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}
		#endregion
		
		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Matrix2)obj == this;
		}

		public override string ToString()
		{
			return string.Format("{0} : {1}", X, Y);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Matrix2 Abs()
		{
			return new Matrix2(X.Abs(), Y.Abs());
		}

		public static void Abs(ref Matrix2 matrix, out Matrix2 result)
		{
			Vector2.Abs(ref matrix.X, out result.X);
			Vector2.Abs(ref matrix.Y, out result.Y);
		}

		public Matrix2 Transpose()
		{
			return new Matrix2
			(
				new Vector2(X.X, Y.X),
				new Vector2(X.Y, Y.Y)
			);
		}

		public static void Transpose(Matrix2 matrix, out Matrix2 result)
		{
			result.X.X = matrix.X.X;
			result.X.Y = matrix.Y.X;

			result.Y.X = matrix.X.Y;
			result.Y.Y = matrix.Y.Y;
		}

		public Matrix2 Multiply(Matrix2 matrix)
		{
			return new Matrix2
			(
				new Vector2((matrix.X.X*X.X) + (matrix.X.Y*Y.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y)),
				new Vector2((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y))
			);
		}

		public static void Multiply(ref Matrix2 matrix1, ref Matrix2 matrix2, out Matrix2 result)
		{
			result.X.X = (matrix1.X.X*matrix2.X.X) + (matrix1.X.Y*matrix2.Y.X);
			result.X.Y = (matrix1.X.X*matrix2.X.Y) + (matrix1.X.Y*matrix2.Y.Y);

			result.Y.X = (matrix1.Y.X*matrix2.X.X) + (matrix1.Y.Y*matrix2.Y.X);
			result.Y.Y = (matrix1.Y.X*matrix2.X.Y) + (matrix1.Y.Y*matrix2.Y.Y);
		}

		public float Determinant()
		{
			return X.X * Y.Y - X.Y * Y.X;
		}

		public static void Determinant(ref Matrix2 matrix, out float result)
		{
			result = matrix.X.X * matrix.Y.Y - matrix.X.Y * matrix.Y.X;
		}

		public Matrix2 Invert()
        {
            float determinant = 1 / (X.X * Y.Y - X.Y * Y.X);
			Matrix2 result;
            result.X.X = Y.Y * determinant;
            result.X.Y = -X.Y * determinant;

            result.Y.X = -Y.X * determinant;
            result.Y.Y = X.X * determinant;

			return result;
        }

		public static void Invert(ref Matrix2 matrix, out Matrix2 result)
        {
            float determinant = 1 / (matrix.X.X * matrix.Y.Y - matrix.X.Y * matrix.Y.X);
            result.X.X = matrix.Y.Y * determinant;
            result.X.Y = -matrix.X.Y * determinant;

            result.Y.X = -matrix.Y.X * determinant;
            result.Y.Y = matrix.X.X * determinant;
        }
		#endregion
	}
}