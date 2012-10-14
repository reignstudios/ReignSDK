using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public class Matrix2
	{
		public Vector3 x, y;

		public Matrix2() {}

		public Matrix2 (Vector3 x, Vector3 y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2 this[int i]
		{
			get {return new Vector2();}
			set {}
		}

		public Vector3 Multiply(Vector3 vector)
		{
			return vector;
		}

		internal static string Output(CompilerOutputs output)
		{
			var baseType = Compiler.getBaseCompilerOutput(output);
			if (baseType == BaseCompilerOutputs.HLSL) return "float2x2";
			if (baseType == BaseCompilerOutputs.GLSL) return "mat2";

			throw new Exception("Matrix2 - Unsuported platform.");
		}
	}
}
