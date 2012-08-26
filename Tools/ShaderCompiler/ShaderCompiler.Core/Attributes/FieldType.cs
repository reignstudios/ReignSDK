using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public enum FieldUsageTypes
	{
		VS,
		PS,
		VS_PS
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class FieldUsage : Attribute
	{
		public FieldUsageTypes Type;

		public FieldUsage(FieldUsageTypes type)
		{
			Type = type;
		}
	}
}
