using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public class Texture2D
	{
		public Vector4 Sample(Vector2 uv)
		{
			return new Vector4();
		}

		internal static string Output(CompilerOutputs output)
		{
			switch (output)
			{
				case (CompilerOutputs.D3D11): return "texture2D";
				case (CompilerOutputs.D3D9): return "sampler2D";
				case (CompilerOutputs.XNA): return "texture2D";
				case (CompilerOutputs.GL3): return "sampler2D";
				case (CompilerOutputs.GL2): return "sampler2D";
				case (CompilerOutputs.GLES2): return "sampler2D";
				default: throw new Exception("Texture2D - Unsuported platform.");
			}
		}
	}
}
