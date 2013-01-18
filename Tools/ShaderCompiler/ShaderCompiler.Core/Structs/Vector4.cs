using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public class Vector4
	{
		#region Properties
		public double x, y, z, w;
		
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
		
		public double b
		{
			get {return z;}
			set {z = value;}
		}
		
		public double a
		{
			get {return w;}
			set {w = value;}
		}
		
		public Vector2 xy
		{
			get {return new Vector2(x, y);}
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		public Vector3 xyz
		{
			get {return new Vector3(x, y, z);}
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
			}
		}
		
		public Vector3 rgb
		{
			get {return new Vector3(x, y, z);}
			set
			{
				this.x = value.x;
				this.y = value.y;
				this.z = value.z;
			}
		}

		public double this[int i]
		{
			get {return 0;}
			set {}
		}
		#endregion
		
		#region Constructors
		public Vector4() {}

		public Vector4(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4(Vector2 xy, double z, double w)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
			this.w = w;
		}
		
		public Vector4(Vector3 xyz, double w)
		{
			this.x = xyz.x;
			this.y = xyz.y;
			this.z = xyz.z;
			this.w = w;
		}
		#endregion

		#region Operators
		// +
		public static Vector4 operator+(Vector4 p1, Vector4 p2) {return new Vector4(p1.x+p2.x, p1.y+p2.y, p1.z+p2.z, p1.w+p2.w);}
		public static Vector4 operator+(Vector4 p1, double p2) {return new Vector4(p1.x+p2, p1.y+p2, p1.z+p2, p1.w+p2);}
		public static Vector4 operator+(double p1, Vector4 p2) {return new Vector4(p1+p2.x, p1+p2.y, p1+p2.z, p1+p2.w);}
		// -
		public static Vector4 operator-(Vector4 p1, Vector4 p2) {return new Vector4(p1.x-p2.x, p1.y-p2.y, p1.z-p2.z, p1.w-p2.w);}
		public static Vector4 operator-(Vector4 p1, double p2) {return new Vector4(p1.x-p2, p1.y-p2, p1.z-p2, p1.w-p2);}
		public static Vector4 operator-(double p1, Vector4 p2) {return new Vector4(p1-p2.x, p1-p2.y, p1-p2.z, p1-p2.w);}
		public static Vector4 operator-(Vector4 p1) {return new Vector4(-p1.x, -p1.y, -p1.z, p1.w);}
		// *
		public static Vector4 operator*(Vector4 p1, Vector4 p2) {return new Vector4(p1.x*p2.x, p1.y*p2.y, p1.z*p2.z, p1.w*p2.w);}
		public static Vector4 operator*(Vector4 p1, double p2) {return new Vector4(p1.x*p2, p1.y*p2, p1.z*p2, p1.w*p2);}
		public static Vector4 operator*(double p1, Vector4 p2) {return new Vector4(p1*p2.x, p1*p2.y, p1*p2.z, p1*p2.w);}
		// /
		public static Vector4 operator/(Vector4 p1, Vector4 p2) {return new Vector4(p1.x/p2.x, p1.y/p2.y, p1.z/p2.z, p1.w/p2.w);}
		public static Vector4 operator/(Vector4 p1, double p2) {return new Vector4(p1.x/p2, p1.y/p2, p1.z/p2, p1.w/p2);}
		public static Vector4 operator/(double p1, Vector4 p2) {return new Vector4(p1/p2.x, p1/p2.y, p1/p2.z, p1/p2.w);}
		// ==
		//public static bool operator==(Vector4 p1, Vector4 p2) {return (p1.x==p2.x && p1.y==p2.y && p1.z==p2.z && p1.w==p2.w);}
		//public static bool operator!=(Vector4 p1, Vector4 p2) {return (p1.x!=p2.x || p1.y!=p2.y || p1.z!=p2.z || p1.w!=p2.w);}
		#endregion

		internal static string Output(CompilerOutputs output)
		{
			var baseType = Compiler.getBaseCompilerOutput(output);
			if (baseType == BaseCompilerOutputs.HLSL) return "float4";
			if (baseType == BaseCompilerOutputs.GLSL) return "vec4";
			if (baseType == BaseCompilerOutputs.CG) return "float4";

			throw new Exception("Vector4 - Unsuported platform.");
		}
	}
}
