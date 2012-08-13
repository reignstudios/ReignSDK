using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ShaderCompiler.Core
{
	class CodeFile
	{
		public string Code
		{
			get {return code;}
			set {code = value;}
		}
		
		private string code, reletivePath;
		private CompilationUnit compilationUnit;
		
		public CodeFile(string code)
		{
			Code = code;
			
			var parser = new CSharpParser();
			compilationUnit = parser.Parse(new StringReader(code), reletivePath);
		}
	
		public CodeFile(string code, string reletivePath, CSharpParser parser)
		{
			this.code = code;
			this.reletivePath = reletivePath;
			
			compilationUnit = parser.Parse(new StringReader(code), reletivePath);
		}
		
		public string FindMethodBlock(Type shader, MethodInfo method)
		{
			foreach (var child in compilationUnit.Children)
			{
				var methodBlock = findMethod(child, shader, method);
				if (methodBlock != null)
				{
					string codeBlock = null;
					foreach (var block in methodBlock.Children)
					{
						var blockStatement = block as BlockStatement;
						if (blockStatement != null)
						{
							var start = blockStatement.StartLocation;
							var end = blockStatement.EndLocation;
							int startIndex = findLine(start.Line, start.Column);
							int endIndex = findLine(end.Line, end.Column);
							return clipCodeBlock(startIndex, endIndex);
						}
					}
					
					return codeBlock;
				}
			}
			
			return null;
		}
		
		private int findLine(int line, int column)
		{
			var indexList = new List<int>();
			int lastLine = 0;
			char lastC = (char)0;
			for (int i = 0; i != code.Length; ++i)
			{
				char c = code[i];
				if (lastC == '\r' || c == '\n')
				{
					indexList.Add(lastLine);
					lastLine = i;
				}
				
				lastC = c;
			}
			
			return indexList[line-1] + column;
		}
		
		private string clipCodeBlock(int startIndex, int endIndex)
		{
			return code.Substring(startIndex, endIndex-startIndex);
		}
		
		private MethodDeclaration findMethod(AstNode node, Type shader, MethodInfo method)
		{
			var classNode = node as TypeDeclaration;
			if (classNode != null)
			{
				var baseShader = shader;
				while (baseShader != null)
				{
					// Find Shader
					if (classNode.ClassType == ClassType.Class && classNode.Name == baseShader.Name)
					{
						foreach (var classChild in classNode.Children)
						{
							// Find Method
							var methodNode = classChild as MethodDeclaration;
							if (methodNode != null && methodNode.Name == method.Name)
							{
								// Create method parameter list
								var parmList = new List<string>();
								foreach (var methodNodeChild in methodNode.Children)
								{
									var parm = methodNodeChild as ParameterDeclaration;
									if (parm != null)
									{
										foreach (var methodNodeChildParm in methodNodeChild.Children)
										{
											var parmType = methodNodeChildParm as PrimitiveType;
											if (parmType != null)
											{
												parmList.Add(parmType.KnownTypeCode.ToString());
											}
										}
									}
								}
							
								// Check if methods match
								var methodParms = method.GetParameters();
								if (parmList.Count == methodParms.Length)
								{
									bool pass = true;
									for (int i = 0; i != parmList.Count; ++i)
									{
										if (parmList[i] != methodParms[i].ParameterType.Name)
										{
											pass = false;
											break;
										}
									}
								
									// If methods match, return this method
									if (pass) return methodNode;
								}
							}
						}
					}
					
					baseShader = baseShader.BaseType;
				}
			}
		
			// If no methods found in current node, search nodes childeren
			foreach (var child in node.Children)
			{
				var block = findMethod(child, shader, method);
				if (block != null) return block;
			}
		
			return null;
		}
	}
}

