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

	public enum MaterialUsages
	{
		Global,
		Instance,
		Instancing
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class FieldUsage : Attribute
	{
		public FieldUsageTypes Type;
		public MaterialUsages MaterialUsages;

		public FieldUsage(FieldUsageTypes type, MaterialUsages materialUsage)
		{
			Type = type;
			MaterialUsages = materialUsage;
		}
	}
}
