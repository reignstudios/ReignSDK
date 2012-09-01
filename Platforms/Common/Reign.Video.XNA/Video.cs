using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using R = Reign.Video.XNA;

namespace Reign.Video.XNA
{
	public class Video : Disposable, VideoI
	{
		#region Properties
		public GraphicsDevice Device {get; private set;}
		public string FileTag {get; private set;}
		#endregion

		#region Constructors
		public Video(DisposableI parent, Application application)
		: base(parent)
		{
			Device = application.GraphicsDevice;
			defualtStates();

			FileTag = "XNA_";
		}

		public override void Dispose()
		{
			disposeChilderen();
			base.Dispose();
		}

		private void defualtStates()
		{
			var rasterizerState = new Microsoft.Xna.Framework.Graphics.RasterizerState();
			rasterizerState.CullMode = CullMode.None;
			rasterizerState.FillMode = FillMode.Solid;
			rasterizerState.MultiSampleAntiAlias = false;
			rasterizerState.ScissorTestEnable = false;
			Device.RasterizerState = rasterizerState;

			var samplerStates = new Microsoft.Xna.Framework.Graphics.SamplerState();
			samplerStates.Filter = TextureFilter.Linear;
			Device.SamplerStates[0] = samplerStates;
		}
		#endregion

		#region Methods
		public void Update()
		{
			// XNA handles lost device.
			// This is a place holder...
		}

		public void Present()
		{
			Device.Present();
		}

		public void EnableRenderTarget()
		{
			Device.SetRenderTarget(null);
		}

		public void EnableRenderTarget(DepthStencilI depthStencil)
		{
			if (depthStencil == null)
			{
				Device.SetRenderTarget(null);
			}
			else
			{
				throw new NotImplementedException();
				//var buffer = (DepthStencil)depthStencil;
				//var buffers = new RenderTargetBinding[2]
				//{
				//    backBuffer,
				//    buffer.depthStencil
				//};
				//Device.SetRenderTargets(buffers);
			}
		}

		public void Clear(float r, float g, float b, float a)
		{
			Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, new Color(r, g, b, a), 1.0f, 0);
		}

		public void ClearColor(float r, float g, float b, float a)
		{
			Device.Clear(ClearOptions.Target, new Color(r, g, b, a), 1.0f, 0);
		}

		public void ClearDepthStencil()
		{
			Device.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil, new Color(0, 0, 0, 0), 1.0f, 0);
		}

		public void DisableVertexBuffer()
		{
			Device.SetVertexBuffer(null);
		}

		internal static SurfaceFormat surfaceFormat(SurfaceFormats surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case (SurfaceFormats.RGBAx8): return SurfaceFormat.Color;
				case (SurfaceFormats.RGBx10_Ax2): return SurfaceFormat.Rgba1010102;
				case (SurfaceFormats.RGBAx16f): return SurfaceFormat.HalfVector4;
				case (SurfaceFormats.RGBAx32f): return SurfaceFormat.Vector4;
				default:
					Debug.ThrowError("RenderTarget", "Unsuported SurfaceFormat");
					return SurfaceFormat.Color;
			}
		}
		#endregion
	}
}
