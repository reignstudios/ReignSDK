#include "pch.h"
#include "DepthStencil.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	IDirect3DSurface9* DepthStencil::Surface::get() {return surface;}
	#pragma endregion

	#pragma region Contructors
	DepthStencil::DepthStencil(DisposableI^ parent, int width, int height, DepthStenicFormats depthStenicFormats)
	: Disposable(parent)
	{
		init(parent, width, height, depthStenicFormats);
	}

	void DepthStencil::init(DisposableI^ parent, int width, int height, DepthStenicFormats depthStenicFormats)
	{
		null();

		this->width = width;
		this->height = height;
		this->depthStenicFormats = depthStenicFormats;
		
		try
		{
			video = parent->FindParentOrSelfWithException<Video^>();

			IDirect3DSurface9* surfaceTEMP = 0;
			if (FAILED(video->Device->CreateDepthStencilSurface(width, height, D3DFMT_D24S8, D3DMULTISAMPLE_NONE, 0, false, &surfaceTEMP, 0)))
			{
				Debug::ThrowError(L"DepthStencil", L"Failed to create DepthStencilSurface");
			}
			surface = surfaceTEMP;

			if (!this->video->IsExDevice && !this->video->deviceReseting)
			{
				video->DeviceLost += gcnew Video::DeviceLostMethod(this, &DepthStencil::deviceLost);
				video->DeviceReset += gcnew Video::DeviceLostMethod(this, &DepthStencil::deviceReset);
			}
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	DepthStencil::~DepthStencil()
	{
		disposeChilderen();
		if (video)
		{
			video->DeviceLost -= gcnew Video::DeviceLostMethod(this, &DepthStencil::deviceLost);
			video->DeviceReset -= gcnew Video::DeviceLostMethod(this, &DepthStencil::deviceReset);
		}
		dispose();
	}

	void DepthStencil::dispose()
	{
		if (surface) surface->Release();
		null();
	}

	void DepthStencil::null()
	{
		surface = 0;
	}

	void DepthStencil::deviceLost()
	{
		if (!video->IsExDevice) dispose();
	}

	void DepthStencil::deviceReset()
	{
		if (!video->IsExDevice) init(video, width, height, depthStenicFormats);
	}
	#pragma endregion

	#pragma region Methods
	void DepthStencil::enable()
	{
		video->Device->SetDepthStencilSurface(surface);
	}
	#pragma endregion
}
}
}