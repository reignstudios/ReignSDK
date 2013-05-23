using System;
using Reign.Core;
using Reign.Video;

namespace Reign.UI
{
	public class VisualRectangle : RectangleShape, VisualI
	{
		#region Properties
		public VisualLayers Layout {get; private set;}
		public Vector4 Color {get; set;}
		public float Fade {get; set;}
		public float Fade2 {get; set;}
		public Texture2DI Texture {get; set;}
		public Texture2DI Texture2 {get; set;}
		public Texture2DI Texture3 {get; set;}

		private GeometryI geometry;
		BufferLayoutI layout;
		private ShaderI shader;
		private ShaderVariableI shaderCamera, shaderPosition, shaderSize, shaderColor, shaderFade, shaderFade2, shaderTexelOffset;
		private ShaderResourceI shaderTexture, shaderTexture2, shaderTexture3;
		#endregion

		#region Constructors
		public VisualRectangle(UI ui, Vector4 color, Texture2DI texture, Texture2DI texture2, Texture2DI texture3, VisualLayers layer, VisualFillModes fillMode)
		{
			Layout = layer;
			Color = color;
			Texture = texture;
			Texture2 = texture2;
			Texture3 = texture3;

			if (texture == null && texture2 == null && texture3 == null) shader = ui.solidColorShader;
			else if (texture != null && texture2 != null && texture3 != null) shader = ui.textureShader3;
			else if (texture != null && texture2 != null && texture3 == null) shader = ui.textureShader2;
			else if (texture != null && texture2 == null && texture3 != null) shader = ui.textureShader2;
			else if (texture != null && texture2 == null && texture3 == null) shader = ui.textureShader;
			else Debug.ThrowError("VisualRectangle", "Unsupported texture params");
			
			shaderCamera = shader.Variable("Camera");
			shaderPosition = shader.Variable("Position");
			shaderSize = shader.Variable("Size");
			shaderColor = shader.Variable("Color");
			if (texture != null)
			{
				shaderTexture = shader.Resource("MainTexture");
				shaderTexelOffset = shader.Variable("TexelOffset");
			}
			if (texture2 != null)
			{
				shaderTexture2 = shader.Resource("MainTexture2");
				shaderFade = shader.Variable("Fade");
			}
			if (texture3 != null)
			{
				shaderTexture3 = shader.Resource("MainTexture3");
				shaderFade2 = shader.Variable(texture2 == null ? "Fade" : "Fade2");
			}
			layout = ui.shaderLayout;
			switch (fillMode)
			{
				case (VisualFillModes.Solid):
					geometry = ui.NewGeometryReference(typeof(RectangleGeometry));
					break;

				case (VisualFillModes.Border):
					geometry = ui.NewGeometryReference(typeof(RectangleBorderGeometry));
					break;
			}
		}
		#endregion

		#region Methods
		public void Update(Rect2 elementRect)
		{
			rect = elementRect;
		}

		public void Render(UI ui)
		{
			shaderCamera.Set(ui.camera.TransformMatrix);
			shaderPosition.Set(Position.ToVector2());
			shaderSize.Set(Size.ToVector2());
			shaderColor.Set(Color);
			if (Texture != null)
			{
				shaderTexture.Set(Texture);
				shaderTexelOffset.Set(Texture.TexelOffset);
			}
			if (Texture2 != null)
			{
				shaderTexture2.Set(Texture2);
				shaderFade.Set(Fade);
			}
			if (Texture3 != null)
			{
				shaderTexture3.Set(Texture3);
				shaderFade2.Set(Fade2);
			}

			layout.Enable();
			shader.Apply();
			geometry.Render();
		}
		#endregion
	}
}
