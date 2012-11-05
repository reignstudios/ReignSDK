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

	RenderTarget::RenderTarget(DisposableI^ parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
	: Texture2D(parent, width, height, multiSampleType, surfaceFormat)
	{}

	RenderTarget::RenderTarget(DisposableI^ parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage)
	: Texture2D(parent, width, height, multiSampleType, surfaceFormat, usage)
	{}

	void RenderTarget::init(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable)
	{
		null();
		Texture2D::init(parent, image, width, height, false, multiSampleType, surfaceFormat, renderTargetUsage, usage, true, lockable);

		try
		{
			if (usage == BufferUsages::Write) Debug::ThrowError(L"RenderTarget", L"Only Textures may be writable");

			if (image == nullptr)
			{
				if (multiSampleType != MultiSampleTypes::None)
				{
					// RenderTarget
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
				width = image->Size.Width;
				height = image->Size.Height;
				surfaceFormat = image->SurfaceFormat;
				renderTarget = surface;
			}

			// Staging Texture
			if (usage == BufferUsages::Read)
			{
				IDirect3DSurface9* stagingSurfaceTEMP = 0;
				if (FAILED(video->Device->CreateOffscreenPlainSurface(width, height, Video::surfaceFormat(surfaceFormat), D3DPOOL_SYSTEMMEM, &stagingSurfaceTEMP, 0)))
				{
					Debug::ThrowError(L"RenderTarget", L"Failed to create Staging Texture");
				}
				stagingSurface = stagingSurfaceTEMP;
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
		if (stagingSurface) stagingSurface->Release();
		null();
		Texture2D::dispose();
	}

	void RenderTarget::null()
	{
		renderTarget = 0;
		stagingSurface = 0;
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

	void RenderTarget::ReadPixels(array<System::Byte>^ data)
	{
		video->Device->GetRenderTargetData(renderTarget, stagingSurface);

		D3DLOCKED_RECT rect;
		stagingSurface->LockRect(&rect, NULL, D3DLOCK_READONLY);
		pin_ptr<byte> dstData = &data[0];
		memcpy(dstData, rect.pBits, data->Length);
		stagingSurface->UnlockRect();
	}

	void RenderTarget::ReadPixels(array<Color4>^ colors)
	{
		video->Device->GetRenderTargetData(renderTarget, stagingSurface);

		D3DLOCKED_RECT rect;
		stagingSurface->LockRect(&rect, NULL, D3DLOCK_READONLY);
		pin_ptr<Color4> dstData = &colors[0];
		memcpy(dstData, rect.pBits, colors->Length * 4);
		stagingSurface->UnlockRect();
	}

	bool RenderTarget::ReadPixel(Point2 position, [Out] Color4% color)
	{
		if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
		{
			color = Color4();
			return false;
		}

		video->Device->GetRenderTargetData(renderTarget, stagingSurface);

		D3DLOCKED_RECT rect;
		stagingSurface->LockRect(&rect, NULL, D3DLOCK_READONLY);
			const byte* colors = (byte*)rect.pBits;
			int index = (position.X * 4) + ((Size.Height-1-position.Y) * rect.Pitch);
			int colorValue = colors[index] << 16;
			colorValue |= colors[index+1] << 8;
			colorValue |= colors[index+2];
			colorValue |= colors[index+3] << 24;
		stagingSurface->UnlockRect();

		color = Color4(colorValue);
		return true;
	}
	#pragma endregion
}
}
}