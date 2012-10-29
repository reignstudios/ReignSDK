using System.Runtime.InteropServices;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2
	{
		#region Properties
		public float X, Y;
		#endregion

		#region Constructors
		public Vector2(float value)
		{
			X = value;
			Y = value;
		}

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static readonly Vector2 One = new Vector2(1);
		public static readonly Vector2 MinusOne = new Vector2(-1);
		public static readonly Vector2 Zero = new Vector2(0);
		public static readonly Vector2 Right = new Vector2(1, 0);
		public static readonly Vector2 Left = new Vector2(-1, 0);
		public static readonly Vector2 Up = new Vector2(0, 1);
		public static readonly Vector2 Down = new Vector2(0, -1);
		#endregion

		#region Operators
		// +
		public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X + value2.X;
			result.Y = value1.Y + value2.Y;
		}

		public static void Add(ref Vector2 value1, float value2, out Vector2 result)
		{
			result.X = value1.X + value2;
			result.Y = value1.Y + value2;
		}

		public static void Add(float value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1 + value2.X;
			result.Y = value1 + value2.Y;
		}

		public static Vector2 operator+(Vector2 p1, Vector2 p2)
		{
			p1.X += p2.X;
			p1.Y += p2.Y;
			return p1;
		}

		public static Vector2 operator+(Vector2 p1, float p2)
		{
			p1.X += p2;
			p1.Y += p2;
			return p1;
		}

		public static Vector2 operator+(float p1, Vector2 p2)
		{
			p2.X = p1 + p2.X;
			p2.Y = p1 + p2.Y;
			return p2;
		}

		// -
		public static void Sub(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X - value2.X;
			result.Y = value1.Y - value2.Y;
		}

		public static void Sub(ref Vector2 value1, float value2, out Vector2 result)
		{
			result.X = value1.X - value2;
			result.Y = value1.Y - value2;
		}

		public static void Sub(float value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1 - value2.X;
			result.Y = value1 - value2.Y;
		}

		public static void Neg(ref Vector2 value, out Vector2 result)
		{
			result.X = -value.X;
			result.Y = -value.Y;
		}

		public static Vector2 operator-(Vector2 p1, Vector2 p2)
		{
			p1.X -= p2.X;
			p1.Y -= p2.Y;
			return p1;
		}

		public static Vector2 operator-(Vector2 p1, float p2)
		{
			p1.X -= p2;
			p1.Y -= p2;
			return p1;
		}

		public static Vector2 operator-(float p1, Vector2 p2)
		{
			p2.X = p1 - p2.X;
			p2.Y = p1 - p2.Y;
			return p2;
		}

		public static Vector2 operator-(Vector2 p2)
		{
			p2.X = -p2.X;
			p2.Y = -p2.Y;
			return p2;
		}

		// *
		public static void Mul(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X * value2.X;
			result.Y = value1.Y * value2.Y;
		}

		public static void Mul(ref Vector2 value1, float value2, out Vector2 result)
		{
			result.X = value1.X * value2;
			result.Y = value1.Y * value2;
		}

		public static void Mul(float value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1 * value2.X;
			result.Y = value1 * value2.Y;
		}

		public static Vector2 operator*(Vector2 p1, Vector2 p2)
		{
			p1.X *= p2.X;
			p1.Y *= p2.Y;
			return p1;
		}

		public static Vector2 operator*(Vector2 p1, float p2)
		{
			p1.X *= p2;
			p1.Y *= p2;
			return p1;
		}

		public static Vector2 operator*(float p1, Vector2 p2)
		{
			p2.X = p1 * p2.X;
			p2.Y = p1 * p2.Y;
			return p2;
		}

		// /
		public static void Div(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1.X / value2.X;
			result.Y = value1.Y / value2.Y;
		}

		public static void Div(ref Vector2 value1, float value2, out Vector2 result)
		{
			result.X = value1.X / value2;
			result.Y = value1.Y / value2;
		}

		public static void Div(float value1, ref Vector2 value2, out Vector2 result)
		{
			result.X = value1 / value2.X;
			result.Y = value1 / value2.Y;
		}

		public static Vector2 operator/(Vector2 p1, Vector2 p2)
		{
			p1.X /= p2.X;
			p1.Y /= p2.Y;
			return p1;
		}

		public static Vector2 operator/(Vector2 p1, float p2)
		{
			p1.X /= p2;
			p1.Y /= p2;
			return p1;
		}

		public static Vector2 operator/(float p1, Vector2 p2)
		{
			p2.X = p1 / p2.X;
			p2.Y = p1 / p2.Y;
			return p2;
		}

		// ==
		public static bool operator==(Vector2 p1, Vector2 p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Vector2 p1, Vector2 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}

		// convert
		public Point ToPoint()
		{
			return new Point((int)X, (int)Y);
		}

		public static void ToPoint(ref Vector2 vector, out Point result)
		{
			result.X = (int)vector.X;
			result.Y = (int)vector.Y;
		}
		#endregion

		#region Methods
		public override bool Equals(object obj)
		{
			return obj != null && (Vector2)obj == this;
		}

		public override string ToString()
		{
			return string.Format("<{0}, {1}>", X, Y);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public Vector2 DegToRad()
		{
			return new Vector2(MathUtilities.DegToRad(X), MathUtilities.DegToRad(Y));
		}

		public static void DegToRad(ref Vector2 vector, out Vector2 result)
		{
			result.X = MathUtilities.DegToRad(vector.X);
			result.Y = MathUtilities.DegToRad(vector.Y);
		}

		public Vector2 RadToDeg()
		{
			return new Vector2(MathUtilities.RadToDeg(X), MathUtilities.RadToDeg(Y));
		}

		public static void RadToDeg(ref Vector2 vector, out Vector2 result)
		{
			result.X = MathUtilities.RadToDeg(vector.X);
			result.Y = MathUtilities.RadToDeg(vector.Y);
		}

		public Vector2 Max(float value)
		{
			return new Vector2(Math.Max(X, value), Math.Max(Y, value));
		}

		public static void Max(ref Vector2 vector, float value, out Vector2 result)
		{
			result.X = Math.Max(vector.X, value);
			result.Y = Math.Max(vector.Y, value);
		}

		public Vector2 Max(Vector2 values)
		{
			return new Vector2(Math.Max(X, values.X), Math.Max(Y, values.Y));
		}

		public static void Max(ref Vector2 vector, ref Vector2 values, out Vector2 result)
		{
			result.X = Math.Max(vector.X, values.X);
			result.Y = Math.Max(vector.Y, values.Y);
		}

		public Vector2 Min(float value)
		{
			return new Vector2(Math.Min(X, value), Math.Min(Y, value));
		}

		public static void Min(ref Vector2 vector, float value, out Vector2 result)
		{
			result.X = Math.Min(vector.X, value);
			result.Y = Math.Min(vector.Y, value);
		}

		public Vector2 Min(Vector2 values)
		{
			return new Vector2(Math.Min(X, values.X), Math.Min(Y, values.Y));
		}

		public static void Min(ref Vector2 vector, ref Vector2 values, out Vector2 result)
		{
			result.X = Math.Min(vector.X, values.X);
			result.Y = Math.Min(vector.Y, values.Y);
		}

		public Vector2 Abs()
		{
			return new Vector2(Math.Abs(X), Math.Abs(Y));
		}

		public static void Abs(ref Vector2 vector, out Vector2 result)
		{
			result.X = Math.Abs(vector.X);
			result.Y = Math.Abs(vector.Y);
		}

		public Vector2 Pow(float value)
		{
			return new Vector2((float)Math.Pow(X, value), (float)Math.Pow(Y, value));
		}

		public static void Pow(ref Vector2 vector, float value, out Vector2 result)
		{
			result.X = (float)Math.Pow(vector.X, value);
			result.Y = (float)Math.Pow(vector.Y, value);
		}

		public Vector2 Floor()
		{
			return new Vector2((float)Math.Floor(X), (float)Math.Floor(Y));
		}

		public static void Floor(ref Vector2 vector, out Vector2 result)
		{
			result.X = (float)Math.Floor(vector.X);
			result.Y = (float)Math.Floor(vector.Y);
		}

		public float Length()
		{
			return (float)Math.Sqrt((X*X) + (Y*Y));
		}

		public static void Length(ref Vector2 vector, out float result)
		{
			result = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y));
		}

		public float LengthSquared()
		{
			return (X*X) + (Y*Y);
		}

		public static void LengthSquared(ref Vector2 vector, out float result)
		{
			result = (vector.X*vector.X) + (vector.Y*vector.Y);
		}

		public float Distance(Vector2 vector)
		{
			return (vector - this).Length();
		}

		public static void Distance(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
			result = (vector2 - vector1).Length();
		}
		
		public float DistanceSquared(Vector2 vector)
		{
			return (vector - this).Dot();
		}

		public static void DistanceSquared(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
			result = (vector2 - vector1).Dot();
		}

		public float Dot()
		{
			return (X*X) + (Y*Y);
		}

		public static void Dot(ref Vector2 vector, out float result)
		{
			result = (vector.X*vector.X) + (vector.Y*vector.Y);
		}

		public float Dot(Vector2 vector)
		{
			return (X*vector.X) + (Y*vector.Y);
		}

		public static void Dot(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
			result =  (vector1.X*vector2.X) + (vector1.Y*vector2.Y);
		}

		public Vector2 Normalize()
		{
			return this * (1 / (float)Math.Sqrt((X*X) + (Y*Y)));
		}

		public static void Normalize(ref Vector2 vector, out Vector2 result)
		{
			result = vector * (1 / (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y)));
		}

		public Vector2 Normalize(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y));
			length = dis;
			return this * (1/dis);
		}

		public static void Normalize(ref Vector2 vector, out Vector2 result, out float length)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y));
			length = dis;
			result = vector * (1/dis);
		}

		public Vector2 NormalizeSafe()
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y));
			if (dis == 0) return new Vector2();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Vector2 vector, out Vector2 result)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y));
			if (dis == 0) result = new Vector2();
			else result = vector * (1/dis);
		}

		public Vector2 NormalizeSafe(out float length)
		{
			float dis = (float)Math.Sqrt((X*X) + (Y*Y));
			length = dis;
			if (dis == 0) return new Vector2();
			else return this * (1/dis);
		}

		public static void NormalizeSafe(ref Vector2 vector, out Vector2 result, out float length)
		{
			float dis = (float)Math.Sqrt((vector.X*vector.X) + (vector.Y*vector.Y));
			length = dis;
			if (dis == 0) result = new Vector2();
			else result = vector * (1/dis);
		}

		public Vector2 Cross()
		{
			return new Vector2(-Y, X);
		}

		public static void Cross(ref Vector2 vector, out Vector2 result)
		{
			result.X = -vector.Y;
			result.Y = vector.X;
		}

		public Vector2 Transform(Matrix2 matrix)
		{
		    return (matrix.X*X) + (matrix.Y*Y);
		}

		public static void Transform(ref Vector2 vector, ref Matrix2 matrix, out Vector2 result)
		{
		    result = (matrix.X*vector.X) + (matrix.Y*vector.Y);
		}

		public Vector3 Transform(Matrix2x3 matrix)
        {
			return new Vector3
			(
				(X * matrix.X.X) + (Y * matrix.Y.X),
				(X * matrix.X.Y) + (Y * matrix.Y.Y),
				(X * matrix.X.Z) + (Y * matrix.Y.Z)
			);
        }

		public static void Transform(ref Vector2 vector, ref Matrix2x3 matrix, out Vector3 result)
        {
            result.X = (vector.X * matrix.X.X) + (vector.Y * matrix.Y.X);
            result.Y = (vector.X * matrix.X.Y) + (vector.Y * matrix.Y.Y);
            result.Z = (vector.X * matrix.X.Z) + (vector.Y * matrix.Y.Z);
        }

		public Vector3 Transform(Matrix3x2 matrix)
        {
			return new Vector3
			(
				(matrix.X.X * X) + (matrix.X.Y * Y),
				(matrix.Y.X * X) + (matrix.Y.Y * Y),
				(matrix.Z.X * X) + (matrix.Z.Y * Y)
			);
        }

		public static void Transform(ref Vector2 vector, ref Matrix3x2 matrix, out Vector3 result)
        {
            result.X = (matrix.X.X * vector.X) + (matrix.X.Y * vector.Y);
            result.Y = (matrix.Y.X * vector.X) + (matrix.Y.Y * vector.Y);
            result.Z = (matrix.Z.X * vector.X) + (matrix.Z.Y * vector.Y);
        }

		public bool AproxEqualsBox(Vector2 vector, float tolerance)
		{
			return
				(Math.Abs(X-vector.X) <= tolerance) &&
				(Math.Abs(Y-vector.Y) <= tolerance);
		}

		public static void AproxEqualsBox(ref Vector2 vector1, ref Vector2 vector2, float tolerance, out bool result)
		{
			result =
				(Math.Abs(vector1.X-vector2.X) <= tolerance) &&
				(Math.Abs(vector1.Y-vector2.Y) <= tolerance);
		}

		public bool ApproxEquals(Vector2 vector, float tolerance)
		{
		    return (this.Distance(vector) <= tolerance);
		}

		public static void ApproxEquals(ref Vector2 vector1, ref Vector2 vector2, float tolerance, out bool result)
		{
		    result = (vector1.Distance(vector2) <= tolerance);
		}

		public float Angle()
		{
			var vec = this.Normalize();
			float val = vec.X;
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			return (float)Math.Acos(val);
		}

		public static void Angle(ref Vector2 vector, out float result)
		{
			var vec = vector.Normalize();
			float val = vec.X;
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			result = (float)Math.Acos(val);
		}

		public float Angle(Vector2 vector)
		{
			var vec = this.Normalize();
			float val = vec.Dot(vector.Normalize());
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			return (float)Math.Acos(val);
		}

		public static void Angle(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
			var vec = vector1.Normalize();
			float val = vec.Dot(vector2.Normalize());
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			result = (float)Math.Acos(val);
		}

		public float Angle90()
		{
			var vec = this.Normalize();
			float val = Math.Abs(vec.X);
			val = (val > 1) ? 1 : val;
			return (float)Math.Acos(val);
		}

		public static void Angle90(ref Vector2 vector, out float result)
		{
			var vec = vector.Normalize();
			float val = Math.Abs(vec.X);
			val = (val > 1) ? 1 : val;
			result = (float)Math.Acos(val);
		}

		public float Angle90(Vector2 vector)
		{
			var vec = this.Normalize();
			float val = Math.Abs(vec.Dot(vector.Normalize()));
			val = (val > 1) ? 1 : val;
			return (float)Math.Acos(val);
		}

		public static void Angle90(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
			var vec = vector1.Normalize();
			float val = Math.Abs(vec.Dot(vector2.Normalize()));
			val = (val > 1) ? 1 : val;
			result = (float)Math.Acos(val);
		}

		public float Angle180()
		{
			var vec = this.Normalize();
			return ((float)Math.Atan2(-vec.Y, vec.X)) % MathUtilities.Pi2;
		}

		public static void Angle180(ref Vector2 vector, out float result)
		{
			var vec = vector.Normalize();
			result = ((float)Math.Atan2(-vec.Y, vec.X)) % MathUtilities.Pi2;
		}

		public float Angle180(Vector2 vector)
		{
		    var vec = this.Normalize();
		    vector = vector.Normalize();
		    return ((float)Math.Atan2((vec.X*vector.Y)-(vec.Y*vector.X), (vec.X*vector.X)+(vec.Y*vector.Y))) % MathUtilities.Pi2;
		}

		public static void Angle180(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
		    var vec = vector1.Normalize();
		    vector2 = vector2.Normalize();
		    result = ((float)Math.Atan2((vec.X*vector2.Y)-(vec.Y*vector2.X), (vec.X*vector2.X)+(vec.Y*vector2.Y))) % MathUtilities.Pi2;
		}

		public float Angle360()
		{
		    var vec = this.Normalize();
		    float value = ((float)Math.Atan2(-vec.Y, vec.X)) % MathUtilities.Pi2;
		    return (value < 0) ? ((MathUtilities.Pi+value)+MathUtilities.Pi) : value;
		}

		public static void Angle360(ref Vector2 vector, out float result)
		{
		    var vec = vector.Normalize();
		    float value = ((float)Math.Atan2(-vec.Y, vec.X)) % MathUtilities.Pi2;
		    result = (value < 0) ? ((MathUtilities.Pi+value)+MathUtilities.Pi) : value;
		}

		public float Angle360(Vector2 vector)
		{
		    var vec = this.Normalize();
		    vector = vector.Normalize();
		    float value = ((float)Math.Atan2((vec.X*vector.Y)-(vec.Y*vector.X), (vec.X*vector.X)+(vec.Y*vector.Y))) % MathUtilities.Pi2;
		    return (value < 0) ? ((MathUtilities.Pi+value)+MathUtilities.Pi) : value;
		}

		public static void Angle360(ref Vector2 vector1, ref Vector2 vector2, out float result)
		{
		    var vec = vector1.Normalize();
		    vector2 = vector2.Normalize();
		    float value = ((float)Math.Atan2((vec.X*vector2.Y)-(vec.Y*vector2.X), (vec.X*vector2.X)+(vec.Y*vector2.Y))) % MathUtilities.Pi2;
		    result = (value < 0) ? ((MathUtilities.Pi+value)+MathUtilities.Pi) : value;
		}

		public static Vector2 Lerp(Vector2 start, Vector2 end, float interpolationAmount)
        {
            float startAmount = 1 - interpolationAmount;
			return new Vector2
			(
				(start.X * startAmount) + (end.X * interpolationAmount),
				(start.Y * startAmount) + (end.Y * interpolationAmount)
			);
        }

		public static void Lerp(ref Vector2 start, ref Vector2 end, float interpolationAmount, out Vector2 result)
        {
            float startAmount = 1 - interpolationAmount;
            result.X = (start.X * startAmount) + (end.X * interpolationAmount);
            result.Y = (start.Y * startAmount) + (end.Y * interpolationAmount);
        }
		#endregion
	}
}