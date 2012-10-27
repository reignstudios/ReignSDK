using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4
	{
		#region Properties
		public float X, Y, Z, W;

		public float R
		{
			get {return X;}
			set {X = value;}
		}

		public float G
		{
			get {return Y;}
			set {Y = value;}
		}

		public float B
		{
			get {return Z;}
			set {Z = value;}
		}

		public float A
		{
			get {return W;}
			set {W = value;}
		}
		#endregion

		#region Constructors
		public Vector4(float value)
		{
			X = value;
			Y = value;
			Z = value;
			W = value;
		}

		public Vector4(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Vector4(Vector2 vector, float z, float w)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
			W = w;
		}

		public Vector4(Vector3 vector, float w)
		{
			X = vector.X;
			Y = vector.Y;
			Z = vector.Z;
			W = w;
		}

		public static readonly Vector4 One = new Vector4(1);
		public static readonly Vector4 MinusOne = new Vector4(-1);
		public static readonly Vector4 Zero = new Vector4(0);
		public static readonly Vector4 Right = new Vector4(1, 0, 0, 0);
		public static readonly Vector4 Left = new Vector4(-1, 0, 0, 0);
		public static readonly Vector4 Up = new Vector4(0, 1, 0, 0);
		public static readonly Vector4 Down = new Vector4(0, -1, 0, 0);
		public static readonly Vector4 Forward = new Vector4(0, 0, 1, 0);
		public static readonly Vector4 Backward = new Vector4(0, 0, -1, 0);
		public static readonly Vector4 High = new Vector4(0, 0, 0, 1);
		public static readonly Vector4 Low = new Vector4(0, 0, 0, -1);
		#endregion

		#region Operators
		// +
		public static void Add(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
			result.Z = value1.Z + value2.Z;
			result.W = value1.W + value2.W;
		}

		public static void Add(ref Vector4 value1, float value2, out Vector4 result)
		{
			result.X = value1.X + value2;
			result.Y = value1.Y + value2;
			result.Z = value1.Z + value2;
			result.W = value1.W + value2;
		}

		public static void Add(float value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1 + value2.X;
			result.Y = value1 + value2.Y;
			result.Z = value1 + value2.Z;
			result.W = value1 + value2.W;
		}

		public static Vector4 operator+(Vector4 p1, Vector4 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			p1.Z += p2.Z;
			p1.W += p2.W;
			return p1;
		}

		public static Vector4 operator+(Vector4 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			p1.Z += p2;
			p1.W += p2;
			return p1;
		}

		public static Vector4 operator+(float p1, Vector4 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			p2.Z = p1 + p2.Z;
			p2.W = p1 + p2.W;
			return p2;
		}

		// -
		public static void Sub(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
			result.Z = value1.Z - value2.Z;
			result.W = value1.W - value2.W;
		}

		public static void Sub(ref Vector4 value1, float value2, out Vector4 result)
		{
			result.X = value1.X - value2;
			result.Y = value1.Y - value2;
			result.Z = value1.Z - value2;
			result.W = value1.W - value2;
		}

		public static void Sub(float value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1 - value2.X;
			result.Y = value1 - value2.Y;
			result.Z = value1 - value2.Z;
			result.W = value1 - value2.W;
		}

		public static void Neg(ref Vector4 value, out Vector4 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
			result.Z = -value.Z;
			result.W = -value.W;
		}

		public static Vector4 operator-(Vector4 p1, Vector4 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			p1.Z -= p2.Z;
			p1.W -= p2.W;
			return p1;
		}

		public static Vector4 operator-(Vector4 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			p1.Z -= p2;
			p1.W -= p2;
			return p1;
		}

		public static Vector4 operator-(float p1, Vector4 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			p2.Z = p1 - p2.Z;
			p2.W = p1 - p2.W;
			return p2;
		}

		public static Vector4 operator-(Vector4 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			p2.Z = -p2.Z;
			p2.W = -p2.W;
			return p2;
		}

		// *
		public static void Mul(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
			result.Z = value1.Z * value2.Z;
			result.W = value1.W * value2.W;
		}

		public static void Mul(ref Vector4 value1, float value2, out Vector4 result)
		{
			result.X = value1.X * value2;
			result.Y = value1.Y * value2;
			result.Z = value1.Z * value2;
			result.W = value1.W * value2;
		}

		public static void Mul(float value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1 * value2.X;
			result.Y = value1 * value2.Y;
			result.Z = value1 * value2.Z;
			result.W = value1 * value2.W;
		}

		public static Vector4 operator*(Vector4 p1, Vector4 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			p1.Z *= p2.Z;
			p1.W *= p2.W;
			return p1;
		}

		public static Vector4 operator*(Vector4 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			p1.Z *= p2;
			p1.W *= p2;
			return p1;
		}

		public static Vector4 operator*(float p1, Vector4 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			p2.Z = p1 * p2.Z;
			p2.W = p1 * p2.W;
			return p2;
		}

		// /
		public static void Div(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
			result.Z = value1.Z / value2.Z;
			result.W = value1.W / value2.W;
		}

		public static void Div(ref Vector4 value1, float value2, out Vector4 result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
			result.Z = value1.Z / value2;
			result.W = value1.W / value2;
		}

		public static void Div(float value1, ref Vector4 value2, out Vector4 result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
			result.Z = value1 / value2.Z;
			result.W = value1 / value2.W;
		}

		public static Vector4 operator/(Vector4 p1, Vector4 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			p1.Z /= p2.Z;
			p1.W /= p2.W;
			return p1;
		}

		public static Vector4 operator/(Vector4 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			p1.Z /= p2;
			p1.W /= p2;
			return p1;
		}

		public static Vector4 operator/(float p1, Vector4 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			p2.Z = p1 / p2.Z;
			p2.W = p1 / p2.W;
			return p2;
		}

		// ==
		public static bool operator==(Vector4 p1, Vector4 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z && p1.W==p2.W);}
		public static bool operator!=(Vector4 p1, Vector4 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z || p1.W!=p2.W);}

		// convert
		public Vector2 ToVector2()
		{
			return new Vector2(X, Y);
		}

		public static void ToVector2(ref Vector4 vector, out Vector2 result)
		{
			result.X = vector.X;
			result.Y = vector.Y;
		}

		public Vector3 ToVector3()
		{
			return new Vector3(X, Y, Z);
		}

		public static void ToVector3(ref Vector4 vector, out Vector3 result)
		{
			result.X = vector.X;
			result.Y = vector.Y;
			result.Z = vector.Z;
		}

		public Color4 ToColor4()
		{
			return new Color4((byte)(X*255), (byte)(Y*255), (byte)(Z*255), (byte)(W*255));
		}

		public static void ToColor4(ref Vector4 vector, out Color4 color)
		{
			color.R = (byte)(vector.X * 255);
			color.G = (byte)(vector.Y * 255);
			color.B = (byte)(vector.Z * 255);
			color.A = (byte)(vector.W * 255);
		}
		#endregion

		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Vector4)obj == this;
		}

		public override string ToString()
		{
			return string.Format("<{0}, {1}, {2}, {3}>", X, Y, Z, W);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vector4 Max(float value)
		{
			return new Vector4(Math.Max(X, value), Math.Max(Y, value), Math.Max(Z, value), Math.Max(W, value));
		}

		public static void Max(ref Vector4 vector, float value, out Vector4 result)
		{
			result.X = Math.Max(vector.X, value);
			result.Y = Math.Max(vector.Y, value);
			result.Z = Math.Max(vector.Z, value);
			result.W = Math.Max(vector.W, value);
		}

		public Vector4 Max(Vector4 values)
		{
			return new Vector4(Math.Max(X, values.X), Math.Max(Y, values.Y), Math.Max(Z, values.Z), Math.Max(W, values.W));
		}

		public static void Max(ref Vector4 vector, ref Vector4 values, out Vector4 result)
		{
			result.X = Math.Max(vector.X, values.X);
			result.Y = Math.Max(vector.Y, values.Y);
			result.Z = Math.Max(vector.Z, values.Z);
			result.W = Math.Max(vector.W, values.W);
		}

		public Vector4 Min(float value)
		{
			return new Vector4(Math.Min(X, value), Math.Min(Y, value), Math.Min(Z, value), Math.Min(W, value));
		}

		public static void Min(ref Vector4 vector, float value, out Vector4 result)
		{
			result.X = Math.Min(vector.X, value);
			result.Y = Math.Min(vector.Y, value);
			result.Z = Math.Min(vector.Z, value);
			result.W = Math.Min(vector.W, value);
		}

		public Vector4 Min(Vector4 values)
		{
			return new Vector4(Math.Min(X, values.X), Math.Min(Y, values.Y), Math.Min(Z, values.Z), Math.Min(W, values.W));
		}

		public static void Min(ref Vector4 vector, ref Vector4 values, out Vector4 result)
		{
			result.X = Math.Min(vector.X, values.X);
			result.Y = Math.Min(vector.Y, values.Y);
			result.Z = Math.Min(vector.Z, values.Z);
			result.W = Math.Min(vector.W, values.W);
		}

		public Vector4 Abs()
		{
			return new Vector4(Math.Abs(X), Math.Abs(Y), Math.Abs(Z), Math.Abs(W));
		}

		public static void Abs(ref Vector4 vector, out Vector4 result)
		{
			result.X = Math.Abs(vector.X);
			result.Y = Math.Abs(vector.Y);
			result.Z = Math.Abs(vector.Z);
			result.W = Math.Abs(vector.W);
		}

		public Vector4 Pow(float power)
		{
			return new Vector4((float)Math.Pow(X, power), (float)Math.Pow(Y, power), (float)Math.Pow(Z, power), (float)Math.Pow(W, power));
		}

		public static void Pow(ref Vector4 vector, float power, out Vector4 result)
		{
			result.X = (float)Math.Pow(vector.X, power);
			result.Y = (float)Math.Pow(vector.Y, power);
			result.Z = (float)Math.Pow(vector.Z, power);
			result.W = (float)Math.Pow(vector.W, power);
		}

		public Vector4 Floor()
		{
			return new Vector4((float)Math.Floor(X), (float)Math.Floor(Y), (float)Math.Floor(Z), (float)Math.Floor(W));
		}

		public static void Floor(ref Vector4 vector, out Vector4 result)
		{
			result.X = (float)Math.Floor(vector.X);
			result.Y = (float)Math.Floor(vector.Y);
			result.Z = (float)Math.Floor(vector.Z);
			result.W = (float)Math.Floor(vector.W);
		}

		public float Length()
		{
			return (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
		}

		public static void Length(ref Vector4 vector, out float result)
		{
			result = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W));
		}

		public float LengthSquared()
		{
			return (X*X) + (Y*Y) + (Z*Z) + (W*W);
		}

		public static void LengthSquared(ref Vector4 vector, out float result)
		{
			result = (vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W);
		}
		
		public float Distance(Vector4 vector)
		{
			return (vector - this).Length();
		}

		public static void Distance(ref Vector4 vector1, ref Vector4 vector2, out float result)
		{
			result = (vector2 - vector1).Length();
		}
		
		public float DistanceSquared(Vector4 vector)
		{
			return (vector - this).Dot();
		}

		public static void DistanceSquared(ref Vector4 vector1, ref Vector4 vector2, out float result)
		{
			result = (vector2 - vector1).Dot();
		}

		public float Dot()
		{
			return (X*X) + (Y*Y) + (Z*Z) + (W*W);
		}

		public static void Dot(ref Vector4 vector, out float result)
		{
			result = (vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W);
		}

		public float Dot(Vector4 vector)
		{
			return (X*vector.X) + (Y*vector.Y) + (Z*vector.Z) + (W*vector.W);
		}

		public static void Dot(ref Vector4 vector1, ref Vector4 vector2, out float result)
		{
			result = (vector1.X*vector2.X) + (vector1.Y*vector2.Y) + (vector1.Z*vector2.Z) + (vector1.W*vector2.W);
		}

		public Vector4 Normalize()
		{
			return this * (1 / (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W)));
		}

		public static void Normalize(ref Vector4 vector, out Vector4 result)
		{
			result = vector * (1 / (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W)));
		}

		public Vector4 Normalize(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			length = dis;
			return this * (1/dis);
		}

		public static void Normalize(ref Vector4 vector, out Vector4 result, out float length)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W));
			length = dis;
			result = vector * (1/dis);
		}

		public Vector4 NormalizeSafe()
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			if (dis == 0) return new Vector4();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Vector4 vector, out Vector4 result)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W));
			if (dis == 0) result = new Vector4();
			else result = vector * (1/dis);
		}

		public Vector4 NormalizeSafe(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			length = dis;
			if (dis == 0) return new Vector4();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Vector4 vector, out Vector4 result, out float length)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y) + (vector.Z*vector.Z) + (vector.W*vector.W));
			length = dis;
			if (dis == 0) result = new Vector4();
			else result = vector * (1/dis);
		}

		public Vector4 Transform(Matrix4 matrix)
		{
		    return (matrix.X*X) + (matrix.Y*Y) + (matrix.Z*Z) + (matrix.W*W);
		}

		public static void Transform(ref Vector4 vector, ref Matrix4 matrix, out Vector4 result)
		{
		    result = (matrix.X*vector.X) + (matrix.Y*vector.Y) + (matrix.Z*vector.Z) + (matrix.W*vector.W);
		}

		public bool AproxEqualsBox(Vector4 vector, float tolerance)
		{
			return
				(Math.Abs(X-vector.X) <= tolerance) &&
				(Math.Abs(Y-vector.Y) <= tolerance) &&
				(Math.Abs(Z-vector.Z) <= tolerance) &&
				(Math.Abs(W-vector.W) <= tolerance);
		}

		public static void AproxEqualsBox(ref Vector4 vector1, ref Vector4 vector2, float tolerance, out bool result)
		{
			result =
				(Math.Abs(vector1.X-vector2.X) <= tolerance) &&
				(Math.Abs(vector1.Y-vector2.Y) <= tolerance) &&
				(Math.Abs(vector1.Z-vector2.Z) <= tolerance) &&
				(Math.Abs(vector1.W-vector2.W) <= tolerance);
		}

		public bool ApproxEquals(Vector4 vector, float tolerance)
		{
		    return (Distance(vector) <= tolerance);
		}

		public static void ApproxEquals(ref Vector4 vector1, ref Vector4 vector2, float tolerance, out bool result)
		{
		    result = (vector1.Distance(vector2) <= tolerance);
		}

		public Vector4 Project(Matrix4 projectionMatrix, Matrix4 viewMatrix, int viewX, int viewY, int viewWidth, int viewHeight)
		{
			var result = this;
			result = result.Transform(viewMatrix);
			result = result.Transform(projectionMatrix);
			
			float wDelta = 1 / result.W;
			result.X *= wDelta;
			result.Y *= wDelta;
			result.Z *= wDelta;
			
			result.X = (result.X * .5f) + .5f;
			result.Y = (result.Y * .5f) + .5f;
			result.Z = (result.Z * .5f) + .5f;

			result.X = (result.X * viewWidth) + viewX;
			result.Y = (result.Y * viewHeight) + viewY;

			return result;
		}

		public static void Project(ref Vector4 vector, ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix, int viewX, int viewY, int viewWidth, int viewHeight, out Vector4 result)
		{
			result = vector;
			result = result.Transform(viewMatrix);
			result = result.Transform(projectionMatrix);
			
			float wDelta = 1 / result.W;
			result.X *= wDelta;
			result.Y *= wDelta;
			result.Z *= wDelta;
			
			result.X = (result.X * .5f) + .5f;
			result.Y = (result.Y * .5f) + .5f;
			result.Z = (result.Z * .5f) + .5f;

			result.X = (result.X * viewWidth) + viewX;
			result.Y = (result.Y * viewHeight) + viewY;
		}

		public Vector4 UnProject(Matrix4 viewProjInverse, int viewX, int viewY, int viewWidth, int viewHeight)
		{
			var result = this;
			result.X = (result.X - viewX) / viewWidth;
			result.Y = (result.Y - viewY) / viewHeight;
			result = (result * 2) - 1;

			result = result.Transform(viewProjInverse);
			float wDelta = 1 / result.W;
			result.X *= wDelta;
			result.Y *= wDelta;
			result.Z *= wDelta;
			
			return result;
		}

		public static void UnProject(ref Vector4 vector, ref Matrix4 viewProjInverse, int viewX, int viewY, int viewWidth, int viewHeight, out Vector4 result)
		{
			result = vector;
			result.X = (result.X - viewX) / viewWidth;
			result.Y = (result.Y - viewY) / viewHeight;
			result = (result * 2) - 1;

			result = result.Transform(viewProjInverse);
			float wDelta = 1 / result.W;
			result.X *= wDelta;
			result.Y *= wDelta;
			result.Z *= wDelta;
		}

		public Vector4 UnProject(Matrix4 projectionMatrix, Matrix4 viewMatrix, int viewX, int viewY, int viewWidth, int viewHeight)
		{
			var viewProjInverse = viewMatrix.Multiply(projectionMatrix).Invert();
			
			var result = this;
			result.X = (result.X - viewX) / viewWidth;
			result.Y = (result.Y - viewY) / viewHeight;
			result = (result * 2) - 1;

			result = result.Transform(viewProjInverse);
			float wDelta = 1 / result.W;
			result.X *= wDelta;
			result.Y *= wDelta;
			result.Z *= wDelta;
			
			return result;
		}

		public static void UnProject(ref Vector4 vector, ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix, int viewX, int viewY, int viewWidth, int viewHeight, out Vector4 result)
		{
			var viewProjInverse = viewMatrix.Multiply(projectionMatrix).Invert();
			
			result = vector;
			result.X = (result.X - viewX) / viewWidth;
			result.Y = (result.Y - viewY) / viewHeight;
			result = (result * 2) - 1;

			result = result.Transform(viewProjInverse);
			float wDelta = 1 / result.W;
			result.X *= wDelta;
			result.Y *= wDelta;
			result.Z *= wDelta;
		}
		#endregion
	}
}