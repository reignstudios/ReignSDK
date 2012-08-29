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

	public enum MaterialTypes
	{
		None,
		Diffuse,
		Specular,
		Emission,
		Shininess,
		IndexOfRefraction
	}

	public enum MaterialUsages
	{
		None,
		Global,
		Instance,
		Instancing
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class FieldUsage : Attribute
	{
		public FieldUsageTypes Type;
		public MaterialTypes MaterialType;
		public MaterialUsages MaterialUsages;

		public FieldUsage(FieldUsageTypes type)
		{
			Type = type;
			MaterialType = MaterialTypes.None;
			MaterialUsages = MaterialUsages.None;
		}

		public FieldUsage(FieldUsageTypes type, MaterialUsages materialUsage)
		{
			Type = type;
			MaterialType = MaterialTypes.None;
			MaterialUsages = materialUsage;
		}

		public FieldUsage(FieldUsageTypes type, MaterialTypes materialType, MaterialUsages materialUsage)
		{
			Type = type;
			MaterialType = materialType;
			MaterialUsages = materialUsage;
		}
	}
}
