using System.Runtime.InteropServices;
using MathS = System.Math;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector4
	{
		#region Properties
		public float X, Y, Z, W;
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

		public static Vector4 AsQuaternion(Matrix3 matrix)
		{
			float w = (float)MathS.Sqrt(1 + matrix.X.X + matrix.Y.Y + matrix.Z.Z) * .5f;
			float w2 = 1 / (w * 4);
			return new Vector4
			(
				(matrix.Z.Y - matrix.Y.Z) * w2,
				(matrix.X.Z - matrix.Z.X) * w2,
				(matrix.Y.X - matrix.X.Y) * w2,
				w
			);
		}

		public static Vector4 AsQuaternion(Vector3 axis, float angle)
		{
			angle *= .5f;
			var sin = (float)MathS.Sin(angle);
			return new Vector4
			(
				axis.X * sin,
				axis.Y * sin,
				axis.Z * sin,
				(float)MathS.Cos(angle)
			);
		}
		#endregion

		#region Operators
		// +
		public static Vector4 operator+(Vector4 p1, Vector4 p2) {return new Vector4(p1.X+p2.X, p1.Y+p2.Y, p1.Z+p2.Z, p1.W+p2.W);}
		public static Vector4 operator+(Vector4 p1, float p2) {return new Vector4(p1.X+p2, p1.Y+p2, p1.Z+p2, p1.W+p2);}
		public static Vector4 operator+(float p1, Vector4 p2) {return new Vector4(p1+p2.X, p1+p2.Y, p1+p2.Z, p1+p2.W);}
		// -
		public static Vector4 operator-(Vector4 p1, Vector4 p2) {return new Vector4(p1.X-p2.X, p1.Y-p2.Y, p1.Z-p2.Z, p1.W-p2.W);}
		public static Vector4 operator-(Vector4 p1, float p2) {return new Vector4(p1.X-p2, p1.Y-p2, p1.Z-p2, p1.W-p2);}
		public static Vector4 operator-(float p1, Vector4 p2) {return new Vector4(p1-p2.X, p1-p2.Y, p1-p2.Z, p1-p2.W);}
		public static Vector4 operator-(Vector4 p1) {return new Vector4(-p1.X, -p1.Y, -p1.Z, p1.W);}
		// *
		public static Vector4 operator*(Vector4 p1, Vector4 p2) {return new Vector4(p1.X*p2.X, p1.Y*p2.Y, p1.Z*p2.Z, p1.W*p2.W);}
		public static Vector4 operator*(Vector4 p1, float p2) {return new Vector4(p1.X*p2, p1.Y*p2, p1.Z*p2, p1.W*p2);}
		public static Vector4 operator*(float p1, Vector4 p2) {return new Vector4(p1*p2.X, p1*p2.Y, p1*p2.Z, p1*p2.W);}
		// /
		public static Vector4 operator/(Vector4 p1, Vector4 p2) {return new Vector4(p1.X/p2.X, p1.Y/p2.Y, p1.Z/p2.Z, p1.W/p2.W);}
		public static Vector4 operator/(Vector4 p1, float p2) {return new Vector4(p1.X/p2, p1.Y/p2, p1.Z/p2, p1.W/p2);}
		public static Vector4 operator/(float p1, Vector4 p2) {return new Vector4(p1/p2.X, p1/p2.Y, p1/p2.Z, p1/p2.W);}
		// ==
		public static bool operator==(Vector4 p1, Vector4 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z && p1.W==p2.W);}
		public static bool operator!=(Vector4 p1, Vector4 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z || p1.W!=p2.W);}
		// convert
		public Vector2 ToVector2() {return new Vector2(X, Y);}
		public Vector3 ToVector3() {return new Vector3(X, Y, Z);}
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
			return new Vector4(MathS.Max(X, value), MathS.Max(Y, value), MathS.Max(Z, value), MathS.Max(W, value));
		}

		public Vector4 Min(float value)
		{
			return new Vector4(MathS.Min(X, value), MathS.Min(Y, value), MathS.Min(Z, value), MathS.Min(W, value));
		}

		public Vector4 Abs()
		{
			return new Vector4(MathS.Abs(X), MathS.Abs(Y), MathS.Abs(Z), MathS.Abs(W));
		}

		public Vector4 Pow(float value)
		{
			return new Vector4((float)MathS.Pow(X, value), (float)MathS.Pow(Y, value), (float)MathS.Pow(Z, value), (float)MathS.Pow(W, value));
		}

		public Vector4 Floor()
		{
			return new Vector4((float)MathS.Floor(X), (float)MathS.Floor(Y), (float)MathS.Floor(Z), (float)MathS.Floor(W));
		}

		public float Dot()
		{
			return (X*X) + (Y*Y) + (Z*Z) + (W*W);
		}

		public float Dot(Vector4 vector)
		{
			return (X*vector.X) + (Y*vector.Y) + (Z*vector.Z) + (W*vector.W);
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
			return (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			#endif
		}		

		public Vector4 Normalize()
		{
			#if SIMD
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			return input * factor.InvSqrt();
			#else
			return this * (1 / (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W)));
			#endif
		}

		public Vector4 Normalize(out float length)
		{
			#if SIMD
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			factor = factor.InvSqrt();
			length = (factor * factor).X;
			return input * factor;
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			length = dis;
			return this * (1/dis);
			#endif
		}

		public Vector4 NormalizeSafe()
		{
			#if SIMD
			if (input.X == 0 && input.Y == 0 && input.Z == 0 && input.W == 0)
			{
				return new Vector4();
			}
			
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			return vector * factor.InvSqrt();
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			if (dis == 0) return new Vector4();
			else return this * (1/dis);
			#endif
		}

		public Vector4 NormalizeSafe(out float distance)
		{
			#if SIMD
			if (input.X == 0 && input.Y == 0 && input.Z == 0 && input.W == 0) return new Vector4();
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			factor = factor.InvSqrt();
			length = (factor * factor).X;
			return input * factor;
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z) + (W*W));
			distance = dis;
			if (dis == 0) return new Vector4();
			else return this * (1/dis);
			#endif
		}

		public Vector4 Transform(Matrix4 matrix)
		{
			#if SIMD
			return
				(matrix.X * input.Shuffle(ShuffleSel.ExpandX)) +
				(matrix.Y * input.Shuffle(ShuffleSel.ExpandY)) +
				(matrix.Z * input.Shuffle(ShuffleSel.ExpandZ)) +
				(matrix.W * input.Shuffle(ShuffleSel.ExpandW));
			#else
		    return (matrix.X*X) + (matrix.Y*Y) + (matrix.Z*Z) + (matrix.W*W);
			#endif
		}

		public Vector4 Multiply(Vector4 quaternion)
		{
			return new Vector4
			(
				W*quaternion.X + X*quaternion.W + Y*quaternion.Z - Z*quaternion.Y,
				W*quaternion.Y - X*quaternion.Z + Y*quaternion.W + Z*quaternion.X,
				W*quaternion.Z + X*quaternion.Y - Y*quaternion.X + Z*quaternion.W,
				W*quaternion.W - X*quaternion.X - Y*quaternion.Y - Z*quaternion.Z
			);
		}

		public bool AproxEqualsBox(Vector4 vector, float tolerance)
		{
			return
				(MathS.Abs(X-vector.X) <= tolerance) &&
				(MathS.Abs(Y-vector.Y) <= tolerance) &&
				(MathS.Abs(Z-vector.Z) <= tolerance) &&
				(MathS.Abs(W-vector.W) <= tolerance);
		}

		public bool ApproxEquals(Vector4 vector, float tolerance)
		{
		    return (Distance(vector) <= tolerance);
		}

		public float Distance(Vector4 vector)
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
		
		public float DistanceSquared(Vector4 vector)
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

		public Vector4 Project(Matrix4 projectionMatrix, Matrix4 viewMatrix, int viewX, int viewY, int viewWidth, int viewHeight)
		{
			var vec = this;
			vec = vec.Transform(viewMatrix);
			vec = vec.Transform(projectionMatrix);
			
			vec.X /= vec.W;
			vec.Y /= vec.W;
			vec.Z /= vec.W;
			
			vec.X = (vec.X * .5f) + .5f;
			vec.Y = (vec.Y * .5f) + .5f;
			vec.Z = (vec.Z * .5f) + .5f;

			vec.X = (vec.X * viewWidth) + viewX;
			vec.Y = (vec.Y * viewHeight) + viewY;

			return vec;
		}

		public Vector4 UnProject(Matrix4 viewProjInverse, int viewX, int viewY, int viewWidth, int viewHeight)
		{
			var vec = this;
			vec.X = (vec.X - viewX) / viewWidth;
			vec.Y = (vec.Y - viewY) / viewHeight;
			vec = (vec * 2) - 1;

			vec = vec.Transform(viewProjInverse);
			vec.X /= vec.W;
			vec.Y /= vec.W;
			vec.Z /= vec.W;
			
			return vec;
		}

		public Vector4 UnProject(Matrix4 projectionMatrix, Matrix4 viewMatrix, int viewX, int viewY, int viewWidth, int viewHeight)
		{
			var viewProjInverse = viewMatrix.Multiply(projectionMatrix).Invert();
			
			var vec = this;
			vec.X = (vec.X - viewX) / viewWidth;
			vec.Y = (vec.Y - viewY) / viewHeight;
			vec = (vec * 2) - 1;

			vec = vec.Transform(viewProjInverse);
			vec.X /= vec.W;
			vec.Y /= vec.W;
			vec.Z /= vec.W;
			
			return vec;
		}
		#endregion
	}
}