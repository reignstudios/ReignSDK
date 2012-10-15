using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public class Matrix4
	{
		public Vector4 x, y, z, w;

		public Matrix4() {}

		public Matrix4 (Vector4 x, Vector4 y, Vector4 z, Vector4 w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4 this[int i]
		{
			get {return new Vector4();}
			set {}
		}

		public Vector4 Multiply(Vector4 vector)
		{
			return vector;
		}

		public Matrix4 Multiply(Matrix4 matrix)
		{
			return matrix;
		}

		public Vector4 MultiplyInvert(Vector4 vector)
		{
			return vector;
		}

		public Matrix4 MultiplyInvert(Matrix4 matrix)
		{
			return matrix;
		}

		internal static string Output(CompilerOutputs output)
		{
			var baseType = Compiler.getBaseCompilerOutput(output);
			if (baseType == BaseCompilerOutputs.HLSL) return "float4x4";
			if (baseType == BaseCompilerOutputs.GLSL) return "mat4";

			throw new Exception("Matrix4 - Unsuported platform.");
		}
	}
}
