#include "pch.h"
#include "Texture2D.h"
#include <d3dx9.h>

using namespace System::Runtime::InteropServices;
using namespace Reign::Core;

namespace Reign
{namespace Video
{namespace D3D9
{
	ref class Texture2DStreamLoader : StreamLoaderI
	{
		private: Texture2D^ texture;
		private: DisposableI^ parent;
		private: string^ fileName;
		private: int width, height;
		private: bool generateMipmaps;
		private: MultiSampleTypes multiSampleType;
		private: SurfaceFormats surfaceFormat;
		private: RenderTargetUsage renderTargetUsage;
		private: BufferUsages usage;
		private: bool isRenderTarget, lockable;

		private: Image^ image;

		public: Texture2DStreamLoader(Texture2D^ texture, DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable)
		{
			image == nullptr;
			this->texture = texture;
			this->parent = parent;
			this->fileName = fileName;
			this->width = width;
			this->height = height;
			this->generateMipmaps = generateMipmaps;
			this->multiSampleType = multiSampleType;
			this->surfaceFormat = surfaceFormat;
			this->renderTargetUsage = renderTargetUsage;
			this->usage = usage;
			this->isRenderTarget = isRenderTarget;
			this->lockable = lockable;
		}

		public: virtual bool Load() override
		{
			if (image == nullptr)
			{
				image = Image::Load(fileName, false);
				return false;
			}
			else if (!image->Loaded)
			{
				return false;
			}

			texture->load(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget, lockable);
			return true;
		}
	};

	#pragma region Properties
	bool Texture2D::Loaded::get() {return loaded;}

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
		gcnew Texture2DStreamLoader(this, parent, fileName, 0, 0, false, MultiSampleTypes::None, SurfaceFormats::RGBAx8, RenderTargetUsage::PlatformDefault, BufferUsages::Default, false, false);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, false, MultiSampleTypes::None, surfaceFormat, RenderTargetUsage::PlatformDefault, usage, false, false);
	}

	Texture2D::Texture2D(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, bool lockable)
	: Disposable(parent)
	{
		gcnew Texture2DStreamLoader(this, parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage::PlatformDefault, BufferUsages::Default, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, bool lockable)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage::PlatformDefault, BufferUsages::Default, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, RenderTargetUsage::PlatformDefault, BufferUsages::Default, false, false);
	}

	Texture2D::Texture2D(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable)
	: Disposable(parent)
	{
		gcnew Texture2DStreamLoader(this, parent, fileName, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, BufferUsages::Default, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, BufferUsages::Default, false, lockable);
	}

	Texture2D::Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage)
	: Disposable(parent)
	{
		init(parent, nullptr, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, BufferUsages::Default, false, false);
	}

	void Texture2D::load(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable)
	{
		init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, isRenderTarget, lockable);
	}

	void Texture2D::init(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable)
	{
		null();

		image_LostDevice = image;
		width_LostDevice = width;
		height_LostDevice = height;
		generateMipmaps_LostDevice = generateMipmaps;
		multiSampleType_LostDevice = multiSampleType;
		surfaceFormat_LostDevice = surfaceFormat;
		renderTargetUsage_LostDevice = renderTargetUsage;
		usage_LostDevice = usage;
		isRenderTarget_LostDevice = isRenderTarget;
		lockable_LostDevice = lockable;

		try
		{
			video = parent->FindParentOrSelfWithException<Video^>();

			pool = D3DPOOL_DEFAULT;
			DWORD d3dUsage = 0;
			if (isRenderTarget)
			{
				pool = D3DPOOL_DEFAULT;
				d3dUsage = D3DUSAGE_RENDERTARGET;
			}
			
			if (image)
			{
				if (video->IsExDevice) d3dUsage |= (usage == BufferUsages::Write || usage == BufferUsages::Read) ? D3DUSAGE_DYNAMIC : 0;
				else pool = D3DPOOL_MANAGED;

				IDirect3DTexture9* textureTEMP = 0;
				if (FAILED(video->Device->CreateTexture(image->Size.Width, image->Size.Height, image->Mipmaps->Length, d3dUsage, Video::surfaceFormat(image->SurfaceFormat), pool, &textureTEMP, 0)))
				{
					Debug::ThrowError(L"Texture2D", L"Failed to create texture");
				}
				texture = textureTEMP;
				size = image->Size;

				IDirect3DTexture9* systemTexture = 0;
				if (FAILED(video->Device->CreateTexture(image->Size.Width, image->Size.Height, image->Mipmaps->Length, 0, Video::surfaceFormat(image->SurfaceFormat), D3DPOOL_SYSTEMMEM, &systemTexture, 0)))
				{
					Debug::ThrowError(L"Texture2D", L"Failed to create system texture");
				}

				for (int i = 0; i != image->Mipmaps->Length; ++i)
				{
					Image::Mipmap^ mipmap = image->Mipmaps[i];

					D3DLOCKED_RECT rect;
					systemTexture->LockRect(i, &rect, NULL, D3DLOCK_DISCARD);
					auto mipmapData = image->Compressed ? mipmap->Data : mipmap->SwapRBColorChannels();
					pin_ptr<byte> srcData = &mipmapData[0];
					memcpy(rect.pBits, srcData, mipmap->Data->Length);
					systemTexture->UnlockRect(i);
				}
				video->Device->UpdateTexture(systemTexture, textureTEMP);
				systemTexture->Release();

				// release image if not needed
				if (video->IsExDevice || pool == D3DPOOL_MANAGED) image_LostDevice = nullptr;
			}
			else
			{
				d3dUsage |= (usage == BufferUsages::Write || usage == BufferUsages::Read) ? D3DUSAGE_DYNAMIC : 0;

				uint mipLvls = generateMipmaps ? 0 : 1;
				IDirect3DTexture9* textureTEMP = 0;
				if (FAILED(video->Device->CreateTexture(width, height, mipLvls, d3dUsage, Video::surfaceFormat(surfaceFormat), pool, &textureTEMP, 0)))
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

		loaded = true;
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
		if (video) video->removeActiveTexture(this);
		if (surface) surface->Release();
		if (texture) texture->Release();
		null();
	}

	void Texture2D::null()
	{
		loaded = false;
		surface = 0;
		texture = 0;
		video = nullptr;
	}

	void Texture2D::deviceLost()
	{
		if (!video->IsExDevice && (pool != D3DPOOL_MANAGED || isRenderTarget_LostDevice)) dispose();
	}

	void Texture2D::deviceReset()
	{
		if (!video->IsExDevice && (pool != D3DPOOL_MANAGED || isRenderTarget_LostDevice)) init(video, image_LostDevice, width_LostDevice, height_LostDevice, generateMipmaps_LostDevice, multiSampleType_LostDevice, surfaceFormat_LostDevice, renderTargetUsage_LostDevice, usage_LostDevice, isRenderTarget_LostDevice, lockable_LostDevice);
	}
	#pragma endregion

	#pragma region Methods
	void Texture2D::Copy(Texture2DI^ texture)
	{
		Texture2D^ textureTEMP = (Texture2D^)texture;
		video->Device->StretchRect(surface, NULL, textureTEMP->surface, NULL, D3DTEXF_NONE);
	}

	void Texture2D::Update(array<byte>^ data)
	{
		IDirect3DTexture9* systemTexture = 0;
		if (FAILED(video->Device->CreateTexture(Size.Width, Size.Height, 1, 0, Video::surfaceFormat(surfaceFormat_LostDevice), D3DPOOL_SYSTEMMEM, &systemTexture, 0)))
		{
			Debug::ThrowError(L"Texture2D", L"Failed to create system texture");
		}

		D3DLOCKED_RECT rect;
		systemTexture->LockRect(0, &rect, NULL, D3DLOCK_DISCARD);
		pin_ptr<byte> srcData = &data[0];
		memcpy(rect.pBits, srcData, data->Length);
		systemTexture->UnlockRect(0);

		video->Device->UpdateTexture(systemTexture, texture);
		systemTexture->Release();
	}

	void Texture2D::WritePixels(array<byte>^ data)
	{
		D3DLOCKED_RECT rect;
		texture->LockRect(0, &rect, NULL, D3DLOCK_DISCARD);
		pin_ptr<byte> srcData = &data[0];
		memcpy(rect.pBits, srcData, data->Length);
		texture->UnlockRect(0);
	}

	void Texture2D::ReadPixels(array<System::Byte>^ data)
	{
		D3DLOCKED_RECT rect;
		texture->LockRect(0, &rect, NULL, D3DLOCK_READONLY);
		pin_ptr<byte> dstData = &data[0];
		memcpy(dstData, rect.pBits, data->Length);
		texture->UnlockRect(0);
	}

	void Texture2D::ReadPixels(array<Color4>^ colors)
	{
		D3DLOCKED_RECT rect;
		texture->LockRect(0, &rect, NULL, D3DLOCK_READONLY);
		pin_ptr<Color4> dstData = &colors[0];
		memcpy(dstData, rect.pBits, colors->Length * 4);
		texture->UnlockRect(0);
	}

	bool Texture2D::ReadPixel(Point2 position, [Out] Color4% color)
	{
		if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
		{
			color = Color4();
			return false;
		}

		D3DLOCKED_RECT rect;
		texture->LockRect(0, &rect, NULL, D3DLOCK_READONLY);
			const byte* colors = (byte*)rect.pBits;
			int index = (position.X * 4) + ((Size.Height-1-position.Y) * rect.Pitch);
			int colorValue = colors[index] << 16;
			colorValue |= colors[index+1] << 8;
			colorValue |= colors[index+2];
			colorValue |= colors[index+3] << 24;
		texture->UnlockRect(0);

		color = Color4(colorValue);
		return true;
	}
	#pragma endregion
}
}
}