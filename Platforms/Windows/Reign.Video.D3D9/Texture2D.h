#pragma once
#include "Video.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class Texture2D : Disposable, Texture2DI
	{
		#pragma region Properties
		private: bool loaded;
		public: property bool Loaded {virtual bool get();}
		protected: Video^ video;

		private: D3DPOOL pool;
		private: Image^ image_LostDevice;
		private: int width_LostDevice, height_LostDevice, generateMipmaps_LostDevice;
		private: MultiSampleTypes multiSampleType_LostDevice;
		private: SurfaceFormats surfaceFormat_LostDevice;
		private: RenderTargetUsage renderTargetUsage_LostDevice;
		private: BufferUsages usage_LostDevice;
		private: bool isRenderTarget_LostDevice, lockable_LostDevice;

		protected: IDirect3DSurface9* surface;
		public: property IDirect3DSurface9* Surface {IDirect3DSurface9* get();}

		private: IDirect3DTexture9* texture;
		public: property IDirect3DTexture9* Texture {IDirect3DTexture9* get();}

		private: Size2 size;
		public: property Size2 Size {virtual Size2 get();}

		private: Vector2 sizeF;
		public: property Vector2 SizeF {virtual Vector2 get();}

		private: Vector2 texelOffset;
		public: property Vector2 TexelOffset {virtual Vector2 get();}
		#pragma endregion

		#pragma region Constructors
		public: static Texture2DI^ New(DisposableI^ parent, string^ fileName);
		public: Texture2D(DisposableI^ parent, string^ fileName);
		public: Texture2D(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, bool lockable);
		public: Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, bool lockable);
		public: Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat);
		protected: Texture2D(DisposableI^ parent, string^ fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable);
		protected: Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, bool lockable);
		protected: Texture2D(DisposableI^ parent, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage);
		internal: void load(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable);
		protected: virtual void init(DisposableI^ parent, Image^ image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable);
		public: ~Texture2D();
		protected: virtual void dispose();
		private: void null();
		private: void deviceLost();
		private: void deviceReset();
		#pragma endregion

		#pragma region Methods
		public: virtual void Copy(Texture2DI^ texture);
		public: virtual void Update(array<byte>^ data);
		public: virtual void WritePixels(array<byte>^ data);
		public: virtual void ReadPixels(array<System::Byte>^ data);
		public: virtual void ReadPixels(array<Color4>^ colors);
		public: virtual bool ReadPixel(Point position, [Out] Color4% color);
		#pragma endregion
	};
}
}
}