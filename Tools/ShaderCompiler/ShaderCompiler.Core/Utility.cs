using System;
using System.Text.RegularExpressions;

namespace ShaderCompiler.Core
{
	public static class Utility
	{
		public static string RemoveKeyword(string code, string keyword)
		{
			return Regex.Replace(code, @"\s*\b" + keyword + @"\b\s*", " ");
		}
		
		public static string ReplaceKeyword(string code, string keyword, string replaceKeyword)
		{
			return Regex.Replace(code, @"\b" + keyword + @"\b", replaceKeyword);
		}
	}
}

