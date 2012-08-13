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
		#if WP7
		private Video video;
		#else
		private ShaderVariableI shaderCamera, shaderLocation, shaderSize, shaderLocationUV, shaderSizeUV, texelOffset, shaderColor;
		private ShaderResourceI shaderTexture;
		
		private IndexBuffer indexBuffer;
		private VertexBuffer vertexBuffer;
		#endif
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

			#if WP7
			video = parent.FindParentOrSelfWithException<Video>();
			var layoutDesc = new BufferLayoutDesc(BufferLayoutTypes.Position3_Color_UV);
			layout = new BufferLayout(this, shader, layoutDesc, true);
			#else
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
			#endif
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (layout != null) layout.Dispose();
			#if !WP7
			if (indexBuffer != null) indexBuffer.Dispose();
			if (vertexBuffer != null) vertexBuffer.Dispose();
			#endif
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void DrawStart(Camera camera)
		{
			#if WP7
			switch (shader.Type)
			{
				case (SimpleShaderTypes.Basic):
					var fx = (BasicEffect)shader.FX;
					fx.TextureEnabled = true;
					fx.Texture = fontTexture.texture;
					fx.World = X.Matrix.Identity;
					fx.View = camera.ViewMatrix.ToMatrixX();
					fx.Projection = camera.ProjectionMatrix.ToMatrixX();
					break;

				default: 
					Debug.ThrowError("Font", "Unsuported Font Shader type: " + shader.Type.ToString());
					break;
			}
			#else
			vertexBuffer.Enable(indexBuffer);
			shaderCamera.Set(camera.TransformMatrix);
			texelOffset.Set(fontTexture.TexelOffset);
			shaderTexture.Set(fontTexture);
			layout.Enable();
			#endif
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
			#if WP7
			var uv0 = convertUV(new Vector2(0, 0), locationUV, sizeUV);
			var uv1 = convertUV(new Vector2(0, 1), locationUV, sizeUV);
			var uv2 = convertUV(new Vector2(1, 1), locationUV, sizeUV);

			var uv3 = convertUV(new Vector2(0, 0), locationUV, sizeUV);
			var uv4 = convertUV(new Vector2(1, 1), locationUV, sizeUV);
			var uv5 = convertUV(new Vector2(1, 0), locationUV, sizeUV);

			var verts = new VertexPositionColorTexture[]
			{
				new VertexPositionColorTexture(new X.Vector3(location.X, location.Y, 0), X.Color.White, new X.Vector2(uv0.X, uv0.Y)),
				new VertexPositionColorTexture(new X.Vector3(location.X, location.Y+size.Y, 0), X.Color.White, new X.Vector2(uv1.X, uv1.Y)),
				new VertexPositionColorTexture(new X.Vector3(location.X+size.X, location.Y+size.Y, 0), X.Color.White, new X.Vector2(uv2.X, uv2.Y)),

				new VertexPositionColorTexture(new X.Vector3(location.X, location.Y, 0), X.Color.White, new X.Vector2(uv3.X, uv3.Y)),
				new VertexPositionColorTexture(new X.Vector3(location.X+size.X, location.Y+size.Y, 0), X.Color.White, new X.Vector2(uv4.X, uv4.Y)),
				new VertexPositionColorTexture(new X.Vector3(location.X+size.X, location.Y, 0), X.Color.White, new X.Vector2(uv5.X, uv5.Y)),
			};

			var fx = (BasicEffect)shader.FX;
			fx.DiffuseColor = new X.Vector3(color.R, color.G, color.B);
			fx.Alpha = color.A;
			shader.Apply();
			video.Device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, verts, 0, 2, layout.layout);
			#else
			shaderLocation.Set(location);
			shaderSize.Set(size);
			shaderLocationUV.Set(locationUV);
			shaderSizeUV.Set(sizeUV);
			shaderColor.Set(color);
			shader.Apply();
			vertexBuffer.Draw();
			#endif
		}
		#endregion
	}
}