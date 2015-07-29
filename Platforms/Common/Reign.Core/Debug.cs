using System;

namespace Reign.Core
{
	public static class Debug
	{
		private static string getErrorText(string source, string message)
		{
			return string.Format("{0} Error: {1}.", source, message);
		}
		
		public static void ThrowError(string source, string message)
		{
			throw new Exception(getErrorText(source, message));
		}

		public static void PrintError(string source, string message)
		{
			Console.WriteLine(getErrorText(source, message));
		}
	}
}