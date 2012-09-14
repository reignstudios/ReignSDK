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
	}
}

