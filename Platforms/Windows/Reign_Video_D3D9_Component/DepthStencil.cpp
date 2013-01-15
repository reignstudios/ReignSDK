#include "pch.h"
#include "DepthStencil.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	DepthStencilErrors DepthStencilCom::Init(VideoCom^ video, int width, int height)
	{
		null();

		IDirect3DSurface9* surfaceTEMP = 0;
		if (FAILED(video->device->CreateDepthStencilSurface(width, height, D3DFMT_D16, D3DMULTISAMPLE_NONE, 0, false, &surfaceTEMP, 0)))//D3DFMT_D24S8
		{
			return DepthStencilErrors::DepthStencilSurface;
		}
		surface = surfaceTEMP;

		/*if (!this->video->IsExDevice && !this->video->deviceReseting)
		{
			video->DeviceLost += gcnew Video::DeviceLostMethod(this, &DepthStencil::deviceLost);
			video->DeviceReset += gcnew Video::DeviceLostMethod(this, &DepthStencil::deviceReset);
		}*/

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