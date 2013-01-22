using System;
using Microsoft.Xna.Framework.Graphics;
using X = Microsoft.Xna.Framework;
using Reign.Core;

namespace Reign.Video.XNA
{
	public class ShaderVariable : ShaderVariableI
	{
		#region Properties
		#if SILVERLIGHT
		private Video video;
		private int vsRegisterIndex, psRegisterIndex;
		#else
		private EffectParameter parameter;
		#endif
		public string Name {get; private set;}
		#endregion

		#region Constructors
		#if SILVERLIGHT
		public ShaderVariable(Video video, int vsRegisterIndex, int psRegisterIndex, string name)
		{
			this.video = video;
			this.vsRegisterIndex = vsRegisterIndex;
			this.psRegisterIndex = psRegisterIndex;
			this.Name = name;
		}
		#else
		public ShaderVariable(EffectParameter parameter, string name)
		{
			this.parameter = parameter;
			this.Name = name;
		}
		#endif
		#endregion

		#region Methods
		public void Set(float value)
		{
			#if SILVERLIGHT
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<float>(vsRegisterIndex, ref value);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<float>(psRegisterIndex, ref value);
			#else
			parameter.SetValue(value);
			#endif
		}

		#if SILVERLIGHT
		public void Set(float x, float y)
		{
			Vector2 vector;
			vector.X = x;
			vector.Y = y;
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Vector2>(vsRegisterIndex, ref vector);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Vector2>(psRegisterIndex, ref vector);
		}
		#else
		public void Set(float x, float y)
		{
			#if XBOX360
			parameter.SetValue(new X.Vector2(x, y));
			#else
			X.Vector2 vector;
			vector.X = x;
			vector.Y = y;
			parameter.SetValue(vector);
			#endif
		}
		#endif

		#if SILVERLIGHT
		public void Set(float x, float y, float z)
		{
			Vector3 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Vector3>(vsRegisterIndex, ref vector);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Vector3>(psRegisterIndex, ref vector);
		}
		#else
		public void Set(float x, float y, float z)
		{
			#if XBOX360
			parameter.SetValue(new X.Vector3(x, y, z));
			#else
			X.Vector3 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			parameter.SetValue(vector);
			#endif
		}
		#endif

		#if SILVERLIGHT
		public void Set(float x, float y, float z, float w)
		{
			Vector4 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			vector.W = w;
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Vector4>(vsRegisterIndex, ref vector);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Vector4>(psRegisterIndex, ref vector);
		}
		#else
		public void Set(float x, float y, float z, float w)
		{
			#if XBOX360
			parameter.SetValue(new X.Vector4(x, y, z, w));
			#else
			X.Vector4 vector;
			vector.X = x;
			vector.Y = y;
			vector.Z = z;
			vector.W = w;
			parameter.SetValue(vector);
			#endif
		}
		#endif

		#if SILVERLIGHT
		public void Set(Vector2 value)
		{
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Vector2>(vsRegisterIndex, ref value);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Vector2>(psRegisterIndex, ref value);
		}
		#else
		public unsafe void Set(Vector2 value)
		{
			parameter.SetValue(*(X.Vector2*)&value);
		}
		#endif

		#if SILVERLIGHT
		public void Set(Vector3 value)
		{
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Vector3>(vsRegisterIndex, ref value);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Vector3>(psRegisterIndex, ref value);
		}
		#else
		public unsafe void Set(Vector3 value)
		{
			parameter.SetValue(*(X.Vector3*)&value);
		}
		#endif

		#if SILVERLIGHT
		public void Set(Vector4 value)
		{
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Vector4>(vsRegisterIndex, ref value);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Vector4>(psRegisterIndex, ref value);
		}
		#else
		public unsafe void Set(Vector4 value)
		{
			parameter.SetValue(*(X.Vector4*)&value);
		}
		#endif

		public void Set(Matrix2 value)
		{
			// use set float array for this
			throw new NotImplementedException();
		}

		public void Set(Matrix3 value)
		{
			// use set float array for this
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Matrix4 value)
		{
			if (vsRegisterIndex != -1) video.Device.SetVertexShaderConstantFloat4<Matrix4>(vsRegisterIndex, ref value);
			if (psRegisterIndex != -1) video.Device.SetPixelShaderConstantFloat4<Matrix4>(psRegisterIndex, ref value);
		}
		#else
		public unsafe void Set(Matrix4 value)
		{
			parameter.SetValue(*(X.Matrix*)&value);
		}
		#endif

		#if SILVERLIGHT
		public void Set(float[] values)
		{
			throw new NotImplementedException();
		}
		#else
		public void Set(float[] values)
		{
			parameter.SetValue(values);
		}
		#endif

		public void Set(Vector2[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values)
		{
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Vector4[] values)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void Set(Vector4[] values)
		{
			var data = new X.Vector4[values.Length];
			fixed (Vector4* ptr = values)
			fixed (X.Vector4* ptr2 = data)
			{
				var ptrOffset = ptr;
				var ptrOffset2 = ptr2;
				for (int i = 0; i != values.Length; ++i)
				{
					*ptrOffset2++ = *(X.Vector4*)(ptrOffset++);
				}
			}
			parameter.SetValue(data);
		}
		#endif

		public void Set(Matrix2[] values)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values)
		{
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Matrix4[] values)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void Set(Matrix4[] values)
		{
			var data = new X.Matrix[values.Length];
			fixed (Matrix4* ptr = values)
			fixed (X.Matrix* ptr2 = data)
			{ 
				var ptrOffset = ptr;
				var ptrOffset2 = ptr2;
				for (int i = 0; i != values.Length; ++i)
				{
					*ptrOffset2++ = *(X.Matrix*)(ptrOffset++);
				}
			}
			parameter.SetValue(data);
		}
		#endif

		#if SILVERLIGHT
		public void Set(float[] values, int count)
		{
			throw new NotImplementedException();
		}
		#else
		public void Set(float[] values, int count)
		{
			var data = new float[count];
			Array.Copy(values, data, count);
			parameter.SetValue(data);
		}
		#endif

		public void Set(Vector2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int count)
		{
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Vector4[] values, int count)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void Set(Vector4[] values, int count)
		{
			var data = new X.Vector4[count];
			fixed (Vector4* ptr = values)
			fixed (X.Vector4* ptr2 = data)
			{ 
				var ptrOffset = ptr;
				var ptrOffset2 = ptr2;
				for (int i = 0; i != count; ++i)
				{
					*ptrOffset2++ = *(X.Vector4*)(ptrOffset++);
				}
			}
			parameter.SetValue(data);
		}
		#endif

		public void Set(Matrix2[] values, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int count)
		{
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Matrix4[] values, int count)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void Set(Matrix4[] values, int count)
		{
			var data = new X.Matrix[count];
			fixed (Matrix4* ptr = values)
			fixed (X.Matrix* ptr2 = data)
			{
				var ptrOffset = ptr;
				var ptrOffset2 = ptr2;
				for (int i = 0; i != count; ++i)
				{
					*ptrOffset2++ = *(X.Matrix*)(ptrOffset++);
				}
			}
			parameter.SetValue(data);
		}
		#endif

		#if SILVERLIGHT
		public void Set(float[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}
		#else
		public void Set(float[] values, int offset, int count)
		{
			var data = new float[count];
			Array.Copy(values, offset, data, 0, count);
			parameter.SetValue(data);
		}
		#endif

		public void Set(Vector2[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Vector3[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Vector4[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void Set(Vector4[] values, int offset, int count)
		{
			var data = new X.Vector4[count];
			fixed (Vector4* ptr = values)
			fixed (X.Vector4* ptr2 = data)
			{
				var ptrOffset = ptr + offset;
				var ptrOffset2 = ptr2;
				for (int i = 0; i != count; ++i)
				{
					*ptrOffset2++ = *(X.Vector4*)(ptrOffset++);
				}
			}
			parameter.SetValue(data);
		}
		#endif

		public void Set(Matrix2[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public void Set(Matrix3[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}

		#if SILVERLIGHT
		public void Set(Matrix4[] values, int offset, int count)
		{
			throw new NotImplementedException();
		}
		#else
		public unsafe void Set(Matrix4[] values, int offset, int count)
		{
			var data = new X.Matrix[count];
			fixed (Matrix4* ptr = values)
			fixed (X.Matrix* ptr2 = data)
			{ 
				var ptrOffset = ptr + offset;
				var ptrOffset2 = ptr2;
				for (int i = 0; i != count; ++i)
				{
					*ptrOffset2++ = *(X.Matrix*)(ptrOffset++);
				}
			}
			parameter.SetValue(data);
		}
		#endif
		#endregion
	}
}