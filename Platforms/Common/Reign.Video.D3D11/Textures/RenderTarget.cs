﻿using Reign.Core;
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

		protected override void init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget)
		{
			base.init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, true);

			try
			{
				if (usage == BufferUsages.Write) Debug.ThrowError("RenderTarget", "Only Textures may be writable");

				if (image != null)
				{
					width = image.Size.Width;
					height = image.Size.Height;
					surfaceFormat = image.SurfaceFormat;
				}

				renderTargetCom = new RenderTargetCom();
				var error = renderTargetCom.Init(video.com, width, height, com, 0, video.surfaceFormat(surfaceFormat), usage == BufferUsages.Read);

				switch (error)
				{
					case (RenderTargetError.RenderTargetView): Debug.ThrowError("RenderTarget", "Failed to create RenderTargetView"); break;
					case (RenderTargetError.StagingTexture): Debug.ThrowError("RenderTarget", "Failed to create Staging Texture"); break;
				}
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

		public unsafe void ReadPixels(byte[] data)
		{
			fixed (byte* ptr = data)
			{
				renderTargetCom.ReadPixels((int)ptr, data.Length);
			}
		}

		public unsafe void ReadPixels(Color4[] colors)
		{
			fixed (Color4* ptr = colors)
			{
				renderTargetCom.ReadPixels((int)ptr, colors.Length * 4);
			}
		}

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			{
				color = new Color4();
				return false;
			}

			color = new Color4(renderTargetCom.ReadPixel(position.X, position.Y, Size.Height));
			return true;
		}
		#endregion
	}
}