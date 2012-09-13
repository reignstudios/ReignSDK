using System.Runtime.InteropServices;
using MathS = System.Math;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2
	{
		#region Properties
		public float X, Y;
		private float z, w;
		#endregion

		#region Constructors
		public Vector2(float value)
		{
			X = value;
			Y = value;
			z = 0;
			w = 0;
		}

		public Vector2(float x, float y)
		{
			X = x;
			Y = y;
			z = 0;
			w = 0;
		}
		#endregion

		#region Operators
		// +
		public static Vector2 operator+(Vector2 p1, Vector2 p2) {return new Vector2(p1.X+p2.X, p1.Y+p2.Y);}
		public static Vector2 operator+(Vector2 p1, float p2) {return new Vector2(p1.X+p2, p1.Y+p2);}
		public static Vector2 operator+(float p1, Vector2 p2) {return new Vector2(p1+p2.X, p1+p2.Y);}
		// -
		public static Vector2 operator-(Vector2 p1, Vector2 p2) {return new Vector2(p1.X-p2.X, p1.Y-p2.Y);}
		public static Vector2 operator-(Vector2 p1, float p2) {return new Vector2(p1.X-p2, p1.Y-p2);}
		public static Vector2 operator-(float p1, Vector2 p2) {return new Vector2(p1-p2.X, p1-p2.Y);}
		public static Vector2 operator-(Vector2 p1) {return new Vector2(-p1.X, -p1.Y);}
		// *
		public static Vector2 operator*(Vector2 p1, Vector2 p2) {return new Vector2(p1.X*p2.X, p1.Y*p2.Y);}
		public static Vector2 operator*(Vector2 p1, float p2) {return new Vector2(p1.X*p2, p1.Y*p2);}
		public static Vector2 operator*(float p1, Vector2 p2) {return new Vector2(p1*p2.X, p1*p2.Y);}
		// /
		public static Vector2 operator/(Vector2 p1, Vector2 p2) {return new Vector2(p1.X/p2.X, p1.Y/p2.Y);}
		public static Vector2 operator/(Vector2 p1, float p2) {return new Vector2(p1.X/p2, p1.Y/p2);}
		public static Vector2 operator/(float p1, Vector2 p2) {return new Vector2(p1/p2.X, p1/p2.Y);}
		// ==
		public static bool operator==(Vector2 p1, Vector2 p2) {return (p1.X==p2.X && p1.Y==p2.Y);}
		public static bool operator!=(Vector2 p1, Vector2 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y);}
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
			return new Vector2(Math.DegToRad(X), Math.DegToRad(Y));
		}

		public Vector2 RadToDeg()
		{
			return new Vector2(Math.RadToDeg(X), Math.RadToDeg(Y));
		}

		public Vector2 Max(float value)
		{
			return new Vector2(MathS.Max(X, value), MathS.Max(Y, value));
		}

		public Vector2 Min(float value)
		{
			return new Vector2(MathS.Min(X, value), MathS.Min(Y, value));
		}

		public Vector2 Abs()
		{
			return new Vector2(MathS.Abs(X), MathS.Abs(Y));
		}

		public Vector2 Pow(float value)
		{
			return new Vector2((float)MathS.Pow(X, value), (float)MathS.Pow(Y, value));
		}

		public Vector2 Floor()
		{
			return new Vector2((float)MathS.Floor(X), (float)MathS.Floor(Y));
		}

		public float Dot()
		{
			return (X*X) + (Y*Y);
		}

		public float Dot(Vector2 vector)
		{
			return (X*vector.X) + (Y*vector.Y);
		}

		public float Length()
		{
			#if SIMD
			var result = input * input;
			result = result.HorizontalAdd(result);
			result = result.HorizontalAdd(result);
			result *= result.InvSqrt();
			return result.X;
			#else
			return (float)MathS.Sqrt((X*X) + (Y*Y));
			#endif
		}

		public Vector2 Normalize()
		{
			#if SIMD
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			return input * factor.InvSqrt();
			#else
			return this * (1 / (float)MathS.Sqrt((X*X) + (Y*Y)));
			#endif
		}

		public Vector2 Normalize(out float length)
		{
			#if SIMD
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			factor = factor.InvSqrt();
			length = (factor * factor).X;
			return input * factor;
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y));
			length = dis;
			return this * (1/dis);
			#endif
		}

		public Vector2 NormalizeSafe()
		{
			#if SIMD
			if (input.X == 0 && input.Y == 0 && input.Z == 0 && input.W == 0)
			{
				return new Vector2();
			}
			
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			return vector * factor.InvSqrt();
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y));
			if (dis == 0) return new Vector2();
			else return this * (1/dis);
			#endif
		}

		public Vector2 NormalizeSafe(out float length)
		{
			#if SIMD
			if (input.X == 0 && input.Y == 0) return new Vector2();
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			factor = factor.InvSqrt();
			length = (factor * factor).X;
			return input * factor;
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y));
			length = dis;
			if (dis == 0) return new Vector2();
			else return this * (1/dis);
			#endif
		}

		public Vector2 Cross()
		{
			return new Vector2(-Y, X);
		}

		//public Vector2 Transform(Matrix4 matrix)
		//{
		//    #if SIMD
		//    return
		//        (matrix.X * input.Shuffle(ShuffleSel.ExpandX)) +
		//        (matrix.Y * input.Shuffle(ShuffleSel.ExpandY));
		//    #else
		//    return (matrix.X*input.X) + (matrix.Y*input.Y);
		//    #endif
		//}

		public bool AproxEqualsBox(Vector2 vector, float tolerance)
		{
			return
				(MathS.Abs(X-vector.X) <= tolerance) &&
				(MathS.Abs(Y-vector.Y) <= tolerance);
		}

		public bool ApproxEquals(Vector2 vector, float tolerance)
		{
		    return (Length(vector) <= tolerance);
		}

		public float Length(Vector2 vector)
		{
			#if SIMD
			var result = vector - input;
			result *= result;
			result = result.HorizontalAdd(result);
			result = result.HorizontalAdd(result);
			result *= result.InvSqrt();
			return result.X;
			#else
			return (vector - this).Length();
			#endif
		}
		
		public float LengthSquared(Vector2 vector)
		{
			#if SIMD
			var result = vector - input;
			result *= result;
			result = result.HorizontalAdd(result);
			result = result.HorizontalAdd(result);
			return result.X;
			#else
			return (vector - this).Dot();
			#endif
		}

		public float Angle()
		{
			var vec = this.Normalize();
			float val = vec.X;
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			return (float)MathS.Acos(val);
		}

		public float Angle(Vector2 vector)
		{
			var vec = this.Normalize();
			float val = vec.Dot(vector.Normalize());
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			return (float)MathS.Acos(val);
		}

		public float Angle90()
		{
			var vec = this.Normalize();
			float val = MathS.Abs(vec.X);
			val = (val > 1) ? 1 : val;
			return (float)MathS.Acos(val);
		}

		public float Angle90(Vector2 vector)
		{
			var vec = this.Normalize();
			float val = MathS.Abs(vec.Dot(vector.Normalize()));
			val = (val > 1) ? 1 : val;
			return (float)MathS.Acos(val);
		}

		public float Angle180()
		{
			var vec = this.Normalize();
			return ((float)MathS.Atan2(-vec.Y, vec.X)) % Math.Pi2;
		}

		public float Angle180(Vector2 vector)
		{
		    var vec = this.Normalize();
		    vector = vector.Normalize();
		    return ((float)MathS.Atan2((vec.X*vector.Y)-(vec.Y*vector.X), (vec.X*vector.X)+(vec.Y*vector.Y))) % Math.Pi2;
		}

		public float Angle360()
		{
		    var vec = this.Normalize();
		    float value = ((float)MathS.Atan2(-vec.Y, vec.X)) % Math.Pi2;
		    return (value < 0) ? ((Math.Pi+value)+Math.Pi) : value;
		}

		public float Angle360(Vector2 vector)
		{
		    var vec = this.Normalize();
		    vector = vector.Normalize();
		    float value = ((float)MathS.Atan2((vec.X*vector.Y)-(vec.Y*vector.X), (vec.X*vector.X)+(vec.Y*vector.Y))) % Math.Pi2;
		    return (value < 0) ? ((Math.Pi+value)+Math.Pi) : value;
		}
		#endregion
	}
}