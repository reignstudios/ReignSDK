using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Matrix4
	{
		#region Properties
		public Vector4 X, Y, Z, W;

		public Vector4 Right {get{return X;}}
		public Vector4 Left {get{return -X;}}
		public Vector4 Up {get{return Y;}}
		public Vector4 Down {get{return -Y;}}
		public Vector4 Front {get{return Z;}}
		public Vector4 Back {get{return -Z;}}
		public Vector4 High {get{return W;}}
		public Vector4 Low {get{return -W;}}

		public Vector3 Translation
		{
			get{return new Vector3(X.W, Y.W, Z.W);}
			set
			{
				X.W = value.X;
				Y.W = value.Y;
				Z.W = value.Z;
			}	
		}

		public static void GetTranslation(ref Matrix4 matrix, out Vector3 result)
		{
			result.X = matrix.X.W;
			result.Y = matrix.Y.W;
			result.Z = matrix.Z.W;
		}
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

		public static Matrix4 FromMatrix3(Matrix3 matrix)
		{
			return new Matrix4
			(
				new Vector4(matrix.X.X, matrix.X.Y, matrix.X.Z, 0),
				new Vector4(matrix.Y.X, matrix.Y.Y, matrix.Y.Z, 0),
				new Vector4(matrix.Z.X, matrix.Z.Y, matrix.Z.Z, 0),
				new Vector4(0, 0, 0, 1)
			);
		}

		public static void FromMatrix3(ref Matrix3 matrix, out Matrix4 result)
		{
			result.X.X = matrix.X.X;
			result.X.Y = matrix.X.Y;
			result.X.Z = matrix.X.Z;
			result.X.W = 0;

			result.Y.X = matrix.Y.X;
			result.Y.Y = matrix.Y.Y;
			result.Y.Z = matrix.Y.Z;
			result.Y.W = 0;

			result.Z.X = matrix.Z.X;
			result.Z.Y = matrix.Z.Y;
			result.Z.Z = matrix.Z.Z;
			result.Z.W = 0;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public static Matrix4 FromAffineTransform(AffineTransform3 transform)
		{
			return new Matrix4
			(
				new Vector4(transform.Transform.X, transform.Translation.X),
				new Vector4(transform.Transform.Y, transform.Translation.Y),
				new Vector4(transform.Transform.Z, transform.Translation.Z),
				new Vector4(0, 0, 0, 1)
			);
		}

		public static void FromAffineTransform(ref AffineTransform3 transform, out Matrix4 result)
		{
			result.X.X = transform.Transform.X.X;
			result.X.Y = transform.Transform.X.Y;
			result.X.Z = transform.Transform.X.Z;
			result.X.W = transform.Translation.X;

			result.Y.X = transform.Transform.Y.X;
			result.Y.Y = transform.Transform.Y.Y;
			result.Y.Z = transform.Transform.Y.Z;
			result.Y.W = transform.Translation.Y;

			result.Z.X = transform.Transform.Z.X;
			result.Z.Y = transform.Transform.Z.Y;
			result.Z.Z = transform.Transform.Z.Z;
			result.Z.W = transform.Translation.Z;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public static Matrix4 FromAffineTransform(Matrix3 transform, Vector3 position)
		{
			return new Matrix4
			(
				new Vector4(transform.X, position.X),
				new Vector4(transform.Y, position.Y),
				new Vector4(transform.Z, position.Z),
				new Vector4(0, 0, 0, 1)
			);
		}

		public static void FromAffineTransform(ref Matrix3 transform, ref Vector3 position, out Matrix4 result)
		{
			result.X.X = transform.X.X;
			result.X.Y = transform.X.Y;
			result.X.Z = transform.X.Z;
			result.X.W = position.X;

			result.Y.X = transform.Y.X;
			result.Y.Y = transform.Y.Y;
			result.Y.Z = transform.Y.Z;
			result.Y.W = position.Y;

			result.Z.X = transform.Z.X;
			result.Z.Y = transform.Z.Y;
			result.Z.Z = transform.Z.Z;
			result.Z.W = position.Z;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public static Matrix4 FromAffineTransform(Matrix3 transform, Vector3 scale, Vector3 position)
		{
			return new Matrix4
			(
				new Vector4(transform.X * scale.X, position.X),
				new Vector4(transform.Y * scale.Y, position.Y),
				new Vector4(transform.Z * scale.Z, position.Z),
				new Vector4(0, 0, 0, 1)
			);
		}

		public static void FromAffineTransform(ref Matrix3 transform, ref Vector3 scale, ref Vector3 position, out Matrix4 result)
		{
			result.X.X = transform.X.X * scale.X;
			result.X.Y = transform.X.Y * scale.X;
			result.X.Z = transform.X.Z * scale.X;
			result.X.W = position.X;

			result.Y.X = transform.Y.X * scale.Y;
			result.Y.Y = transform.Y.Y * scale.Y;
			result.Y.Z = transform.Y.Z * scale.Y;
			result.Y.W = position.Y;

			result.Z.X = transform.Z.X * scale.Z;
			result.Z.Y = transform.Z.Y * scale.Z;
			result.Z.Z = transform.Z.Z * scale.Z;
			result.Z.W = position.Z;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public static Matrix4 FromQuaternion(Quaternion quaternion)
		{
			var squared = new Vector4(quaternion.X*quaternion.X, quaternion.Y*quaternion.Y, quaternion.Z*quaternion.Z, quaternion.W*quaternion.W);
			float invSqLength = 1 / (squared.X + squared.Y + squared.Z + squared.W);

			float temp1 = quaternion.X * quaternion.Y;
			float temp2 = quaternion.Z * quaternion.W;
			float temp3 = quaternion.X * quaternion.Z;
			float temp4 = quaternion.Y * quaternion.W;
			float temp5 = quaternion.Y * quaternion.Z;
			float temp6 = quaternion.X * quaternion.W;

			return new Matrix4
			(
				new Vector4((squared.X-squared.Y-squared.Z+squared.W) * invSqLength, 2*(temp1-temp2) * invSqLength, 2*(temp3+temp4) * invSqLength, 0),
				new Vector4(2*(temp1+temp2) * invSqLength, (-squared.X+squared.Y-squared.Z+squared.W) * invSqLength, 2*(temp5-temp6) * invSqLength, 0),
				new Vector4(2*(temp3-temp4) * invSqLength, 2*(temp5+temp6) * invSqLength, (-squared.X-squared.Y+squared.Z+squared.W) * invSqLength, 0),
				new Vector4(0, 0, 0, 1)
			);
		}

		public static void FromQuaternion(ref Quaternion quaternion, out Matrix4 result)
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
			result.X.W = 0;

			result.Y.X = 2*(temp1+temp2) * invSqLength;
			result.Y.Y = (-squared.X+squared.Y-squared.Z+squared.W) * invSqLength;
			result.Y.Z = 2*(temp5-temp6) * invSqLength;
			result.Y.W = 0;

			result.Z.X = 2*(temp3-temp4) * invSqLength;
			result.Z.Y = 2*(temp5+temp6) * invSqLength;
			result.Z.Z = (-squared.X-squared.Y+squared.Z+squared.W) * invSqLength;
			result.Z.W = 0;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public Matrix4 FromRotationAxis(Vector3 axis, float angle)
		{
			Core.Quaternion quaternion;
			Core.Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			return Matrix4.FromQuaternion(quaternion);
		}

		public static void FromRotationAxis(ref Vector3 axis, float angle, out Matrix4 result)
		{
			Core.Quaternion quaternion;
			Core.Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			Matrix4.FromQuaternion(ref quaternion, out result);
		}

		public static Matrix4 FromRigidTransform(RigidTransform3 transform)
		{
			return Matrix4.FromAffineTransform(Matrix3.FromQuaternion(transform.Orientation), transform.Position);
		}

		public static void FromRigidTransform(ref RigidTransform3 transform, out Matrix4 result)
		{
			var qMat = Matrix3.FromQuaternion(transform.Orientation);
			Core.Matrix4.FromAffineTransform(ref qMat, ref transform.Position, out result);
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
		public static Matrix4 operator+(Matrix4 p1, Matrix4 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			p1.Z += p2.Z;
			p1.W += p2.W;
			return p1;
		}

		public static Matrix4 operator+(Matrix4 p1, Vector4 p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			p1.W += p2;
			return p1;
		}

		public static Matrix4 operator+(Vector4 p1, Matrix4 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			p2.W = p1 + p2.W;
			return p2;
		}

		public static Matrix4 operator+(Matrix4 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			p1.W += p2;
			return p1;
		}

		public static Matrix4 operator+(float p1, Matrix4 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			p2.W = p1 + p2.W;
			return p2;
		}

		// -
		public static Matrix4 operator-(Matrix4 p1, Matrix4 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			p1.Z -= p2.Z;
			p1.W -= p2.W;
			return p1;
		}

		public static Matrix4 operator-(Matrix4 p1, Vector4 p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			p1.W -= p2;
			return p1;
		}

		public static Matrix4 operator-(Vector4 p1, Matrix4 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			p2.W = p1 - p2.W;
			return p2;
		}

		public static Matrix4 operator-(Matrix4 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			p1.W -= p2;
			return p1;
		}

		public static Matrix4 operator-(float p1, Matrix4 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			p2.W = p1 - p2.W;
			return p2;
		}

		public static Matrix4 operator-(Matrix4 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			p2.Z = -p2.Z;
			p2.W = -p2.W;
			return p2;
		}

		// *
		public static Matrix4 operator*(Matrix4 p1, Matrix4 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			p1.Z *= p2.Z;
			p1.W *= p2.W;
			return p1;
		}

		public static Matrix4 operator*(Matrix4 p1, Vector4 p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			p1.W *= p2;
			return p1;
		}

		public static Matrix4 operator*(Vector4 p1, Matrix4 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			p2.W = p1 * p2.W;
			return p2;
		}

		public static Matrix4 operator*(Matrix4 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			p1.W *= p2;
			return p1;
		}

		public static Matrix4 operator*(float p1, Matrix4 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			p2.W = p1 * p2.W;
			return p2;
		}

		// /
		public static void Div(ref Matrix4 value1, ref Matrix4 value2, out Matrix4 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
			result.W = value1.W / value2.W;
		}

		public static void Div(ref Matrix4 value1, float value2, out Matrix4 result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
			result.Z = value1.Z / value2;
			result.W = value1.W / value2;
		}

		public static void Div(float value1, ref Matrix4 value2, out Matrix4 result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
			result.Z = value1 / value2.Z;
			result.W = value1 / value2.W;
		}

		public static Matrix4 operator/(Matrix4 p1, Matrix4 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			p1.Z /= p2.Z;
			p1.W /= p2.W;
			return p1;
		}

		public static Matrix4 operator/(Matrix4 p1, Vector4 p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			p1.W /= p2;
			return p1;
		}

		public static Matrix4 operator/(Vector4 p1, Matrix4 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			p2.W = p1 / p2.W;
			return p2;
		}

		public static Matrix4 operator/(Matrix4 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			p1.W /= p2;
			return p1;
		}

		public static Matrix4 operator/(float p1, Matrix4 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			p2.W = p1 / p2.W;
			return p2;
		}

		// ==
		public static bool operator==(Matrix4 p1, Matrix4 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z && p1.W==p2.W);}
		public static bool operator!=(Matrix4 p1, Matrix4 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z || p1.W!=p2.W);}

		// convert
		public Matrix3 ToMatrix3()
		{
			return new Matrix3(X.ToVector3(), Y.ToVector3(), Z.ToVector3());
		}
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

		public static void Abs(ref Matrix4 matrix, out Matrix4 result)
		{
			Vector4.Abs(ref matrix.X, out result.X);
			Vector4.Abs(ref matrix.Y, out result.Y);
			Vector4.Abs(ref matrix.Z, out result.Z);
			Vector4.Abs(ref matrix.W, out result.W);
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

		public static void Transpose(Matrix4 matrix, out Matrix4 result)
		{
			result.X.X = matrix.X.X;
			result.X.Y = matrix.Y.X;
			result.X.Z = matrix.Z.X;
			result.X.W = matrix.W.X;

			result.Y.X = matrix.X.Y;
			result.Y.Y = matrix.Y.Y;
			result.Y.Z = matrix.Z.Y;
			result.Y.W = matrix.W.Y;

			result.Z.X = matrix.X.Z;
			result.Z.Y = matrix.Y.Z;
			result.Z.Z = matrix.Z.Z;
			result.Z.W = matrix.W.Z;

			result.W.X = matrix.X.W;
			result.W.Y = matrix.Y.W;
			result.W.Z = matrix.Z.W;
			result.W.W = matrix.W.W;
		}

		public Matrix4 Multiply(Matrix2 matrix)
		{
			return new Matrix4
			(
				new Vector4((matrix.X.X*X.X) + (matrix.X.Y*Y.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y), X.Z, X.W),
				new Vector4((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y), Y.Z, Y.W),
				Z,
				W
			);
		}

		public static void Multiply(ref Matrix4 matrix1, ref Matrix2 matrix2, out Matrix4 result)
		{
			result.X.X = (matrix1.X.X*matrix2.X.X) + (matrix1.X.Y*matrix2.Y.X);
			result.X.Y = (matrix1.X.X*matrix2.X.Y) + (matrix1.X.Y*matrix2.Y.Y);
			result.X.Z = matrix1.X.Z;
			result.X.W = matrix1.X.W;

			result.Y.X = (matrix1.Y.X*matrix2.X.X) + (matrix1.Y.Y*matrix2.Y.X);
			result.Y.Y = (matrix1.Y.X*matrix2.X.Y) + (matrix1.Y.Y*matrix2.Y.Y);
			result.Y.Z = matrix1.Y.Z;
			result.Y.W = matrix1.Y.W;

			result.Z = matrix1.Z;
			result.W = matrix1.W;
		}

		public Matrix4 Multiply(Matrix3 matrix)
		{
			return new Matrix4
			(
				new Vector4((matrix.X.X*X.X) + (matrix.X.Y*Y.X) + (matrix.X.Z*Z.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y) + (matrix.X.Z*Z.Y), (matrix.X.X*X.Z) + (matrix.X.Y*Y.Z) + (matrix.X.Z*Z.Z), X.W),
				new Vector4((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X) + (matrix.Y.Z*Z.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y) + (matrix.Y.Z*Z.Y), (matrix.Y.X*X.Z) + (matrix.Y.Y*Y.Z) + (matrix.Y.Z*Z.Z), Y.W),
				new Vector4((matrix.Z.X*X.X) + (matrix.Z.Y*Y.X) + (matrix.Z.Z*Z.X), (matrix.Z.X*X.Y) + (matrix.Z.Y*Y.Y) + (matrix.Z.Z*Z.Y), (matrix.Z.X*X.Z) + (matrix.Z.Y*Y.Z) + (matrix.Z.Z*Z.Z), Z.W),
				W
			);
		}

		public static void Multiply(ref Matrix4 matrix1, ref Matrix3 matrix2, out Matrix4 result)
		{
			result.X.X = (matrix1.X.X*matrix2.X.X) + (matrix1.X.Y*matrix2.Y.X) + (matrix1.X.Z*matrix2.Z.X);
			result.X.Y = (matrix1.X.X*matrix2.X.Y) + (matrix1.X.Y*matrix2.Y.Y) + (matrix1.X.Z*matrix2.Z.Y);
			result.X.Z = (matrix1.X.X*matrix2.X.Z) + (matrix1.X.Y*matrix2.Y.Z) + (matrix1.X.Z*matrix2.Z.Z);
			result.X.W = matrix1.X.W;

			result.Y.X = (matrix1.Y.X*matrix2.X.X) + (matrix1.Y.Y*matrix2.Y.X) + (matrix1.Y.Z*matrix2.Z.X);
			result.Y.Y = (matrix1.Y.X*matrix2.X.Y) + (matrix1.Y.Y*matrix2.Y.Y) + (matrix1.Y.Z*matrix2.Z.Y);
			result.Y.Z = (matrix1.Y.X*matrix2.X.Z) + (matrix1.Y.Y*matrix2.Y.Z) + (matrix1.Y.Z*matrix2.Z.Z);
			result.Y.W = matrix1.Y.W;

			result.Z.X = (matrix1.Z.X*matrix2.X.X) + (matrix1.Z.Y*matrix2.Y.X) + (matrix1.Z.Z*matrix2.Z.X);
			result.Z.Y = (matrix1.Z.X*matrix2.X.Y) + (matrix1.Z.Y*matrix2.Y.Y) + (matrix1.Z.Z*matrix2.Z.Y);
			result.Z.Z = (matrix1.Z.X*matrix2.X.Z) + (matrix1.Z.Y*matrix2.Y.Z) + (matrix1.Z.Z*matrix2.Z.Z);
			result.Z.W = matrix1.Z.W;

			result.W = matrix1.W;
		}

		public Matrix4 Multiply(Matrix4 matrix)
		{
			return new Matrix4
			(
				new Vector4((matrix.X.X*X.X) + (matrix.X.Y*Y.X) + (matrix.X.Z*Z.X) + (matrix.X.W*W.X), (matrix.X.X*X.Y) + (matrix.X.Y*Y.Y) + (matrix.X.Z*Z.Y) + (matrix.X.W*W.Y), (matrix.X.X*X.Z) + (matrix.X.Y*Y.Z) + (matrix.X.Z*Z.Z) + (matrix.X.W*W.Z), (matrix.X.X*X.W) + (matrix.X.Y*Y.W) + (matrix.X.Z*Z.W) + (matrix.X.W*W.W)),
				new Vector4((matrix.Y.X*X.X) + (matrix.Y.Y*Y.X) + (matrix.Y.Z*Z.X) + (matrix.Y.W*W.X), (matrix.Y.X*X.Y) + (matrix.Y.Y*Y.Y) + (matrix.Y.Z*Z.Y) + (matrix.Y.W*W.Y), (matrix.Y.X*X.Z) + (matrix.Y.Y*Y.Z) + (matrix.Y.Z*Z.Z) + (matrix.Y.W*W.Z), (matrix.Y.X*X.W) + (matrix.Y.Y*Y.W) + (matrix.Y.Z*Z.W) + (matrix.Y.W*W.W)),
				new Vector4((matrix.Z.X*X.X) + (matrix.Z.Y*Y.X) + (matrix.Z.Z*Z.X) + (matrix.Z.W*W.X), (matrix.Z.X*X.Y) + (matrix.Z.Y*Y.Y) + (matrix.Z.Z*Z.Y) + (matrix.Z.W*W.Y), (matrix.Z.X*X.Z) + (matrix.Z.Y*Y.Z) + (matrix.Z.Z*Z.Z) + (matrix.Z.W*W.Z), (matrix.Z.X*X.W) + (matrix.Z.Y*Y.W) + (matrix.Z.Z*Z.W) + (matrix.Z.W*W.W)),
				new Vector4((matrix.W.X*X.X) + (matrix.W.Y*Y.X) + (matrix.W.Z*Z.X) + (matrix.W.W*W.X), (matrix.W.X*X.Y) + (matrix.W.Y*Y.Y) + (matrix.W.Z*Z.Y) + (matrix.W.W*W.Y), (matrix.W.X*X.Z) + (matrix.W.Y*Y.Z) + (matrix.W.Z*Z.Z) + (matrix.W.W*W.Z), (matrix.W.X*X.W) + (matrix.W.Y*Y.W) + (matrix.W.Z*Z.W) + (matrix.W.W*W.W))
			);
		}

		public static void Multiply(ref Matrix4 matrix1, ref Matrix4 matrix2, out Matrix4 result)
		{
			result.X.X = (matrix1.X.X*matrix2.X.X) + (matrix1.X.Y*matrix2.Y.X) + (matrix1.X.Z*matrix2.Z.X) + (matrix1.X.W*matrix2.W.X);
			result.X.Y = (matrix1.X.X*matrix2.X.Y) + (matrix1.X.Y*matrix2.Y.Y) + (matrix1.X.Z*matrix2.Z.Y) + (matrix1.X.W*matrix2.W.Y);
			result.X.Z = (matrix1.X.X*matrix2.X.Z) + (matrix1.X.Y*matrix2.Y.Z) + (matrix1.X.Z*matrix2.Z.Z) + (matrix1.X.W*matrix2.W.Z);
			result.X.W = (matrix1.X.X*matrix2.X.W) + (matrix1.X.Y*matrix2.Y.W) + (matrix1.X.Z*matrix2.Z.W) + (matrix1.X.W*matrix2.W.W);

			result.Y.X = (matrix1.Y.X*matrix2.X.X) + (matrix1.Y.Y*matrix2.Y.X) + (matrix1.Y.Z*matrix2.Z.X) + (matrix1.Y.W*matrix2.W.X);
			result.Y.Y = (matrix1.Y.X*matrix2.X.Y) + (matrix1.Y.Y*matrix2.Y.Y) + (matrix1.Y.Z*matrix2.Z.Y) + (matrix1.Y.W*matrix2.W.Y);
			result.Y.Z = (matrix1.Y.X*matrix2.X.Z) + (matrix1.Y.Y*matrix2.Y.Z) + (matrix1.Y.Z*matrix2.Z.Z) + (matrix1.Y.W*matrix2.W.Z);
			result.Y.W = (matrix1.Y.X*matrix2.X.W) + (matrix1.Y.Y*matrix2.Y.W) + (matrix1.Y.Z*matrix2.Z.W) + (matrix1.Y.W*matrix2.W.W);

			result.Z.X = (matrix1.Z.X*matrix2.X.X) + (matrix1.Z.Y*matrix2.Y.X) + (matrix1.Z.Z*matrix2.Z.X) + (matrix1.Z.W*matrix2.W.X);
			result.Z.Y = (matrix1.Z.X*matrix2.X.Y) + (matrix1.Z.Y*matrix2.Y.Y) + (matrix1.Z.Z*matrix2.Z.Y) + (matrix1.Z.W*matrix2.W.Y);
			result.Z.Z = (matrix1.Z.X*matrix2.X.Z) + (matrix1.Z.Y*matrix2.Y.Z) + (matrix1.Z.Z*matrix2.Z.Z) + (matrix1.Z.W*matrix2.W.Z);
			result.Z.W = (matrix1.Z.X*matrix2.X.W) + (matrix1.Z.Y*matrix2.Y.W) + (matrix1.Z.Z*matrix2.Z.W) + (matrix1.Z.W*matrix2.W.W);

			result.W.X = (matrix1.W.X*matrix2.X.X) + (matrix1.W.Y*matrix2.Y.X) + (matrix1.W.Z*matrix2.Z.X) + (matrix1.W.W*matrix2.W.X);
			result.W.Y = (matrix1.W.X*matrix2.X.Y) + (matrix1.W.Y*matrix2.Y.Y) + (matrix1.W.Z*matrix2.Z.Y) + (matrix1.W.W*matrix2.W.Y);
			result.W.Z = (matrix1.W.X*matrix2.X.Z) + (matrix1.W.Y*matrix2.Y.Z) + (matrix1.W.Z*matrix2.Z.Z) + (matrix1.W.W*matrix2.W.Z);
			result.W.W = (matrix1.W.X*matrix2.X.W) + (matrix1.W.Y*matrix2.Y.W) + (matrix1.W.Z*matrix2.Z.W) + (matrix1.W.W*matrix2.W.W);
		}

		public float Determinant()
        {
            float det1 = Z.Z * W.W - Z.W * W.Z;
            float det2 = Z.Y * W.W - Z.W * W.Y;
            float det3 = Z.Y * W.Z - Z.Z * W.Y;
            float det4 = Z.X * W.W - Z.W * W.X;
            float det5 = Z.X * W.Z - Z.Z * W.X;
            float det6 = Z.X * W.Y - Z.Y * W.X;

            return
                (X.X * ((Y.Y * det1 - Y.Z * det2) + Y.W * det3)) -
                (X.Y * ((Y.X * det1 - Y.Z * det4) + Y.W * det5)) +
                (X.Z * ((Y.X * det2 - Y.Y * det4) + Y.W * det6)) -
                (X.W * ((Y.X * det3 - Y.Y * det5) + Y.Z * det6));
        }

		public static void Determinant(ref Matrix4 matrix, out float result)
        {
            float det1 = matrix.Z.Z * matrix.W.W - matrix.Z.W * matrix.W.Z;
            float det2 = matrix.Z.Y * matrix.W.W - matrix.Z.W * matrix.W.Y;
            float det3 = matrix.Z.Y * matrix.W.Z - matrix.Z.Z * matrix.W.Y;
            float det4 = matrix.Z.X * matrix.W.W - matrix.Z.W * matrix.W.X;
            float det5 = matrix.Z.X * matrix.W.Z - matrix.Z.Z * matrix.W.X;
            float det6 = matrix.Z.X * matrix.W.Y - matrix.Z.Y * matrix.W.X;

            result =
                (matrix.X.X * ((matrix.Y.Y * det1 - matrix.Y.Z * det2) + matrix.Y.W * det3)) -
                (matrix.X.Y * ((matrix.Y.X * det1 - matrix.Y.Z * det4) + matrix.Y.W * det5)) +
                (matrix.X.Z * ((matrix.Y.X * det2 - matrix.Y.Y * det4) + matrix.Y.W * det6)) -
                (matrix.X.W * ((matrix.Y.X * det3 - matrix.Y.Y * det5) + matrix.Y.Z * det6));
        }

		public Matrix4 Invert()
		{
			float determinant = 1 / Determinant();

			var mat = new Matrix4
			(
				new Vector4
				(
					((Y.Y * Z.Z * W.W) + (Y.Z * Z.W * W.Y) + (Y.W * Z.Y * W.Z) - (Y.Y * Z.W * W.Z) - (Y.Z * Z.Y * W.W) - (Y.W * Z.Z * W.Y)) * determinant,
					((X.Y * Z.W * W.Z) + (X.Z * Z.Y * W.W) + (X.W * Z.Z * W.Y) - (X.Y * Z.Z * W.W) - (X.Z * Z.W * W.Y) - (X.W * Z.Y * W.Z)) * determinant,
					((X.Y * Y.Z * W.W) + (X.Z * Y.W * W.Y) + (X.W * Y.Y * W.Z) - (X.Y * Y.W * W.Z) - (X.Z * Y.Y * W.W) - (X.W * Y.Z * W.Y)) * determinant,
					((X.Y * Y.W * Z.Z) + (X.Z * Y.Y * Z.W) + (X.W * Y.Z * Z.Y) - (X.Y * Y.Z * Z.W) - (X.Z * Y.W * Z.Y) - (X.W * Y.Y * Z.Z)) * determinant
				),
				new Vector4
				(
					((Y.X * Z.W * W.Z) + (Y.Z * Z.X * W.W) + (Y.W * Z.Z * W.X) - (Y.X * Z.Z * W.W) - (Y.Z * Z.W * W.X) - (Y.W * Z.X * W.Z)) * determinant,
					((X.X * Z.Z * W.W) + (X.Z * Z.W * W.X) + (X.W * Z.X * W.Z) - (X.X * Z.W * W.Z) - (X.Z * Z.X * W.W) - (X.W * Z.Z * W.X)) * determinant,
					((X.X * Y.W * W.Z) + (X.Z * Y.X * W.W) + (X.W * Y.Z * W.X) - (X.X * Y.Z * W.W) - (X.Z * Y.W * W.X) - (X.W * Y.X * W.Z)) * determinant,
					((X.X * Y.Z * Z.W) + (X.Z * Y.W * Z.X) + (X.W * Y.X * Z.Z) - (X.X * Y.W * Z.Z) - (X.Z * Y.X * Z.W) - (X.W * Y.Z * Z.X)) * determinant
				),
				new Vector4
				(
					((Y.X * Z.Y * W.W) + (Y.Y * Z.W * W.X) + (Y.W * Z.X * W.Y) - (Y.X * Z.W * W.Y) - (Y.Y * Z.X * W.W) - (Y.W * Z.Y * W.X)) * determinant,
					((X.X * Z.W * W.Y) + (X.Y * Z.X * W.W) + (X.W * Z.Y * W.X) - (X.X * Z.Y * W.W) - (X.Y * Z.W * W.X) - (X.W * Z.X * W.Y)) * determinant,
					((X.X * Y.Y * W.W) + (X.Y * Y.W * W.X) + (X.W * Y.X * W.Y) - (X.X * Y.W * W.Y) - (X.Y * Y.X * W.W) - (X.W * Y.Y * W.X)) * determinant,
					((X.X * Y.W * Z.Y) + (X.Y * Y.X * Z.W) + (X.W * Y.Y * Z.X) - (X.X * Y.Y * Z.W) - (X.Y * Y.W * Z.X) - (X.W * Y.X * Z.Y)) * determinant
				),
				new Vector4
				(
					((Y.X * Z.Z * W.Y) + (Y.Y * Z.X * W.Z) + (Y.Z * Z.Y * W.X) - (Y.X * Z.Y * W.Z) - (Y.Y * Z.Z * W.X) - (Y.Z * Z.X * W.Y)) * determinant,
					((X.X * Z.Y * W.Z) + (X.Y * Z.Z * W.X) + (X.Z * Z.X * W.Y) - (X.X * Z.Z * W.Y) - (X.Y * Z.X * W.Z) - (X.Z * Z.Y * W.X)) * determinant,
					((X.X * Y.Z * W.Y) + (X.Y * Y.X * W.Z) + (X.Z * Y.Y * W.X) - (X.X * Y.Y * W.Z) - (X.Y * Y.Z * W.X) - (X.Z * Y.X * W.Y)) * determinant,
					((X.X * Y.Y * Z.Z) + (X.Y * Y.Z * Z.X) + (X.Z * Y.X * Z.Y) - (X.X * Y.Z * Z.Y) - (X.Y * Y.X * Z.Z) - (X.Z * Y.Y * Z.X)) * determinant
				)
			);
				  
			Transpose(mat, out mat);
			return mat;
		}

		public static void Invert(ref Matrix4 matrix, out Matrix4 result)
		{
			float determinant = 1 / matrix.Determinant();
			
			result.X.X = ((matrix.Y.Y * matrix.Z.Z * matrix.W.W) + (matrix.Y.Z * matrix.Z.W * matrix.W.Y) + (matrix.Y.W * matrix.Z.Y * matrix.W.Z) - (matrix.Y.Y * matrix.Z.W * matrix.W.Z) - (matrix.Y.Z * matrix.Z.Y * matrix.W.W) - (matrix.Y.W * matrix.Z.Z * matrix.W.Y)) * determinant;
			result.X.Y = ((matrix.X.Y * matrix.Z.W * matrix.W.Z) + (matrix.X.Z * matrix.Z.Y * matrix.W.W) + (matrix.X.W * matrix.Z.Z * matrix.W.Y) - (matrix.X.Y * matrix.Z.Z * matrix.W.W) - (matrix.X.Z * matrix.Z.W * matrix.W.Y) - (matrix.X.W * matrix.Z.Y * matrix.W.Z)) * determinant;
			result.X.Z = ((matrix.X.Y * matrix.Y.Z * matrix.W.W) + (matrix.X.Z * matrix.Y.W * matrix.W.Y) + (matrix.X.W * matrix.Y.Y * matrix.W.Z) - (matrix.X.Y * matrix.Y.W * matrix.W.Z) - (matrix.X.Z * matrix.Y.Y * matrix.W.W) - (matrix.X.W * matrix.Y.Z * matrix.W.Y)) * determinant;
			result.X.W = ((matrix.X.Y * matrix.Y.W * matrix.Z.Z) + (matrix.X.Z * matrix.Y.Y * matrix.Z.W) + (matrix.X.W * matrix.Y.Z * matrix.Z.Y) - (matrix.X.Y * matrix.Y.Z * matrix.Z.W) - (matrix.X.Z * matrix.Y.W * matrix.Z.Y) - (matrix.X.W * matrix.Y.Y * matrix.Z.Z)) * determinant;
			
			result.Y.X = ((matrix.Y.X * matrix.Z.W * matrix.W.Z) + (matrix.Y.Z * matrix.Z.X * matrix.W.W) + (matrix.Y.W * matrix.Z.Z * matrix.W.X) - (matrix.Y.X * matrix.Z.Z * matrix.W.W) - (matrix.Y.Z * matrix.Z.W * matrix.W.X) - (matrix.Y.W * matrix.Z.X * matrix.W.Z)) * determinant;
			result.Y.Y = ((matrix.X.X * matrix.Z.Z * matrix.W.W) + (matrix.X.Z * matrix.Z.W * matrix.W.X) + (matrix.X.W * matrix.Z.X * matrix.W.Z) - (matrix.X.X * matrix.Z.W * matrix.W.Z) - (matrix.X.Z * matrix.Z.X * matrix.W.W) - (matrix.X.W * matrix.Z.Z * matrix.W.X)) * determinant;
			result.Y.Z = ((matrix.X.X * matrix.Y.W * matrix.W.Z) + (matrix.X.Z * matrix.Y.X * matrix.W.W) + (matrix.X.W * matrix.Y.Z * matrix.W.X) - (matrix.X.X * matrix.Y.Z * matrix.W.W) - (matrix.X.Z * matrix.Y.W * matrix.W.X) - (matrix.X.W * matrix.Y.X * matrix.W.Z)) * determinant;
			result.Y.W = ((matrix.X.X * matrix.Y.Z * matrix.Z.W) + (matrix.X.Z * matrix.Y.W * matrix.Z.X) + (matrix.X.W * matrix.Y.X * matrix.Z.Z) - (matrix.X.X * matrix.Y.W * matrix.Z.Z) - (matrix.X.Z * matrix.Y.X * matrix.Z.W) - (matrix.X.W * matrix.Y.Z * matrix.Z.X)) * determinant;
			
			result.Z.X = ((matrix.Y.X * matrix.Z.Y * matrix.W.W) + (matrix.Y.Y * matrix.Z.W * matrix.W.X) + (matrix.Y.W * matrix.Z.X * matrix.W.Y) - (matrix.Y.X * matrix.Z.W * matrix.W.Y) - (matrix.Y.Y * matrix.Z.X * matrix.W.W) - (matrix.Y.W * matrix.Z.Y * matrix.W.X)) * determinant;
			result.Z.Y = ((matrix.X.X * matrix.Z.W * matrix.W.Y) + (matrix.X.Y * matrix.Z.X * matrix.W.W) + (matrix.X.W * matrix.Z.Y * matrix.W.X) - (matrix.X.X * matrix.Z.Y * matrix.W.W) - (matrix.X.Y * matrix.Z.W * matrix.W.X) - (matrix.X.W * matrix.Z.X * matrix.W.Y)) * determinant;
			result.Z.Z = ((matrix.X.X * matrix.Y.Y * matrix.W.W) + (matrix.X.Y * matrix.Y.W * matrix.W.X) + (matrix.X.W * matrix.Y.X * matrix.W.Y) - (matrix.X.X * matrix.Y.W * matrix.W.Y) - (matrix.X.Y * matrix.Y.X * matrix.W.W) - (matrix.X.W * matrix.Y.Y * matrix.W.X)) * determinant;
			result.Z.W = ((matrix.X.X * matrix.Y.W * matrix.Z.Y) + (matrix.X.Y * matrix.Y.X * matrix.Z.W) + (matrix.X.W * matrix.Y.Y * matrix.Z.X) - (matrix.X.X * matrix.Y.Y * matrix.Z.W) - (matrix.X.Y * matrix.Y.W * matrix.Z.X) - (matrix.X.W * matrix.Y.X * matrix.Z.Y)) * determinant;
			
			result.W.X = ((matrix.Y.X * matrix.Z.Z * matrix.W.Y) + (matrix.Y.Y * matrix.Z.X * matrix.W.Z) + (matrix.Y.Z * matrix.Z.Y * matrix.W.X) - (matrix.Y.X * matrix.Z.Y * matrix.W.Z) - (matrix.Y.Y * matrix.Z.Z * matrix.W.X) - (matrix.Y.Z * matrix.Z.X * matrix.W.Y)) * determinant;
			result.W.Y = ((matrix.X.X * matrix.Z.Y * matrix.W.Z) + (matrix.X.Y * matrix.Z.Z * matrix.W.X) + (matrix.X.Z * matrix.Z.X * matrix.W.Y) - (matrix.X.X * matrix.Z.Z * matrix.W.Y) - (matrix.X.Y * matrix.Z.X * matrix.W.Z) - (matrix.X.Z * matrix.Z.Y * matrix.W.X)) * determinant;
			result.W.Z = ((matrix.X.X * matrix.Y.Z * matrix.W.Y) + (matrix.X.Y * matrix.Y.X * matrix.W.Z) + (matrix.X.Z * matrix.Y.Y * matrix.W.X) - (matrix.X.X * matrix.Y.Y * matrix.W.Z) - (matrix.X.Y * matrix.Y.Z * matrix.W.X) - (matrix.X.Z * matrix.Y.X * matrix.W.Y)) * determinant;
			result.W.W = ((matrix.X.X * matrix.Y.Y * matrix.Z.Z) + (matrix.X.Y * matrix.Y.Z * matrix.Z.X) + (matrix.X.Z * matrix.Y.X * matrix.Z.Y) - (matrix.X.X * matrix.Y.Z * matrix.Z.Y) - (matrix.X.Y * matrix.Y.X * matrix.Z.Z) - (matrix.X.Z * matrix.Y.Y * matrix.Z.X)) * determinant;
				  
			Transpose(result, out result);
		}

		public static Matrix4 View(Vector3 position, Vector3 lookAt, Vector3 upVector)
		{
			Matrix4 result;
			View(ref position, ref lookAt, ref upVector, out result);
			return result;
		}

		public static void View(ref Vector3 position, ref Vector3 lookAt, ref Vector3 upVector, out Matrix4 result)
		{
			var forward = (lookAt - position).Normalize();
			var xVec = forward.Cross(upVector).Normalize();
			upVector = xVec.Cross(forward);
			
			result.X.X = xVec.X;
			result.X.Y = xVec.Y;
			result.X.Z = xVec.Z;
			result.X.W = position.Dot(-xVec);

			result.Y.X = upVector.X;
			result.Y.Y = upVector.Y;
			result.Y.Z = upVector.Z;
			result.Y.W = position.Dot(-upVector);

			result.Z.X = -forward.X;
			result.Z.Y = -forward.Y;
			result.Z.Z = -forward.Z;
			result.Z.W = position.Dot(forward);

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public static Matrix4 Perspective(float fov, float aspect, float near, float far)
		{
			float top = near * (float)Math.Tan(fov * .5f);
			float bottom = -top;
			float right = top * aspect;
			float left = -right;

			return Frustum(left, right, bottom, top, near, far);
		}

		public static void Perspective(float fov, float aspect, float near, float far, out Matrix4 result)
		{
			float top = near * (float)Math.Tan(fov * .5f);
			float bottom = -top;
			float right = top * aspect;
			float left = -right;

			Frustum(left, right, bottom, top, near, far, out result);
		}

		public static Matrix4 Frustum(float left, float right, float bottom, float top, float near, float far)
		{
			Matrix4 result;
			Frustum(left, right, bottom, top, near, far, out result);
			return result;
		}

		public static void Frustum(float left, float right, float bottom, float top, float near, float far, out Matrix4 result)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;
			float n = near * 2;

			result.X.X = n/width;
			result.X.Y = 0;
			result.X.Z = (right+left)/width;
			result.X.W = 0;

			result.Y.X = 0;
			result.Y.Y = n/height;
			result.Y.Z = (top+bottom)/height;
			result.Y.W = 0;

			result.Z.X = 0;
			result.Z.Y = 0;
			result.Z.Z = -(far+near)/depth;
			result.Z.W = -(n*far)/depth;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = -1;
			result.W.W = 0;
		}

		public static Matrix4 Orthographic(float width, float height, float near, float far)
		{
			return Orthographic(0, width, 0, height, near, far);
		}

		public static void Orthographic(float width, float height, float near, float far, out Matrix4 result)
		{
			Orthographic(0, width, 0, height, near, far, out result);
		}

		public static Matrix4 Orthographic(float left, float right, float bottom, float top, float near, float far)
		{
			Matrix4 result;
			Orthographic(left, right, bottom, top, near, far, out result);
			return result;
		}

		public static void Orthographic(float left, float right, float bottom, float top, float near, float far, out Matrix4 result)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;

			result.X.X = 2/width;
			result.X.Y = 0;
			result.X.Z = 0;
			result.X.W = -(right+left)/width;

			result.Y.X = 0;
			result.Y.Y = 2/height;
			result.Y.Z = 0;
			result.Y.W = -(top+bottom)/height;

			result.Z.X = 0;
			result.Z.Y = 0;
			result.Z.Z = -2/depth;
			result.Z.W = -(far+near)/depth;

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}

		public static Matrix4 OrthographicCentered(float width, float height, float near, float far)
		{
			return OrthographicCentered(0, width, 0, height, near, far);
		}

		public static void OrthographicCentered(float width, float height, float near, float far, out Matrix4 result)
		{
			OrthographicCentered(0, width, 0, height, near, far, out result);
		}

		public static Matrix4 OrthographicCentered(float left, float right, float bottom, float top, float near, float far)
		{
			Matrix4 result;
			OrthographicCentered(left, right, bottom, top, near, far, out result);
			return result;
		}

		public static void OrthographicCentered(float left, float right, float bottom, float top, float near, float far, out Matrix4 result)
		{
			float width = right - left;
			float height = top - bottom;
			float depth = far - near;

			result.X.X = (2/width);
			result.X.Y = 0;
			result.X.Z = 0;
			result.X.W = 0;

			result.Y.X = 0;
			result.Y.Y = (2/height);
			result.Y.Z = 0;
			result.Y.W = 0;

			result.Z.X = 0;
			result.Z.Y = 0;
			result.Z.Z = (-2)/depth;
			result.Z.W = -((far+near)/depth);

			result.W.X = 0;
			result.W.Y = 0;
			result.W.Z = 0;
			result.W.W = 1;
		}
		#endregion
	}
}