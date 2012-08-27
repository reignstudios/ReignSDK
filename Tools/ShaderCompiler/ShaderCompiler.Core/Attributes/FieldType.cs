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

	[AttributeUsage(AttributeTargets.Field)]
	public class FieldUsage : Attribute
	{
		public FieldUsageTypes Type;
		public MaterialTypes MaterialType;

		public FieldUsage(FieldUsageTypes type)
		{
			Type = type;
			MaterialType = MaterialTypes.None;
		}

		public FieldUsage(FieldUsageTypes type, MaterialTypes materialType)
		{
			Type = type;
			MaterialType = materialType;
		}
	}
}
