#if XNA
using X = Microsoft.Xna.Framework;
#endif

namespace Reign.Core
{
	public static class Extensions
	{
		#region Strings and Text
		public static byte[] CastToBytes(this char[] input)
		{
			byte[] output = new byte[input.Length];
			for (int i = 0; i != input.Length; ++i) output[i] = (byte)input[i];
			return output;
		}
		
		public static byte[] CastToBytes(this string input)
		{
			byte[] output = new byte[input.Length];
			for (int i = 0; i != input.Length; ++i) output[i] = (byte)input[i];
			return output;
		}
		#endregion

		#if XNA
		public static X.Matrix ToMatrixX(this Matrix4 matrix)
		{
			return new X.Matrix
			(
				matrix.X.X, matrix.X.Y, matrix.X.Z, matrix.X.W,
				matrix.Y.X, matrix.Y.Y, matrix.Y.Z, matrix.Y.W,
				matrix.Z.X, matrix.Z.Y, matrix.Z.Z, matrix.Z.W,
				matrix.W.X, matrix.W.Y, matrix.W.Z, matrix.W.W
			);
		}

		public static Matrix4 ToMatrix4(this X.Matrix matrix)
		{
			return new Matrix4
			(
				new Vector4(matrix.M11, matrix.M12, matrix.M13, matrix.M14),
				new Vector4(matrix.M21, matrix.M22, matrix.M23, matrix.M24),
				new Vector4(matrix.M31, matrix.M32, matrix.M33, matrix.M34),
				new Vector4(matrix.M41, matrix.M42, matrix.M43, matrix.M44)
			);
		}

		public static X.Vector3 ToVector3X(this Vector3 vector)
		{
			return new X.Vector3(vector.X, vector.Y, vector.Z);
		}
		#endif
	}
}