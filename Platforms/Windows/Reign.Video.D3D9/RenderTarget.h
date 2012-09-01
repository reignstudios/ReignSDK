#pragma once
#include "Texture2D.h"
#include "DepthStencil.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class RenderTarget : Texture2D, RenderTargetI
	{
		#pragma region Properties
		private: IDirect3DSurface9* renderTarget;
		#pragma endregion

		#pragma region Constructors
		public: RenderTarget(DisposableI^ parent, string^ fileName);
		public: RenderTarget(DisposableI^ parent, string^ fileName, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable);
		public: RenderTarget(DisposableI^ parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable);
		public: RenderTarget(DisposableI^ parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage);
		protected: virtual void init(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable) override;
		public: ~RenderTarget();
		protected: virtual void dispose() override;
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: virtual void Enable();
		public: virtual void Enable(DepthStencilI^ depthStencil);
		public: void ResolveMultisampled();
		public: void CopyToSystemTexture(Texture2D^ texture2D);
		#pragma endregion
	};
}
}
}