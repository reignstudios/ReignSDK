#include "pch.h"
#include "Texture2D.h"
#include <d3dx9.h>

using namespace System::Runtime::InteropServices;
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	IDirect3DSurface9* Texture2D::Surface::get() {return surface;}
	IDirect3DTexture9* Texture2D::Texture::get() {return texture;}

	Size2 Texture2D::Size::get() {return size;}
	Vector2 Texture2D::SizeF::get() {return sizeF;}
	Vector2 Texture2D::TexelOffset::get() {return texelOffset;}
	#pragma endregion

	#pragma region Constructors
	Texture2DI^ Texture2D::New(DisposableI^ parent, string^ fileName)
	{
		Texture2D^ texture = parent->FindChild<Texture2D^>(gcnew string(L"New"),
			gcnew ConstructorParam(DisposableI::typeid, parent),
			gcnew ConstructorParam(string::typeid, fileName));
		if (texture)
		{
			++texture->referenceCount;
			return texture;
		}
		return gcnew Texture2D(parent, fileName);
	}

	Texture2D::Texture2D(DisposableI^ parent, string^ fileName)
	: Disposable(parent)
	{
		init(parent, fileName, 0, 0, false, MultiSampleTypes::None, SurfaceFormats::RGBAx8, RenderTargetUsage::PlatformDefault, false, false);
	}

	Texture2D::Texture2D(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, bool lockable)
	: Disposable(parent)
	{
		init(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage::PlatformDefault, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, bool lockable)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage::PlatformDefault, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage::PlatformDefault, false, false);
	}

	Texture2D::Texture2D(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable)
	: Disposable(parent)
	{
		init(parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, false, false);
	}

	void Texture2D::init(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool isRenderTarget, bool lockable)
	{
		null();

		fileName_LostDevice = fileName;
		width_LostDevice = width;
		height_LostDevice = height;
		generateMipmaps_LostDevice = generateMipmaps;
		multiSampleType_LostDevice = multiSampleType;
		surfaceFormat_LostDevice = surfaceFormat;
		renderTargetUsage_LostDevice = renderTargetUsage;
		isRenderTarget_LostDevice = isRenderTarget;
		lockable_LostDevice = lockable;

		IntPtr fileNamePtr = Marshal::StringToHGlobalUni(fileName);
		try
		{
			video = parent->FindParentOrSelfWithException<Video^>();

			pool = video->IsExDevice ? D3DPOOL_DEFAULT : D3DPOOL_MANAGED;
			DWORD usage = 0;
			//D3DUSAGE_DYNAMIC - Need for updating textures:: Cannot be used with D3DPOOL_MANAGED
			if (isRenderTarget)
			{
				pool = D3DPOOL_DEFAULT;
				usage = D3DUSAGE_RENDERTARGET;
			}
			
			if (fileName)
			{
				uint mipLvls = generateMipmaps ? D3DX_DEFAULT : D3DX_FROM_FILE;
				IDirect3DTexture9* textureTEMP = 0;
				if (FAILED(D3DXCreateTextureFromFileEx(video->Device, (wchar_t*)fileNamePtr.ToPointer(), width, height, mipLvls, usage, Video::surfaceFormat(surfaceFormat), pool, D3DX_FILTER_TRIANGLE, D3DX_FILTER_TRIANGLE, 0, 0, 0, &textureTEMP)))
				{
					Debug::ThrowError(L"Texture2D", L"Could not load texture: " + fileName);
				}
				texture = textureTEMP;

				// Get image information
				D3DXIMAGE_INFO imageInfo;
				ZeroMemory(&imageInfo, sizeof(D3DXIMAGE_INFO));
				D3DXGetImageInfoFromFile((wchar_t*)fileNamePtr.ToPointer(), &imageInfo);
				size = Size2(imageInfo.Width, imageInfo.Height);
			}
			else
			{
				uint mipLvls = generateMipmaps ? 0 : 1;
				IDirect3DTexture9* textureTEMP = 0;
				if (FAILED(video->Device->CreateTexture(width, height, mipLvls, usage, Video::surfaceFormat(surfaceFormat), pool, &textureTEMP, 0)))
				{
					Debug::ThrowError(L"Texture2D", L"Failed to create Texture2D");
				}
				texture = textureTEMP;
				size = Size2(width, height);
			}

			IDirect3DSurface9* surfaceTEMP;
			texture->GetSurfaceLevel(0, &surfaceTEMP);
			surface = surfaceTEMP;
			texelOffset = (1 / size.ToVector2()) * .5f;
			sizeF = size.ToVector2();

			if (pool == D3DPOOL_DEFAULT && !this->video->IsExDevice && !this->video->deviceReseting)
			{
				video->DeviceLost += gcnew Video::DeviceLostMethod(this, &Texture2D::deviceLost);
				video->DeviceReset += gcnew Video::DeviceLostMethod(this, &Texture2D::deviceReset);
			}
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
		finally
		{
			Marshal::FreeHGlobal(fileNamePtr);
		}
	}

	Texture2D::~Texture2D()
	{
		disposeChilderen();
		if (video)
		{
			video->DeviceLost -= gcnew Video::DeviceLostMethod(this, &Texture2D::deviceLost);
			video->DeviceReset -= gcnew Video::DeviceLostMethod(this, &Texture2D::deviceReset);
		}
		dispose();
	}

	void Texture2D::dispose()
	{
		video->removeActiveTexture(this);
		if (surface) surface->Release();
		if (texture) texture->Release();
		null();
	}

	void Texture2D::null()
	{
		surface = 0;
		texture = 0;
	}

	void Texture2D::deviceLost()
	{
		if (!video->IsExDevice && (pool != D3DPOOL_MANAGED || isRenderTarget_LostDevice)) dispose();
	}

	void Texture2D::deviceReset()
	{
		if (!video->IsExDevice &&( pool != D3DPOOL_MANAGED || isRenderTarget_LostDevice)) init(video, fileName_LostDevice, width_LostDevice, height_LostDevice, generateMipmaps_LostDevice, multiSampleType_LostDevice, surfaceFormat_LostDevice, renderTargetUsage_LostDevice, isRenderTarget_LostDevice, lockable_LostDevice);
	}
	#pragma endregion

	#pragma region Methods
	void Texture2D::Update(array<byte>^ data)
	{
		throw gcnew NotImplementedException();
	}

	void Texture2D::UpdateDynamic(array<byte>^ data)
	{
		throw gcnew NotImplementedException();
	}

	void Texture2D::Copy(Texture2DI^ texture)
	{
		throw gcnew NotImplementedException();
	}

	array<System::Byte>^ Texture2D::Copy()
	{
		throw gcnew NotImplementedException();
	}
	#pragma endregion
}
}
}