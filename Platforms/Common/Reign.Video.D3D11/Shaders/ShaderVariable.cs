using System;
using Reign.Core;
using Reign.Video;
using Reign_Video_D3D11_Component;

#if WP8
using System.Runtime.InteropServices;
#endif

namespace Reign.Video.D3D11
{
	public class ShaderVariable : ShaderVariableI
	{
		#region Properties
		private ShaderVariableCom com;
		public string Name {get; private set;}

		#if WP8
		private int vertexOffset, pixelOffset;
		private IntPtr vertexBytes, pixelBytes;
		#endif
		#endregion

		#region Constructors
		public ShaderVariable(string name, ShaderModelCom vertexShader, ShaderModelCom pixelShader, int vertexOffset, int pixelOffset)
		{
			Name = name;
			com = new ShaderVariableCom(vertexShader, pixelShader, vertexOffset, pixelOffset);
			#if WP8
			this.vertexOffset = vertexOffset;
			this.pixelOffset = pixelOffset;
			int vsPtr, psPtr;
			com.GetDataPtrs(out vsPtr, out psPtr);
			vertexBytes = new IntPtr(vsPtr);
			pixelBytes = new IntPtr(psPtr);
			#endif
		}
		#endregion

		#region Methods
		#if WP8
		public void Set(float value)
		{
			if (vertexOffset != -1) Marshal.StructureToPtr(value, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(value, pixelBytes + pixelOffset, false);
		}

		public void Set(float x, float y)
		{
			Vector2 vector;
			vector.X = x;
			vector.Y = y;
			if (vertexOffset != -1) Marshal.StructureToPtr(vector, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(vector, pixelBytes + pixelOffset, false);
		}

		public void Set(float x, float y, float z)
		{
			Vector3 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			if (vertexOffset != -1) Marshal.StructureToPtr(vector, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(vector, pixelBytes + pixelOffset, false);
		}

		public void Set(float x, float y, float z, float w)
		{
			Vector4 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			vector.W = w;
			if (vertexOffset != -1) Marshal.StructureToPtr(vector, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(vector, pixelBytes + pixelOffset, false);
		}

		public void Set(Vector2 value)
		{
			if (vertexOffset != -1) Marshal.StructureToPtr(value, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(value, pixelBytes + pixelOffset, false);
		}

		public void Set(Vector3 value)
		{
			if (vertexOffset != -1) Marshal.StructureToPtr(value, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(value, pixelBytes + pixelOffset, false);
		}

		public void Set(Vector4 value)
		{
			if (vertexOffset != -1) Marshal.StructureToPtr(value, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(value, pixelBytes + pixelOffset, false);
		}

		public void Set(Matrix2 value)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3 value)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix4 value)
		{
			if (vertexOffset != -1) Marshal.StructureToPtr(value, vertexBytes + vertexOffset, false);
			if (pixelOffset != -1) Marshal.StructureToPtr(value, pixelBytes + pixelOffset, false);
		}

		public void Set(float[] values)
		{
			if (vertexOffset != -1) Marshal.Copy(values, 0, vertexBytes + vertexOffset, values.Length);
			if (pixelOffset != -1) Marshal.Copy(values, 0, pixelBytes + pixelOffset, values.Length);
		}

		public void Set(Vector2[] values)
		{
			int size = Marshal.SizeOf(values[0]);
			if (vertexOffset != -1)
			{
				int offset = vertexOffset;
				for (int i = 0; i != values.Length; ++i)
				{
					Marshal.StructureToPtr(values[i], vertexBytes + offset, false);
					offset += size;
				}
			}
			if (pixelOffset != -1)
			{
				int offset = pixelOffset;
				for (int i = 0; i != values.Length; ++i)
				{
					Marshal.StructureToPtr(values[i], pixelBytes + offset, false);
					offset += size;
				}
			}
		}

		public void Set(Vector3[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values)
		{
			int size = Marshal.SizeOf(values[0]);
			if (vertexOffset != -1)
			{
				int offset = vertexOffset;
				for (int i = 0; i != values.Length; ++i)
				{
					Marshal.StructureToPtr(values[i], vertexBytes + offset, false);
					offset += size;
				}
			}
			if (pixelOffset != -1)
			{
				int offset = pixelOffset;
				for (int i = 0; i != values.Length; ++i)
				{
					Marshal.StructureToPtr(values[i], pixelBytes + offset, false);
					offset += size;
				}
			}
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
			throw new NotImplementedException();
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
			int size = Marshal.SizeOf(values[0]);
			if (vertexOffset != -1)
			{
				int offset = vertexOffset;
				for (int i = 0; i != count; ++i)
				{
					Marshal.StructureToPtr(values[i], vertexBytes + offset, false);
					offset += size;
				}
			}
			if (pixelOffset != -1)
			{
				int offset = pixelOffset;
				for (int i = 0; i != count; ++i)
				{
					Marshal.StructureToPtr(values[i], pixelBytes + offset, false);
					offset += size;
				}
			}
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
			int size = Marshal.SizeOf(values[0]);
			if (vertexOffset != -1)
			{
				int offset = vertexOffset;
				for (int i = 0; i != count; ++i)
				{
					Marshal.StructureToPtr(values[i], vertexBytes + offset, false);
					offset += size;
				}
			}
			if (pixelOffset != -1)
			{
				int offset = pixelOffset;
				for (int i = 0; i != count; ++i)
				{
					Marshal.StructureToPtr(values[i], pixelBytes + offset, false);
					offset += size;
				}
			}
		}

		public void Set(float[] values, int offset, int count)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
		#else
		public unsafe void Set(float value)
		{
			com.Set((int)&value, sizeof(float));
		}

		public unsafe void Set(float x, float y)
		{
			Vector2 vector;
			vector.X = x;
			vector.Y = y;
			com.Set((int)&vector, sizeof(Vector2));
		}

		public unsafe void Set(float x, float y, float z)
		{
			Vector3 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			com.Set((int)&vector, sizeof(Vector3));
		}

		public unsafe void Set(float x, float y, float z, float w)
		{
			Vector4 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			vector.W = w;
			com.Set((int)&vector, sizeof(Vector4));
		}

		public unsafe void Set(Vector2 value)
		{
			com.Set((int)&value, sizeof(Vector2));
		}

		public unsafe void Set(Vector3 value)
		{
			com.Set((int)&value, sizeof(Vector3));
		}

		public unsafe void Set(Vector4 value)
		{
			com.Set((int)&value, sizeof(Vector4));
		}

		public void Set(Matrix2 value)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3 value)
		{
			throw new NotImplementedException();
		}

		public unsafe void Set(Matrix4 value)
		{
			com.Set((int)&value, sizeof(Matrix4));
		}

		public unsafe void Set(float[] values)
		{
			fixed (float* data = values) com.Set((int)data, sizeof(float), values.Length, 16);
		}

		public unsafe void Set(Vector2[] values)
		{
			fixed (Vector2* data = values) com.Set((int)data, sizeof(Vector2), values.Length, 16);
		}

		public void Set(Vector3[] values)
		{
			throw new NotImplementedException();
		}

		public unsafe void Set(Vector4[] values)
		{
			fixed (Vector4* data = values) com.Set((int)data, sizeof(Vector4) * values.Length);
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
			throw new NotImplementedException();
		}

		public void Set(Vector2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int count)
		{
			throw new NotImplementedException();
		}

		public unsafe void Set(Vector4[] values, int count)
		{
			fixed (Vector4* data = values) com.Set((int)data, sizeof(Vector4) * count);
		}

		public void Set(Matrix2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int count)
		{
			throw new NotImplementedException();
		}

		public unsafe void Set(Matrix4[] values, int count)
		{
			fixed (Matrix4* data = values) com.Set((int)data, sizeof(Matrix4) * count);
		}

		public void Set(float[] values, int offset, int count)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
		#endif
		#endregion
	}
}
