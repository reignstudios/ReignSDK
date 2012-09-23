using System.Runtime.InteropServices;
using MathS = System.Math;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix3
	{
		#region Properties
		public Vector3 X, Y, Z;
		private Vector3 w;
		#endregion

		#region Constructors
		public Matrix3(float value)
		{
			X = new Vector3(value);
			Y = new Vector3(value);
			Z = new Vector3(value);
			w = new Vector3();
		}

		public Matrix3(Vector3 x, Vector3 y, Vector3 z)
		{
			X = x;
			Y = y;
			Z = z;
			w = new Vector3();
		}

		public static Matrix3 FromEuler(Vector3 euler)
		{
			float cx = (float)System.Math.Cos(euler.X);
			float sx = (float)System.Math.Sin(euler.X);
			float cy = (float)System.Math.Cos(euler.Y);
			float sy = (float)System.Math.Sin(euler.Y);
			float cz = (float)System.Math.Cos(euler.Z);
			float sz = (float)System.Math.Sin(euler.Z);

			// multiply ZYX
			return new Matrix3
			(
			    new Vector3(cy*cz, cz*sx*sy - cx*sz, cx*cz*sy + sx*sz),
			    new Vector3(cy*sz, cx*cz + sx*sy*sz, -cz*sx + cx*sy*sz),
			    new Vector3(-sy, cy*sx, cx*cy)
			);
		}

		public static Matrix3 FromEuler(float eulerX, float eulerY, float eulerZ)
		{
			float cx = (float)System.Math.Cos(eulerX);
			float sx = (float)System.Math.Sin(eulerX);
			float cy = (float)System.Math.Cos(eulerY);
			float sy = (float)System.Math.Sin(eulerY);
			float cz = (float)System.Math.Cos(eulerZ);
			float sz = (float)System.Math.Sin(eulerZ);

			// multiply ZYX
			return new Matrix3
			(
			    new Vector3(cy*cz, cz*sx*sy - cx*sz, cx*cz*sy + sx*sz),
			    new Vector3(cy*sz, cx*cz + sx*sy*sz, -cz*sx + cx*sy*sz),
			    new Vector3(-sy, cy*sx, cx*cy)
			);
		}

		public static Matrix3 FromQuaternion(Vector4 quaternion)
		{
			var squared = new Vector4(quaternion.X*quaternion.X, quaternion.Y*quaternion.Y, quaternion.Z*quaternion.Z, quaternion.W*quaternion.W);
			float invSqLength = 1 / (squared.X + squared.Y + squared.Z + squared.W);

			float temp1 = quaternion.X * quaternion.Y;
			float temp2 = quaternion.Z * quaternion.W;
			float temp3 = quaternion.X * quaternion.Z;
			float temp4 = quaternion.Y * quaternion.W;
			float temp5 = quaternion.Y * quaternion.Z;
			float temp6 = quaternion.X * quaternion.W;

			return new Matrix3
			(
				new Vector3((squared.X-squared.Y-squared.Z+squared.W) * invSqLength, 2*(temp1-temp2) * invSqLength, 2*(temp3+temp4) * invSqLength),
				new Vector3(2*(temp1+temp2) * invSqLength, (-squared.X+squared.Y-squared.Z+squared.W) * invSqLength, 2*(temp5-temp6) * invSqLength),
				new Vector3(2*(temp3-temp4) * invSqLength, 2*(temp5+temp6) * invSqLength, (-squared.X-squared.Y+squared.Z+squared.W) * invSqLength)
			);
		}

		public static Matrix3 FromQuaternion(float quaternionX, float quaternionY, float quaternionZ, float quaternionW)
		{
			var squared = new Vector4(quaternionX*quaternionX, quaternionY*quaternionY, quaternionZ*quaternionZ, quaternionW*quaternionW);
			float invSqLength = 1 / (squared.X + squared.Y + squared.Z + squared.W);

			float temp1 = quaternionX * quaternionY;
			float temp2 = quaternionZ * quaternionW;
			float temp3 = quaternionX * quaternionZ;
			float temp4 = quaternionY * quaternionW;
			float temp5 = quaternionY * quaternionZ;
			float temp6 = quaternionX * quaternionW;

			return new Matrix3
			(
				new Vector3((squared.X-squared.Y-squared.Z+squared.W) * invSqLength, 2*(temp1-temp2) * invSqLength, 2*(temp3+temp4) * invSqLength),
				new Vector3(2*(temp1+temp2) * invSqLength, (-squared.X+squared.Y-squared.Z+squared.W) * invSqLength, 2*(temp5-temp6) * invSqLength),
				new Vector3(2*(temp3-temp4) * invSqLength, 2*(temp5+temp6) * invSqLength, (-squared.X-squared.Y+squared.Z+squared.W) * invSqLength)
			);
		}

		public static Matrix3 Cross(Vector3 zVector, Vector3 yVector)
		{
			var Z = zVector.Normalize();
			var X = yVector.Cross(Z).Normalize();
			var Y = Z.Cross(X);

			return new Matrix3(X, Y, Z);
		}

		public static readonly Matrix3 Identity = new Matrix3
		(
		    new Vector3(1, 0, 0),
		    new Vector3(0, 1, 0),
		    new Vector3(0, 0, 1)
		);
		#endregion

		#region Operators
		// +
		public static Matrix3 operator+(Matrix3 p1, Matrix3 p2) {return new Matrix3(p1.X+p2.X, p1.Y+p2.Y, p1.Z+p2.Z);}
		public static Matrix3 operator+(Matrix3 p1, float p2) {return new Matrix3(p1.X+p2, p1.Y+p2, p1.Z+p2);}
		public static Matrix3 operator+(float p1, Matrix3 p2) {return new Matrix3(p1+p2.X, p1+p2.Y, p1+p2.Z);}
		// -
		public static Matrix3 operator-(Matrix3 p1, Matrix3 p2) {return new Matrix3(p1.X-p2.X, p1.Y-p2.Y, p1.Z-p2.Z);}
		public static Matrix3 operator-(Matrix3 p1, float p2) {return new Matrix3(p1.X-p2, p1.Y-p2, p1.Z-p2);}
		public static Matrix3 operator-(float p1, Matrix3 p2) {return new Matrix3(p1-p2.X, p1-p2.Y, p1-p2.Z);}
		// *
		public static Matrix3 operator*(Matrix3 p1, Matrix3 p2) {return new Matrix3(p1.X*p2.X, p1.Y*p2.Y, p1.Z*p2.Z);}
		public static Matrix3 operator*(Matrix3 p1, float p2) {return new Matrix3(p1.X*p2, p1.Y*p2, p1.Z*p2);}
		public static Matrix3 operator*(float p1, Matrix3 p2) {return new Matrix3(p1*p2.X, p1*p2.Y, p1*p2.Z);}
		// /
		public static Matrix3 operator/(Matrix3 p1, Matrix3 p2) {return new Matrix3(p1.X/p2.X, p1.Y/p2.Y, p1.Z/p2.Z);}
		public static Matrix3 operator/(Matrix3 p1, float p2) {return new Matrix3(p1.X/p2, p1.Y/p2, p1.Z/p2);}
		public static Matrix3 operator/(float p1, Matrix3 p2) {return new Matrix3(p1/p2.X, p1/p2.Y, p1/p2.Z);}
		// ==
		public static bool operator==(Matrix3 p1, Matrix3 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z);}
		public static bool operator!=(Matrix3 p1, Matrix3 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z);}
		#endregion
		
		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Matrix3)obj == this;
		}

		public override string ToString()
		{
			return string.Format("{0} : {1} : {2}", X, Y, Z);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vector3 Euler()
		{
			if (Z.X < 1)
			{
				if (Z.X > -1)
				{
					return new Vector3((float)MathS.Atan2(Z.Y, Z.Z), (float)MathS.Asin(-Z.X), (float)MathS.Atan2(Y.X, X.X));
				}
				else
				{
					return new Vector3(0, Math.Pi*.5f, -(float)MathS.Atan2(Y.Z, Y.Y));
				}
			}
			else
			{
				return new Vector3(0, -Math.Pi*.5f, (float)MathS.Atan2(-Y.Z, Y.Y));
			}
		}

		public Vector4 Quaternion()
		{
			float w = (float)MathS.Sqrt(1 + X.X + Y.Y + Z.Z) * .5f;
			float delta = 1 / (w * 4);
			return new Vector4
			(
				(Z.Y - Y.Z) * delta,
				(X.Z - Z.X) * delta,
				(Y.X - X.Y) * delta,
				w
			);
		}
		
		public Matrix3 Abs()
		{
			return new Matrix3(X.Abs(), Y.Abs(), Z.Abs());
		}

		public Matrix3 Transpose()
		{
			return new Matrix3
			(
				new Vector3(X.X, Y.X, Z.X),
				new Vector3(X.Y, Y.Y, Z.Y),
				new Vector3(X.Z, Y.Z, Z.Z)
			);
		}

		public Matrix3 Multiply(Matrix3 matrix)
		{
			return new Matrix3
			(
				new Vector3((matrix.X.X*X.X) + (matrix.X.Y*Y.X) + (matrix.X.Z*Z.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y) + (matrix.X.Z*Z.Y), (matrix.X.X*X.Z) + (matrix.X.Y*Y.Z) + (matrix.X.Z*Z.Z)),
				new Vector3((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X) + (matrix.Y.Z*Z.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y) + (matrix.Y.Z*Z.Y), (matrix.Y.X*X.Z) + (matrix.Y.Y*Y.Z) + (matrix.Y.Z*Z.Z)),
				new Vector3((matrix.Z.X*X.X) + (matrix.Z.Y*Y.X) + (matrix.Z.Z*Z.X), (matrix.Z.X*X.Y) + (matrix.Z.Y*Y.Y) + (matrix.Z.Z*Z.Y), (matrix.Z.X*X.Z) + (matrix.Z.Y*Y.Z) + (matrix.Z.Z*Z.Z))
			);
		}

		public Matrix3 RotateAroundAxisX(float angle)
		{
			float tCos = (float)MathS.Cos(angle), tSin = (float)MathS.Sin(angle);
			return new Matrix3
			(
				X,
				new Vector3((Y.X*tCos) - (Z.X*tSin), (Y.Y*tCos) - (Z.Y*tSin), (Y.Z*tCos) - (Z.Z*tSin)),
				new Vector3((Y.X*tSin) + (Z.X*tCos), (Y.Y*tSin) + (Z.Y*tCos), (Y.Z*tSin) + (Z.Z*tCos))
			);
		}

		public Matrix3 RotateAroundAxisY(float angle)
		{
			float tCos = (float)MathS.Cos(angle), tSin = (float)MathS.Sin(angle);
			return new Matrix3
			(
				new Vector3((Z.X*tSin) + (X.X*tCos), (Z.Y*tSin) + (X.Y*tCos), (Z.Z*tSin) + (X.Z*tCos)),
				Y,
				new Vector3((Z.X*tCos) - (X.X*tSin), (Z.Y*tCos) - (X.Y*tSin), (Z.Z*tCos) - (X.Z*tSin))
			);
		}

		public Matrix3 RotateAroundAxisZ(float angle)
		{
			float tCos = (float)MathS.Cos(angle), tSin = (float)MathS.Sin(angle);
			return new Matrix3
			(
				new Vector3((X.X*tCos) - (Y.X*tSin), (X.Y*tCos) - (Y.Y*tSin), (X.Z*tCos) - (Y.Z*tSin)),
				new Vector3((X.X*tSin) + (Y.X*tCos), (X.Y*tSin) + (Y.Y*tCos), (X.Z*tSin) + (Y.Z*tCos)),
				Z
			);
		}

		public Matrix3 RotateAroundWorldAxisX(float angle)
		{
			angle = -angle;
			float tCos = (float)MathS.Cos(angle), tSin = (float)MathS.Sin(angle);
			return new Matrix3
			(
				new Vector3(X.X, (X.Y*tCos) - (X.Z*tSin), (X.Y*tSin) + (X.Z*tCos)),
				new Vector3(Y.X, (Y.Y*tCos) - (Y.Z*tSin), (Y.Y*tSin) + (Y.Z*tCos)),
				new Vector3(Z.X, (Z.Y*tCos) - (Z.Z*tSin), (Z.Y*tSin) + (Z.Z*tCos))
			);
		}

		public Matrix3 RotateAroundWorldAxisY(float angle)
		{
			angle = -angle;
			float tCos = (float)MathS.Cos(angle), tSin = (float)MathS.Sin(angle);
			return new Matrix3
			(
				new Vector3((X.Z*tSin) + (X.X*tCos), X.Y, (X.Z*tCos) - (X.X*tSin)),
				new Vector3((Y.Z*tSin) + (Y.X*tCos), Y.Y, (Y.Z*tCos) - (Y.X*tSin)),
				new Vector3((Z.Z*tSin) + (Z.X*tCos), Z.Y, (Z.Z*tCos) - (Z.X*tSin))
			);
		}

		public Matrix3 RotateAroundWorldAxisZ(float angle)
		{
			angle = -angle;
			float tCos = (float)MathS.Cos(angle), tSin = (float)MathS.Sin(angle);
			return new Matrix3
			(
				new Vector3((X.X*tCos) - (X.Y*tSin), (X.X*tSin) + (X.Y*tCos), X.Z),
				new Vector3((Y.X*tCos) - (Y.Y*tSin), (Y.X*tSin) + (Y.Y*tCos), Y.Z),
				new Vector3((Z.X*tCos) - (Z.Y*tSin), (Z.X*tSin) + (Z.Y*tCos), Z.Z)
			);
		}

		public Matrix3 Rotate(Vector3 axis, float angle)
		{
			return new Matrix3(X.RotateAround(axis, angle), Y.RotateAround(axis, angle), Z.RotateAround(axis, angle));
		}
		#endregion
	}
}