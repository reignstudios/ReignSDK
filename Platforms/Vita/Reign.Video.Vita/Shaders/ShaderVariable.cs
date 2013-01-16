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

		private unsafe void setVector2()
		{
			fixed (void* ptr = &valueObject.Vector2)
			{
				C.Vector2* vector = (C.Vector2*)ptr;
				program.SetUniformValue(index, ref *vector);
			}
		}

		private unsafe void setVector3()
		{
			fixed (void* ptr = &valueObject.Vector3)
			{
				C.Vector3* vector = (C.Vector3*)ptr;
				program.SetUniformValue(index, ref *vector);
			}
		}

		private unsafe void setVector4()
		{
			fixed (void* ptr = &valueObject.Vector4)
			{
				C.Vector4* vector = (C.Vector4*)ptr;
				program.SetUniformValue(index, ref *vector);
			}
		}

		private void setMatrix2()
		{
			throw new NotImplementedException();
		}

		private void setMatrix3()
		{
			throw new NotImplementedException();
		}

		private unsafe void setMatrix4()
		{
			fixed (void* ptr = &valueObject.Matrix4)
			{
				C.Matrix4* vector = (C.Matrix4*)ptr;
				program.SetUniformValue(index, ref *vector);
			}
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