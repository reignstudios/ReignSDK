#include "pch.h"
#include "RenderTarget.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	RenderTargetError RenderTargetCom::Init(VideoCom^ video, Texture2DCom^ texture, int width, int height, int multiSampleMultiple, REIGN_D3DFORMAT surfaceFormat, bool readable, bool lockable)
	{
		null();
		this->video = video;
		D3DFORMAT nativeSurfaceFormat = (D3DFORMAT)surfaceFormat;

		// RenderTarget
		if (multiSampleMultiple != 0)
		{
			isMultiSampled = true;
			isMultiSampledResolved = false;
			IDirect3DSurface9* renderTargetTEMP = 0;
			if (FAILED(video->device->CreateRenderTarget(width, height, nativeSurfaceFormat, D3DMULTISAMPLE_NONE, 0, lockable, &renderTargetTEMP, 0)))
			{
				return RenderTargetError::RenderTarget;
			}
			renderTarget = renderTargetTEMP;
			video->device->StretchRect(surface, 0, renderTarget, 0, D3DTEXF_NONE);
		}
		else
		{
			isMultiSampled = false;
			renderTarget = texture->surface;
		}
		
		width = width;
		height = height;
		surface = texture->surface;

		// Staging Texture
		if (readable)
		{
			IDirect3DSurface9* stagingSurfaceTEMP = 0;
			if (FAILED(video->device->CreateOffscreenPlainSurface(width, height, nativeSurfaceFormat, D3DPOOL_SYSTEMMEM, &stagingSurfaceTEMP, 0)))
			{
				return RenderTargetError::StagingTexture;
			}
			stagingSurface = stagingSurfaceTEMP;
		}

		return RenderTargetError::None;
	}

	RenderTargetCom::~RenderTargetCom()
	{
		if (renderTarget && renderTarget != surface) renderTarget->Release();
		if (stagingSurface) stagingSurface->Release();
		null();
	}

	void RenderTargetCom::null()
	{
		renderTarget = 0;
		stagingSurface = 0;
	}
	#pragma endregion

	#pragma region Methods
	void RenderTargetCom::Enable()
	{
		video->device->SetRenderTarget(0, renderTarget);
		video->device->SetDepthStencilSurface(0);
		isMultiSampledResolved = false;
	}

	void RenderTargetCom::Enable(DepthStencilCom^ depthStencil)
	{
		video->device->SetRenderTarget(0, renderTarget);
		video->device->SetDepthStencilSurface(depthStencil->surface);
		isMultiSampledResolved = false;
	}

	void RenderTargetCom::ResolveMultisampled()
	{
		if (isMultiSampled && !isMultiSampledResolved)
		{
			video->device->StretchRect(renderTarget, 0, surface, 0, D3DTEXF_NONE);
			isMultiSampledResolved = true;
		}
	}

	void RenderTargetCom::ReadPixels(void* data, int dataLength)
	{
		video->device->GetRenderTargetData(renderTarget, stagingSurface);

		D3DLOCKED_RECT rect;
		stagingSurface->LockRect(&rect, NULL, D3DLOCK_READONLY);
		memcpy(data, rect.pBits, dataLength);
		stagingSurface->UnlockRect();
	}

	int RenderTargetCom::ReadPixel(int x, int y, int height)
	{
		video->device->GetRenderTargetData(renderTarget, stagingSurface);

		D3DLOCKED_RECT rect;
		stagingSurface->LockRect(&rect, NULL, D3DLOCK_READONLY);
			const byte* colors = (byte*)rect.pBits;
			int index = (x * 4) + ((height-1-y) * rect.Pitch);
			int colorValue = colors[index] << 16;
			colorValue |= colors[index+1] << 8;
			colorValue |= colors[index+2];
			colorValue |= colors[index+3] << 24;
		stagingSurface->UnlockRect();

		return colorValue;
	}
	#pragma endregion
}