using System;

namespace Reign.Core
{
	/// <summary>
	/// Provides constants and common mathematical functions.
	/// </summary>
	public static class MathUtilities
	{
		#region Properties
		/// <summary>
        /// Large tolerance value.
        /// </summary>
        public const float BigEpsilon = 1E-5f;

        /// <summary>
        /// Tolerance value.
        /// </summary>
        public const float Epsilon = 1E-7f;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
		/// </summary>
		public const float Pi = 3.1415926535897932384626433832795f;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, multiplied by 2.
		/// </summary>
		public const float Pi2 = 6.283185307179586476925286766559f;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, multiplied by 4.
		/// </summary>
		public const float Pi4 = 12.566370614359172953850573533118f;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 2.
		/// </summary>
		public const float PiHalf = 1.5707963267948966192313216916398f;

		/// <summary>
		/// Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 4.
		/// </summary>
		public const float PiQuarter = 0.78539816339744830961566084581988f;

		/// <summary>
		/// Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π.
		/// </summary>
		public const float PiDelta = 0.31830988618379067153776752674503f;

		/// <summary>
		/// Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 2.
		/// </summary>
		public const float PiHalfDelta = 0.63661977236758134307553505349004f;

		/// <summary>
		/// Represents 1 divided by the ratio of the circumference of a circle to its diameter, specified by the constant, π, divided by 4.
		/// </summary>
		public const float PiQuarterDelta = 1.2732395447351626861510701069801f;
		#endregion

		#region Methods
		/// <summary>
		/// Converts Degrees to Radians.
		/// </summary>
		/// <param name="degrees">Degrees</param>
		/// <returns>Radians</returns>
		public static float DegToRad(float degrees)
		{
			return ((degrees / 180) * Pi);
		}

		public static void DegToRad(float degrees, out float result)
		{
			result = ((degrees / 180) * Pi);
		}

		/// <summary>
		/// Converts Radians to Degrees.
		/// </summary>
		/// <param name="radians">Radians</param>
		/// <returns>Degrees</returns>
		public static float RadToDeg(float radians)
		{
			return ((radians / Pi) * 180);
		}

		public static void RadToDeg(float radians, out float result)
		{
			result = ((radians / Pi) * 180);
		}

		public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            else if (value > max) return max;

            return value;
        }

		public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder(angle, Pi2);
            if (angle < -Pi)
            {
                angle += Pi2;
                return angle;
            }

            if (angle >= Pi)
            {
                angle -= Pi2;
            }

            return angle;
        }
		#endregion
	}
}