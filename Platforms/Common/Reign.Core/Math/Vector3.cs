using System.Runtime.InteropServices;
using MathS = System.Math;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3
	{
		#region Properties
		public float X, Y, Z;
		private float w;
		#endregion

		#region Constructors
		public Vector3(float value)
		{
			X = value;
			Y = value;
			Z = value;
			w = 0;
		}

		public Vector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
			w = 0;
		}

		public Vector3(Vector2 vector, float z)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
			w = 0;
		}
		#endregion

		#region Operators
		// +
		public static Vector3 operator+(Vector3 p1, Vector3 p2) {return new Vector3(p1.X+p2.X, p1.Y+p2.Y, p1.Z+p2.Z);}
		public static Vector3 operator+(Vector3 p1, float p2) {return new Vector3(p1.X+p2, p1.Y+p2, p1.Z+p2);}
		public static Vector3 operator+(float p1, Vector3 p2) {return new Vector3(p1+p2.X, p1+p2.Y, p1+p2.Z);}
		// -
		public static Vector3 operator-(Vector3 p1, Vector3 p2) {return new Vector3(p1.X-p2.X, p1.Y-p2.Y, p1.Z-p2.Z);}
		public static Vector3 operator-(Vector3 p1, float p2) {return new Vector3(p1.X-p2, p1.Y-p2, p1.Z-p2);}
		public static Vector3 operator-(float p1, Vector3 p2) {return new Vector3(p1-p2.X, p1-p2.Y, p1-p2.Z);}
		public static Vector3 operator-(Vector3 p1) {return new Vector3(-p1.X, -p1.Y, -p1.Z);}
		// *
		public static Vector3 operator*(Vector3 p1, Vector3 p2) {return new Vector3(p1.X*p2.X, p1.Y*p2.Y, p1.Z*p2.Z);}
		public static Vector3 operator*(Vector3 p1, float p2) {return new Vector3(p1.X*p2, p1.Y*p2, p1.Z*p2);}
		public static Vector3 operator*(float p1, Vector3 p2) {return new Vector3(p1*p2.X, p1*p2.Y, p1*p2.Z);}
		// /
		public static Vector3 operator/(Vector3 p1, Vector3 p2) {return new Vector3(p1.X/p2.X, p1.Y/p2.Y, p1.Z/p2.Z);}
		public static Vector3 operator/(Vector3 p1, float p2) {return new Vector3(p1.X/p2, p1.Y/p2, p1.Z/p2);}
		public static Vector3 operator/(float p1, Vector3 p2) {return new Vector3(p1/p2.X, p1/p2.Y, p1/p2.Z);}
		// ==
		public static bool operator==(Vector3 p1, Vector3 p2) {return (p1.X==p2.X && p1.Y==p2.Y && p1.Z==p2.Z);}
		public static bool operator!=(Vector3 p1, Vector3 p2) {return (p1.X!=p2.X || p1.Y!=p2.Y || p1.Z!=p2.Z);}
		// convert
		public Vector2 ToVector2() {return new Vector2(X, Y);}
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
			return new Vector3(Math.DegToRad(X), Math.DegToRad(Y), Math.DegToRad(Z));
		}

		public Vector3 RadToDeg()
		{
			return new Vector3(Math.RadToDeg(X), Math.RadToDeg(Y), Math.RadToDeg(Z));
		}

		public Vector3 Max(float value)
		{
			return new Vector3(MathS.Max(X, value), MathS.Max(Y, value), MathS.Max(Z, value));
		}

		public Vector3 Min(float value)
		{
			return new Vector3(MathS.Min(X, value), MathS.Min(Y, value), MathS.Min(Z, value));
		}

		public Vector3 Abs()
		{
			return new Vector3(MathS.Abs(X), MathS.Abs(Y), MathS.Abs(Z));
		}

		public Vector3 Pow(float value)
		{
			return new Vector3((float)MathS.Pow(X, value), (float)MathS.Pow(Y, value), (float)MathS.Pow(Z, value));
		}

		public Vector3 Floor()
		{
			return new Vector3((float)MathS.Floor(X), (float)MathS.Floor(Y), (float)MathS.Floor(Z));
		}

		public float Dot()
		{
			return (X*X) + (Y*Y) + (Z*Z);
		}

		public float Dot(Vector3 vector)
		{
			return (X*vector.X) + (Y*vector.Y) + (Z*vector.Z);
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
			return (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z));
			#endif
		}

		public Vector3 Normalize()
		{
			#if SIMD
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			return input * factor.InvSqrt();
			#else
			return this * (1 / (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z)));
			#endif
		}

		public Vector3 Normalize(out float length)
		{
			#if SIMD
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			factor = factor.InvSqrt();
			length = (factor * factor).X;
			return input * factor;
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z));
			length = dis;
			return this * (1/dis);
			#endif
		}

		public Vector3 NormalizeSafe()
		{
			#if SIMD
			if (input.X == 0 && input.Y == 0 && input.Z == 0 && input.W == 0)
			{
				return new Vector3();
			}
			
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			return vector * factor.InvSqrt();
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z));
			if (dis == 0) return new Vector3();
			else return this * (1/dis);
			#endif
		}

		public Vector3 NormalizeSafe(out float distance)
		{
			#if SIMD
			if (input.X == 0 && input.Y == 0 && input.Z == 0) return new Vector3();
			var factor = input * input;
			factor = factor.HorizontalAdd(factor);
			factor = factor.HorizontalAdd(factor);
			factor = factor.InvSqrt();
			length = (factor * factor).X;
			return input * factor;
			#else
			float dis = (float)MathS.Sqrt((X*X) + (Y*Y) + (Z*Z));
			distance = dis;
			if (dis == 0) return new Vector3();
			else return this * (1/dis);
			#endif
		}

		public Vector3 Cross(Vector3 vector)
		{
			return new Vector3(((Y*vector.Z) - (Z*vector.Y)), ((Z*vector.X) - (X*vector.Z)), ((X*vector.Y) - (Y*vector.X)));
		}

		public Vector3 Transform(Matrix3 matrix)
		{
		    #if SIMD
		    return
		        (matrix.X * input.Shuffle(ShuffleSel.ExpandX)) +
		        (matrix.Y * input.Shuffle(ShuffleSel.ExpandY)) +
		        (matrix.Z * input.Shuffle(ShuffleSel.ExpandZ));
		    #else
		    return (matrix.X*X) + (matrix.Y*Y) + (matrix.Z*Z);
		    #endif
		}

		public bool AproxEqualsBox(Vector3 vector, float tolerance)
		{
			return
				(MathS.Abs(X-vector.X) <= tolerance) &&
				(MathS.Abs(Y-vector.Y) <= tolerance) &&
				(MathS.Abs(Z-vector.Z) <= tolerance);
		}

		public bool ApproxEquals(Vector3 vector, float tolerance)
		{
		    return (Distance(vector) <= tolerance);
		}

		public float Distance(Vector3 vector)
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
		
		public float DistanceSquared(Vector3 vector)
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

		public Vector3 Rotate(Vector3 vectorNormalized, float radians)
		{
			throw new System.NotImplementedException();

			var y = new Vector3();
			/*if (pVectorNormalized.Dot() != 0)
			{
				y = this.InersectRay(pVectorNormalized) - this;
				if (y.Dot() != 0)
				{
					Matrix3f Matrix = new Matrix3f(pVectorNormalized, y);
					Matrix3f MatrixTran = Matrix.Transpose();
					Matrix = Matrix.RotateAroundAxisZ(pRadians);
					y = this.Transform(MatrixTran);
					y = y.Transform(Matrix);
				}
			}*/
			return y;
		}

		public bool WithinTriangle(Triangle triangle)
		{      
			var v0 = triangle.P2 - triangle.P1;
			var v1 = triangle.P3 - triangle.P1;
			var v2 = this - triangle.P1;
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

		public Vector3 Reflect(Vector3 planeNormal)
		{
			return this - (planeNormal * this.Dot(planeNormal) * 2);
		}

		public Vector3 InersectNormal3(Vector3 normal)
		{
			return (normal * this.Dot(normal));
		}

		public Vector3 InersectRay3(Vector3 rayLocation, Vector3 rayNormal)
		{
			return (rayNormal * (this-rayLocation).Dot(rayNormal)) + rayLocation;
		}

		public Vector3 InersectLine(Line3 line)
		{
			Vector3 pointOffset = (this-line.P1), vector = (line.P2-line.P1).Normalize();
			return (vector * pointOffset.Dot(vector)) + line.P1;
		}

		public Vector3 InersectPlane3(Vector3 planeNormal, Vector3 planeLocation)
		{
			return this - (planeNormal * (this-planeLocation).Dot(planeNormal));
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
			return (float)MathS.Acos(val);
		}
		#endregion
	}
}