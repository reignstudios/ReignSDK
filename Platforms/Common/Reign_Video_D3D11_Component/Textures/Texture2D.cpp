#include "pch.h"
#include "Texture2D.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	TextureError Texture2DCom::Init(VideoCom^ video, int width, int height, bool generateMipmaps, const array<IntPtr>^ mipmaps, const array<int>^ mipmapSizes, const array<int>^ mipmapPitches, int multiSampleMultiple, REIGN_DXGI_FORMAT surfaceFormat, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage, bool isRenderTarget)
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
		if (mipmaps)
		{
			subData = new D3D11_SUBRESOURCE_DATA[mipmaps->Length];
			for (int i = 0; i != mipmaps->Length; ++i)
			{
				ZeroMemory(&subData[i], sizeof(D3D11_SUBRESOURCE_DATA));
				subData[i].pSysMem = mipmaps[i].ToPointer();
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
	/*void Texture2DCom::Update(array<byte>^ data)
	{
		pin_ptr<byte> dataT = &data[0];
		video->Device->UpdateSubresource(texture, D3D11CalcSubresource(0, 0, 1), 0, dataT, sizeof(byte)*4*size.X, 0);
	}

	void Texture2DCom::UpdateDynamic(array<byte>^ data)
	{
		D3D11_MAPPED_TEXTURE2D buff;
		texture->Map(D3D11CalcSubresource(0, 0, 1), D3D11_MAP_WRITE_DISCARD, 0, &buff);
		pin_ptr<byte> dataT = &data[0];
		memcpy(buff.pData, dataT, data->Length);
		texture->Unmap(D3D11CalcSubresource(0, 0, 1));
	}

	void Texture2DCom::Copy(Texture2DI^ texture)
	{
		ID3D11Texture2D* srcTexture = ((Texture2D^)texture)->texture;
		video->Device->CopySubresourceRegion(this->texture, D3D11CalcSubresource(0, 0, 1), 0, 0, 0, srcTexture, D3D11CalcSubresource(0, 0, 1), 0);
	}

	array<System::Byte>^ Texture2DCom::Copy()
	{
		D3D11_MAPPED_TEXTURE2D buff;
		texture->Map(D3D11CalcSubresource(0, 0, 1), D3D11_MAP_READ_WRITE, 0, &buff);
		array<System::Byte>^ data = gcnew array<System::Byte>(size.X * size.Y * 4);
		pin_ptr<byte> dataT = &data[0];
		memcpy(dataT, buff.pData, data->Length);
		texture->Unmap(D3D11CalcSubresource(0, 0, 1));

		return data;
	}*/
	#pragma endregion
}