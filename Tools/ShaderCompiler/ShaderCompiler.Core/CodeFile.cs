using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ShaderCompiler.Core
{
	class CodeFile
	{
		public string Code;
		private string reletivePath;

		public CodeFile(string code)
		{
			Code = code;
		}

		public CodeFile(string code, string reletivePath)
		{
			Code = code;
			this.reletivePath = reletivePath;
		}

		public string FindMethodBlock(Type shader, MethodInfo method, CompilerOutputs outputType)
		{
			// find shaders namespace
			var match = Regex.Match(Code, shader.Namespace + @"\s*?\{(.*\})", RegexOptions.Singleline);
			if (match.Success && match.Groups.Count == 2)
			{
				// find shader class
				var namespaceBlock = match.Groups[1].Value;
				int end = getBlockEnd(namespaceBlock);
				namespaceBlock = namespaceBlock.Remove(end);
				match = Regex.Match(namespaceBlock, @"class\s*" + shader.Name + @".*?\{(.*\})", RegexOptions.Singleline);
				if (match.Success && match.Groups.Count == 2)
				{
					// find method
					var classBlock = match.Groups[1].Value;
					end = getBlockEnd(classBlock);
					classBlock = classBlock.Remove(end);
					match = Regex.Match(classBlock, Compiler.convertToBasicType(method.ReturnType, outputType, false) + @"\s*" + method.Name + @".*?\{(.*?\})", RegexOptions.Singleline);
					if (match.Success && match.Groups.Count == 2)
					{
						var methodBlock = match.Groups[1].Value;
						end = getBlockEnd(methodBlock);
						methodBlock = methodBlock.Remove(end);
						return '{' + methodBlock + '}';
					}
				}
			}

			return null;
		}

		private int getBlockEnd(string code)
		{
			int openBrackes = 1;
			for (int i = 0; i != code.Length; ++i)
			{
				var c = code[i];
				if (c == '{') ++openBrackes;
				if (c == '}') --openBrackes;
				if (openBrackes == 0) return i - 1;
			}

			throw new Exception("Failed to find code block end");
		}
	}
}

