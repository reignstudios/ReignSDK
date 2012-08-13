using Reign.Core;
using Reign_Video_D3D11_Component;
using System;

namespace Reign.Video.D3D11
{
	public class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		private RenderTargetCom renderTargetCom;
		#endregion

		#region Constructors
		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
		: base(parent, width, height, surfaceFormat)
		{
		}

		public RenderTarget(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage)
		: base(parent, width, height, surfaceFormat, usage)
		{
		}

		public RenderTarget(DisposableI parent, string fileName, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage)
		: base(parent, fileName, width, height, false, surfaceFormat, usage)
		{
		}

		protected override void init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			base.init(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, true);

			try
			{
				renderTargetCom = new RenderTargetCom();
				var error = renderTargetCom.Init(video.com, com, 0, video.surfaceFormat(surfaceFormat));

				if (error == RenderTargetError.RenderTargetView) Debug.ThrowError("RenderTarget", "Failed to create RenderTargetView");
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
			if (renderTargetCom != null)
			{
				renderTargetCom.Dispose();
				renderTargetCom = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Enable()
		{
			renderTargetCom.Enable();
		}

		public void Enable(DepthStencilI depthStencil)
		{
			renderTargetCom.Enable(((DepthStencil)depthStencil).com);
		}
		#endregion
	}
}