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

		private RasterizerStateI rasterizerState;
		private DepthStencilStateI depthStencilState;
		private BlendStateI blendState;
		private SamplerStateI samplerState;

		public Color4 BackgroundColor, ForgroundColor, BorderColor;
		internal ShaderI solidColorShader, textureShader;
		internal Font font;
		#endregion

		#region Constructors
		public UI(DisposableI parent, VideoI video, ShaderI solidColorShader, ShaderI textureShader, Font font, MouseI mouse)
		: base(parent)
		{
			try
			{
				this.mouse = mouse;

				this.video = video;
				geometries = new List<GeometryI>();
				elements = new List<Element>();
				viewPort = ViewPortAPI.New(video, 0, 0, video.BackBufferSize.Width, video.BackBufferSize.Height);
				camera = new Camera(viewPort, new Vector3(0, 0, 10), new Vector3(), new Vector3(0, 1, 10));

				rasterizerState = RasterizerStateAPI.New(video, RasterizerStateDescAPI.New(RasterizerStateTypes.Solid_CullNone));
				depthStencilState = DepthStencilStateAPI.New(video, DepthStencilStateDescAPI.New(DepthStencilStateTypes.None));
				blendState = BlendStateAPI.New(video, BlendStateDescAPI.New(BlendStateTypes.Alpha));
				samplerState = SamplerStateAPI.New(video, SamplerStateDescAPI.New(SamplerStateTypes.Linear_Wrap));

				BackgroundColor = new Color4(0, 112, 159, 255);
				ForgroundColor = new Color4(255, 255, 255, 255);
				BorderColor = new Color4(0, 255, 0, 255);
				this.solidColorShader = solidColorShader;
				this.textureShader = textureShader;
				this.font = font;
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
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
