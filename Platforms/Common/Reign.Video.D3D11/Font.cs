using Reign.Core;

namespace Reign.Video.D3D11
{
	class FonttreamLoader : StreamLoaderI
	{
		private Font font;
		private ShaderI shader;
		private Texture2DI texture;
		private string metricsFileName;

		public FonttreamLoader(Font font, ShaderI shader, Texture2DI texture, string metricsFileName)
		{
			this.font = font;
			this.shader = shader;
			this.texture = texture;
			this.metricsFileName = metricsFileName;
		}

		public override bool Load()
		{
			if (!shader.Loaded || !texture.Loaded) return false;
			font.load(shader, texture, metricsFileName);
			return true;
		}
	}

	public class Font : FontI
	{
		#region Constructors
		private ShaderI shader;
		private ShaderVariableI shaderCamera, shaderLocation, shaderSize, shaderLocationUV, shaderSizeUV, texelOffset, shaderColor;
		private ShaderResourceI shaderTexture;
		private Texture2DI texture;
		private BufferLayoutDesc layoutDesc;
		private BufferLayout layout;
		private IndexBuffer indexBuffer;
		private VertexBuffer vertexBuffer;
		private bool instancing;
		#endregion

		#region Constructors
		public Font(DisposableI parent, ShaderI shader, Texture2DI texture)
		: base(parent)
		{
			new FonttreamLoader(this, shader, texture, null);
		}

		public Font(DisposableI parent, ShaderI shader, Texture2DI texture, string metricsFileName)
		: base(parent, metricsFileName)
		{
			new FonttreamLoader(this, shader, texture, metricsFileName);
		}

		internal void load(ShaderI shader, Texture2DI texture, string metricsFileName)
		{
			init(shader, texture, metricsFileName);
		}

		protected override void init(ShaderI shader, Texture2DI texture, string metricsFileName)
		{
			if (metricsFileName != null) base.init(shader, texture, metricsFileName);

			this.texture = texture;
			this.shader = shader;
			shaderCamera = shader.Variable("Camera");
			shaderLocation = shader.Variable("Location");
			shaderSize = shader.Variable("Size");
			shaderLocationUV = shader.Variable("LocationUV");
			shaderSizeUV = shader.Variable("SizeUV");
			texelOffset = shader.Variable("TexelOffset");
			shaderColor = shader.Variable("Color");
			shaderTexture = shader.Resource("DiffuseTexture");

			layoutDesc = new BufferLayoutDesc(BufferLayoutTypes.Position2);
			layout = new BufferLayout(this, shader, layoutDesc);

			indexBuffer = new IndexBuffer(this, BufferUsages.Default, Indices);
			vertexBuffer = new VertexBuffer(this, layoutDesc, BufferUsages.Default, VertexBufferTopologys.Triangle, Vertices);
			Loaded = true;
		}
		#endregion

		#region Methods
		public override void DrawStart(Camera camera)
		{
			vertexBuffer.Enable(indexBuffer);
			shaderCamera.Set(camera.TransformMatrix);
			texelOffset.Set(texture.TexelOffset);
			shaderTexture.Set(texture);
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
				draw(text, texture.SizeF, Location, color, size, centeredX, centeredY);
			}
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