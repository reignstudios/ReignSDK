#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class TextureError
	{
		None,
		Texture,
		SystemTexture
	};

	public ref class Texture2DCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		internal: IDirect3DSurface9* surface;
		internal: IDirect3DTexture9* texture;
		#pragma endregion

		#pragma region Constructors
		public: TextureError Init(VideoCom^ video, int width, int height, bool generateMipmaps, array<array<byte>^>^ mipmaps, array<int>^ mipmapSizes, array<int>^ mipmapPitches, int multiSampleMultiple, REIGN_D3DPOOL pool, REIGN_D3DUSAGE usage, REIGN_D3DFORMAT format, bool isRenderTarget);
		public: ~Texture2DCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Copy(Texture2DCom^ texture);
		public: void Update(array<byte>^ data, int width, int height, REIGN_D3DFORMAT surfaceFormat);
		public: void WritePixels(array<byte>^ data);
		#pragma endregion
	};
}