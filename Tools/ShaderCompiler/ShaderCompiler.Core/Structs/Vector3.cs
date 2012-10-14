using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShaderCompiler.Core
{
	public class Vector3
	{
		#region Properties
		public double x, y, z;
		
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
		
		public Vector2 xy
		{
			get {return new Vector2(x, y);}
			set
			{
				this.x = value.x;
				this.y = value.y;
			}
		}

		public double this[int i]
		{
			get {return 0;}
			set {}
		}
		#endregion
		
		#region Constructors
		public Vector3() {}

		public Vector3(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Vector3(Vector2 xy, double z)
		{
			this.x = xy.x;
			this.y = xy.y;
			this.z = z;
		}
		#endregion

		#region Operators
		// +
		public static Vector3 operator+(Vector3 p1, Vector3 p2) {return new Vector3(p1.x+p2.x, p1.y+p2.y, p1.z+p2.z);}
		public static Vector3 operator+(Vector3 p1, double p2) {return new Vector3(p1.x+p2, p1.y+p2, p1.z+p2);}
		public static Vector3 operator+(double p1, Vector3 p2) {return new Vector3(p1+p2.x, p1+p2.y, p1+p2.z);}
		// -
		public static Vector3 operator-(Vector3 p1, Vector3 p2) {return new Vector3(p1.x-p2.x, p1.y-p2.y, p1.z-p2.z);}
		public static Vector3 operator-(Vector3 p1, double p2) {return new Vector3(p1.x-p2, p1.y-p2, p1.z-p2);}
		public static Vector3 operator-(double p1, Vector3 p2) {return new Vector3(p1-p2.x, p1-p2.y, p1-p2.z);}
		public static Vector3 operator-(Vector3 p1) {return new Vector3(-p1.x, -p1.y, -p1.z);}
		// *
		public static Vector3 operator*(Vector3 p1, Vector3 p2) {return new Vector3(p1.x*p2.x, p1.y*p2.y, p1.z*p2.z);}
		public static Vector3 operator*(Vector3 p1, double p2) {return new Vector3(p1.x*p2, p1.y*p2, p1.z*p2);}
		public static Vector3 operator*(double p1, Vector3 p2) {return new Vector3(p1*p2.x, p1*p2.y, p1*p2.z);}
		// /
		public static Vector3 operator/(Vector3 p1, Vector3 p2) {return new Vector3(p1.x/p2.x, p1.y/p2.y, p1.z/p2.z);}
		public static Vector3 operator/(Vector3 p1, double p2) {return new Vector3(p1.x/p2, p1.y/p2, p1.z/p2);}
		public static Vector3 operator/(double p1, Vector3 p2) {return new Vector3(p1/p2.x, p1/p2.y, p1/p2.z);}
		// ==
		//public static bool operator==(Vector3 p1, Vector3 p2) {return (p1.x==p2.x && p1.y==p2.y && p1.z==p2.z);}
		//public static bool operator!=(Vector3 p1, Vector3 p2) {return (p1.x!=p2.x || p1.y!=p2.y || p1.z!=p2.z);}
		#endregion

		internal static string Output(CompilerOutputs output)
		{
			var baseType = Compiler.getBaseCompilerOutput(output);
			if (baseType == BaseCompilerOutputs.HLSL) return "float3";
			if (baseType == BaseCompilerOutputs.GLSL) return "vec3";

			throw new Exception("Vector3 - Unsuported platform.");
		}
	}
}
