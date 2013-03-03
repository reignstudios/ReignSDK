#include "pch.h"
#include "DepthStencil.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	DepthStencilErrors DepthStencilCom::Init(VideoCom^ video, int width, int height, int depthBit, int stencilBit)
	{
		null();

		D3DFORMAT format = D3DFMT_D16;
		if (depthBit == 24 && stencilBit == 8) format = D3DFMT_D24S8;
		if (depthBit == 24 && stencilBit == 0) format = D3DFMT_D24X8;
		else if (depthBit == 16 && stencilBit == 0) format = D3DFMT_D16;
		else if (depthBit == 32 && stencilBit == 0) format = D3DFMT_D32;

		IDirect3DSurface9* surfaceTEMP = 0;
		if (FAILED(video->device->CreateDepthStencilSurface(width, height, format, D3DMULTISAMPLE_NONE, 0, false, &surfaceTEMP, 0)))
		{
			return DepthStencilErrors::DepthStencilSurface;
		}
		surface = surfaceTEMP;

		return DepthStencilErrors::None;
	}

	DepthStencilCom::~DepthStencilCom()
	{
		if (surface) surface->Release();
		null();
	}

	void DepthStencilCom::null()
	{
		surface = 0;
	}
	#pragma endregion
}