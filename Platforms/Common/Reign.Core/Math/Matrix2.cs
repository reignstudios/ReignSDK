using System.Runtime.InteropServices;
using MathS = System.Math;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix2
	{
		#region Properties
		public Vector2 X, Y;
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

		public static Matrix2 Cross(Vector2 xVector)
		{
			return new Matrix2(xVector, new Vector2(-xVector.Y, xVector.X));
		}

		public static readonly Matrix2 Identity = new Matrix2
		(
		    new Vector2(1, 0),
		    new Vector2(0, 1)
		);
		#endregion

		#region Operators
		// +
		public static Matrix2 operator+(Matrix2 p1, Matrix2 p2) {return new Matrix2(p1.X+p2.X, p1.Y+p2.Y);}
		public static Matrix2 operator+(Matrix2 p1, float p2) {return new Matrix2(p1.X+p2, p1.Y+p2);}
		public static Matrix2 operator+(float p1, Matrix2 p2) {return new Matrix2(p1+p2.X, p1+p2.Y);}
		// -
		public static Matrix2 operator-(Matrix2 p1, Matrix2 p2) {return new Matrix2(p1.X-p2.X, p1.Y-p2.Y);}
		public static Matrix2 operator-(Matrix2 p1, float p2) {return new Matrix2(p1.X-p2, p1.Y-p2);}
		public static Matrix2 operator-(float p1, Matrix2 p2) {return new Matrix2(p1-p2.X, p1-p2.Y);}
		// *
		public static Matrix2 operator*(Matrix2 p1, Matrix2 p2) {return new Matrix2(p1.X*p2.X, p1.Y*p2.Y);}
		public static Matrix2 operator*(Matrix2 p1, float p2) {return new Matrix2(p1.X*p2, p1.Y*p2);}
		public static Matrix2 operator*(float p1, Matrix2 p2) {return new Matrix2(p1*p2.X, p1*p2.Y);}
		// /
		public static Matrix2 operator/(Matrix2 p1, Matrix2 p2) {return new Matrix2(p1.X/p2.X, p1.Y/p2.Y);}
		public static Matrix2 operator/(Matrix2 p1, float p2) {return new Matrix2(p1.X/p2, p1.Y/p2);}
		public static Matrix2 operator/(float p1, Matrix2 p2) {return new Matrix2(p1/p2.X, p1/p2.Y);}
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

		public Matrix2 Transpose()
		{
			return new Matrix2
			(
				new Vector2(X.X, Y.X),
				new Vector2(X.Y, Y.Y)
			);
		}

		public Matrix2 Multiply(Matrix2 matrix)
		{
			return new Matrix2
			(
				new Vector2((matrix.X.X*X.X) + (matrix.X.Y*Y.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y)),
				new Vector2((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y))
			);
		}
		#endregion
	}
}