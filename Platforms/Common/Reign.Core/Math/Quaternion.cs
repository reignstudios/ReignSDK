using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion
	{
		#region Properties
		public float X, Y, Z, W;
		#endregion

		#region Constructors
		public Quaternion(float value)
		{
			X = value;
			Y = value;
			Z = value;
			W = value;
		}

		public Quaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Quaternion(Vector2 vector, float z, float w)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
			W = w;
		}

		public Quaternion(Vector3 vector, float w)
		{
			X = vector.X;
			Y = vector.Y;
			Z = vector.Z;
			W = w;
		}

		public static Quaternion LookAt(Vector3 forward, Vector3 up)
		{
			//Quaternion Quaternion::LookRotation(Vector& lookAt, Vector& upDirection) {
			//Vector forward = lookAt; Vector up = upDirection;
			//Vector::OrthoNormalize(&forward, &up);
			//Vector right = Vector::Cross(up, forward);

			//#define m00 right.x
			//#define m01 up.x
			//#define m02 forward.x
			//#define m10 right.y
			//#define m11 up.y
			//#define m12 forward.y
			//#define m20 right.z
			//#define m21 up.z
			//#define m22 forward.z

			//        Quaternion ret;
			//        ret.w = sqrtf(1.0f + m00 + m11 + m22) * 0.5f;
			//        float w4_recip = 1.0f / (4.0f * ret.w);
			//        ret.x = (m21 - m12) * w4_recip;
			//        ret.y = (m02 - m20) * w4_recip;
			//        ret.z = (m10 - m01) * w4_recip;

			//#undef m00
			//#undef m01
			//#undef m02
			//#undef m10
			//#undef m11
			//#undef m12
			//#undef m20
			//#undef m21
			//#undef m22

			//        return ret;
			//}
			throw new NotImplementedException();
		}

		public static Quaternion FromMatrix3(Matrix3 matrix)
		{
			float w = (float)Math.Sqrt(1 + matrix.X.X + matrix.Y.Y + matrix.Z.Z) * .5f;
			float delta = 1 / (w * 4);
			return new Quaternion
			(
				(matrix.Z.Y - matrix.Y.Z) * delta,
				(matrix.X.Z - matrix.Z.X) * delta,
				(matrix.Y.X - matrix.X.Y) * delta,
				w
			);
		}

		public static void FromMatrix3(ref Matrix3 matrix, out Quaternion result)
		{
			float w = (float)Math.Sqrt(1 + matrix.X.X + matrix.Y.Y + matrix.Z.Z) * .5f;
			float delta = 1 / (w * 4);
			result.X = (matrix.Z.Y - matrix.Y.Z) * delta;
			result.Y = (matrix.X.Z - matrix.Z.X) * delta;
			result.Z = (matrix.Y.X - matrix.X.Y) * delta;
			result.W = w;
		}

		public static Quaternion FromMatrix4(Matrix4 matrix)
		{
			float w = (float)Math.Sqrt(1 + matrix.X.X + matrix.Y.Y + matrix.Z.Z) * .5f;
			float delta = 1 / (w * 4);
			return new Quaternion
			(
				(matrix.Z.Y - matrix.Y.Z) * delta,
				(matrix.X.Z - matrix.Z.X) * delta,
				(matrix.Y.X - matrix.X.Y) * delta,
				w
			);
		}

		public static void FromMatrix4(ref Matrix4 matrix, out Quaternion result)
		{
			float w = (float)Math.Sqrt(1 + matrix.X.X + matrix.Y.Y + matrix.Z.Z) * .5f;
			float delta = 1 / (w * 4);
			result.X = (matrix.Z.Y - matrix.Y.Z) * delta;
			result.Y = (matrix.X.Z - matrix.Z.X) * delta;
			result.Z = (matrix.Y.X - matrix.X.Y) * delta;
			result.W = w;
		}

		public static Quaternion FromRotationAxis(Vector3 axis, float angle)
		{
			angle *= .5f;
			var sin = (float)Math.Sin(angle);
			return new Quaternion
			(
				axis.X * sin,
				axis.Y * sin,
				axis.Z * sin,
				(float)Math.Cos(angle)
			);
		}

		public static void FromRotationAxis(ref Vector3 axis, float angle, out Quaternion result)
		{
			angle *= .5f;
			var sin = (float)Math.Sin(angle);
			result.X = axis.X * sin;
			result.Y = axis.Y * sin;
			result.Z = axis.Z * sin;
			result.W = (float)Math.Cos(angle);
		}

		public static Quaternion FromRotationAxis(float axisX, float axisY, float axisZ, float angle)
		{
			angle *= .5f;
			var sin = (float)Math.Sin(angle);
			return new Quaternion
			(
				axisX * sin,
				axisY * sin,
				axisZ * sin,
				(float)Math.Cos(angle)
			);
		}

		public static void FromRotationAxis(float axisX, float axisY, float axisZ, float angle, out Quaternion result)
		{
			angle *= .5f;
			var sin = (float)Math.Sin(angle);
			result.X = axisX * sin;
			result.Y = axisY * sin;
			result.Z = axisZ * sin;
			result.W = (float)Math.Cos(angle);
		}

		public static Quaternion FromSphericalRotation(float latitude, float longitude, float angle)
		{
			angle *= .5f;
			float sa = (float)Math.Sin(angle);
			float cLat = (float)Math.Cos(latitude);
			float sLat = (float)Math.Sin(latitude);
			float cLong = (float)Math.Cos(longitude);
			float sLong = (float)Math.Sin(longitude);
			return new Quaternion(sa*cLat*sLong, sa*sLat, sa*sLat*cLong, (float)Math.Cos(angle));
		}

		public static void FromSphericalRotation(float latitude, float longitude, float angle, out Quaternion result)
		{
			angle *= .5f;
			float sa = (float)Math.Sin(angle);
			float cLat = (float)Math.Cos(latitude);
			float sLat = (float)Math.Sin(latitude);
			float cLong = (float)Math.Cos(longitude);
			float sLong = (float)Math.Sin(longitude);
			result.X = sa*cLat*sLong;
			result.Y = sa*sLat;
			result.Z = sa*sLat*cLong;
			result.W = (float)Math.Cos(angle);
		}

		public static Quaternion FromEuler(Vector3 euler)
		{
			euler.X *= .5f;
			euler.Y *= .5f;
			euler.Z *= .5f;
			float cosYaw = (float)Math.Cos(euler.X);
            float cosPitch = (float)Math.Cos(euler.Y);
            float cosRoll = (float)Math.Cos(euler.Z);
            float sinYaw = (float)Math.Sin(euler.X);
            float sinPitch = (float)Math.Sin(euler.Y);
            float sinRoll = (float)Math.Sin(euler.Z);

            float cosYawCosPitch = cosYaw * cosPitch;
            float cosYawSinPitch = cosYaw * sinPitch;
            float sinYawCosPitch = sinYaw * cosPitch;
            float sinYawSinPitch = sinYaw * sinPitch;

			return new Quaternion
			(
				cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll,
				sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll,
				cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll,
				cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll
			);
		}

		public static void FromEuler(Vector3 euler, out Quaternion result)
		{
			euler.X *= .5f;
			euler.Y *= .5f;
			euler.Z *= .5f;
			float cosYaw = (float)Math.Cos(euler.X);
            float cosPitch = (float)Math.Cos(euler.Y);
            float cosRoll = (float)Math.Cos(euler.Z);
            float sinYaw = (float)Math.Sin(euler.X);
            float sinPitch = (float)Math.Sin(euler.Y);
            float sinRoll = (float)Math.Sin(euler.Z);

            float cosYawCosPitch = cosYaw * cosPitch;
            float cosYawSinPitch = cosYaw * sinPitch;
            float sinYawCosPitch = sinYaw * cosPitch;
            float sinYawSinPitch = sinYaw * sinPitch;

			result.X = cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll;
			result.Y = sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll;
			result.Z = cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll;
			result.W = cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll;
		}

		public static Quaternion FromEuler(float eulerX, float eulerY, float eulerZ)
		{
			eulerX *= .5f;
			eulerY *= .5f;
			eulerZ *= .5f;
			float cosYaw = (float)Math.Cos(eulerX);
            float cosPitch = (float)Math.Cos(eulerY);
            float cosRoll = (float)Math.Cos(eulerZ);
            float sinYaw = (float)Math.Sin(eulerX);
            float sinPitch = (float)Math.Sin(eulerY);
            float sinRoll = (float)Math.Sin(eulerZ);

            float cosYawCosPitch = cosYaw * cosPitch;
            float cosYawSinPitch = cosYaw * sinPitch;
            float sinYawCosPitch = sinYaw * cosPitch;
            float sinYawSinPitch = sinYaw * sinPitch;

			return new Quaternion
			(
				cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll,
				sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll,
				cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll,
				cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll
			);
		}

		public static void FromEuler(float eulerX, float eulerY, float eulerZ, out Quaternion result)
		{
			eulerX *= .5f;
			eulerY *= .5f;
			eulerZ *= .5f;
			float cosYaw = (float)Math.Cos(eulerX);
            float cosPitch = (float)Math.Cos(eulerY);
            float cosRoll = (float)Math.Cos(eulerZ);
            float sinYaw = (float)Math.Sin(eulerX);
            float sinPitch = (float)Math.Sin(eulerY);
            float sinRoll = (float)Math.Sin(eulerZ);

            float cosYawCosPitch = cosYaw * cosPitch;
            float cosYawSinPitch = cosYaw * sinPitch;
            float sinYawCosPitch = sinYaw * cosPitch;
            float sinYawSinPitch = sinYaw * sinPitch;

			result.X = cosYawCosPitch * cosRoll + sinYawSinPitch * sinRoll;
			result.Y = sinYawCosPitch * cosRoll - cosYawSinPitch * sinRoll;
			result.Z = cosYawSinPitch * cosRoll + sinYawCosPitch * sinRoll;
			result.W = cosYawCosPitch * sinRoll - sinYawSinPitch * cosRoll;
		}

		public static readonly Quaternion Identity = new Quaternion(0, 0, 0, 1);
		#endregion

		#region Operators
		// +
		public static void Add(ref Quaternion value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
			result.W = value1.W + value2.W;
		}

		public static void Add(ref Quaternion value1, float value2, out Quaternion result)
		{
			result.X = value1.X + value2;
			result.Y = value1.Y + value2;
			result.Z = value1.Z + value2;
			result.W = value1.W + value2;
		}

		public static void Add(float value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1 + value2.X;
			result.Y = value1 + value2.Y;
			result.Z = value1 + value2.Z;
			result.W = value1 + value2.W;
		}

		public static Quaternion operator+(Quaternion p1, Quaternion p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			p1.Z += p2.Z;
			p1.W += p2.W;
			return p1;
		}

		public static Quaternion operator+(Quaternion p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			p1.W += p2;
			return p1;
		}

		public static Quaternion operator+(float p1, Quaternion p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			p2.W = p1 + p2.W;
			return p2;
		}

		// -
		public static void Sub(ref Quaternion value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
			result.W = value1.W - value2.W;
		}

		public static void Sub(ref Quaternion value1, float value2, out Quaternion result)
		{
			result.X = value1.X - value2;
			result.Y = value1.Y - value2;
			result.Z = value1.Z - value2;
			result.W = value1.W - value2;
		}

		public static void Sub(float value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1 - value2.X;
			result.Y = value1 - value2.Y;
			result.Z = value1 - value2.Z;
			result.W = value1 - value2.W;
		}

		public static void Neg(ref Quaternion value, out Quaternion result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.W = -value.W;
		}

		public static Quaternion operator-(Quaternion p1, Quaternion p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			p1.Z -= p2.Z;
			p1.W -= p2.W;
			return p1;
		}

		public static Quaternion operator-(Quaternion p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			p1.W -= p2;
			return p1;
		}

		public static Quaternion operator-(float p1, Quaternion p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			p2.W = p1 - p2.W;
			return p2;
		}

		public static Quaternion operator-(Quaternion p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			p2.Z = -p2.Z;
			p2.W = -p2.W;
			return p2;
		}

		// *
		public static void Mul(ref Quaternion value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
			result.W = value1.W * value2.W;
		}

		public static void Mul(ref Quaternion value1, float value2, out Quaternion result)
		{
			result.X = value1.X * value2;
			result.Y = value1.Y * value2;
			result.Z = value1.Z * value2;
			result.W = value1.W * value2;
		}

		public static void Mul(float value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1 * value2.X;
			result.Y = value1 * value2.Y;
			result.Z = value1 * value2.Z;
			result.W = value1 * value2.W;
		}

		public static Quaternion operator*(Quaternion p1, Quaternion p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			p1.Z *= p2.Z;
			p1.W *= p2.W;
			return p1;
		}

		public static Quaternion operator*(Quaternion p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			p1.W *= p2;
			return p1;
		}

		public static Quaternion operator*(float p1, Quaternion p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			p2.W = p1 * p2.W;
			return p2;
		}

		// /
		public static void Div(ref Quaternion value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
			result.W = value1.W / value2.W;
		}

		public static void Div(ref Quaternion value1, float value2, out Quaternion result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
			result.Z = value1.Z / value2;
			result.W = value1.W / value2;
		}

		public static void Div(float value1, ref Quaternion value2, out Quaternion result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
			result.Z = value1 / value2.Z;
			result.W = value1 / value2.W;
		}

		public static Quaternion operator/(Quaternion p1, Quaternion p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			p1.Z /= p2.Z;
			p1.W /= p2.W;
			return p1;
		}

		public static Quaternion operator/(Quaternion p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			p1.W /= p2;
			return p1;
		}

		public static Quaternion operator/(float p1, Quaternion p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			p2.W = p1 / p2.W;
			return p2;
		}

		// ==
		public static bool operator==(Quaternion p1, Quaternion p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z && p1.W==p2.W);}
		public static bool operator!=(Quaternion p1, Quaternion p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z || p1.W!=p2.W);}

		// convert
		public Vector4 ToVector4() {return new Vector4(X, Y, Z, W);}
		#endregion

		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Quaternion)obj == this;
		}

		public override string ToString()
		{
			return string.Format("<{0}, {1}, {2}, {3}>", X, Y, Z, W);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public float Length()
		{
			return (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
		}

		public static void Length(ref Quaternion quaternion, out float result)
		{
			result = (float)Math.Sqrt((quaternion.X*quaternion.X) + (quaternion.Y*quaternion.Y) + (quaternion.Z*quaternion.Z) + (quaternion.W*quaternion.W));
		}

		public Quaternion Normalize()
		{
			return this * (1 / (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W)));
		}

		public static void Normalize(ref Quaternion quaternion, out Quaternion result)
		{
			result = quaternion * (1 / (float)Math.Sqrt((quaternion.X*quaternion.X) + (quaternion.Y*quaternion.Y) + (quaternion.Z*quaternion.Z) + (quaternion.W*quaternion.W)));
		}

		public Quaternion Normalize(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			length = dis;
			return this * (1/dis);
		}

		public static void Normalize(ref Quaternion quaternion, out Quaternion result, out float length)
		{
			float dis = (float)Math.Sqrt((quaternion.X*quaternion.X) + (quaternion.Y*quaternion.Y) + (quaternion.Z*quaternion.Z) + (quaternion.W*quaternion.W));
			length = dis;
			result = quaternion * (1/dis);
		}

		public Quaternion NormalizeSafe()
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			if (dis == 0) return new Quaternion();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Quaternion quaternion, out Quaternion result)
		{
			float dis = (float)Math.Sqrt((quaternion.X*quaternion.X) + (quaternion.Y*quaternion.Y) + (quaternion.Z*quaternion.Z) + (quaternion.W*quaternion.W));
			if (dis == 0) result = new Quaternion();
			else result = quaternion * (1/dis);
		}

		public Quaternion NormalizeSafe(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			length = dis;
			if (dis == 0) return new Quaternion();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Quaternion quaternion, out Quaternion result, out float length)
		{
			float dis = (float)Math.Sqrt((quaternion.X*quaternion.X) + (quaternion.Y*quaternion.Y) + (quaternion.Z*quaternion.Z) + (quaternion.W*quaternion.W));
			length = dis;
			if (dis == 0) result = new Quaternion();
			else result = quaternion * (1/dis);
		}

		public Quaternion Multiply(Quaternion quaternion)
		{
			return new Quaternion
			(
				W*quaternion.X + X*quaternion.W + Y*quaternion.Z - Z*quaternion.Y,
				W*quaternion.Y - X*quaternion.Z + Y*quaternion.W + Z*quaternion.X,
				W*quaternion.Z + X*quaternion.Y - Y*quaternion.X + Z*quaternion.W,
				W*quaternion.W - X*quaternion.X - Y*quaternion.Y - Z*quaternion.Z
			);
		}

		public static void Multiply(ref Quaternion quaternion1, ref Quaternion quaternion2, out Quaternion result)
		{
			result.X = quaternion1.W*quaternion2.X + quaternion1.X*quaternion2.W + quaternion1.Y*quaternion2.Z - quaternion1.Z*quaternion2.Y;
			result.Y = quaternion1.W*quaternion2.Y - quaternion1.X*quaternion2.Z + quaternion1.Y*quaternion2.W + quaternion1.Z*quaternion2.X;
			result.Z = quaternion1.W*quaternion2.Z + quaternion1.X*quaternion2.Y - quaternion1.Y*quaternion2.X + quaternion1.Z*quaternion2.W;
			result.W = quaternion1.W*quaternion2.W - quaternion1.X*quaternion2.X - quaternion1.Y*quaternion2.Y - quaternion1.Z*quaternion2.Z;
		}

		public Quaternion Concatenate(Quaternion quaternion)
		{
			return new Quaternion
			(
				quaternion.W*X + quaternion.X*W + quaternion.Y*Z - quaternion.Z*Y,
				quaternion.W*Y - quaternion.X*Z + quaternion.Y*W + quaternion.Z*X,
				quaternion.W*Z + quaternion.X*Y - quaternion.Y*X + quaternion.Z*W,
				quaternion.W*W - quaternion.X*X - quaternion.Y*Y - quaternion.Z*Z
			);
		}

		public static void Concatenate(ref Quaternion quaternion2, ref Quaternion quaternion1, out Quaternion result)
		{
			result.X = quaternion1.W*quaternion2.X + quaternion1.X*quaternion2.W + quaternion1.Y*quaternion2.Z - quaternion1.Z*quaternion2.Y;
			result.Y = quaternion1.W*quaternion2.Y - quaternion1.X*quaternion2.Z + quaternion1.Y*quaternion2.W + quaternion1.Z*quaternion2.X;
			result.Z = quaternion1.W*quaternion2.Z + quaternion1.X*quaternion2.Y - quaternion1.Y*quaternion2.X + quaternion1.Z*quaternion2.W;
			result.W = quaternion1.W*quaternion2.W - quaternion1.X*quaternion2.X - quaternion1.Y*quaternion2.Y - quaternion1.Z*quaternion2.Z;
		}

		public Quaternion Conjugate()
		{
			return new Quaternion(-X, -Y, -Z, W);
		}

		public static void Conjugate(ref Quaternion quaternoin, out Quaternion result)
		{
			result.X = -quaternoin.X;
			result.Y = -quaternoin.Y;
			result.Z = -quaternoin.Z;
			result.W = quaternoin.W;
		}

		public void RotationAxis(out Vector3 axis, out float angle)
		{
			angle = (float)Math.Acos(W) * MathUtilities.Pi2;
			float sinAngle = (float)Math.Sqrt(1 - (W*W));
			if (sinAngle == 0) sinAngle = 1;
			sinAngle = 1 / sinAngle;
			axis = new Vector3(X*sinAngle, Y*sinAngle, Z*sinAngle);
		}

		public static void RotationAxis(ref Quaternion quaternion, out Vector3 axis, out float angle)
		{
			angle = (float)Math.Acos(quaternion.W) * MathUtilities.Pi2;
			float sinAngle = (float)Math.Sqrt(1 - (quaternion.W*quaternion.W));
			if (sinAngle == 0) sinAngle = 1;
			sinAngle = 1 / sinAngle;
			axis = new Vector3(quaternion.X*sinAngle, quaternion.Y*sinAngle, quaternion.Z*sinAngle);
		}

		public void SphericalRotation(out float latitude, out float longitude, out float angle)
		{
			angle = (float)Math.Acos(W) * MathUtilities.Pi2;
			float sinAngle = (float)Math.Sqrt(1 - (W*W));
			if (sinAngle == 0) sinAngle = 1;
			sinAngle = 1 / sinAngle;

			float x = X * sinAngle;
			float y = Y * sinAngle;
			float z = Z * sinAngle;

			latitude = -(float)Math.Asin(y);
			if ((x*x) + (z*z) == 0) longitude = 0;
			else longitude = (float)Math.Atan2(x, z) * MathUtilities.Pi;
			if (longitude < 0) longitude += MathUtilities.Pi2;
		}

		public static void SphericalRotation(ref Quaternion quaternion, out float latitude, out float longitude, out float angle)
		{
			angle = (float)Math.Acos(quaternion.W) * MathUtilities.Pi2;
			float sinAngle = (float)Math.Sqrt(1 - (quaternion.W*quaternion.W));
			if (sinAngle == 0) sinAngle = 1;
			sinAngle = 1 / sinAngle;

			float x = quaternion.X * sinAngle;
			float y = quaternion.Y * sinAngle;
			float z = quaternion.Z * sinAngle;

			latitude = -(float)Math.Asin(y);
			if ((x*x) + (z*z) == 0) longitude = 0;
			else longitude = (float)Math.Atan2(x, z) * MathUtilities.Pi;
			if (longitude < 0) longitude += MathUtilities.Pi2;
		}

		public static Quaternion Slerp(Quaternion start, Quaternion end, float interpolationAmount)
		{
			float cosHalfTheta = start.W * end.W + start.X * end.X + start.Y * end.Y + start.Z * end.Z;
            if (cosHalfTheta < 0)
            {
                //Negating a quaternion results in the same orientation, but we need cosHalfTheta to be positive to get the shortest path.
                end = -end;
                cosHalfTheta = -cosHalfTheta;
            }

            // If the orientations are similar enough, then just pick one of the inputs.
            if (cosHalfTheta > .999999) return start;

            // Calculate temporary values.
            float halfTheta = (float)Math.Acos(cosHalfTheta);
            float sinHalfTheta = (float)Math.Sqrt(1.0 - cosHalfTheta * cosHalfTheta);

            //Check to see if we're 180 degrees away from the target.
            if (Math.Abs(sinHalfTheta) < 0.00001) return (start + end) * .5f;

            //Blend the two quaternions to get the result!
			float aFraction = (float)Math.Sin((1 - interpolationAmount) * halfTheta) / sinHalfTheta;
            float bFraction = (float)Math.Sin(interpolationAmount * halfTheta) / sinHalfTheta;
			return start * aFraction + end * bFraction;
		}

		public static void Slerp(ref Quaternion start, ref Quaternion end, float interpolationAmount, out Quaternion result)
		{
			float cosHalfTheta = start.W * end.W + start.X * end.X + start.Y * end.Y + start.Z * end.Z;
            if (cosHalfTheta < 0)
            {
                //Negating a quaternion results in the same orientation, but we need cosHalfTheta to be positive to get the shortest path.
                end = -end;
                cosHalfTheta = -cosHalfTheta;
            }

            // If the orientations are similar enough, then just pick one of the inputs.
            if (cosHalfTheta > .999999)
			{
				result = start;
				return;
			}

            // Calculate temporary values.
            float halfTheta = (float)Math.Acos(cosHalfTheta);
            float sinHalfTheta = (float)Math.Sqrt(1.0 - cosHalfTheta * cosHalfTheta);

            //Check to see if we're 180 degrees away from the target.
            if (Math.Abs(sinHalfTheta) < 0.00001)
			{
				result = (start + end) * .5f;
				return;
			}

            //Blend the two quaternions to get the result!
			float aFraction = (float)Math.Sin((1 - interpolationAmount) * halfTheta) / sinHalfTheta;
            float bFraction = (float)Math.Sin(interpolationAmount * halfTheta) / sinHalfTheta;
			result = start * aFraction + end * bFraction;
		}
		#endregion
	}
}