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
			parameter.SetValue(new X.Vector3(value.X, value.Y, value.Z));
		}

		public void Set(Vector4 value)
		{
			parameter.SetValue(new X.Vector4(value.X, value.Y, value.Z, value.W));
		}

		public void Set(Matrix2 value)
		{
			parameter.SetValue(new float[]
			{
				value.X.X, value.X.Y,
				value.Y.X, value.Y.Y
			});
		}

		public void Set(Matrix3 value)
		{
			parameter.SetValue(new float[]
			{
				value.X.X, value.X.Y, value.X.Z,
				value.Y.X, value.Y.Y, value.Y.Z,
				value.Z.X, value.Z.Y, value.Z.Z
			});
		}

		public void Set(Matrix4 value)
		{
			parameter.SetValue(new X.Matrix
			(
				value.X.X, value.X.Y, value.X.Z, value.X.W,
				value.Y.X, value.Y.Y, value.Y.Z, value.Y.W,
				value.Z.X, value.Z.Y, value.Z.Z, value.Z.W,
				value.W.X, value.W.Y, value.W.Z, value.W.W)
			);
		}

		public void Set(float[] values)
		{
			parameter.SetValue(values);
		}

		public void Set(Vector2[] values)
		{
			var data = new X.Vector2[values.Length];
			for (int i = 0; i != values.Length; ++i)
			{
				var value = values[i];
				data[i] = new X.Vector2(value.X, value.Y);
			}
			parameter.SetValue(data);
		}

		public void Set(Vector3[] values)
		{
			var data = new X.Vector3[values.Length];
			for (int i = 0; i != values.Length; ++i)
			{
				var value = values[i];
				data[i] = new X.Vector3(value.X, value.Y, value.Z);
			}
			parameter.SetValue(data);
		}

		public void Set(Vector4[] values)
		{
			var data = new X.Vector4[values.Length];
			for (int i = 0; i != values.Length; ++i)
			{
				var value = values[i];
				data[i] = new X.Vector4(value.X, value.Y, value.Z, value.W);
			}
			parameter.SetValue(data);
		}

		public void Set(Matrix2[] values)
		{
			var data = new float[values.Length * 4];
			Array.Copy(values, data, values.Length);
			parameter.SetValue(data);
		}

		public void Set(Matrix3[] values)
		{
			var data = new float[values.Length * 9];
			Array.Copy(values, data, values.Length);
			parameter.SetValue(data);
		}

		public void Set(Matrix4[] values)
		{
			var data = new float[values.Length * 12];
			Array.Copy(values, data, values.Length);
			parameter.SetValue(data);
		}

		public void Set(float[] values, int count)
		{
			var data = new float[count];
			Array.Copy(values, data, count);
			parameter.SetValue(data);
		}

		public void Set(Vector2[] values, int count)
		{
			var data = new X.Vector2[count];
			for (int i = 0; i != count; ++i)
			{
				var value = values[i];
				data[i] = new X.Vector2(value.X, value.Y);
			}
			parameter.SetValue(data);
		}

		public void Set(Vector3[] values, int count)
		{
			var data = new X.Vector3[count];
			for (int i = 0; i != count; ++i)
			{
				var value = values[i];
				data[i] = new X.Vector3(value.X, value.Y, value.Z);
			}
			parameter.SetValue(data);
		}

		public void Set(Vector4[] values, int count)
		{
			var data = new X.Vector4[count];
			for (int i = 0; i != count; ++i)
			{
				var value = values[i];
				data[i] = new X.Vector4(value.X, value.Y, value.Z, value.W);
			}
			parameter.SetValue(data);
		}

		public void Set(Matrix2[] values, int count)
		{
			var data = new float[values.Length * 4];
			Array.Copy(values, data, count);
			parameter.SetValue(data);
		}

		public void Set(Matrix3[] values, int count)
		{
			var data = new float[values.Length * 9];
			Array.Copy(values, data, count);
			parameter.SetValue(data);
		}

		public void Set(Matrix4[] values, int count)
		{
			var data = new float[values.Length * 12];
			Array.Copy(values, data, count);
			parameter.SetValue(data);
		}

		public void Set(float[] values, int offset, int count)
		{
			var data = new float[count];
			Array.Copy(values, offset, data, 0, count);
			parameter.SetValue(data);
		}

		public void Set(Vector2[] values, int offset, int count)
		{
			var data = new X.Vector2[count];
			int i2 = offset;
			for (int i = 0; i != count; ++i)
			{
				var value = values[i2];
				data[i] = new X.Vector2(value.X, value.Y);
				++i2;
			}
			parameter.SetValue(data);
		}

		public void Set(Vector3[] values, int offset, int count)
		{
			var data = new X.Vector3[count];
			int i2 = offset;
			for (int i = 0; i != count; ++i)
			{
				var value = values[i2];
				data[i] = new X.Vector3(value.X, value.Y, value.Z);
				++i2;
			}
			parameter.SetValue(data);
		}

		public void Set(Vector4[] values, int offset, int count)
		{
			var data = new X.Vector4[count];
			int i2 = offset;
			for (int i = 0; i != count; ++i)
			{
				var value = values[i2];
				data[i] = new X.Vector4(value.X, value.Y, value.Z, value.W);
				++i2;
			}
			parameter.SetValue(data);
		}

		public void Set(Matrix2[] values, int offset, int count)
		{
			var data = new float[values.Length * 4];
			Array.Copy(values, offset, data, 0, count);
			parameter.SetValue(data);
		}

		public void Set(Matrix3[] values, int offset, int count)
		{
			var data = new float[values.Length * 9];
			Array.Copy(values, offset, data, 0, count);
			parameter.SetValue(data);
		}

		public void Set(Matrix4[] values, int offset, int count)
		{
			var data = new float[values.Length * 12];
			Array.Copy(values, offset, data, 0, count);
			parameter.SetValue(data);
		}
		#endregion
	}
}