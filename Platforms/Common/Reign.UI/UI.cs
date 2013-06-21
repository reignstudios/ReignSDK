using System;
using System.Collections.Generic;
using Reign.Core;
using Reign.Video;
using Reign.Input;

namespace Reign.UI
{
	public class UI : Disposable
	{
		#region Properties
		private MouseI mouse;

		internal VideoI video;
		internal GeometryI lastGeometryRendered;
		private List<GeometryI> geometries;
		private List<Element> elements;

		private ViewPortI viewPort;
		internal Camera camera;

		public bool AutoTVOverscanSize;
		public int TVOverscanSize, ManualLeft, ManualRight, ManualBottom, ManualTop;
		public int Left {get; private set;}
		public int Right {get; private set;}
		public int Bottom {get; private set;}
		public int Top {get; private set;}

		private RasterizerStateI rasterizerState;
		private DepthStencilStateI depthStencilState;
		private BlendStateI blendState;
		private SamplerStateI samplerState;

		public Vector4 BackgroundColorIdle, BackgroundColorRollover, BackgroundColorPressed, ForegroundColorIdle, ForegroundColorRollover, ForegroundColorPressed, BorderColorIdle, BorderColorRollover, BorderColorPressed;
		internal ShaderI solidColorShader, textureShader, textureShader2, textureShader3;
		internal BufferLayoutI shaderLayout;
		internal Font font;
		internal float fontSize;

		public float AutoScale {get; private set;}
		public float ManualScale;
		public bool AutoScaleAllDefault = true;
		#endregion

		#region Constructors
		public UI(DisposableI parent, VideoI video, ShaderI solidColorShader, ShaderI textureShader, ShaderI textureShader2, ShaderI textureShader3, Font font, float fontSize, MouseI mouse)
		: base(parent)
		{
			try
			{
				this.mouse = mouse;

				this.video = video;
				geometries = new List<GeometryI>();
				elements = new List<Element>();
				viewPort = ViewPortAPI.New(video, Point2.Zero, video.BackBufferSize);
				camera = new Camera(viewPort, new Vector3(0, 0, 10), new Vector3(), new Vector3(0, 1, 10), 9, 11, MathUtilities.DegToRad(90));

				rasterizerState = RasterizerStateAPI.New(video, RasterizerStateDescAPI.New(RasterizerStateTypes.Solid_CullNone));
				depthStencilState = DepthStencilStateAPI.New(video, DepthStencilStateDescAPI.New(DepthStencilStateTypes.None));
				blendState = BlendStateAPI.New(video, BlendStateDescAPI.New(BlendStateTypes.Alpha));
				samplerState = SamplerStateAPI.New(video, SamplerStateDescAPI.New(SamplerStateTypes.Linear_Clamp));

				BackgroundColorIdle = new Color4(240, 240, 240, 255).ToVector4();
				BackgroundColorRollover = new Color4(164, 216, 221, 255).ToVector4();
				BackgroundColorPressed = new Color4(102, 188, 198, 255).ToVector4();

				ForegroundColorIdle = new Color4(0, 0, 0, 255).ToVector4();
				ForegroundColorRollover = new Color4(0, 0, 0, 255).ToVector4();
				ForegroundColorPressed = new Color4(0, 0, 0, 255).ToVector4();

				BorderColorIdle = new Color4(115, 115, 115, 255).ToVector4();
				BorderColorRollover = new Color4(74, 147, 155, 255).ToVector4();
				BorderColorPressed = new Color4(57, 113, 119, 255).ToVector4();

				this.solidColorShader = solidColorShader;
				this.textureShader = textureShader;
				this.textureShader2 = textureShader2;
				this.textureShader3 = textureShader3;
				this.font = font;
				this.fontSize = fontSize;

				shaderLayout = BufferLayoutAPI.New(video, solidColorShader, BufferLayoutDescAPI.New(BufferLayoutTypes.Position2));
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (rasterizerState != null) rasterizerState.Dispose();
			if (depthStencilState != null) depthStencilState.Dispose();
			if (blendState != null) blendState.Dispose();
			if (samplerState != null) samplerState.Dispose();

			if (shaderLayout != null) shaderLayout.Dispose();
			base.Dispose();
		}
		#endregion

		#region Methods
		public void AddElement(Element element)
		{
			AddElement(element, true);
		}

		public void AddElement(Element element, bool enabled)
		{
			if (elements.Contains(element)) Debug.ThrowError("UI", "Element has been added to the UI");
			elements.Add(element);
		}

		public void RemoveElement(Element element)
		{
			if (elements.Contains(element)) elements.Remove(element);
		}

		public void Update()
		{
			// calculate screen attributes
			if (ManualScale == 0) AutoScale = viewPort.Size.ToVector2().Length() / new Vector2(1280, 720).Length();
			else AutoScale = ManualScale;

			if (AutoTVOverscanSize)
			{
				// TODO: set a good TVOverscanSize reletive to screen size
			}

			if (ManualLeft == 0) Left = TVOverscanSize;
			else Left = ManualLeft;

			if (ManualRight == 0) Right = viewPort.Size.Width - TVOverscanSize;
			else Right = ManualRight;

			if (ManualBottom == 0) Bottom = TVOverscanSize;
			else Bottom = ManualBottom;

			if (ManualTop == 0) Top = viewPort.Size.Height - TVOverscanSize;
			else Top = ManualTop;

			foreach (var element in elements) element.Update(mouse);
		}

		public void Render()
		{
			rasterizerState.Enable();
			depthStencilState.Enable();
			blendState.Enable();
			samplerState.Enable(0);
			
			viewPort.Size = video.BackBufferSize;
			viewPort.Apply();
			camera.ApplyOrthographic();
			lastGeometryRendered = null;
			foreach (var element in elements) element.Render();
		}

		public GeometryI NewGeometryReference(Type geometryType)
		{
			foreach (var g in geometries)
			{
				if (g.GetType() == geometryType) return g;
			}

			if (geometryType == typeof(RectangleGeometry)) return new RectangleGeometry(this);
			else if (geometryType == typeof(RectangleBorderGeometry)) return new RectangleBorderGeometry(this);
			
			Debug.ThrowError("UI", "Unsuported Geometry Type");
			return null;
		}
		#endregion
	}
}
