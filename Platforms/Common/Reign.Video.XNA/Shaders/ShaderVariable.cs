using System;
using G = Microsoft.Xna.Framework.Graphics;
using X = Microsoft.Xna.Framework;
using Reign.Core;

namespace Reign.Video.XNA
{
	public class ShaderVariable : ShaderVariableI
	{
		#region Properties
		private G.EffectParameter parameter;
		public string Name {get; private set;}
		#endregion

		#region Constructors
		public ShaderVariable(G.EffectParameter parameter, string name)
		{
			this.parameter = parameter;
			this.Name = name;
		}
		#endregion

		#region Methods
		public void Set(float value) {parameter.SetValue(value);}
		public void Set(float x, float y) {parameter.SetValue(new X.Vector2(x, y));}
		public void Set(float x, float y, float z) {parameter.SetValue(new X.Vector3(x, y, z));}
		public void Set(float x, float y, float z, float w) {parameter.SetValue(new X.Vector4(x, y, z, w));}

		public void Set(Vector2 value)
		{
			parameter.SetValue(new X.Vector2(value.X, value.Y));
		}

		public void Set(Vector3 value)
		{
			#if XBOX360
			parameter.SetValue(new X.Vector3(value.X, value.Y, value.Z));
			#else
			X.Vector3 vector;
			vector.X = value.X;
			vector.Y = value.Y;
			vector.Z = value.Z;
			parameter.SetValue(vector);
			#endif
		}

		public void Set(Vector4 value)
		{
			#if XBOX360
			parameter.SetValue(new X.Vector3(value.X, value.Y, value.Z));
			#else
			X.Vector4 vector;
			vector.X = value.X;
			vector.Y = value.Y;
			vector.Z = value.Z;
			vector.W = value.W;
			parameter.SetValue(vector);
			#endif
		}

		public unsafe void Set(Matrix2 value)
		{
			parameter.SetValue(*(X.Matrix*)&value);
		}

		public unsafe void Set(Matrix3 value)
		{
			parameter.SetValue(*(X.Matrix*)&value);
		}

		public unsafe void Set(Matrix4 value)
		{
			parameter.SetValue(*(X.Matrix*)&value);
		}

		public void Set(float[] values)
		{
			parameter.SetValue(values);
		}

		public unsafe void Set(Vector4[] values)
		{
			var data = new X.Vector4[values.Length];
			fixed (Vector4* ptr = values)
			{
				var ptrOffset = ptr;
				fixed (X.Vector4* ptr2 = data)
				{ 
					var ptrOffset2 = ptr2;
					for (int i = 0; i != values.Length; ++i)
					{
						*ptrOffset2++ = *(X.Vector4*)(ptrOffset++);
					}
				}
			}
			parameter.SetValue(data);
		}

		public unsafe void Set(Matrix4[] values)
		{
			var data = new X.Matrix[values.Length];
			fixed (Matrix4* ptr = values)
			{
				var ptrOffset = ptr;
				fixed (X.Matrix* ptr2 = data)
				{ 
					var ptrOffset2 = ptr2;
					for (int i = 0; i != values.Length; ++i)
					{
						*ptrOffset2++ = *(X.Matrix*)(ptrOffset++);
					}
				}
			}
			parameter.SetValue(data);
		}

		public void Set(float[] values, int count)
		{
			var data = new float[count];
			Array.Copy(values, data, count);
			parameter.SetValue(data);
		}

		public unsafe void Set(Vector4[] values, int count)
		{
			var data = new X.Vector4[count];
			fixed (Vector4* ptr = values)
			{
				var ptrOffset = ptr;
				fixed (X.Vector4* ptr2 = data)
				{ 
					var ptrOffset2 = ptr2;
					for (int i = 0; i != count; ++i)
					{
						*ptrOffset2++ = *(X.Vector4*)(ptrOffset++);
					}
				}
			}
			parameter.SetValue(data);
		}

		public unsafe void Set(Matrix4[] values, int count)
		{
			var data = new X.Matrix[count];
			fixed (Matrix4* ptr = values)
			{
				var ptrOffset = ptr;
				fixed (X.Matrix* ptr2 = data)
				{ 
					var ptrOffset2 = ptr2;
					for (int i = 0; i != count; ++i)
					{
						*ptrOffset2++ = *(X.Matrix*)(ptrOffset++);
					}
				}
			}
			parameter.SetValue(data);
		}

		public void Set(float[] values, int offset, int count)
		{
			var data = new float[count];
			Array.Copy(values, offset, data, 0, count);
			parameter.SetValue(data);
		}

		public unsafe void Set(Vector4[] values, int offset, int count)
		{
			var data = new X.Vector4[count];
			fixed (Vector4* ptr = values)
			{
				var ptrOffset = ptr + offset;
				fixed (X.Vector4* ptr2 = data)
				{ 
					var ptrOffset2 = ptr2;
					for (int i = 0; i != count; ++i)
					{
						*ptrOffset2++ = *(X.Vector4*)(ptrOffset++);
					}
				}
			}
			parameter.SetValue(data);
		}

		public unsafe void Set(Matrix4[] values, int offset, int count)
		{
			var data = new X.Matrix[count];
			fixed (Matrix4* ptr = values)
			{
				var ptrOffset = ptr + offset;
				fixed (X.Matrix* ptr2 = data)
				{ 
					var ptrOffset2 = ptr2;
					for (int i = 0; i != count; ++i)
					{
						*ptrOffset2++ = *(X.Matrix*)(ptrOffset++);
					}
				}
			}
			parameter.SetValue(data);
		}
		#endregion
	}
}