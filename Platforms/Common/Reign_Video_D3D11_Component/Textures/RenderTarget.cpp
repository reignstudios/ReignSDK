#include "pch.h"
#include "RenderTarget.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	
	#pragma endregion

	#pragma region Methods
	RenderTargetError RenderTargetCom::Init(VideoCom^ video, Texture2DCom^ texture, int multiSampleMultiple, REIGN_DXGI_FORMAT surfaceFormat)
	{
		null();
		this->video = video;
		this->texture = texture;
		DXGI_FORMAT surfaceFormatD3D = (DXGI_FORMAT)surfaceFormat;

		D3D11_RENDER_TARGET_VIEW_DESC desc;
		ZeroMemory(&desc, sizeof(D3D11_RENDER_TARGET_VIEW_DESC));
		desc.Format = surfaceFormatD3D;
		desc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;
		desc.Texture2D.MipSlice = 0;
		ID3D11RenderTargetView* renderTargetTEMP = 0;
		if (FAILED(video->device->CreateRenderTargetView(texture->texture, &desc, &renderTargetTEMP)))
		{
			return RenderTargetError::RenderTargetView;
		}
		renderTarget = renderTargetTEMP;

		return RenderTargetError::None;
	}

	RenderTargetCom::~RenderTargetCom()
	{
		if (renderTarget) renderTarget->Release();
		null();
	}

	void RenderTargetCom::null()
	{
		renderTarget = 0;
	}
	#pragma endregion

	#pragma region Methods
	void RenderTargetCom::Enable()
	{
		video->removeActiveResource(texture->shaderResource);
		ID3D11RenderTargetView* renderTargetTEMP = renderTarget;
		video->deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, 0);
		video->currentRenderTarget = renderTarget;
		video->currentDepthStencil = 0;
	}

	void RenderTargetCom::Enable(DepthStencilCom^ depthStencil)
	{
		video->removeActiveResource(texture->shaderResource);
		ID3D11RenderTargetView* renderTargetTEMP = renderTarget;
		video->currentRenderTarget = renderTarget;
		
		if (depthStencil)
		{
			ID3D11DepthStencilView* surface = ((DepthStencilCom^)depthStencil)->surface;
			video->currentDepthStencil = surface;
			video->deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, surface);
		}
		else
		{
			video->deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, 0);
		}
	}
	#pragma endregion
}