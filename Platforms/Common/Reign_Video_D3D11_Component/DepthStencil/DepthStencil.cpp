#include "pch.h"
#include "DepthStencil.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	DepthStencilErrors DepthStencilCom::Init(VideoCom^ video, int width, int height, int depthBit, int stencilBit)
	{
		null();

		//Texture
		D3D11_TEXTURE2D_DESC descDepth;
		ZeroMemory(&descDepth, sizeof(D3D11_TEXTURE2D_DESC));
		descDepth.Width = width;
		descDepth.Height = height;
		descDepth.MipLevels = 1;
		descDepth.ArraySize = 1;

		descDepth.Format = DXGI_FORMAT_R16_UINT;
		if (depthBit == 24 && (stencilBit == 8 || stencilBit == 0)) descDepth.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
		else if (depthBit == 16 && stencilBit == 0) descDepth.Format = DXGI_FORMAT_D16_UNORM;
		else if (depthBit == 32 && stencilBit == 0) descDepth.Format = DXGI_FORMAT_D32_FLOAT;

		descDepth.SampleDesc.Count = 1;
		descDepth.SampleDesc.Quality = 0;
		descDepth.Usage = D3D11_USAGE_DEFAULT;
		descDepth.BindFlags = D3D11_BIND_DEPTH_STENCIL;
		descDepth.CPUAccessFlags = 0;
		descDepth.MiscFlags = 0;
		ID3D11Texture2D* textureTEMP;
		if (FAILED(video->device->CreateTexture2D(&descDepth, 0, &textureTEMP)))
		{
			return DepthStencilErrors::Textrue;
		}
		texture = textureTEMP;
		
		//DepthStencil
		D3D11_DEPTH_STENCIL_VIEW_DESC descDSV;
		ZeroMemory(&descDSV, sizeof(D3D11_DEPTH_STENCIL_VIEW_DESC));
		descDSV.Format = descDepth.Format;
		if (descDepth.SampleDesc.Count > 1) descDSV.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2DMS;
		else descDSV.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
		descDSV.Texture2D.MipSlice = 0;
		ID3D11DepthStencilView* surfaceTEMP;
		if (FAILED(video->device->CreateDepthStencilView(texture, &descDSV, &surfaceTEMP)))
		{
			return DepthStencilErrors::DepthStencilView;
		}
		surface = surfaceTEMP;

		return DepthStencilErrors::None;
	}

	DepthStencilCom::~DepthStencilCom()
	{
		if (texture) texture->Release();
		if (surface) surface->Release();
		null();
	}

	void DepthStencilCom::null()
	{
		texture = 0;
		surface = 0;
	}
	#pragma endregion
}