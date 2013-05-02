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
	
	public enum ShaderMethodPrecisions
	{
		High,
		Med,
		Low
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class ShaderMethod : Attribute
	{
		public ShaderMethodTypes Type;
		public ShaderMethodPrecisions Precision;

		public ShaderMethod(ShaderMethodTypes type)
		{
			this.Type = type;
			if (type == ShaderMethodTypes.VS) Precision = ShaderMethodPrecisions.High;
			else Precision = ShaderMethodPrecisions.Low;
		}
		
		public ShaderMethod(ShaderMethodTypes type, ShaderMethodPrecisions precision)
		{
			this.Type = type;
			this.Precision = precision;
		}
	}
}
