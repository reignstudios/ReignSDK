using System;

namespace ShaderCompiler.Core
{
	public class ArrayType : Attribute
	{
		public int Length;
	
		public ArrayType(int length)
		{
			Length = length;
		}
	}
}

