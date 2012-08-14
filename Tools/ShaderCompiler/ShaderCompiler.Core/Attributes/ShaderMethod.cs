using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public enum ShaderMethodTypes
	{
		VS,
		PS
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ShaderMethod : Attribute
	{
		public ShaderMethodTypes Type;

		public ShaderMethod(ShaderMethodTypes type)
		{
			this.Type = type;
		}
	}
}
