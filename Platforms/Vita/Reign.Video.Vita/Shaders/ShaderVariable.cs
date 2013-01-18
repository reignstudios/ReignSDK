using System;
using Reign.Core;
using System.Runtime.InteropServices;
using Sce.PlayStation.Core.Graphics;
using C = Sce.PlayStation.Core;

namespace Reign.Video.Vita
{
	public class ShaderVariable : ShaderVariableI
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct ValueObject
		{
			[FieldOffset(0)]
			public float X;

			[FieldOffset(4)]
			public float Y;

			[FieldOffset(8)]
			public float Z;

			[FieldOffset(12)]
			public float W;

			[FieldOffset(0)]
			public Vector2 Vector2;

			[FieldOffset(0)]
			public Vector3 Vector3;

			[FieldOffset(0)]
			public Vector4 Vector4;

			[FieldOffset(0)]
			public Matrix2 Matrix2;

			[FieldOffset(0)]
			public Matrix3 Matrix3;

			[FieldOffset(0)]
			public Matrix4 Matrix4;
		}

		#region Properties
		internal delegate void ApplyFunc();
		internal ApplyFunc Apply;
		private ValueObject valueObject;
		private WeakReference valueArrayObject;
		private int valueArrayOffset, valueArrayCount;
		
		private ShaderProgram program;
		private int index;
		public string Name {get; private set;}
		#endregion

		#region Constructors
		public ShaderVariable(ShaderProgram program, int index, string name)
		{
			this.program = program;
			this.index = index;
			this.Name = name;

			Apply = setNothing;
			valueArrayObject = new WeakReference(null);
		}
		#endregion

		#region Methods
		private void setNothing()
		{
			// Place holder.
		}

		private void setFloat()
		{
			program.SetUniformValue(index, valueObject.X);
		}

		private void setVector2()
		{
			C.Vector2 vector;
			vector.X = valueObject.Vector2.X;
			vector.Y = valueObject.Vector2.Y;
			program.SetUniformValue(index, ref vector);
		}

		private void setVector3()
		{
			C.Vector3 vector;
			vector.X = valueObject.Vector3.X;
			vector.Y = valueObject.Vector3.Y;
			vector.Z = valueObject.Vector3.Z;
			program.SetUniformValue(index, ref vector);
		}

		private void setVector4()
		{
			C.Vector4 vector;
			vector.X = valueObject.Vector4.X;
			vector.Y = valueObject.Vector4.Y;
			vector.Z = valueObject.Vector4.Z;
			vector.W = valueObject.Vector4.W;
			program.SetUniformValue(index, ref vector);
		}

		private void setMatrix2()
		{
			throw new NotImplementedException();
		}

		private void setMatrix3()
		{
			throw new NotImplementedException();
		}

		private void setMatrix4()
		{
			C.Matrix4 matrix;
			matrix.M11 = valueObject.Matrix4.X.X;
			matrix.M21 = valueObject.Matrix4.X.Y;
			matrix.M31 = valueObject.Matrix4.X.Z;
			matrix.M41 = valueObject.Matrix4.X.W;
			
			matrix.M12 = valueObject.Matrix4.Y.X;
			matrix.M22 = valueObject.Matrix4.Y.Y;
			matrix.M32 = valueObject.Matrix4.Y.Z;
			matrix.M42 = valueObject.Matrix4.Y.W;
			
			matrix.M13 = valueObject.Matrix4.Z.X;
			matrix.M23 = valueObject.Matrix4.Z.Y;
			matrix.M33 = valueObject.Matrix4.Z.Z;
			matrix.M43 = valueObject.Matrix4.Z.W;
			
			matrix.M14 = valueObject.Matrix4.W.X;
			matrix.M24 = valueObject.Matrix4.W.Y;
			matrix.M34 = valueObject.Matrix4.W.Z;
			matrix.M44 = valueObject.Matrix4.W.W;
			program.SetUniformValue(index, ref matrix);
		}

		private void setFloatArray()
		{
			throw new NotImplementedException();
		}

		private void setVector4Array()
		{
			throw new NotImplementedException();
		}

		private void setMatrix4Array()
		{
			throw new NotImplementedException();
		}

		public void Set(float value)
		{
			valueObject.X = value;
			Apply = setFloat;
		}

		public void Set(float x, float y)
		{
			valueObject.X = x;
			valueObject.Y = y;
			Apply = setVector2;
		}

		public void Set(float x, float y, float z)
		{
			valueObject.X = x;
			valueObject.Y = y;
			valueObject.Z = z;
			Apply = setVector3;
		}

		public void Set(float x, float y, float z, float w)
		{
			valueObject.X = x;
			valueObject.Y = y;
			valueObject.Z = z;
			valueObject.W = w;
			Apply = setVector4;
		}

		public void Set(Vector2 value)
		{
			valueObject.Vector2 = value;
			Apply = setVector2;
		}

		public void Set(Vector3 value)
		{
			valueObject.Vector3 = value;
			Apply = setVector3;
		}

		public void Set(Vector4 value)
		{
			valueObject.Vector4 = value;
			Apply = setVector4;
		}

		public void Set(Matrix2 value)
		{
			valueObject.Matrix2 = value;
			Apply = setMatrix2;
		}

		public void Set(Matrix3 value)
		{
			valueObject.Matrix3 = value;
			Apply = setMatrix3;
		}

		public void Set(Matrix4 value)
		{
			valueObject.Matrix4 = value;
			Apply = setMatrix4;
		}

		public void Set(float[] values)
		{
			valueArrayOffset = 0;
			valueArrayCount = values.Length;
			valueArrayObject.Target = values;
			Apply = setFloatArray;
		}

		public void Set(Vector2[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values)
		{
			valueArrayOffset = 0;
			valueArrayCount = values.Length;
			valueArrayObject.Target = values;
			Apply = setVector4Array;
		}

		public void Set(Matrix2[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(float[] values, int count)
		{
			valueArrayOffset = 0;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setFloatArray;
		}

		public void Set(Vector2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values, int count)
		{
			valueArrayOffset = 0;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setVector4Array;
		}

		public void Set(Matrix2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4[] values, int count)
		{
			valueArrayOffset = 0;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setMatrix4Array;
		}

		public void Set(float[] values, int offset, int count)
		{
			valueArrayOffset = offset;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setFloatArray;
		}

		public void Set(Vector2[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values, int offset, int count)
		{
			valueArrayOffset = offset;
			valueArrayCount = count;
			valueArrayObject.Target = values;
			Apply = setVector4Array;
		}

		public void Set(Matrix2[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}