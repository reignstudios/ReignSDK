using System;

namespace Reign.Core
{
	public static class Debug
	{
		public static void ThrowError(string source, string message)
		{
			throw new Exception(string.Format("{0} Error: {1}.", source, message));
		}
	}
}