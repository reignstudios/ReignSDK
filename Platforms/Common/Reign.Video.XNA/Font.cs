using Reign.Core;
using System;
using Microsoft.Xna.Framework.Graphics;
using X = Microsoft.Xna.Framework;

namespace Reign.Video.XNA
{
	public class Font : FontI
	{
		#region Constructors
		private Shader shader;
		private Texture2D fontTexture;
		private bool instancing;
		private BufferLayout layout;
		
		private ShaderVariableI shaderCamera, shaderLocation, shaderSize, shaderLocationUV, shaderSizeUV, texelOffset, shaderColor;
		private ShaderResourceI shaderTexture;
		
		private IndexBuffer indexBuffer;
		private VertexBuffer vertexBuffer;
		#endregion

		#region Constructors
		public Font(DisposableI parent, ShaderI shader, Texture2DI fontTexture)
		: base(parent)
		{
			init(parent, shader, fontTexture);
		}

		public Font(DisposableI parent, ShaderI shader, Texture2DI fontTexture, string metricsFileName)
		: base(parent, metricsFileName)
		{
			init(parent, shader, fontTexture);
		}

		private void init(DisposableI parent, ShaderI shader, Texture2DI fontTexture)
		{
			this.fontTexture = (Texture2D)fontTexture;
			this.shader = (Shader)shader;

			shaderCamera = shader.Variable("Camera");
			shaderLocation = shader.Variable("Location");
			shaderSize = shader.Variable("Size");
			shaderLocationUV = shader.Variable("LocationUV");
			shaderSizeUV = shader.Variable("SizeUV");
			texelOffset = shader.Variable("TexelOffset");
			shaderColor = shader.Variable("Color");
			shaderTexture = shader.Resource("DiffuseTexture");

			var layoutDesc = new BufferLayoutDesc(BufferLayoutTypes.Position2);
			layout = new BufferLayout(this, shader, layoutDesc);
			indexBuffer = new IndexBuffer(this, BufferUsages.Default, Indices);
			vertexBuffer = new VertexBuffer(this, layoutDesc, BufferUsages.Default, VertexBufferTopologys.Triangle, Vertices);
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (layout != null) layout.Dispose();
			if (indexBuffer != null) indexBuffer.Dispose();
			if (vertexBuffer != null) vertexBuffer.Dispose();
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void DrawStart(Camera camera)
		{
			vertexBuffer.Enable(indexBuffer);
			shaderCamera.Set(camera.TransformMatrix);
			texelOffset.Set(fontTexture.TexelOffset);
			shaderTexture.Set(fontTexture);
			layout.Enable();
			instancing = false;
		}

		public override void Draw(string text, Vector2 Location, Vector4 color, float size, bool centeredX, bool centeredY)
		{
			if (instancing)
			{
			
			}
			else
			{
				draw(text, fontTexture.SizeF, Location, color, size, centeredX, centeredY);
			}
		}

		private Vector2 convertUV(Vector2 uv, Vector2 locationUV, Vector2 sizeUV)
		{
			return (new Vector2(uv.X, 1-uv.Y) * sizeUV) + locationUV;
		}

		protected override void draw(Vector2 location, Vector2 size, Vector2 locationUV, Vector2 sizeUV, Vector4 color)
		{
			shaderLocation.Set(location);
			shaderSize.Set(size);
			shaderLocationUV.Set(locationUV);
			shaderSizeUV.Set(sizeUV);
			shaderColor.Set(color);
			shader.Apply();
			vertexBuffer.Draw();
		}
		#endregion
	}
}