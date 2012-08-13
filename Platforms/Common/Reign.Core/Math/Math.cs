using MathS = System.Math;

namespace Reign.Core
{
	/// <summary>
	/// Provides constants and common mathematical functions.
	/// </summary>
	public static class Math
	{
		#region Properties
		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
		/// </summary>
		public const float Pi = (float)3.1415926535897932384626433832795;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, multiplied by 2.
		/// </summary>
		public const float Pi2 = (float)6.283185307179586476925286766559;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 2.
		/// </summary>
		public const float PiHalf = (float)1.5707963267948966192313216916398;

		/// <summary>
		/// Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π.
		/// </summary>
		public const float PiMul = (float)0.31830988618379067153776752674503;
		#endregion

		#region Methods
		/// <summary>
		/// Converts Degrees to Radians.
		/// </summary>
		/// <param name="degrees">Degrees</param>
		/// <returns>Radians</returns>
		public static float DegToRad(float degrees) {return ((degrees / 180) * Pi);}

		/// <summary>
		/// Converts Radians to Degrees.
		/// </summary>
		/// <param name="radians">Radians</param>
		/// <returns>Degrees</returns>
		public static float RadToDeg(float radians) {return ((radians / Pi) * 180);}
		#endregion
	}
}