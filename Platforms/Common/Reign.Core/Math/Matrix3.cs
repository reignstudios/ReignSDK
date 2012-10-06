using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix3
	{
		#region Properties
		public Vector3 X, Y, Z;
		public Vector3 w;

		public Vector3 Right {get{return X;}}
		public Vector3 Left {get{return -X;}}
		public Vector3 Up {get{return Y;}}
		public Vector3 Down {get{return -Y;}}
		public Vector3 Front {get{return Z;}}
		public Vector3 Back {get{return -Z;}}
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

		public static Matrix3 FromScale(float scale)
		{
			return new Matrix3
			(
				new Vector3(scale, 0, 0),
				new Vector3(0, scale, 0),
				new Vector3(0, 0, scale)
			);
		}

		public static void FromScale(float scale, out Matrix3 result)
		{
			result.X = new Vector3(scale, 0, 0);
			result.Y = new Vector3(0, scale, 0);
			result.Z = new Vector3(0, 0, scale);
			result.w = new Vector3();
		}

		public static Matrix3 FromScale(Vector3 scale)
		{
			return new Matrix3
			(
				new Vector3(scale.X, 0, 0),
				new Vector3(0, scale.Y, 0),
				new Vector3(0, 0, scale.Z)
			);
		}

		public static void FromScale(Vector3 scale, out Matrix3 result)
		{
			result.X = new Vector3(scale.X, 0, 0);
			result.Y = new Vector3(0, scale.Y, 0);
			result.Z = new Vector3(0, 0, scale.Z);
			result.w = new Vector3();
		}

		public static Matrix3 FromOuterProduct(ref Vector3 vector1, ref Vector3 vector2)
        {
			return new Matrix3
			(
				new Vector3(vector1.X * vector2.X, vector1.X * vector2.Y, vector1.X * vector2.Z),
				new Vector3(vector1.Y * vector2.X, vector1.Y * vector2.Y, vector1.Y * vector2.Z),
				new Vector3(vector1.Z * vector2.X, vector1.Z * vector2.Y, vector1.Z * vector2.Z)
			);
        }

		public static void FromOuterProduct(ref Vector3 vector1, ref Vector3 vector2, out Matrix3 result)
        {
            result.X.X = vector1.X * vector2.X;
            result.X.Y = vector1.X * vector2.Y;
            result.X.Z = vector1.X * vector2.Z;
			result.X.w = 0;

            result.Y.X = vector1.Y * vector2.X;
            result.Y.Y = vector1.Y * vector2.Y;
            result.Y.Z = vector1.Y * vector2.Z;
			result.Y.w = 0;

            result.Z.X = vector1.Z * vector2.X;
            result.Z.Y = vector1.Z * vector2.Y;
            result.Z.Z = vector1.Z * vector2.Z;
			result.Z.w = 0;

			result.w = Vector3.Zero;
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

		public static void FromEuler(ref Vector3 euler, out Matrix3 result)
		{
			float cx = (float)System.Math.Cos(euler.X);
			float sx = (float)System.Math.Sin(euler.X);
			float cy = (float)System.Math.Cos(euler.Y);
			float sy = (float)System.Math.Sin(euler.Y);
			float cz = (float)System.Math.Cos(euler.Z);
			float sz = (float)System.Math.Sin(euler.Z);

			// multiply ZYX
			result.X.X = cy*cz;
			result.X.Y = cz*sx*sy - cx*sz;
			result.X.Z = cx*cz*sy + sx*sz;
			result.X.w = 0;

			result.Y.X = cy*sz;
			result.Y.Y = cx*cz + sx*sy*sz;
			result.Y.Z = -cz*sx + cx*sy*sz;
			result.Y.w = 0;

			result.Z.X = -sy;
			result.Z.Y = cy*sx;
			result.Z.Z = cx*cy;
			result.Z.w = 0;

			result.w = Vector3.Zero;
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

		public static void FromEuler(float eulerX, float eulerY, float eulerZ, out Matrix3 result)
		{
			float cx = (float)System.Math.Cos(eulerX);
			float sx = (float)System.Math.Sin(eulerX);
			float cy = (float)System.Math.Cos(eulerY);
			float sy = (float)System.Math.Sin(eulerY);
			float cz = (float)System.Math.Cos(eulerZ);
			float sz = (float)System.Math.Sin(eulerZ);

			// multiply ZYX
			result.X.X = cy*cz;
			result.X.Y = cz*sx*sy - cx*sz;
			result.X.Z = cx*cz*sy + sx*sz;
			result.X.w = 0;

			result.Y.X = cy*sz;
			result.Y.Y = cx*cz + sx*sy*sz;
			result.Y.Z = -cz*sx + cx*sy*sz;
			result.Y.w = 0;

			result.Z.X = -sy;
			result.Z.Y = cy*sx;
			result.Z.Z = cx*cy;
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public static Matrix3 FromQuaternion(Quaternion quaternion)
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

		public static void FromQuaternion(ref Quaternion quaternion, out Matrix3 result)
		{
			var squared = new Vector4(quaternion.X*quaternion.X, quaternion.Y*quaternion.Y, quaternion.Z*quaternion.Z, quaternion.W*quaternion.W);
			float invSqLength = 1 / (squared.X + squared.Y + squared.Z + squared.W);

			float temp1 = quaternion.X * quaternion.Y;
			float temp2 = quaternion.Z * quaternion.W;
			float temp3 = quaternion.X * quaternion.Z;
			float temp4 = quaternion.Y * quaternion.W;
			float temp5 = quaternion.Y * quaternion.Z;
			float temp6 = quaternion.X * quaternion.W;

			result.X.X = (squared.X-squared.Y-squared.Z+squared.W) * invSqLength;
			result.X.Y = 2*(temp1-temp2) * invSqLength;
			result.X.Z = 2*(temp3+temp4) * invSqLength;
			result.X.w = 0;

			result.Y.X = 2*(temp1+temp2) * invSqLength;
			result.Y.Y = (-squared.X+squared.Y-squared.Z+squared.W) * invSqLength;
			result.Y.Z = 2*(temp5-temp6) * invSqLength;
			result.Y.w = 0;

			result.Z.X = 2*(temp3-temp4) * invSqLength;
			result.Z.Y = 2*(temp5+temp6) * invSqLength;
			result.Z.Z = (-squared.X-squared.Y+squared.Z+squared.W) * invSqLength;
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 FromRotationAxis(Vector3 axis, float angle)
		{
			Core.Quaternion quaternion;
			Core.Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			return Matrix3.FromQuaternion(quaternion);
		}

		public static void FromRotationAxis(ref Vector3 axis, float angle, out Matrix3 result)
		{
			Core.Quaternion quaternion;
			Core.Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			Matrix3.FromQuaternion(ref quaternion, out result);
		}

		public static Matrix3 FromCross(Vector3 zVector, Vector3 yVector)
		{
			var Z = zVector.Normalize();
			var X = yVector.Cross(Z).Normalize();
			var Y = Z.Cross(X);

			return new Matrix3(X, Y, Z);
		}

		public static void FromCross(ref Vector3 zVector, ref Vector3 yVector, out Matrix3 result)
		{
			var Z = zVector.Normalize();
			var X = yVector.Cross(Z).Normalize();
			var Y = Z.Cross(X);

			result.X = X;
			result.Y = Y;
			result.Z = Z;
			result.w = Vector3.Zero;
		}

		public static void FromCross(ref Vector3 vector, out Matrix3 result)
        {
            result.X.X = 0;
            result.X.Y = -vector.Z;
            result.X.Z = vector.Y;
			result.X.w = 0;

            result.Y.X = vector.Z;
            result.Y.Y = 0;
            result.Y.Z = -vector.X;
			result.Y.w = 0;

            result.Z.X = -vector.Y;
            result.Z.Y = vector.X;
            result.Z.Z = 0;
			result.Z.w = 0;

			result.w = Vector3.Zero;
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
		public static void Add(ref Matrix3 value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
			result.w = Vector3.Zero;
		}

		public static void Add(ref Matrix3 value1, float value2, out Matrix3 result)
		{
			result.X = value1.X + value2;
			result.Y = value1.Y + value2;
			result.Z = value1.Z + value2;
			result.w = Vector3.Zero;
		}

		public static void Add(float value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1 + value2.X;
			result.Y = value1 + value2.Y;
			result.Z = value1 + value2.Z;
			result.w = Vector3.Zero;
		}

		public static Matrix3 operator+(Matrix3 p1, Matrix3 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			p1.Z += p2.Z;
			return p1;
		}

		public static Matrix3 operator+(Matrix3 p1, Vector3 p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			return p1;
		}

		public static Matrix3 operator+(Vector3 p1, Matrix3 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			return p2;
		}

		public static Matrix3 operator+(Matrix3 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			return p1;
		}

		public static Matrix3 operator+(float p1, Matrix3 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			return p2;
		}

		// -
		public static void Sub(ref Matrix3 value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
			result.w = Vector3.Zero;
		}

		public static void Sub(ref Matrix3 value1, float value2, out Matrix3 result)
		{
			result.X = value1.X - value2;
			result.Y = value1.Y - value2;
			result.Z = value1.Z - value2;
			result.w = Vector3.Zero;
		}

		public static void Sub(float value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1 - value2.X;
			result.Y = value1 - value2.Y;
			result.Z = value1 - value2.Z;
			result.w = Vector3.Zero;
		}

		public static void Neg(ref Matrix3 value, out Matrix3 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.w = Vector3.Zero;
		}

		public static Matrix3 operator-(Matrix3 p1, Matrix3 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			p1.Z -= p2.Z;
			return p1;
		}

		public static Matrix3 operator-(Matrix3 p1, Vector3 p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			return p1;
		}

		public static Matrix3 operator-(Vector3 p1, Matrix3 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			return p2;
		}

		public static Matrix3 operator-(Matrix3 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			return p1;
		}

		public static Matrix3 operator-(float p1, Matrix3 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			return p2;
		}

		public static Matrix3 operator-(Matrix3 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			p2.Z = -p2.Z;
			return p2;
		}

		// *
		public static void Mul(ref Matrix3 value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
			result.w = Vector3.Zero;
		}

		public static void Mul(ref Matrix3 value1, float value2, out Matrix3 result)
		{
			result.X = value1.X * value2;
			result.Y = value1.Y * value2;
			result.Z = value1.Z * value2;
			result.w = Vector3.Zero;
		}

		public static void Mul(float value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1 * value2.X;
			result.Y = value1 * value2.Y;
			result.Z = value1 * value2.Z;
			result.w = Vector3.Zero;
		}

		public static Matrix3 operator*(Matrix3 p1, Matrix3 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			p1.Z *= p2.Z;
			return p1;
		}

		public static Matrix3 operator*(Matrix3 p1, Vector3 p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			return p1;
		}

		public static Matrix3 operator*(Vector3 p1, Matrix3 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			return p2;
		}

		public static Matrix3 operator*(Matrix3 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			return p1;
		}

		public static Matrix3 operator*(float p1, Matrix3 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			return p2;
		}

		// /
		public static void Div(ref Matrix3 value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
			result.w = Vector3.Zero;
		}

		public static void Div(ref Matrix3 value1, float value2, out Matrix3 result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
			result.Z = value1.Z / value2;
			result.w = Vector3.Zero;
		}

		public static void Div(float value1, ref Matrix3 value2, out Matrix3 result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
			result.Z = value1 / value2.Z;
			result.w = Vector3.Zero;
		}

		public static Matrix3 operator/(Matrix3 p1, Matrix3 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			p1.Z /= p2.Z;
			return p1;
		}

		public static Matrix3 operator/(Matrix3 p1, Vector3 p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			return p1;
		}

		public static Matrix3 operator/(Vector3 p1, Matrix3 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			return p2;
		}

		public static Matrix3 operator/(Matrix3 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			return p1;
		}

		public static Matrix3 operator/(float p1, Matrix3 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			return p2;
		}

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
					return new Vector3((float)Math.Atan2(Z.Y, Z.Z), (float)Math.Asin(-Z.X), (float)Math.Atan2(Y.X, X.X));
				}
				else
				{
					return new Vector3(0, MathUtilities.PiHalf, -(float)Math.Atan2(Y.Z, Y.Y));
				}
			}
			else
			{
				return new Vector3(0, -MathUtilities.PiHalf, (float)Math.Atan2(-Y.Z, Y.Y));
			}
		}

		public static void Euler(ref Matrix3 matrix, out Vector3 result)
		{
			if (matrix.Z.X < 1)
			{
				if (matrix.Z.X > -1)
				{
					result.X = (float)Math.Atan2(matrix.Z.Y, matrix.Z.Z);
					result.Y = (float)Math.Asin(-matrix.Z.X);
					result.Z = (float)Math.Atan2(matrix.Y.X, matrix.X.X);
					result.w = 0;
				}
				else
				{
					result.X = 0;
					result.Y = MathUtilities.PiHalf;
					result.Z = -(float)Math.Atan2(matrix.Y.Z, matrix.Y.Y);
					result.w = 0;
				}
			}
			else
			{
				result.X = 0;
				result.Y = -MathUtilities.PiHalf;
				result.Z = (float)Math.Atan2(-matrix.Y.Z, matrix.Y.Y);
				result.w = 0;
			}
		}
		
		public Matrix3 Abs()
		{
			return new Matrix3(X.Abs(), Y.Abs(), Z.Abs());
		}

		public static void Abs(ref Matrix3 matrix, out Matrix3 result)
		{
			Vector3.Abs(ref matrix.X, out result.X);
			Vector3.Abs(ref matrix.Y, out result.Y);
			Vector3.Abs(ref matrix.Z, out result.Z);
			result.w = Vector3.Zero;
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

		public static void Transpose(Matrix3 matrix, out Matrix3 result)
		{
			result.X.X = matrix.X.X;
			result.X.Y = matrix.Y.X;
			result.X.Z = matrix.Z.X;
			result.X.w = 0;

			result.Y.X = matrix.X.Y;
			result.Y.Y = matrix.Y.Y;
			result.Y.Z = matrix.Z.Y;
			result.Y.w = 0;

			result.Z.X = matrix.X.Z;
			result.Z.Y = matrix.Y.Z;
			result.Z.Z = matrix.Z.Z;
			result.Z.w = 0;

			result.w = Vector3.Zero;
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

		public static void Multiply(ref Matrix3 matrix1, ref Matrix3 matrix2, out Matrix3 result)
		{
			result.X.X = (matrix1.X.X*matrix2.X.X) + (matrix1.X.Y*matrix2.Y.X) + (matrix1.X.Z*matrix2.Z.X);
			result.X.Y = (matrix1.X.X*matrix2.X.Y) + (matrix1.X.Y*matrix2.Y.Y) + (matrix1.X.Z*matrix2.Z.Y);
			result.X.Z = (matrix1.X.X*matrix2.X.Z) + (matrix1.X.Y*matrix2.Y.Z) + (matrix1.X.Z*matrix2.Z.Z);
			result.X.w = 0;

			result.Y.X = (matrix1.Y.X*matrix2.X.X) + (matrix1.Y.Y*matrix2.Y.X) + (matrix1.Y.Z*matrix2.Z.X);
			result.Y.Y = (matrix1.Y.X*matrix2.X.Y) + (matrix1.Y.Y*matrix2.Y.Y) + (matrix1.Y.Z*matrix2.Z.Y);
			result.Y.Z = (matrix1.Y.X*matrix2.X.Z) + (matrix1.Y.Y*matrix2.Y.Z) + (matrix1.Y.Z*matrix2.Z.Z);
			result.Y.w = 0;

			result.Z.X = (matrix1.Z.X*matrix2.X.X) + (matrix1.Z.Y*matrix2.Y.X) + (matrix1.Z.Z*matrix2.Z.X);
			result.Z.Y = (matrix1.Z.X*matrix2.X.Y) + (matrix1.Z.Y*matrix2.Y.Y) + (matrix1.Z.Z*matrix2.Z.Y);
			result.Z.Z = (matrix1.Z.X*matrix2.X.Z) + (matrix1.Z.Y*matrix2.Y.Z) + (matrix1.Z.Z*matrix2.Z.Z);
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 MultiplyTransposed(Matrix3 matrix)
        {
			return new Matrix3
			(
				new Vector3
				(
					X.X * matrix.X.X + Y.X * matrix.Y.X + Z.X * matrix.Z.X,
					X.X * matrix.X.Y + Y.X * matrix.Y.Y + Z.X * matrix.Z.Y,
					X.X * matrix.X.Z + Y.X * matrix.Y.Z + Z.X * matrix.Z.Z
				),
				new Vector3
				(
					X.Y * matrix.X.X + Y.Y * matrix.Y.X + Z.Y * matrix.Z.X,
					X.Y * matrix.X.Y + Y.Y * matrix.Y.Y + Z.Y * matrix.Z.Y,
					X.Y * matrix.X.Z + Y.Y * matrix.Y.Z + Z.Y * matrix.Z.Z
				),
				new Vector3
				(
					X.Z * matrix.X.X + Y.Z * matrix.Y.X + Z.Z * matrix.Z.X,
					X.Z * matrix.X.Y + Y.Z * matrix.Y.Y + Z.Z * matrix.Z.Y,
					X.Z * matrix.X.Z + Y.Z * matrix.Y.Z + Z.Z * matrix.Z.Z
				)
			);
        }

		public static void MultiplyTransposed(ref Matrix3 transpose, ref Matrix3 matrix, out Matrix3 result)
        {
            result.X.X = transpose.X.X * matrix.X.X + transpose.Y.X * matrix.Y.X + transpose.Z.X * matrix.Z.X;
            result.X.Y = transpose.X.X * matrix.X.Y + transpose.Y.X * matrix.Y.Y + transpose.Z.X * matrix.Z.Y;
            result.X.Z = transpose.X.X * matrix.X.Z + transpose.Y.X * matrix.Y.Z + transpose.Z.X * matrix.Z.Z;
			result.X.w = 0;

            result.Y.X = transpose.X.Y * matrix.X.X + transpose.Y.Y * matrix.Y.X + transpose.Z.Y * matrix.Z.X;
            result.Y.Y = transpose.X.Y * matrix.X.Y + transpose.Y.Y * matrix.Y.Y + transpose.Z.Y * matrix.Z.Y;
            result.Y.Z = transpose.X.Y * matrix.X.Z + transpose.Y.Y * matrix.Y.Z + transpose.Z.Y * matrix.Z.Z;
			result.Y.w = 0;

            result.Z.X = transpose.X.Z * matrix.X.X + transpose.Y.Z * matrix.Y.X + transpose.Z.Z * matrix.Z.X;
            result.Z.Y = transpose.X.Z * matrix.X.Y + transpose.Y.Z * matrix.Y.Y + transpose.Z.Z * matrix.Z.Y;
            result.Z.Z = transpose.X.Z * matrix.X.Z + transpose.Y.Z * matrix.Y.Z + transpose.Z.Z * matrix.Z.Z;
			result.Z.w = 0;

			result.w = Vector3.Zero;
        }

		public float Determinant()
        {
            return X.X * Y.Y * Z.Z + X.Y * Y.Z * Z.X + X.Z * Y.X * Z.Y -
                   Z.X * Y.Y * X.Z - Z.Y * Y.Z * X.X - Z.Z * Y.X * X.Y;
        }

		public static void Determinant(ref Matrix3 matrix, out float result)
        {
            result =
				matrix.X.X * matrix.Y.Y * matrix.Z.Z + matrix.X.Y * matrix.Y.Z * matrix.Z.X + matrix.X.Z * matrix.Y.X * matrix.Z.Y -
                matrix.Z.X * matrix.Y.Y * matrix.X.Z - matrix.Z.Y * matrix.Y.Z * matrix.X.X - matrix.Z.Z * matrix.Y.X * matrix.X.Y;
        }

		public Matrix3 Invert()
        {
            float determinant = 1 / this.Determinant();

			return new Matrix3
			(
				new Vector3
				(
					(Y.Y * Z.Z - Y.Z * Z.Y) * determinant,
					(X.Z * Z.Y - Z.Z * X.Y) * determinant,
					(X.Y * Y.Z - Y.Y * X.Z) * determinant
				),
				new Vector3
				(
					(Y.Z * Z.X - Y.X * Z.Z) * determinant,
					(X.X * Z.Z - X.Z * Z.X) * determinant,
					(X.Z * Y.X - X.X * Y.Z) * determinant
				),
				new Vector3
				(
					(Y.X * Z.Y - Y.Y * Z.X) * determinant,
					(X.Y * Z.X - X.X * Z.Y) * determinant,
					(X.X * Y.Y - X.Y * Y.X) * determinant
				)
			);
        }

		public static void Invert(ref Matrix3 matrix, out Matrix3 result)
        {
            float determinant = 1 / matrix.Determinant();

            result.X.X = (matrix.Y.Y * matrix.Z.Z - matrix.Y.Z * matrix.Z.Y) * determinant;
            result.X.Y = (matrix.X.Z * matrix.Z.Y - matrix.Z.Z * matrix.X.Y) * determinant;
            result.X.Z = (matrix.X.Y * matrix.Y.Z - matrix.Y.Y * matrix.X.Z) * determinant;
			result.X.w = 0;

            result.Y.X = (matrix.Y.Z * matrix.Z.X - matrix.Y.X * matrix.Z.Z) * determinant;
            result.Y.Y = (matrix.X.X * matrix.Z.Z - matrix.X.Z * matrix.Z.X) * determinant;
            result.Y.Z = (matrix.X.Z * matrix.Y.X - matrix.X.X * matrix.Y.Z) * determinant;
			result.Y.w = 0;

            result.Z.X = (matrix.Y.X * matrix.Z.Y - matrix.Y.Y * matrix.Z.X) * determinant;
            result.Z.Y = (matrix.X.Y * matrix.Z.X - matrix.X.X * matrix.Z.Y) * determinant;
            result.Z.Z = (matrix.X.X * matrix.Y.Y - matrix.X.Y * matrix.Y.X) * determinant;
			result.Z.w = 0;

			result.w = Vector3.Zero;
        }

		public float AdaptiveDeterminant(out int subMatrixCode)
        {
            //Try the full matrix first.
            float determinant = X.X * Y.Y * Z.Z + X.Y * Y.Z * Z.X + X.Z * Y.X * Z.Y -
                                Z.X * Y.Y * X.Z - Z.Y * Y.Z * X.X - Z.Z * Y.X * X.Y;

            if (determinant != 0) //This could be a little numerically flimsy.  Fortunately, the way this method is used, that doesn't matter!
            {
                subMatrixCode = 0;
                return determinant;
            }

            //Try m11, m12, m21, m22.
            determinant = X.X * Y.Y - X.Y * Y.X;
            if (determinant != 0)
            {
                subMatrixCode = 1;
                return determinant;
            }

            //Try m22, m23, m32, m33.
            determinant = Y.Y * Z.Z - Y.Z * Z.Y;
            if (determinant != 0)
            {
                subMatrixCode = 2;
                return determinant;
            }

            //Try m11, m13, m31, m33.
            determinant = X.X * Z.Z - X.Z * X.Y;
            if (determinant != 0)
            {
                subMatrixCode = 3;
                return determinant;
            }

            //Try m11.
            if (X.X != 0)
            {
                subMatrixCode = 4;
                return X.X;
            }

            //Try m22.
            if (Y.Y != 0)
            {
                subMatrixCode = 5;
                return Y.Y;
            }

            //Try m33.
            if (Z.Z != 0)
            {
                subMatrixCode = 6;
                return Z.Z;
            }

            //It's completely singular!
            subMatrixCode = -1;
            return 0;
        }

		public static void AdaptiveInvert(ref Matrix3 matrix, out Matrix3 result)
        {
            int submatrix;
            float determinantInverse = 1 / matrix.AdaptiveDeterminant(out submatrix);
            float m11, m12, m13, m21, m22, m23, m31, m32, m33;
            switch (submatrix)
            {
                case 0: //Full matrix.
                    m11 = (matrix.Y.Y * matrix.Z.Z - matrix.Y.Z * matrix.Z.Y) * determinantInverse;
                    m12 = (matrix.X.Z * matrix.Z.Y - matrix.Z.Z * matrix.X.Y) * determinantInverse;
                    m13 = (matrix.X.Y * matrix.Y.Z - matrix.Y.Y * matrix.X.Z) * determinantInverse;

                    m21 = (matrix.Y.Z * matrix.Z.X - matrix.Y.X * matrix.Z.Z) * determinantInverse;
                    m22 = (matrix.X.X * matrix.Z.Z - matrix.X.Z * matrix.Z.X) * determinantInverse;
                    m23 = (matrix.X.Z * matrix.Y.X - matrix.X.X * matrix.Y.Z) * determinantInverse;

                    m31 = (matrix.Y.X * matrix.Z.Y - matrix.Y.Y * matrix.Z.X) * determinantInverse;
                    m32 = (matrix.X.Y * matrix.Z.X - matrix.X.X * matrix.Z.Y) * determinantInverse;
                    m33 = (matrix.X.X * matrix.Y.Y - matrix.X.Y * matrix.Y.X) * determinantInverse;
                    break;

                case 1: //Upper left matrix, m11, m12, m21, m22.
                    m11 = matrix.Y.Y * determinantInverse;
                    m12 = -matrix.X.Y * determinantInverse;
                    m13 = 0;

                    m21 = -matrix.Y.X * determinantInverse;
                    m22 = matrix.X.X * determinantInverse;
                    m23 = 0;

                    m31 = 0;
                    m32 = 0;
                    m33 = 0;
                    break;

                case 2: //Lower right matrix, m22, m23, m32, m33.
                    m11 = 0;
                    m12 = 0;
                    m13 = 0;

                    m21 = 0;
                    m22 = matrix.Z.Z * determinantInverse;
                    m23 = -matrix.Y.Z * determinantInverse;

                    m31 = 0;
                    m32 = -matrix.Z.Y * determinantInverse;
                    m33 = matrix.Y.Y * determinantInverse;
                    break;

                case 3: //Corners, m11, m31, m13, m33.
                    m11 = matrix.Z.Z * determinantInverse;
                    m12 = 0;
                    m13 = -matrix.X.Z * determinantInverse;

                    m21 = 0;
                    m22 = 0;
                    m23 = 0;

                    m31 = -matrix.Z.X * determinantInverse;
                    m32 = 0;
                    m33 = matrix.X.X * determinantInverse;
                    break;

                case 4: //X.X
                    m11 = 1 / matrix.X.X;
                    m12 = 0;
                    m13 = 0;

                    m21 = 0;
                    m22 = 0;
                    m23 = 0;

                    m31 = 0;
                    m32 = 0;
                    m33 = 0;
                    break;

                case 5: //Y.Y
                    m11 = 0;
                    m12 = 0;
                    m13 = 0;

                    m21 = 0;
                    m22 = 1 / matrix.Y.Y;
                    m23 = 0;

                    m31 = 0;
                    m32 = 0;
                    m33 = 0;
                    break;

                case 6: //Z.Z
                    m11 = 0;
                    m12 = 0;
                    m13 = 0;

                    m21 = 0;
                    m22 = 0;
                    m23 = 0;

                    m31 = 0;
                    m32 = 0;
                    m33 = 1 / matrix.Z.Z;
                    break;

                default: //Completely singular.
                    m11 = 0; m12 = 0; m13 = 0; m21 = 0; m22 = 0; m23 = 0; m31 = 0; m32 = 0; m33 = 0;
                    break;
            }

            result.X.X = m11;
            result.X.Y = m12;
            result.X.Z = m13;
			result.X.w = 0;

            result.Y.X = m21;
            result.Y.Y = m22;
            result.Y.Z = m23;
			result.Y.w = 0;

            result.Z.X = m31;
            result.Z.Y = m32;
            result.Z.Z = m33;
			result.Z.w = 0;

			result.w = Vector3.Zero;
        }

		public Matrix3 RotateAroundAxisX(float angle)
		{
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			return new Matrix3
			(
				X,
				new Vector3((Y.X*tCos) - (Z.X*tSin), (Y.Y*tCos) - (Z.Y*tSin), (Y.Z*tCos) - (Z.Z*tSin)),
				new Vector3((Y.X*tSin) + (Z.X*tCos), (Y.Y*tSin) + (Z.Y*tCos), (Y.Z*tSin) + (Z.Z*tCos))
			);
		}

		public static void RotateAroundAxisX(ref Matrix3 matrix, float angle, out Matrix3 result)
		{
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			result.X = matrix.X;

			result.Y.X = (matrix.Y.X*tCos) - (matrix.Z.X*tSin);
			result.Y.Y = (matrix.Y.Y*tCos) - (matrix.Z.Y*tSin);
			result.Y.Z = (matrix.Y.Z*tCos) - (matrix.Z.Z*tSin);
			result.Y.w = 0;

			result.Z.X = (matrix.Y.X*tSin) + (matrix.Z.X*tCos);
			result.Z.Y = (matrix.Y.Y*tSin) + (matrix.Z.Y*tCos);
			result.Z.Z = (matrix.Y.Z*tSin) + (matrix.Z.Z*tCos);
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 RotateAroundAxisY(float angle)
		{
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			return new Matrix3
			(
				new Vector3((Z.X*tSin) + (X.X*tCos), (Z.Y*tSin) + (X.Y*tCos), (Z.Z*tSin) + (X.Z*tCos)),
				Y,
				new Vector3((Z.X*tCos) - (X.X*tSin), (Z.Y*tCos) - (X.Y*tSin), (Z.Z*tCos) - (X.Z*tSin))
			);
		}

		public static void RotateAroundAxisY(ref Matrix3 matrix, float angle, out Matrix3 result)
		{
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			result.X.X = (matrix.Z.X*tSin) + (matrix.X.X*tCos);
			result.X.Y = (matrix.Z.Y*tSin) + (matrix.X.Y*tCos);
			result.X.Z = (matrix.Z.Z*tSin) + (matrix.X.Z*tCos);
			result.X.w = 0;

			result.Y = matrix.Y;

			result.Z.X = (matrix.Z.X*tCos) - (matrix.X.X*tSin);
			result.Z.Y = (matrix.Z.Y*tCos) - (matrix.X.Y*tSin);
			result.Z.Z = (matrix.Z.Z*tCos) - (matrix.X.Z*tSin);
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 RotateAroundAxisZ(float angle)
		{
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			return new Matrix3
			(
				new Vector3((X.X*tCos) - (Y.X*tSin), (X.Y*tCos) - (Y.Y*tSin), (X.Z*tCos) - (Y.Z*tSin)),
				new Vector3((X.X*tSin) + (Y.X*tCos), (X.Y*tSin) + (Y.Y*tCos), (X.Z*tSin) + (Y.Z*tCos)),
				Z
			);
		}

		public static void RotateAroundAxisZ(ref Matrix3 matrix, float angle, out Matrix3 result)
		{
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			result.X.X = (matrix.X.X*tCos) - (matrix.Y.X*tSin);
			result.X.Y = (matrix.X.Y*tCos) - (matrix.Y.Y*tSin);
			result.X.Z = (matrix.X.Z*tCos) - (matrix.Y.Z*tSin);
			result.X.w = 0;

			result.Y.X = (matrix.X.X*tSin) + (matrix.Y.X*tCos);
			result.Y.Y = (matrix.X.Y*tSin) + (matrix.Y.Y*tCos);
			result.Y.Z = (matrix.X.Z*tSin) + (matrix.Y.Z*tCos);
			result.Y.w = 0;

			result.Z = matrix.Z;
			result.w = Vector3.Zero;
		}

		public Matrix3 RotateAroundWorldAxisX(float angle)
		{
			angle = -angle;
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			return new Matrix3
			(
				new Vector3(X.X, (X.Y*tCos) - (X.Z*tSin), (X.Y*tSin) + (X.Z*tCos)),
				new Vector3(Y.X, (Y.Y*tCos) - (Y.Z*tSin), (Y.Y*tSin) + (Y.Z*tCos)),
				new Vector3(Z.X, (Z.Y*tCos) - (Z.Z*tSin), (Z.Y*tSin) + (Z.Z*tCos))
			);
		}

		public static void RotateAroundWorldAxisX(ref Matrix3 matrix, float angle, out Matrix3 result)
		{
			angle = -angle;
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			result.X.X = matrix.X.X;
			result.X.Y = (matrix.X.Y*tCos) - (matrix.X.Z*tSin);
			result.X.Z = (matrix.X.Y*tSin) + (matrix.X.Z*tCos);
			result.X.w = 0;

			result.Y.X = matrix.Y.X;
			result.Y.Y = (matrix.Y.Y*tCos) - (matrix.Y.Z*tSin);
			result.Y.Z = (matrix.Y.Y*tSin) + (matrix.Y.Z*tCos);
			result.Y.w = 0;

			result.Z.X = matrix.Z.X;
			result.Z.Y = (matrix.Z.Y*tCos) - (matrix.Z.Z*tSin);
			result.Z.Z = (matrix.Z.Y*tSin) + (matrix.Z.Z*tCos);
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 RotateAroundWorldAxisY(float angle)
		{
			angle = -angle;
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			return new Matrix3
			(
				new Vector3((X.Z*tSin) + (X.X*tCos), X.Y, (X.Z*tCos) - (X.X*tSin)),
				new Vector3((Y.Z*tSin) + (Y.X*tCos), Y.Y, (Y.Z*tCos) - (Y.X*tSin)),
				new Vector3((Z.Z*tSin) + (Z.X*tCos), Z.Y, (Z.Z*tCos) - (Z.X*tSin))
			);
		}

		public static void RotateAroundWorldAxisY(ref Matrix3 matrix, float angle, out Matrix3 result)
		{
			angle = -angle;
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			result.X.X = (matrix.X.Z*tSin) + (matrix.X.X*tCos);
			result.X.Y = matrix.X.Y;
			result.X.Z = (matrix.X.Z*tCos) - (matrix.X.X*tSin);
			result.X.w = 0;

			result.Y.X = (matrix.Y.Z*tSin) + (matrix.Y.X*tCos);
			result.Y.Y = matrix.Y.Y;
			result.Y.Z = (matrix.Y.Z*tCos) - (matrix.Y.X*tSin);
			result.Y.w = 0;

			result.Z.X = (matrix.Z.Z*tSin) + (matrix.Z.X*tCos);
			result.Z.Y = matrix.Z.Y;
			result.Z.Z = (matrix.Z.Z*tCos) - (matrix.Z.X*tSin);
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 RotateAroundWorldAxisZ(float angle)
		{
			angle = -angle;
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			return new Matrix3
			(
				new Vector3((X.X*tCos) - (X.Y*tSin), (X.X*tSin) + (X.Y*tCos), X.Z),
				new Vector3((Y.X*tCos) - (Y.Y*tSin), (Y.X*tSin) + (Y.Y*tCos), Y.Z),
				new Vector3((Z.X*tCos) - (Z.Y*tSin), (Z.X*tSin) + (Z.Y*tCos), Z.Z)
			);
		}

		public static void RotateAroundWorldAxisZ(ref Matrix3 matrix, float angle, out Matrix3 result)
		{
			angle = -angle;
			float tCos = (float)Math.Cos(angle), tSin = (float)Math.Sin(angle);
			result.X.X = (matrix.X.X*tCos) - (matrix.X.Y*tSin);
			result.X.Y = (matrix.X.X*tSin) + (matrix.X.Y*tCos);
			result.X.Z = matrix.X.Z;
			result.X.w = 0;

			result.Y.X = (matrix.Y.X*tCos) - (matrix.Y.Y*tSin);
			result.Y.Y = (matrix.Y.X*tSin) + (matrix.Y.Y*tCos);
			result.Y.Z = matrix.Y.Z;
			result.Y.w = 0;

			result.Z.X = (matrix.Z.X*tCos) - (matrix.Z.Y*tSin);
			result.Z.Y = (matrix.Z.X*tSin) + (matrix.Z.Y*tCos);
			result.Z.Z = matrix.Z.Z;
			result.Z.w = 0;

			result.w = Vector3.Zero;
		}

		public Matrix3 RotateAround(ref Vector3 axis, float angle)
		{
			// rotate into world space
			var quaternion = Reign.Core.Quaternion.FromRotationAxis(axis, 0);
			Reign.Core.Quaternion.Conjugate(ref quaternion, out quaternion);
			var worldSpaceMatrix = Matrix3.FromQuaternion(quaternion);
			Matrix3.Multiply(ref this, ref worldSpaceMatrix, out worldSpaceMatrix);

			// rotate back to matrix space
			Reign.Core.Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			var qMat = Matrix3.FromQuaternion(quaternion);
			Matrix3.Multiply(ref worldSpaceMatrix, ref qMat, out worldSpaceMatrix);
			return worldSpaceMatrix;
		}

		public static void RotateAround(ref Matrix3 matrix, ref Vector3 axis, float angle, out Matrix3 result)
		{
			// rotate into world space
			var quaternion = Reign.Core.Quaternion.FromRotationAxis(axis, 0);
			Reign.Core.Quaternion.Conjugate(ref quaternion, out quaternion);
			Matrix3.FromQuaternion(ref quaternion, out result);
			Matrix3.Multiply(ref matrix, ref result, out result);

			// rotate back to matrix space
			Reign.Core.Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			var qMat = Matrix3.FromQuaternion(quaternion);
			Matrix3.Multiply(ref result, ref qMat, out result);
		}
		#endregion
	}
}