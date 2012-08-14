using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public enum VSOutputPSInputTypes
	{
		Position,
		InOut
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class VSOutputPSInput : Attribute
	{
		public VSOutputPSInputTypes Type;
		public int Index;

		public VSOutputPSInput(VSOutputPSInputTypes type, int index)
		{
			this.Type = type;
			this.Index = index;
		}
	}
}
