using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3
	{
		#region Properties
		public float X, Y, Z;
		#endregion

		#region Constructors
		public Vector3(float value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Vector3(Vector2 vector, float z)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
		}

		public static readonly Vector3 One = new Vector3(1);
		public static readonly Vector3 MinusOne = new Vector3(-1);
		public static readonly Vector3 Zero = new Vector3(0);
		public static readonly Vector3 Right = new Vector3(1, 0, 0);
		public static readonly Vector3 Left = new Vector3(-1, 0, 0);
		public static readonly Vector3 Up = new Vector3(0, 1, 0);
		public static readonly Vector3 Down = new Vector3(0, -1, 0);
		public static readonly Vector3 Forward = new Vector3(0, 0, 1);
		public static readonly Vector3 Backward = new Vector3(0, 0, -1);
		#endregion

		#region Operators
		// +
		public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
		}

		public static void Add(ref Vector3 value1, float value2, out Vector3 result)
		{
			result.X = value1.X + value2;
			result.Y = value1.Y + value2;
			result.Z = value1.Z + value2;
		}

		public static void Add(float value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1 + value2.X;
			result.Y = value1 + value2.Y;
			result.Z = value1 + value2.Z;
		}

		public static Vector3 operator+(Vector3 p1, Vector3 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			p1.Z += p2.Z;
			return p1;
		}

		public static Vector3 operator+(Vector3 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			return p1;
		}

		public static Vector3 operator+(float p1, Vector3 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			return p2;
		}

		// -
		public static void Sub(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
		}

		public static void Sub(ref Vector3 value1, float value2, out Vector3 result)
		{
			result.X = value1.X - value2;
			result.Y = value1.Y - value2;
			result.Z = value1.Z - value2;
		}

		public static void Sub(float value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1 - value2.X;
			result.Y = value1 - value2.Y;
			result.Z = value1 - value2.Z;
		}

		public static void Neg(ref Vector3 value, out Vector3 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
		}

		public static Vector3 operator-(Vector3 p1, Vector3 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			p1.Z -= p2.Z;
			return p1;
		}

		public static Vector3 operator-(Vector3 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			return p1;
		}

		public static Vector3 operator-(float p1, Vector3 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			return p2;
		}

		public static Vector3 operator-(Vector3 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			p2.Z = -p2.Z;
			return p2;
		}

		// *
		public static void Mul(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
		}

		public static void Mul(ref Vector3 value1, float value2, out Vector3 result)
		{
			result.X = value1.X * value2;
			result.Y = value1.Y * value2;
			result.Z = value1.Z * value2;
		}

		public static void Mul(float value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1 * value2.X;
			result.Y = value1 * value2.Y;
			result.Z = value1 * value2.Z;
		}

		public static Vector3 operator*(Vector3 p1, Vector3 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			p1.Z *= p2.Z;
			return p1;
		}

		public static Vector3 operator*(Vector3 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			return p1;
		}

		public static Vector3 operator*(float p1, Vector3 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			return p2;
		}

		// /
		public static void Div(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
		}

		public static void Div(ref Vector3 value1, float value2, out Vector3 result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
			result.Z = value1.Z / value2;
		}

		public static void Div(float value1, ref Vector3 value2, out Vector3 result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
			result.Z = value1 / value2.Z;
		}

		public static Vector3 operator/(Vector3 p1, Vector3 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			p1.Z /= p2.Z;
			return p1;
		}

		public static Vector3 operator/(Vector3 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			return p1;
		}

		public static Vector3 operator/(float p1, Vector3 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			return p2;
		}

		// ==
		public static bool operator==(Vector3 p1, Vector3 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z);}
		public static bool operator!=(Vector3 p1, Vector3 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z);}
		// convert
		public Vector2 ToVector2()
		{
			return new Vector2(X, Y);
		}

		public static void ToVector2(ref Vector3 vector, out Vector2 result)
		{
			result.X = vector.X;
			result.Y = vector.Y;
		}
		#endregion

		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Vector3)obj == this;
		}

		public override string ToString()
		{
			return string.Format("<{0}, {1}, {2}>", X, Y, Z);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vector3 DegToRad()
		{
			return new Vector3(MathUtilities.DegToRad(X), MathUtilities.DegToRad(Y), MathUtilities.DegToRad(Z));
		}

		public static void DegToRad(ref Vector3 vector, out Vector3 result)
		{
			result.X = MathUtilities.DegToRad(vector.X);
			result.Y = MathUtilities.DegToRad(vector.Y);
			result.Z = MathUtilities.DegToRad(vector.Z);
		}

		public Vector3 RadToDeg()
		{
			return new Vector3(MathUtilities.RadToDeg(X), MathUtilities.RadToDeg(Y), MathUtilities.RadToDeg(Z));
		}

		public static void RadToDeg(ref Vector3 vector, out Vector3 result)
		{
			result.X = MathUtilities.RadToDeg(vector.X);
			result.Y = MathUtilities.RadToDeg(vector.Y);
			result.Z = MathUtilities.RadToDeg(vector.Z);
		}

		public Vector3 Max(float value)
		{
			return new Vector3(Math.Max(X, value), Math.Max(Y, value), Math.Max(Z, value));
		}

		public static void Max(ref Vector3 vector, float value, out Vector3 result)
		{
			result.X = Math.Max(vector.X, value);
			result.Y = Math.Max(vector.Y, value);
			result.Z = Math.Max(vector.Z, value);
		}

		public Vector3 Max(Vector3 values)
		{
			return new Vector3(Math.Max(X, values.X), Math.Max(Y, values.Y), Math.Max(Z, values.Z));
		}

		public static void Max(ref Vector3 vector, ref Vector3 values, out Vector3 result)
		{
			result.X = Math.Max(vector.X, values.X);
			result.Y = Math.Max(vector.Y, values.Y);
			result.Z = Math.Max(vector.Z, values.Z);
		}

		public Vector3 Min(float value)
		{
			return new Vector3(Math.Min(X, value), Math.Min(Y, value), Math.Min(Z, value));
		}

		public static void Min(ref Vector3 vector, float value, out Vector3 result)
		{
			result.X = Math.Min(vector.X, value);
			result.Y = Math.Min(vector.Y, value);
			result.Z = Math.Min(vector.Z, value);
		}

		public Vector3 Min(Vector3 values)
		{
			return new Vector3(Math.Min(X, values.X), Math.Min(Y, values.Y), Math.Min(Z, values.Z));
		}

		public static void Min(ref Vector3 vector, ref Vector3 values, out Vector3 result)
		{
			result.X = Math.Min(vector.X, values.X);
			result.Y = Math.Min(vector.Y, values.Y);
			result.Z = Math.Min(vector.Z, values.Z);
		}

		public Vector3 Abs()
		{
			return new Vector3(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
		}

		public static void Abs(ref Vector3 vector, out Vector3 result)
		{
			result.X = Math.Abs(vector.X);
			result.Y = Math.Abs(vector.Y);
			result.Z = Math.Abs(vector.Z);
		}

		public Vector3 Pow(float value)
		{
			return new Vector3((float)Math.Pow(X, value), (float)Math.Pow(Y, value), (float)Math.Pow(Z, value));
		}

		public static void Pow(ref Vector3 vector, float value, out Vector3 result)
		{
			result.X = (float)Math.Pow(vector.X, value);
			result.Y = (float)Math.Pow(vector.Y, value);
			result.Z = (float)Math.Pow(vector.Z, value);
		}

		public Vector3 Floor()
		{
			return new Vector3((float)Math.Floor(X), (float)Math.Floor(Y), (float)Math.Floor(Z));
		}

		public static void Floor(ref Vector3 vector, out Vector3 result)
		{
			result.X = (float)Math.Floor(vector.X);
			result.Y = (float)Math.Floor(vector.Y);
			result.Z = (float)Math.Floor(vector.Z);
		}

		public float Length()
		{
			return (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z));
		}

		public static void Length(ref Vector3 vector, out float result)
		{
			result = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z));
		}

		public float LengthSquared()
		{
			return (X*X) + (Y*Y) + (Z*Z);
		}

		public static void LengthSquared(ref Vector3 vector, out float result)
		{
			result = (vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z);
		}

		public float Distance(Vector3 vector)
		{
			return (vector - this).Length();
		}

		public static void Distance(ref Vector3 vector1, ref Vector3 vector2, out float result)
		{
			result = (vector2 - vector1).Length();
		}
		
		public float DistanceSquared(Vector3 vector)
		{
			return (vector - this).Dot();
		}

		public static void DistanceSquared(ref Vector3 vector1, ref Vector3 vector2, out float result)
		{
			result = (vector2 - vector1).Dot();
		}

		public float Dot()
		{
			return (X*X) + (Y*Y) + (Z*Z);
		}

		public static void Dot(ref Vector3 vector, out float result)
		{
			result = (vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z);
		}

		public float Dot(Vector3 vector)
		{
			return (X*vector.X) + (Y*vector.Y) + (Z*vector.Z);
		}

		public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
		{
			result = (vector1.X*vector2.X) + (vector1.Y*vector2.Y) + (vector1.Z*vector2.Z);
		}

		public Vector3 Normalize()
		{
			return this * (1 / (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z)));
		}

		public static void Normalize(ref Vector3 vector, out Vector3 result)
		{
			result = vector * (1 / (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z)));
		}

		public Vector3 Normalize(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z));
			length = dis;
			return this * (1/dis);
		}

		public static void Normalize(ref Vector3 vector, ref Vector3 result, out float length)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z));
			length = dis;
			result = vector * (1/dis);
		}

		public Vector3 NormalizeSafe()
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z));
			if (dis == 0) return new Vector3();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Vector3 vector, out Vector3 result)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z));
			if (dis == 0) result = new Vector3();
			else result = vector * (1/dis);
		}

		public Vector3 NormalizeSafe(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z));
			length = dis;
			if (dis == 0) return new Vector3();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Vector3 vector, out Vector3 result, out float length)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z));
			length = dis;
			if (dis == 0) result = new Vector3();
			else result = vector * (1/dis);
		}

		public Vector3 Cross(Vector3 vector)
		{
			return new Vector3(((Y*vector.Z) - (Z*vector.Y)), ((Z*vector.X) - (X*vector.Z)), ((X*vector.Y) - (Y*vector.X)));
		}

		public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
		{
			result.X = ((vector1.Y*vector2.Z) - (vector1.Z*vector2.Y));
			result.Y = ((vector1.Z*vector2.X) - (vector1.X*vector2.Z));
			result.Z = ((vector1.X*vector2.Y) - (vector1.Y*vector2.X));
		}

		public Vector3 Transform(Matrix4 matrix)
		{
			return new Vector3
			(
				(X * matrix.X.X) + (Y * matrix.Y.X) + (Z * matrix.Z.X) + matrix.X.W,
				(X * matrix.X.Y) + (Y * matrix.Y.Y) + (Z * matrix.Z.Y) + matrix.Y.W,
				(X * matrix.X.Z) + (Y * matrix.Y.Z) + (Z * matrix.Z.Z) + matrix.Z.W
			);
		}

		public static void Transform(ref Vector3 vector, ref Matrix4 matrix, out Vector3 result)
		{
            result.X = (vector.X * matrix.X.X) + (vector.Y * matrix.Y.X) + (vector.Z * matrix.Z.X) + matrix.X.W;
            result.Y = (vector.X * matrix.X.Y) + (vector.Y * matrix.Y.Y) + (vector.Z * matrix.Z.Y) + matrix.Y.W;
            result.Z = (vector.X * matrix.X.Z) + (vector.Y * matrix.Y.Z) + (vector.Z * matrix.Z.Z) + matrix.Z.W;
		}

		public Vector3 TransformNormal(Matrix4 matrix)
		{
			return new Vector3
			(
				(X * matrix.X.X) + (Y * matrix.Y.X) + (Z * matrix.Z.X),
				(X * matrix.X.Y) + (Y * matrix.Y.Y) + (Z * matrix.Z.Y),
				(X * matrix.X.Z) + (Y * matrix.Y.Z) + (Z * matrix.Z.Z)
			);
		}

		public static void TransformNormal(ref Vector3 vector, ref Matrix4 matrix, out Vector3 result)
		{
            result.X = (vector.X * matrix.X.X) + (vector.Y * matrix.Y.X) + (vector.Z * matrix.Z.X);
            result.Y = (vector.X * matrix.X.Y) + (vector.Y * matrix.Y.Y) + (vector.Z * matrix.Z.Y);
            result.Z = (vector.X * matrix.X.Z) + (vector.Y * matrix.Y.Z) + (vector.Z * matrix.Z.Z);
		}

		public Vector3 Transform(Matrix3 matrix)
		{
		    return (matrix.X*X) + (matrix.Y*Y) + (matrix.Z*Z);
		}

		public static void Transform(ref Vector3 vector, ref Matrix3 matrix, out Vector3 result)
		{
		    result = (matrix.X*vector.X) + (matrix.Y*vector.Y) + (matrix.Z*vector.Z);
		}

		public Vector3 TransformTranspose(Matrix3 matrix)
        {
			return new Vector3
			(
				(X * matrix.X.X) + (Y * matrix.X.Y) + (Z * matrix.X.Z),
				(X * matrix.Y.X) + (Y * matrix.Y.Y) + (Z * matrix.Y.Z),
				(X * matrix.Z.X) + (Y * matrix.Z.Y) + (Z * matrix.Z.Z)
			);
        }

		public static void TransformTranspose(ref Vector3 vector, ref Matrix3 matrix, out Vector3 result)
        {
            result.X = (vector.X * matrix.X.X) + (vector.Y * matrix.X.Y) + (vector.Z * matrix.X.Z);
            result.Y = (vector.X * matrix.Y.X) + (vector.Y * matrix.Y.Y) + (vector.Z * matrix.Y.Z);
            result.Z = (vector.X * matrix.Z.X) + (vector.Y * matrix.Z.Y) + (vector.Z * matrix.Z.Z);
        }

		public Vector2 Transform(Matrix2x3 matrix)
        {
			return new Vector2
			(
				(matrix.X.X * X) + (matrix.X.Y * Y) + (matrix.X.Z * Z),
				(matrix.Y.X * X) + (matrix.Y.Y * Y) + (matrix.Y.Z * Z)
			);
        }

		public static void Transform(ref Vector3 vector, ref Matrix2x3 matrix, out Vector2 result)
        {
            result.X = (matrix.X.X * vector.X) + (matrix.X.Y * vector.Y) + (matrix.X.Z * vector.Z);
            result.Y = (matrix.Y.X * vector.X) + (matrix.Y.Y * vector.Y) + (matrix.Y.Z * vector.Z);
        }

		public Vector2 Transform(Matrix3x2 matrix)
        {
			return new Vector2
			(
				(X * matrix.X.X) + (Y * matrix.Y.X) + (Z * matrix.Z.X),
				(X * matrix.X.Y) + (Y * matrix.Y.Y) + (Z * matrix.Z.Y)
			);
        }

		public static void Transform(ref Vector3 vector, ref Matrix3x2 matrix, out Vector2 result)
        {
            result.X = (vector.X * matrix.X.X) + (vector.Y * matrix.Y.X) + (vector.Z * matrix.Z.X);
            result.Y = (vector.X * matrix.X.Y) + (vector.Y * matrix.Y.Y) + (vector.Z * matrix.Z.Y);
        }

		public Vector3 Transform(AffineTransform3 transform)
		{
		    return this.Transform(transform.Transform) + transform.Translation;
		}

		public static void Transform(ref Vector3 vector, ref AffineTransform3 transform, out Vector3 result)
        {
            Vector3.Transform(ref vector, ref transform.Transform, out result);
            result += transform.Translation;
        }

		public Vector3 Transform(RigidTransform3 transform)
        {
            return this.Transform(transform.Orientation) + transform.Position;
        }

		public static void Transform(ref Vector3 vector, ref RigidTransform3 transform, out Vector3 result)
        {
            Vector3.Transform(ref vector, ref transform.Orientation, out result);
			result += transform.Position;
        }

		public Vector3 TransformInversed(RigidTransform3 transform)
        {
			Vector3 result = this - transform.Position;
			Quaternion.Conjugate(ref transform.Orientation, out transform.Orientation);
            Vector3.Transform(ref this, ref transform.Orientation, out result);
			return result;
        }

		public static void TransformInversed(ref Vector3 vector, ref RigidTransform3 transform, out Vector3 result)
        {
			result = vector - transform.Position;
			Quaternion.Conjugate(ref transform.Orientation, out transform.Orientation);
            Vector3.Transform(ref vector, ref transform.Orientation, out result);
        }

		public Vector3 Transform(Quaternion quaternion)
		{
            float x2 = quaternion.X + quaternion.X;
            float y2 = quaternion.Y + quaternion.Y;
            float z2 = quaternion.Z + quaternion.Z;
            float xx2 = quaternion.X * x2;
            float xy2 = quaternion.X * y2;
            float xz2 = quaternion.X * z2;
            float yy2 = quaternion.Y * y2;
            float yz2 = quaternion.Y * z2;
            float zz2 = quaternion.Z * z2;
            float wx2 = quaternion.W * x2;
            float wy2 = quaternion.W * y2;
            float wz2 = quaternion.W * z2;

			return new Vector3
			(
				X * ((1f - yy2) - zz2) + Y * (xy2 - wz2) + Z * (xz2 + wy2),
				X * (xy2 + wz2) + Y * ((1f - xx2) - zz2) + Z * (yz2 - wx2),
				X * (xz2 - wy2) + Y * (yz2 + wx2) + Z * ((1f - xx2) - yy2)
			);
		}

		public static void Transform(ref Vector3 vector, ref Quaternion quaternion, out Vector3 result)
		{
            float x2 = quaternion.X + quaternion.X;
            float y2 = quaternion.Y + quaternion.Y;
            float z2 = quaternion.Z + quaternion.Z;
            float xx2 = quaternion.X * x2;
            float xy2 = quaternion.X * y2;
            float xz2 = quaternion.X * z2;
            float yy2 = quaternion.Y * y2;
            float yz2 = quaternion.Y * z2;
            float zz2 = quaternion.Z * z2;
            float wx2 = quaternion.W * x2;
            float wy2 = quaternion.W * y2;
            float wz2 = quaternion.W * z2;

			result.X = vector.X * ((1f - yy2) - zz2) + vector.Y * (xy2 - wz2) + vector.Z * (xz2 + wy2);
			result.Y = vector.X * (xy2 + wz2) + vector.Y * ((1f - xx2) - zz2) + vector.Z * (yz2 - wx2);
			result.Z = vector.X * (xz2 - wy2) + vector.Y * (yz2 + wx2) + vector.Z * ((1f - xx2) - yy2);
		}

		public bool AproxEqualsBox(Vector3 vector, float tolerance)
		{
			return
				(Math.Abs(X-vector.X) <= tolerance) &&
				(Math.Abs(Y-vector.Y) <= tolerance) &&
				(Math.Abs(Z-vector.Z) <= tolerance);
		}

		public static void AproxEqualsBox(ref Vector3 vector1, ref Vector3 vector2, float tolerance, out bool result)
		{
			result =
				(Math.Abs(vector1.X-vector2.X) <= tolerance) &&
				(Math.Abs(vector1.Y-vector2.Y) <= tolerance) &&
				(Math.Abs(vector1.Z-vector2.Z) <= tolerance);
		}

		public bool ApproxEquals(Vector3 vector, float tolerance)
		{
		    return (Distance(vector) <= tolerance);
		}

		public static void ApproxEquals(ref Vector3 vector1, ref Vector3 vector2, float tolerance, out bool result)
		{
		    result = (vector1.Distance(vector2) <= tolerance);
		}

		public Vector3 RotateAround(Vector3 axis, float angle)
		{
			// rotate into world space
			var quaternion = Quaternion.FromRotationAxis(axis, 0);
			Quaternion.Conjugate(ref quaternion, out quaternion);
			var worldSpaceVector = this.Transform(quaternion);

			// rotate back to vector space
			Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			Vector3.Transform(ref worldSpaceVector, ref quaternion, out worldSpaceVector);
			return worldSpaceVector;
		}

		public static void RotateAround(ref Vector3 vector, ref Vector3 axis, float angle, out Vector3 result)
		{
			// rotate into world space
			var quaternion = Quaternion.FromRotationAxis(axis, 0);
			Quaternion.Conjugate(ref quaternion, out quaternion);
			Vector3.Transform(ref vector, ref quaternion, out result);

			// rotate back to vector space
			Quaternion.FromRotationAxis(ref axis, angle, out quaternion);
			Vector3.Transform(ref result, ref quaternion, out result);
		}

		public bool WithinTriangle(Triangle3 triangle)
		{      
			var v0 = triangle.Point2 - triangle.Point1;
			var v1 = triangle.Point3 - triangle.Point1;
			var v2 = this - triangle.Point1;

			float dot00 = v0.Dot();
			float dot01 = v0.Dot(v1);
			float dot02 = v0.Dot(v2);
			float dot11 = v1.Dot();
			float dot12 = v1.Dot(v2);

			float invDenom = 1 / ((dot00*dot11) - (dot01*dot01));
			float u = ((dot11*dot02) - (dot01*dot12)) * invDenom;
			float v = ((dot00*dot12) - (dot01*dot02)) * invDenom;
			return (u>0) && (v>0) && ((u+v) < 1);
		}

		public static void WithinTriangle(ref Vector3 vector, ref Triangle3 triangle, out bool result)
		{      
			var v0 = triangle.Point2 - triangle.Point1;
			var v1 = triangle.Point3 - triangle.Point1;
			var v2 = vector - triangle.Point1;

			float dot00 = v0.Dot();
			float dot01 = v0.Dot(v1);
			float dot02 = v0.Dot(v2);
			float dot11 = v1.Dot();
			float dot12 = v1.Dot(v2);

			float invDenom = 1 / ((dot00*dot11) - (dot01*dot01));
			float u = ((dot11*dot02) - (dot01*dot12)) * invDenom;
			float v = ((dot00*dot12) - (dot01*dot02)) * invDenom;
			result = (u>0) && (v>0) && ((u+v) < 1);
		}

		public Vector3 Reflect(Vector3 planeNormal)
		{
			return this - (planeNormal * this.Dot(planeNormal) * 2);
		}

		public static void Reflect(ref Vector3 vector, ref Vector3 planeNormal, out Vector3 result)
		{
			result = vector - (planeNormal * vector.Dot(planeNormal) * 2);
		}

		public Vector3 InersectNormal(Vector3 normal)
		{
			return (normal * this.Dot(normal));
		}

		public static void InersectNormal(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
		{
			result = (normal * vector.Dot(normal));
		}

		public Vector3 InersectRay(Vector3 rayOrigin, Vector3 rayDirection)
		{
			return (rayDirection * (this-rayOrigin).Dot(rayDirection)) + rayOrigin;
		}

		public static void InersectRay(ref Vector3 vector, ref Vector3 rayOrigin, ref Vector3 rayDirection, out Vector3 result)
		{
			result = (rayDirection * (vector-rayOrigin).Dot(rayDirection)) + rayOrigin;
		}

		public Vector3 InersectLine(Line3 line)
		{
			Vector3 pointOffset = (this-line.Point1), vector = (line.Point2-line.Point1).Normalize();
			return (vector * pointOffset.Dot(vector)) + line.Point1;
		}

		public static void InersectLine(ref Vector3 vector, ref Line3 line, out Vector3 result)
		{
			Vector3 pointOffset = (vector-line.Point1), vec = (line.Point2-line.Point1).Normalize();
			result = (vec * pointOffset.Dot(vec)) + line.Point1;
		}

		public Vector3 InersectPlane(Vector3 planeNormal)
		{
			return this - (planeNormal * this.Dot(planeNormal));
		}

		public static void InersectPlane(ref Vector3 vector, ref Vector3 planeNormal, out Vector3 result)
		{
			result = vector - (planeNormal * vector.Dot(planeNormal));
		}

		public Vector3 InersectPlane(Vector3 planeNormal, Vector3 planeLocation)
		{
			return this - (planeNormal * (this-planeLocation).Dot(planeNormal));
		}

		public static void InersectPlane(ref Vector3 vector, ref Vector3 planeNormal, ref Vector3 planeLocation, out Vector3 result)
		{
			result = vector - (planeNormal * (vector-planeLocation).Dot(planeNormal));
		}

		/*public bool InersectTriangle(out Vector3f pInersectPoint, Vector3f pPolygonPoint1, Vector3f pPolygonPoint2, Vector3f pPolygonPoint3, Vector3f pPolygonNormal, Bound3D pPolygonBoundingBox, Vector3f pPoint)
		{
			pInersectPoint = pPoint.InersectPlane(pPolygonNormal, pPolygonPoint1);
			if (pInersectPoint.WithinTriangle(pPolygonBoundingBox) == false) return false;
			return Within(pPolygonPoint1, pPolygonPoint2, pPolygonPoint3);
		}*/

		public float Angle(Vector3 vector)
		{
			var vec = this.Normalize();
			float val = vec.Dot(vector.Normalize());
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			return (float)Math.Acos(val);
		}

		public static void Angle(ref Vector3 vector1, ref Vector3 vector2, out float result)
		{
			var vec = vector1.Normalize();
			float val = vec.Dot(vector2.Normalize());
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			result = (float)Math.Acos(val);
		}

		public static Vector3 Lerp(Vector3 start, Vector3 end, float interpolationAmount)
        {
            float startAmount = 1 - interpolationAmount;
			return new Vector3
			(
				start.X * startAmount + end.X * interpolationAmount,
				start.Y * startAmount + end.Y * interpolationAmount,
				start.Z * startAmount + end.Z * interpolationAmount
			);
        }

		public static void Lerp(ref Vector3 start, ref Vector3 end, float interpolationAmount, out Vector3 result)
        {
            float startAmount = 1 - interpolationAmount;
            result.X = start.X * startAmount + end.X * interpolationAmount;
            result.Y = start.Y * startAmount + end.Y * interpolationAmount;
            result.Z = start.Z * startAmount + end.Z * interpolationAmount;
        }

		public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float interpolationAmount)
        {
            float weightSquared = interpolationAmount * interpolationAmount;
            float weightCubed = interpolationAmount * weightSquared;
            float value1Blend = 2 * weightCubed - 3 * weightSquared + 1;
            float tangent1Blend = weightCubed - 2 * weightSquared + interpolationAmount;
            float value2Blend = -2 * weightCubed + 3 * weightSquared;
            float tangent2Blend = weightCubed - weightSquared;

			return new Vector3
			(
				value1.X * value1Blend + value2.X * value2Blend + tangent1.X * tangent1Blend + tangent2.X * tangent2Blend,
				value1.Y * value1Blend + value2.Y * value2Blend + tangent1.Y * tangent1Blend + tangent2.Y * tangent2Blend,
				value1.Z * value1Blend + value2.Z * value2Blend + tangent1.Z * tangent1Blend + tangent2.Z * tangent2Blend
			);
        }

		public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float interpolationAmount, out Vector3 result)
        {
            float weightSquared = interpolationAmount * interpolationAmount;
            float weightCubed = interpolationAmount * weightSquared;
            float value1Blend = 2 * weightCubed - 3 * weightSquared + 1;
            float tangent1Blend = weightCubed - 2 * weightSquared + interpolationAmount;
            float value2Blend = -2 * weightCubed + 3 * weightSquared;
            float tangent2Blend = weightCubed - weightSquared;

            result.X = value1.X * value1Blend + value2.X * value2Blend + tangent1.X * tangent1Blend + tangent2.X * tangent2Blend;
            result.Y = value1.Y * value1Blend + value2.Y * value2Blend + tangent1.Y * tangent1Blend + tangent2.Y * tangent2Blend;
            result.Z = value1.Z * value1Blend + value2.Z * value2Blend + tangent1.Z * tangent1Blend + tangent2.Z * tangent2Blend;
        }
		#endregion
	}
}