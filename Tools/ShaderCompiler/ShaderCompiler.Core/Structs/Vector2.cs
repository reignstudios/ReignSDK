using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public class Vector2
	{
		#region Properties
		public double x, y;
		
		public double r
		{
			get {return x;}
			set {x = value;}
		}
		
		public double g
		{
			get {return y;}
			set {y = value;}
		}

		public double this[int i]
		{
			get {return 0;}
			set {}
		}
		#endregion
		
		#region Constructors
		public Vector2() {}

		public Vector2(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
		#endregion
		
		#region Operators
		// +
		public static Vector2 operator+(Vector2 p1, Vector2 p2) {return new Vector2(p1.x+p2.x, p1.y+p2.y);}
		public static Vector2 operator+(Vector2 p1, double p2) {return new Vector2(p1.x+p2, p1.y+p2);}
		public static Vector2 operator+(double p1, Vector2 p2) {return new Vector2(p1+p2.x, p1+p2.y);}
		// -
		public static Vector2 operator-(Vector2 p1, Vector2 p2) {return new Vector2(p1.x-p2.x, p1.y-p2.y);}
		public static Vector2 operator-(Vector2 p1, double p2) {return new Vector2(p1.x-p2, p1.y-p2);}
		public static Vector2 operator-(double p1, Vector2 p2) {return new Vector2(p1-p2.x, p1-p2.y);}
		public static Vector2 operator-(Vector2 p1) {return new Vector2(-p1.x, -p1.y);}
		// *
		public static Vector2 operator*(Vector2 p1, Vector2 p2) {return new Vector2(p1.x*p2.x, p1.y*p2.y);}
		public static Vector2 operator*(Vector2 p1, double p2) {return new Vector2(p1.x*p2, p1.y*p2);}
		public static Vector2 operator*(double p1, Vector2 p2) {return new Vector2(p1*p2.x, p1*p2.y);}
		// /
		public static Vector2 operator/(Vector2 p1, Vector2 p2) {return new Vector2(p1.x/p2.x, p1.y/p2.y);}
		public static Vector2 operator/(Vector2 p1, double p2) {return new Vector2(p1.x/p2, p1.y/p2);}
		public static Vector2 operator/(double p1, Vector2 p2) {return new Vector2(p1/p2.x, p1/p2.y);}
		// ==
		//public static bool operator==(Vector2 p1, Vector2 p2) {return (p1.x==p2.x && p1.y==p2.y);}
		//public static bool operator!=(Vector2 p1, Vector2 p2) {return (p1.x!=p2.x || p1.y!=p2.y);}
		#endregion

		internal static string Output(CompilerOutputs output)
		{
			var baseType = Compiler.getBaseCompilerOutput(output);
			if (baseType == BaseCompilerOutputs.HLSL) return "float2";
			if (baseType == BaseCompilerOutputs.GLSL) return "vec2";
			if (baseType == BaseCompilerOutputs.CG) return "float2";

			throw new Exception("Vector2 - Unsuported platform.");
		}
	}
}
