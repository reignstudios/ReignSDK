#include "pch.h"
#include "RenderTarget.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	
	#pragma endregion

	#pragma region Methods
	RenderTargetError RenderTargetCom::Init(VideoCom^ video, int width, int height, Texture2DCom^ texture, int multiSampleMultiple, REIGN_DXGI_FORMAT surfaceFormat, bool readable)
	{
		null();
		this->video = video;
		this->texture = texture;
		DXGI_FORMAT surfaceFormatD3D = (DXGI_FORMAT)surfaceFormat;

		// RenderTarget View
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

		// Staging Texture
		if (readable)
		{
			D3D11_TEXTURE2D_DESC desc;
			ZeroMemory(&desc, sizeof(D3D11_TEXTURE2D_DESC));
			desc.Width = width;
			desc.Height = height;
			desc.MipLevels = 1;
			desc.ArraySize = 1;
			desc.Format = surfaceFormatD3D;
			desc.SampleDesc.Count = 1;
			desc.Usage = D3D11_USAGE_STAGING;
			desc.CPUAccessFlags = D3D11_CPU_ACCESS_READ;

			ID3D11Texture2D* textureTEMP;
			if (FAILED(video->device->CreateTexture2D(&desc, 0, &textureTEMP)))
			{
				return RenderTargetError::StagingTexture;
			}
			stagingTexture = textureTEMP;
		}

		return RenderTargetError::None;
	}

	RenderTargetCom::~RenderTargetCom()
	{
		if (renderTarget) renderTarget->Release();
		if (stagingTexture) stagingTexture->Release();
		null();
	}

	void RenderTargetCom::null()
	{
		renderTarget = 0;
		stagingTexture = 0;
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

		ID3D11DepthStencilView* surface = ((DepthStencilCom^)depthStencil)->surface;
		video->currentDepthStencil = surface;
		video->deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, surface);
	}

	void RenderTargetCom::ReadPixels(int dataPtr, int dataLength)
	{
		video->deviceContext->CopySubresourceRegion(stagingTexture, D3D11CalcSubresource(0, 0, 1), 0, 0, 0, texture->texture, D3D11CalcSubresource(0, 0, 1), 0);

		D3D11_MAPPED_SUBRESOURCE source;
		video->deviceContext->Map(stagingTexture, 0, D3D11_MAP_READ, NULL, &source);
		memcpy((void*)dataPtr, source.pData, dataLength);
		video->deviceContext->Unmap(stagingTexture, 0);
	}

	int RenderTargetCom::ReadPixel(int x, int y, int height)
	{
		video->deviceContext->CopySubresourceRegion(stagingTexture, D3D11CalcSubresource(0, 0, 1), 0, 0, 0, texture->texture, D3D11CalcSubresource(0, 0, 1), 0);

		D3D11_MAPPED_SUBRESOURCE source;
		video->deviceContext->Map(stagingTexture, 0, D3D11_MAP_READ, NULL, &source);
			byte* colors = (byte*)source.pData;
			int index = (x * 4) + ((height-1-y) * source.RowPitch);
			int color = colors[index];
			color |= colors[index+1] << 8;
			color |= colors[index+2] << 16;
			color |= colors[index+3] << 24;
		video->deviceContext->Unmap(stagingTexture, 0);

		return color;
	}
	#pragma endregion
}