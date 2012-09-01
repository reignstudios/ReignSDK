#include "pch.h"
#include "RenderTarget.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	RenderTarget::RenderTarget(DisposableI^ parent, string^ fileName)
	: Texture2D(parent, fileName)
	{}

	RenderTarget::RenderTarget(DisposableI^ parent, string^ fileName, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable)
	: Texture2D(parent, fileName, width, height, false, multiSampleType, surfaceFormat, lockable)
	{}

	RenderTarget::RenderTarget(DisposableI^ parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable)
	: Texture2D(parent, width, height, false, multiSampleType, surfaceFormat, lockable)
	{}

	RenderTarget::RenderTarget(DisposableI^ parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
	: Texture2D(parent, width, height, false, multiSampleType, surfaceFormat)
	{}

	void RenderTarget::init(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable)
	{
		null();
		Texture2D::init(parent, image, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, lockable);

		try
		{
			if (image == nullptr)
			{
				if (multiSampleType != MultiSampleTypes::None)
				{
					// TODO: Handle multisampling types
					IDirect3DSurface9* renderTargetTEMP = 0;
					if (FAILED(video->Device->CreateRenderTarget(width, height, Video::surfaceFormat(surfaceFormat), D3DMULTISAMPLE_NONE, 0, lockable, &renderTargetTEMP, 0)))
					{
						Debug::ThrowError(L"RenderTarget", L"Failed to create RenderTarget");
					}
					renderTarget = renderTargetTEMP;
				}
				else
				{
					renderTarget = surface;
				}
			}
			else
			{
				renderTarget = surface;
			}
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	RenderTarget::~RenderTarget()
	{
		disposeChilderen();
		dispose();
	}

	void RenderTarget::dispose()
	{
		if (renderTarget && renderTarget != surface) renderTarget->Release();
		null();
		Texture2D::dispose();
	}

	void RenderTarget::null()
	{
		renderTarget = 0;
	}
	#pragma endregion

	#pragma region Methods
	void RenderTarget::Enable()
	{
		// TODO: disable unsused active renderTargets
		video->removeActiveTexture(this);
		video->Device->SetRenderTarget(0, renderTarget);
		video->Device->SetDepthStencilSurface(0);
	}

	void RenderTarget::Enable(DepthStencilI^ depthStencil)
	{
		video->removeActiveTexture(this);
		video->Device->SetRenderTarget(0, renderTarget);
		if (depthStencil != nullptr)
		{
			((DepthStencil^)depthStencil)->enable();
		}
		else
		{
			video->Device->SetDepthStencilSurface(0);
		}
	}

	void RenderTarget::ResolveMultisampled()
	{
		video->Device->StretchRect(renderTarget, 0, surface, 0, D3DTEXF_NONE);
	}

	void RenderTarget::CopyToSystemTexture(Texture2D^ texture2D)
	{
		video->Device->GetRenderTargetData(renderTarget, texture2D->Surface);
	}
	#pragma endregion
}
}
}