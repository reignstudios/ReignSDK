using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public struct Matrix3
	{
		public Vector3 x, y, z;

		public Matrix3 (Vector3 x, Vector3 y, Vector3 z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3 Multiply(Vector3 vector)
		{
			return vector;
		}

		internal static string Output(CompilerOutputs output)
		{
			var baseType = Compiler.getBaseCompilerOutput(output);
			if (baseType == BaseCompilerOutputs.HLSL) return "float3x3";
			if (baseType == BaseCompilerOutputs.GLSL) return "mat3";

			throw new Exception("Matrix3 - Unsuported platform.");
		}
	}
}
