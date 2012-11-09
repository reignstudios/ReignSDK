#include "pch.h"
#include "Texture2D.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	TextureError Texture2DCom::Init(VideoCom^ video, int width, int height, bool generateMipmaps, bool hasMipmaps, const array<__int64>^ mipmaps, const array<int>^ mipmapSizes, const array<int>^ mipmapPitches, int multiSampleMultiple, REIGN_DXGI_FORMAT surfaceFormat, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage, bool isRenderTarget)
	{
		null();
		this->video = video;
		DXGI_FORMAT surfaceFormatD3D = (DXGI_FORMAT)surfaceFormat;
		D3D11_USAGE usageType = (D3D11_USAGE)usage;
		D3D11_CPU_ACCESS_FLAG cpuAccessFlags = (D3D11_CPU_ACCESS_FLAG)cpuUsage;

		if (isRenderTarget) generateMipmaps = false;
		
		// Texture2D
		uint mipLvls = generateMipmaps ? 0 : 1;
		D3D11_TEXTURE2D_DESC desc;
		ZeroMemory(&desc, sizeof(D3D11_TEXTURE2D_DESC));
		desc.Width = width;
		desc.Height = height;
		desc.MipLevels = mipLvls;
		desc.ArraySize = 1;
		desc.Format = surfaceFormatD3D;
		desc.SampleDesc.Count = 1;
		desc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
		if (isRenderTarget) desc.BindFlags |= D3D11_BIND_RENDER_TARGET;
		desc.Usage = usageType;
		desc.CPUAccessFlags = cpuAccessFlags;

		D3D11_SUBRESOURCE_DATA* subData = 0;
		if (hasMipmaps && mipmaps)
		{
			subData = new D3D11_SUBRESOURCE_DATA[mipmaps->Length];
			for (int i = 0; i != mipmaps->Length; ++i)
			{
				ZeroMemory(&subData[i], sizeof(D3D11_SUBRESOURCE_DATA));
				subData[i].pSysMem = (void*)mipmaps[i];
				subData[i].SysMemPitch = mipmapPitches[i];
			}
		}

		ID3D11Texture2D* textureTEMP;
		if (FAILED(video->device->CreateTexture2D(&desc, subData, &textureTEMP)))
		{
			if (subData) delete subData;
			return TextureError::Texture;
		}
		if (subData) delete subData;
		texture = textureTEMP;
		
		// ShaderResource
		D3D11_TEXTURE2D_DESC textureDesc;
		ZeroMemory(&textureDesc, sizeof(D3D11_TEXTURE2D_DESC));
		texture->GetDesc(&textureDesc);
		D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc;
		ZeroMemory(&srvDesc, sizeof(D3D11_SHADER_RESOURCE_VIEW_DESC));
		srvDesc.Format = surfaceFormatD3D;
		srvDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
		srvDesc.Texture2D.MipLevels = textureDesc.MipLevels;
		srvDesc.Texture2D.MostDetailedMip = 0;

		ID3D11ShaderResourceView* shaderResourceTEMP;
		if (FAILED(video->device->CreateShaderResourceView(texture, &srvDesc, &shaderResourceTEMP)))
		{
			return TextureError::ShaderResourceView;
		}
		shaderResource = shaderResourceTEMP;

		return TextureError::None;
	}

	Texture2DCom::~Texture2DCom()
	{
		if (shaderResource)
		{
			video->removeActiveResource(shaderResource);
			shaderResource->Release();
		}
		if (texture) texture->Release();
		null();
	}

	void Texture2DCom::null()
	{
		texture = 0;
		shaderResource = 0;
		video = nullptr;
	}
	#pragma endregion

	#pragma region Methods
	void Texture2DCom::Copy(Texture2DCom^ texture)
	{
		video->deviceContext->CopySubresourceRegion(texture->texture, D3D11CalcSubresource(0, 0, 1), 0, 0, 0, this->texture, D3D11CalcSubresource(0, 0, 1), 0);
	}

	void Texture2DCom::Update(const array<byte>^ data, int width)
	{
		PinPtr(byte) dataT = &data[0];
		video->deviceContext->UpdateSubresource(texture, D3D11CalcSubresource(0, 0, 1), 0, dataT, sizeof(byte)*4*width, 0);
	}

	void Texture2DCom::WritePixels(const array<byte>^ data)
	{
		D3D11_MAPPED_SUBRESOURCE source;
		video->deviceContext->Map(texture, 0, D3D11_MAP_WRITE_DISCARD, NULL, &source);
		PinPtr(byte) dataT = &data[0];
		memcpy(source.pData, dataT, data->Length);
		video->deviceContext->Unmap(texture, 0);
	}
	#pragma endregion
}