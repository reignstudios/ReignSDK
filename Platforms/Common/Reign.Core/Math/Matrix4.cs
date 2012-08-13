using System.Runtime.InteropServices;
using MathS = System.Math;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix4
	{
		#region Properties
		public Vector4 X, Y, Z, W;
		#endregion

		#region Constructors
		public Matrix4(float value)
		{
			X = new Vector4(value);
			Y = new Vector4(value);
			Z = new Vector4(value);
			W = new Vector4(value);
		}

		public Matrix4(Vector4 x, Vector4 y, Vector4 z, Vector4 w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public static readonly Matrix4 Identity = new Matrix4
		(
		    new Vector4(1, 0, 0, 0),
		    new Vector4(0, 1, 0, 0),
		    new Vector4(0, 0, 1, 0),
		    new Vector4(0, 0, 0, 1)
		);
		#endregion

		#region Operators
		// +
		public static Matrix4 operator+(Matrix4 p1, Matrix4 p2) {return new Matrix4(p1.X+p2.X, p1.Y+p2.Y, p1.Z+p2.Z, p1.W+p2.W);}
		public static Matrix4 operator+(Matrix4 p1, float p2) {return new Matrix4(p1.X+p2, p1.Y+p2, p1.Z+p2, p1.W+p2);}
		public static Matrix4 operator+(float p1, Matrix4 p2) {return new Matrix4(p1+p2.X, p1+p2.Y, p1+p2.Z, p1+p2.W);}
		// -
		public static Matrix4 operator-(Matrix4 p1, Matrix4 p2) {return new Matrix4(p1.X-p2.X, p1.Y-p2.Y, p1.Z-p2.Z, p1.W-p2.W);}
		public static Matrix4 operator-(Matrix4 p1, float p2) {return new Matrix4(p1.X-p2, p1.Y-p2, p1.Z-p2, p1.W-p2);}
		public static Matrix4 operator-(float p1, Matrix4 p2) {return new Matrix4(p1-p2.X, p1-p2.Y, p1-p2.Z, p1-p2.W);}
		// *
		public static Matrix4 operator*(Matrix4 p1, Matrix4 p2) {return new Matrix4(p1.X*p2.X, p1.Y*p2.Y, p1.Z*p2.Z, p1.W*p2.W);}
		public static Matrix4 operator*(Matrix4 p1, float p2) {return new Matrix4(p1.X*p2, p1.Y*p2, p1.Z*p2, p1.W*p2);}
		public static Matrix4 operator*(float p1, Matrix4 p2) {return new Matrix4(p1*p2.X, p1*p2.Y, p1*p2.Z, p1*p2.W);}
		// /
		public static Matrix4 operator/(Matrix4 p1, Matrix4 p2) {return new Matrix4(p1.X/p2.X, p1.Y/p2.Y, p1.Z/p2.Z, p1.W/p2.W);}
		public static Matrix4 operator/(Matrix4 p1, float p2) {return new Matrix4(p1.X/p2, p1.Y/p2, p1.Z/p2, p1.W/p2);}
		public static Matrix4 operator/(float p1, Matrix4 p2) {return new Matrix4(p1/p2.X, p1/p2.Y, p1/p2.Z, p1/p2.W);}
		// ==
		public static bool operator==(Matrix4 p1, Matrix4 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z && p1.W==p2.W);}
		public static bool operator!=(Matrix4 p1, Matrix4 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z || p1.W!=p2.W);}
		#endregion
		
		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Matrix4)obj == this;
		}

		public override string ToString()
		{
			return string.Format("{0} : {1} : {2} : {3}", X, Y, Z, W);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Matrix4 Abs()
		{
			return new Matrix4(X.Abs(), Y.Abs(), Z.Abs(), W.Abs());
		}

		public Matrix4 Transpose()
		{
			return new Matrix4
			(
				new Vector4(X.X, Y.X, Z.X, W.X),
				new Vector4(X.Y, Y.Y, Z.Y, W.Y),
				new Vector4(X.Z, Y.Z, Z.Z, W.Z),
				new Vector4(X.W, Y.W, Z.W, W.W)
			);
		}

		public Matrix4 Multiply(Matrix2 matrix)
		{
			return new Matrix4(
				new Vector4((matrix.X.X*X.X) + (matrix.X.Y*Y.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y), X.Z, X.W),
				new Vector4((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y), Y.Z, Y.W),
				Z,
				W);
		}

		public Matrix4 Multiply(Matrix3 matrix)
		{
			return new Matrix4(
				new Vector4((matrix.X.X*X.X) + (matrix.X.Y*Y.X) + (matrix.X.Z*Z.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y) + (matrix.X.Z*Z.Y), (matrix.X.X*X.Z) + (matrix.X.Y*Y.Z) + (matrix.X.Z*Z.Z), X.W),
				new Vector4((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X) + (matrix.Y.Z*Z.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y) + (matrix.Y.Z*Z.Y), (matrix.Y.X*X.Z) + (matrix.Y.Y*Y.Z) + (matrix.Y.Z*Z.Z), Y.W),
				new Vector4((matrix.Z.X*X.X) + (matrix.Z.Y*Y.X) + (matrix.Z.Z*Z.X), (matrix.Z.X*X.Y) + (matrix.Z.Y*Y.Y) + (matrix.Z.Z*Z.Y), (matrix.Z.X*X.Z) + (matrix.Z.Y*Y.Z) + (matrix.Z.Z*Z.Z), Z.W),
				W);
		}

		public Matrix4 Multiply(Matrix4 matrix)
		{
			return new Matrix4(
				new Vector4((matrix.X.X*X.X) + (matrix.X.Y*Y.X) + (matrix.X.Z*Z.X) + (matrix.X.W*W.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y) + (matrix.X.Z*Z.Y) + (matrix.X.W*W.Y), (matrix.X.X*X.Z) + (matrix.X.Y*Y.Z) + (matrix.X.Z*Z.Z) + (matrix.X.W*W.Z), (matrix.X.X*X.W) + (matrix.X.Y*Y.W) + (matrix.X.Z*Z.W) + (matrix.X.W*W.W)),
				new Vector4((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X) + (matrix.Y.Z*Z.X) + (matrix.Y.W*W.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y) + (matrix.Y.Z*Z.Y) + (matrix.Y.W*W.Y), (matrix.Y.X*X.Z) + (matrix.Y.Y*Y.Z) + (matrix.Y.Z*Z.Z) + (matrix.Y.W*W.Z), (matrix.Y.X*X.W) + (matrix.Y.Y*Y.W) + (matrix.Y.Z*Z.W) + (matrix.Y.W*W.W)),
				new Vector4((matrix.Z.X*X.X) + (matrix.Z.Y*Y.X) + (matrix.Z.Z*Z.X) + (matrix.Z.W*W.X), (matrix.Z.X*X.Y) + (matrix.Z.Y*Y.Y) + (matrix.Z.Z*Z.Y) + (matrix.Z.W*W.Y), (matrix.Z.X*X.Z) + (matrix.Z.Y*Y.Z) + (matrix.Z.Z*Z.Z) + (matrix.Z.W*W.Z), (matrix.Z.X*X.W) + (matrix.Z.Y*Y.W) + (matrix.Z.Z*Z.W) + (matrix.Z.W*W.W)),
				new Vector4((matrix.W.X*X.X) + (matrix.W.Y*Y.X) + (matrix.W.Z*Z.X) + (matrix.W.W*W.X), (matrix.W.X*X.Y) + (matrix.W.Y*Y.Y) + (matrix.W.Z*Z.Y) + (matrix.W.W*W.Y), (matrix.W.X*X.Z) + (matrix.W.Y*Y.Z) + (matrix.W.Z*Z.Z) + (matrix.W.W*W.Z), (matrix.W.X*X.W) + (matrix.W.Y*Y.W) + (matrix.W.Z*Z.W) + (matrix.W.W*W.W)));
		}

		public Matrix4 Invert()
		{
			float delta =
			(
				(X.W * Y.Z * Z.Y * W.X) - (X.Z * Y.W * Z.Y * W.X) - (X.W * Y.Y * Z.Z * W.X) + (X.Y * Y.W * Z.Z * W.X) +
				(X.Z * Y.Y * Z.W * W.X) - (X.Y * Y.Z * Z.W * W.X) - (X.W * Y.Z * Z.X * W.Y) + (X.Z * Y.W * Z.X * W.Y) +
				(X.W * Y.X * Z.Z * W.Y) - (X.X * Y.W * Z.Z * W.Y) - (X.Z * Y.X * Z.W * W.Y) + (X.X * Y.Z * Z.W * W.Y) +
				(X.W * Y.Y * Z.X * W.Z) - (X.Y * Y.W * Z.X * W.Z) - (X.W * Y.X * Z.Y * W.Z) + (X.X * Y.W * Z.Y * W.Z) +
				(X.Y * Y.X * Z.W * W.Z) - (X.X * Y.Y * Z.W * W.Z) - (X.Z * Y.Y * Z.X * W.W) + (X.Y * Y.Z * Z.X * W.W) +
				(X.Z * Y.X * Z.Y * W.W) - (X.X * Y.Z * Z.Y * W.W) - (X.Y * Y.X * Z.Z * W.W) + (X.X * Y.Y * Z.Z * W.W)
			);
			delta = 1 / delta;
			
			var mat = new Matrix4
			(
				new Vector4(
					((Y.Y * Z.Z * W.W) + (Y.Z * Z.W * W.Y) + (Y.W * Z.Y * W.Z) - (Y.Y * Z.W * W.Z) - (Y.Z * Z.Y * W.W) - (Y.W * Z.Z * W.Y)) * delta,
					((X.Y * Z.W * W.Z) + (X.Z * Z.Y * W.W) + (X.W * Z.Z * W.Y) - (X.Y * Z.Z * W.W) - (X.Z * Z.W * W.Y) - (X.W * Z.Y * W.Z)) * delta,
					((X.Y * Y.Z * W.W) + (X.Z * Y.W * W.Y) + (X.W * Y.Y * W.Z) - (X.Y * Y.W * W.Z) - (X.Z * Y.Y * W.W) - (X.W * Y.Z * W.Y)) * delta,
					((X.Y * Y.W * Z.Z) + (X.Z * Y.Y * Z.W) + (X.W * Y.Z * Z.Y) - (X.Y * Y.Z * Z.W) - (X.Z * Y.W * Z.Y) - (X.W * Y.Y * Z.Z)) * delta),
				new Vector4(
					((Y.X * Z.W * W.Z) + (Y.Z * Z.X * W.W) + (Y.W * Z.Z * W.X) - (Y.X * Z.Z * W.W) - (Y.Z * Z.W * W.X) - (Y.W * Z.X * W.Z)) * delta,
					((X.X * Z.Z * W.W) + (X.Z * Z.W * W.X) + (X.W * Z.X * W.Z) - (X.X * Z.W * W.Z) - (X.Z * Z.X * W.W) - (X.W * Z.Z * W.X)) * delta,
					((X.X * Y.W * W.Z) + (X.Z * Y.X * W.W) + (X.W * Y.Z * W.X) - (X.X * Y.Z * W.W) - (X.Z * Y.W * W.X) - (X.W * Y.X * W.Z)) * delta,
					((X.X * Y.Z * Z.W) + (X.Z * Y.W * Z.X) + (X.W * Y.X * Z.Z) - (X.X * Y.W * Z.Z) - (X.Z * Y.X * Z.W) - (X.W * Y.Z * Z.X)) * delta),
				new Vector4(
					((Y.X * Z.Y * W.W) + (Y.Y * Z.W * W.X) + (Y.W * Z.X * W.Y) - (Y.X * Z.W * W.Y) - (Y.Y * Z.X * W.W) - (Y.W * Z.Y * W.X)) * delta,
					((X.X * Z.W * W.Y) + (X.Y * Z.X * W.W) + (X.W * Z.Y * W.X) - (X.X * Z.Y * W.W) - (X.Y * Z.W * W.X) - (X.W * Z.X * W.Y)) * delta,
					((X.X * Y.Y * W.W) + (X.Y * Y.W * W.X) + (X.W * Y.X * W.Y) - (X.X * Y.W * W.Y) - (X.Y * Y.X * W.W) - (X.W * Y.Y * W.X)) * delta,
					((X.X * Y.W * Z.Y) + (X.Y * Y.X * Z.W) + (X.W * Y.Y * Z.X) - (X.X * Y.Y * Z.W) - (X.Y * Y.W * Z.X) - (X.W * Y.X * Z.Y)) * delta),
				new Vector4(
					((Y.X * Z.Z * W.Y) + (Y.Y * Z.X * W.Z) + (Y.Z * Z.Y * W.X) - (Y.X * Z.Y * W.Z) - (Y.Y * Z.Z * W.X) - (Y.Z * Z.X * W.Y)) * delta,
					((X.X * Z.Y * W.Z) + (X.Y * Z.Z * W.X) + (X.Z * Z.X * W.Y) - (X.X * Z.Z * W.Y) - (X.Y * Z.X * W.Z) - (X.Z * Z.Y * W.X)) * delta,
					((X.X * Y.Z * W.Y) + (X.Y * Y.X * W.Z) + (X.Z * Y.Y * W.X) - (X.X * Y.Y * W.Z) - (X.Y * Y.Z * W.X) - (X.Z * Y.X * W.Y)) * delta,
					((X.X * Y.Y * Z.Z) + (X.Y * Y.Z * Z.X) + (X.Z * Y.X * Z.Y) - (X.X * Y.Z * Z.Y) - (X.Y * Y.X * Z.Z) - (X.Z * Y.Y * Z.X)) * delta)
			);
				  
			return mat.Transpose();
		}

		public Matrix4 RotateAroundAxisX(float radians)
		{
			float tCos = (float)MathS.Cos(-radians), tSin = (float)MathS.Sin(-radians);
			return new Matrix4
			(
				X,
				new Vector4((Y.X*tCos) - (Z.X*tSin), (Y.Y*tCos) - (Z.Y*tSin), (Y.Z*tCos) - (Z.Z*tSin), Y.W),
				new Vector4((Y.X*tSin) + (Z.X*tCos), (Y.Y*tSin) + (Z.Y*tCos), (Y.Z*tSin) + (Z.Z*tCos), Z.W),
				W
			);
		}

		public Matrix4 RotateAroundAxisY(float radians)
		{
			float tCos = (float)MathS.Cos(-radians), tSin = (float)MathS.Sin(-radians);
			return new Matrix4
			(
				new Vector4((Z.X*tSin) + (X.X*tCos), (Z.Y*tSin) + (X.Y*tCos), (Z.Z*tSin) + (X.Z*tCos), X.W),
				Y,
				new Vector4((Z.X*tCos) - (X.X*tSin), (Z.Y*tCos) - (X.Y*tSin), (Z.Z*tCos) - (X.Z*tSin), Z.W),
				W
			);
		}

		public Matrix4 RotateAroundAxisZ(float radians)
		{
			float tCos = (float)MathS.Cos(-radians), tSin = (float)MathS.Sin(-radians);
			return new Matrix4
			(
				new Vector4((X.X*tCos) - (Y.X*tSin), (X.Y*tCos) - (Y.Y*tSin), (X.Z*tCos) - (Y.Z*tSin), X.W),
				new Vector4((X.X*tSin) + (Y.X*tCos), (X.Y*tSin) + (Y.Y*tCos), (X.Z*tSin) + (Y.Z*tCos), Y.W),
				Z,
				W
			);
		}

		public Matrix4 RotateAroundWorldAxisX(float radians)
		{
			float tCos = (float)MathS.Cos(radians), tSin = (float)MathS.Sin(radians);
			return new Matrix4
			(
				new Vector4(X.X, (X.Y*tCos) - (X.Z*tSin), (X.Y*tSin) + (X.Z*tCos), 0),
				new Vector4(Y.X, (Y.Y*tCos) - (Y.Z*tSin), (Y.Y*tSin) + (Y.Z*tCos), 0),
				new Vector4(Z.X, (Z.Y*tCos) - (Z.Z*tSin), (Z.Y*tSin) + (Z.Z*tCos), 0),
				W
			);
		}

		public Matrix4 RotateAroundWorldAxisY(float radians)
		{
			float tCos = (float)MathS.Cos(radians), tSin = (float)MathS.Sin(radians);
			return new Matrix4
			(
				new Vector4((X.Z*tSin) + (X.X*tCos), X.Y, (X.Z*tCos) - (X.X*tSin), 0),
				new Vector4((Y.Z*tSin) + (Y.X*tCos), Y.Y, (Y.Z*tCos) - (Y.X*tSin), 0),
				new Vector4((Z.Z*tSin) + (Z.X*tCos), Z.Y, (Z.Z*tCos) - (Z.X*tSin), 0),
				W
			);
		}

		public Matrix4 RotateAroundWorldAxisZ(float radians)
		{
			float tCos = (float)MathS.Cos(radians), tSin = (float)MathS.Sin(radians);
			return new Matrix4
			(
				new Vector4((X.X*tCos) - (X.Y*tSin), (X.X*tSin) + (X.Y*tCos), X.Z, 0),
				new Vector4((Y.X*tCos) - (Y.Y*tSin), (Y.X*tSin) + (Y.Y*tCos), Y.Z, 0),
				new Vector4((Z.X*tCos) - (Z.Y*tSin), (Z.X*tSin) + (Z.Y*tCos), Z.Z, 0),
				W
			);
		}

		public static Matrix4 LookAt(Vector3 location, Vector3 lookAt, Vector3 upVector)
		{
			var forward = (lookAt - location).Normalize();
			var xVec = forward.Cross(upVector).Normalize();
			upVector = xVec.Cross(forward);
			
			return new Matrix4(
				new Vector4(xVec.X, xVec.Y, xVec.Z, location.Dot(-xVec)),
				new Vector4(upVector.X, upVector.Y, upVector.Z, location.Dot(-upVector)),
				new Vector4(-forward.X, -forward.Y, -forward.Z, location.Dot(forward)),
				new Vector4(0, 0, 0, 1));
		}

		public static Matrix4 Perspective(float fov, float aspect, float near, float far)
		{
			float top = near * (float)MathS.Tan(fov * .5f);
			float bottom = -top;
			float right = top * aspect;
			float left = -right;

			return Frustum(left, right, bottom, top, near, far);
		}

		public static Matrix4 Frustum(float left, float right, float bottom, float top, float near, float far)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;
			float n = near * 2;

			return new Matrix4(
				new Vector4(n/width, 0, (right+left)/width, 0),
				new Vector4(0, n/height, (top+bottom)/height, 0),
				new Vector4(0, 0, -(far+near)/depth, -(n*far)/depth),
				new Vector4(0, 0, -1, 0));
		}

		public static Matrix4 Orthographic(float width, float height, float near, float far)
		{
			return Orthographic(0, width, 0, height, near, far);
		}

		public static Matrix4 Orthographic(float left, float right, float bottom, float top, float near, float far)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;

			return new Matrix4(
				new Vector4((2/width), 0, 0, -(right+left)/width),
				new Vector4(0, (2/height), 0, -(top+bottom)/height),
				new Vector4(0, 0, -2/depth, 0),//-(far+near)/depth
				new Vector4(0, 0, 0, 1));
		}

		public static Matrix4 OrthographicCentered(float width, float height, float near, float far)
		{
			return OrthographicCentered(0, width, 0, height, near, far);
		}

		public static Matrix4 OrthographicCentered(float left, float right, float bottom, float top, float near, float far)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;

			return new Matrix4(
				new Vector4((2/width), 0, 0, 0),
				new Vector4(0, (2/height), 0, 0),
				new Vector4(0, 0, -2/depth, 0),//-(far+near)/depth
				new Vector4(0, 0, 0, 1));
		}
		#endregion
	}
}