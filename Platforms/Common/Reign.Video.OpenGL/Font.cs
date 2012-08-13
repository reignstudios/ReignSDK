using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class Font : FontI
	{
		#region Constructors
		private ShaderI shader;
		private ShaderVariableI shaderCamera, shaderLocation, shaderSize, shaderLocationUV, shaderSizeUV, texelOffset, shaderColor;
		private ShaderResourceI shaderTexture;
		private Texture2DI fontTexture;
		private BufferLayoutDesc layoutDesc;
		private BufferLayout layout;
		private IndexBuffer indexBuffer;
		private VertexBuffer vertexBuffer;
		private bool instancing;
		#endregion

		#region Constructors
		public Font(DisposableI parent, ShaderI shader, Texture2DI fontTexture)
		: base(parent)
		{
			init(shader, fontTexture);
		}

		public Font(DisposableI parent, ShaderI shader, Texture2DI fontTexture, string metricsFileName)
		: base(parent, metricsFileName)
		{
			init(shader, fontTexture);
		}

		private void init(ShaderI shader, Texture2DI fontTexture)
		{
			this.fontTexture = fontTexture;
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