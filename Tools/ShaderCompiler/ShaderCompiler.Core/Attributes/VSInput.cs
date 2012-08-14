using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public enum VSInputTypes
	{
		Position,
		Color,
		UV,
		Index,
		IndexClassic
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class VSInput : Attribute
	{
		public VSInputTypes Type;
		public int Index;

		public VSInput(VSInputTypes type, int index)
		{
			this.Type = type;
			this.Index = index;
		}
	}
}
