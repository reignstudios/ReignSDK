using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class Video : Disposable, VideoI
	{
		#region Properties
		private ApplicationI application;
		internal GraphicsContext context;
		internal FrameBuffer currentFrameBuffer;
		internal Texture2D[] currentPixelTextures;
		internal SamplerState[] currentSamplerStates;
		internal EnableMode currentEnableMode;
		
		public string FileTag {get; private set;}
		public Size2 BackBufferSize {get; private set;}
		#endregion
		
		#region Constructors
		public Video(DisposableI parent, ApplicationI application, DepthStencilFormats depthStencilFormats, bool vSync)
		: base(parent)
		{
			try
			{
				this.application = application;
				FileTag = "Vita_";
				currentPixelTextures = new Texture2D[4];
				currentSamplerStates = new SamplerState[4];
				
				PixelFormat format = PixelFormat.None;
				switch (depthStencilFormats)
				{
					case DepthStencilFormats.None: format = PixelFormat.None; break;
					case DepthStencilFormats.Defualt: format = PixelFormat.Depth16; break;
					case DepthStencilFormats.Depth24Stencil8: format = PixelFormat.Depth24Stencil8; break;
					case DepthStencilFormats.Depth16: format = PixelFormat.Depth16; break;
					case DepthStencilFormats.Depth24: format = PixelFormat.Depth24; break;

					default:
						Debug.ThrowError("Video", "Unsuported DepthStencilFormat type");
						break;
				}
				
				context = new GraphicsContext(0, 0, PixelFormat.Rgba, format, MultiSampleMode.None);
				currentFrameBuffer = context.Screen;
				BackBufferSize = new Size2(currentFrameBuffer.Width, currentFrameBuffer.Height);
				((VitaApplication)application).Vita_SetFrameSize(currentFrameBuffer.Width, currentFrameBuffer.Height);
				
				currentEnableMode = EnableMode.None;
				context.Enable(currentEnableMode);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		
		public override void Dispose ()
		{
			disposeChilderen();
			if (context != null)
			{
				context.Dispose();
				context = null;
			}
			base.Dispose ();
		}
		#endregion
		
		#region Methods
		public void Update()
		{
			BackBufferSize = application.FrameSize;
		}
		
		public void EnableRenderTarget()
		{
			currentFrameBuffer = context.Screen;
			context.SetFrameBuffer(currentFrameBuffer);
		}
		
		public void EnableRenderTarget(DepthStencilI depthStencil)
		{
			currentFrameBuffer = context.Screen;
			((DepthStencil)depthStencil).enable();
			context.SetFrameBuffer(currentFrameBuffer);
		}
		
		public void ClearAll(float r, float g, float b, float a) 
		{
			context.SetClearColor(r, g, b, a);
			context.Clear(ClearMask.All);
		}
		
		public void ClearColor(float r, float g, float b, float a)
		{
			context.SetClearColor(r, g, b, a);
			context.Clear(ClearMask.Color);
		}
		
		public void ClearColorDepth(float r, float g, float b, float a)
		{
			context.SetClearColor(r, g, b, a);
			context.Clear(ClearMask.Color | ClearMask.Depth);
		}
		
		public void ClearDepthStencil()
		{
			context.Clear(ClearMask.Depth | ClearMask.Stencil);
		}
		
		public void Present()
		{
			context.SwapBuffers();
		}
		
		internal void disableActiveTextures(Texture2D texture)
		{
			for (int i = 0; i != currentPixelTextures.Length; ++i)
			{
				if (currentPixelTextures[i] == texture)
				{
					context.SetTexture(i, null);
					currentPixelTextures[i] = null;
				}
			}
		}
		
		internal static PixelFormat surfaceFormat(SurfaceFormats surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case SurfaceFormats.Defualt: return PixelFormat.Rgba4444;
				case SurfaceFormats.RGBx565: return PixelFormat.Rgb565;
				case SurfaceFormats.RGBAx4: return PixelFormat.Rgba4444;
				case SurfaceFormats.RGBx5_Ax1: return PixelFormat.Rgba5551;
				case SurfaceFormats.RGBAx8: return PixelFormat.Rgba;
				case SurfaceFormats.RGBAx16f: return PixelFormat.RgbaH;
				
				default:
					Debug.ThrowError("RenderTarget", "Unsuported SurfaceFormat");
					return PixelFormat.Rgba;
			}
		}
		#endregion
	}
}

