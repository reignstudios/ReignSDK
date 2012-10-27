using System;
using Reign.Core;
using Reign.Video;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class ShaderVariable : ShaderVariableI
	{
		#region Properties
		private ShaderVariableCom com;
		public string Name {get; private set;}
		#endregion

		#region Constructors
		public ShaderVariable(string name, ShaderModelCom vertexShader, ShaderModelCom pixelShader, int vertexOffset, int pixelOffset)
		{
			Name = name;
			com = new ShaderVariableCom(vertexShader, pixelShader, vertexOffset, pixelOffset);
		}
		#endregion

		#region Methods
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
			var vector = new Vector4(x, y, z, w);
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

		public unsafe void Set(Matrix2 value)
		{
			throw new NotImplementedException();
		}

		public unsafe void Set(Matrix3 value)
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

		public unsafe void Set(Vector4[] values)
		{
			throw new NotImplementedException();
		}

		public unsafe void Set(Matrix4[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(float[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector4[] values, int count)
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

		public void Set(Vector4[] values, int offset, int count)
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
