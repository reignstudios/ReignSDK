using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public enum PSOutputTypes
	{
		Color
	}

	[AttributeUsage(AttributeTargets.Field)]
	public class PSOutput : Attribute
	{
		public PSOutputTypes Type;
		public int Index;

		public PSOutput(PSOutputTypes type, int index)
		{
			this.Type = type;
			this.Index = index;
		}
	}
}
