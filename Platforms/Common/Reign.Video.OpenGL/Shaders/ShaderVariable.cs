using System;
using Reign.Core;
using System.Runtime.InteropServices;

namespace Reign.Video.OpenGL
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

		private int location;
		public string Name {get; private set;}
		#endregion

		#region Constructors
		public ShaderVariable(int location, string name)
		{
			this.location = location;
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
			GL.Uniform1f(location, valueObject.X);
		}

		private void setVector2()
		{
			GL.Uniform2f(location, valueObject.X, valueObject.Y);
		}

		private void setVector3()
		{
			GL.Uniform3f(location, valueObject.X, valueObject.Y, valueObject.Z);
		}

		private void setVector4()
		{
			GL.Uniform4f(location, valueObject.X, valueObject.Y, valueObject.Z, valueObject.W);
		}

		private unsafe void setMatrix2()
		{
			fixed (void* ptr = &valueObject.Matrix2)
			{
				GL.UniformMatrix2fv(location, 1, false, (float*)ptr);
			}
		}

		private unsafe void setMatrix3()
		{
			fixed (void* ptr = &valueObject.Matrix3)
			{
				GL.UniformMatrix3fv(location, 1, false, (float*)ptr);
			}
		}

		private unsafe void setMatrix4()
		{
			fixed (void* ptr = &valueObject.Matrix4)
			{
				GL.UniformMatrix4fv(location, 1, false, (float*)ptr);
			}
		}

		private unsafe void setFloatArray()
		{
			fixed (void* ptr = ((float[])valueArrayObject.Target))
			{
				GL.Uniform1fv(location, valueArrayCount, (float*)ptr + valueArrayOffset);
			}
		}

		private unsafe void setVector4Array()
		{
			fixed (void* ptr = ((Vector4[])valueArrayObject.Target))
			{
				GL.Uniform4fv(location, valueArrayCount, (float*)ptr + (valueArrayOffset * 4));
			}
		}

		private unsafe void setMatrix4Array()
		{
			fixed (void* ptr = ((Matrix4[])valueArrayObject.Target))
			{
				GL.UniformMatrix4fv(location, valueArrayCount, false, (float*)ptr + (valueArrayOffset * 12));
			}
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