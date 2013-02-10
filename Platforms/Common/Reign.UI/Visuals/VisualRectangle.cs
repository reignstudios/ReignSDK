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
		Texture2DI Texture;

		private GeometryI geometry;
		private ShaderI shader;
		private ShaderVariableI shaderCamera, shaderPosition, shaderSize, shaderColor;
		private ShaderResourceI shaderTexture;
		#endregion

		#region Constructors
		public VisualRectangle(UI ui, Vector4 color, Texture2DI texture, VisualLayers layer, VisualFillModes fillMode)
		{
			Layout = layer;
			Color = color;
			Texture = texture;

			shader = texture == null ? ui.solidColorShader : ui.textureShader;
			shaderCamera = shader.Variable("Camera");
			shaderPosition = shader.Variable("Position");
			shaderSize = shader.Variable("Size");
			shaderColor = shader.Variable("Color");
			if (texture != null) shaderTexture = shader.Resource("MainTexture");

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

		public void Render(Camera camera)
		{
			shaderCamera.Set(camera.TransformMatrix);
			shaderPosition.Set(Position.ToVector2());
			shaderSize.Set(Size.ToVector2());
			shaderColor.Set(Color);
			if (Texture != null) shaderTexture.Set(Texture);
			shader.Apply();
			geometry.Render();
		}
		#endregion
	}
}
