#include "pch.h"
#include "Texture2D.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	TextureError Texture2DCom::Init(VideoCom^ video, int width, int height, bool generateMipmaps, array<array<byte>^>^ mipmaps, array<int>^ mipmapSizes, array<int>^ mipmapPitches, int multiSampleMultiple, REIGN_D3DPOOL pool, REIGN_D3DUSAGE usage, REIGN_D3DFORMAT format, bool isRenderTarget)
	{
		null();
		this->video = video;

		D3DPOOL nativePool = (D3DPOOL)pool;
		DWORD nativeUsage = (DWORD)usage;
		D3DFORMAT nativeFormat = (D3DFORMAT)format;
			
		if (mipmaps != nullptr)
		{
			IDirect3DTexture9* textureTEMP = 0;
			if (FAILED(video->device->CreateTexture(width, height, mipmaps->Length, nativeUsage, nativeFormat, nativePool, &textureTEMP, 0)))
			{
				return TextureError::Texture;
			}
			texture = textureTEMP;

			IDirect3DTexture9* systemTexture = 0;
			if (FAILED(video->device->CreateTexture(width, height, mipmaps->Length, 0, nativeFormat, D3DPOOL_SYSTEMMEM, &systemTexture, 0)))
			{
				return TextureError::SystemTexture;
			}

			for (int i = 0; i != mipmaps->Length; ++i)
			{
				array<byte>^ mipmapData = mipmaps[i];

				D3DLOCKED_RECT rect;
				systemTexture->LockRect(i, &rect, NULL, D3DLOCK_DISCARD);
				pin_ptr<byte> srcData = &mipmapData[0];
				memcpy(rect.pBits, srcData, mipmapData->Length);
				systemTexture->UnlockRect(i);
			}
			video->device->UpdateTexture(systemTexture, textureTEMP);
			systemTexture->Release();
		}
		else
		{
			uint mipLvls = generateMipmaps ? 0 : 1;
			IDirect3DTexture9* textureTEMP = 0;
			if (FAILED(video->device->CreateTexture(width, height, mipLvls, nativeUsage, nativeFormat, nativePool, &textureTEMP, 0)))
			{
				return TextureError::Texture;
			}
			texture = textureTEMP;
		}

		IDirect3DSurface9* surfaceTEMP;
		texture->GetSurfaceLevel(0, &surfaceTEMP);
		surface = surfaceTEMP;
	}

	Texture2DCom::~Texture2DCom()
	{
		if (surface) surface->Release();
		if (texture) texture->Release();
		null();
	}

	void Texture2DCom::null()
	{
		surface = 0;
		texture = 0;
	}
	#pragma endregion

	#pragma region Methods
	void Texture2DCom::Copy(Texture2DCom^ texture)
	{
		video->device->StretchRect(surface, NULL, texture->surface, NULL, D3DTEXF_NONE);
	}

	void Texture2DCom::Update(array<byte>^ data, int width, int height, REIGN_D3DFORMAT surfaceFormat)
	{
		IDirect3DTexture9* systemTexture = 0;
		if (FAILED(video->device->CreateTexture(width, height, 1, 0, (D3DFORMAT)surfaceFormat, D3DPOOL_SYSTEMMEM, &systemTexture, 0)))
		{
			throw gcnew Exception(L"Texture2D" + L"Failed to create system texture");
		}

		D3DLOCKED_RECT rect;
		systemTexture->LockRect(0, &rect, NULL, D3DLOCK_DISCARD);
		pin_ptr<byte> srcData = &data[0];
		memcpy(rect.pBits, srcData, data->Length);
		systemTexture->UnlockRect(0);

		video->device->UpdateTexture(systemTexture, texture);
		systemTexture->Release();
	}

	void Texture2DCom::WritePixels(array<byte>^ data)
	{
		D3DLOCKED_RECT rect;
		texture->LockRect(0, &rect, NULL, D3DLOCK_DISCARD);
		pin_ptr<byte> srcData = &data[0];
		memcpy(rect.pBits, srcData, data->Length);
		texture->UnlockRect(0);
	}
	#pragma endregion
}