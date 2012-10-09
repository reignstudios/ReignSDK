using System;

namespace ShaderCompiler.Core
{
	public static class SL
	{
		public static double Distance(Vector2 vector, Vector2 vector2)
		{
			var dis = vector - vector2;
			return Math.Sqrt((dis.x*dis.x) + (dis.y*dis.y));
		}
		
		public static double Distance(Vector3 vector, Vector3 vector2)
		{
			var dis = vector - vector2;
			return Math.Sqrt((dis.x*dis.x) + (dis.y*dis.y) + (dis.z*dis.z));
		}
		
		public static double Distance(Vector4 vector, Vector4 vector2)
		{
			var dis = vector - vector2;
			return Math.Sqrt((dis.x*dis.x) + (dis.y*dis.y) + (dis.z*dis.z) + (dis.w*dis.w));
		}

		public static double Dot(Vector2 vector, Vector2 vector2)
		{
			return 0;
		}

		public static double Dot(Vector3 vector, Vector3 vector2)
		{
			return 0;
		}

		public static double Dot(Vector4 vector, Vector4 vector2)
		{
			return 0;
		}
		
		public static void Clip(double value)
		{
			// Do nothing
		}
		
		public static Vector3 Normalize(Vector3 value)
		{
			throw new NotImplementedException();
		}
		
		public static Vector3 Cross(Vector3 vector, Vector3 vector2)
		{
			throw new NotImplementedException();
		}

		public static double Sin(double value)
		{
			throw new NotImplementedException();
		}

		public static double Cos(double value)
		{
			throw new NotImplementedException();
		}

		public static double Max(double value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector2 Max(Vector2 value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector3 Max(Vector3 value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector4 Max(Vector4 value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector2 Max(Vector2 value, Vector2 value2)
		{
			throw new NotImplementedException();
		}

		public static Vector3 Max(Vector3 value, Vector3 value2)
		{
			throw new NotImplementedException();
		}

		public static Vector4 Max(Vector4 value, Vector4 value2)
		{
			throw new NotImplementedException();
		}

		public static double Min(double value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector2 Min(Vector2 value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector3 Min(Vector3 value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector4 Min(Vector4 value, double value2)
		{
			throw new NotImplementedException();
		}

		public static Vector2 Min(Vector2 value, Vector2 value2)
		{
			throw new NotImplementedException();
		}

		public static Vector3 Min(Vector3 value, Vector3 value2)
		{
			throw new NotImplementedException();
		}

		public static Vector4 Min(Vector4 value, Vector4 value2)
		{
			throw new NotImplementedException();
		}

		public static double DDX(double value)
		{
			throw new NotImplementedException();
		}

		public static Vector2 DDX(Vector2 value)
		{
			throw new NotImplementedException();
		}

		public static Vector3 DDX(Vector3 value)
		{
			throw new NotImplementedException();
		}

		public static Vector4 DDX(Vector4 value)
		{
			throw new NotImplementedException();
		}

		public static double DDY(double value)
		{
			throw new NotImplementedException();
		}

		public static Vector2 DDY(Vector2 value)
		{
			throw new NotImplementedException();
		}

		public static Vector3 DDY(Vector3 value)
		{
			throw new NotImplementedException();
		}

		public static Vector4 DDY(Vector4 value)
		{
			throw new NotImplementedException();
		}
	}
}

