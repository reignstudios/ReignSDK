using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class Video : Disposable, VideoI
	{
		#region Properties
		internal GraphicsContext context;
		internal FrameBuffer currentFrameBuffer;
		internal Texture2D[] currentPixelTextures;
		internal SamplerState[] currentSamplerStates;
		internal EnableMode currentEnableMode;
		
		public string FileTag {get; private set;}
		public Size2 BackBufferSize {get; private set;}
		#endregion
		
		#region Constructors
		public Video(DisposableI parent, Application application)
		: base(parent)
		{
			try
			{
				FileTag = "Vita_";
				currentPixelTextures = new Texture2D[4];
				currentSamplerStates = new SamplerState[4];
				
				context = new GraphicsContext(0, 0, PixelFormat.Rgba, PixelFormat.Depth16, MultiSampleMode.None);
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
			// Do nothing...
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
					context.SetTexture(i, texture.texture);
					currentPixelTextures[i] = null;
				}
			}
		}
		
		internal static PixelFormat surfaceFormat(SurfaceFormats surfaceFormat)
		{
			switch (surfaceFormat)
			{
				case (SurfaceFormats.RGBAx8): return PixelFormat.Rgba;
				default:
					Debug.ThrowError("RenderTarget", "Unsuported SurfaceFormat");
					return PixelFormat.Rgba;
			}
		}
		#endregion
	}
}

