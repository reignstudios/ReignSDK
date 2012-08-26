using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public enum MaterialTypes
	{
		Camera,
		Diffuse,
		Specular,
		Emission,
		Shininess,
		IndexOfRefraction
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class MaterialType : Attribute
	{
		public MaterialTypes Type;

		public MaterialType(MaterialTypes type)
		{
			Type = type;
		}
	}
}
