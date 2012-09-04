#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class TextureError
	{
		None,
		Texture,
		ShaderResourceView
	};

	public ref class Texture2DCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		internal: ID3D11Texture2D* texture;
		internal: ID3D11ShaderResourceView* shaderResource;
		#pragma endregion
	
		#pragma region Constructors
		public: TextureError Init(VideoCom^ video, int width, int height, bool generateMipmaps, const array<__int64>^ mipmaps, const array<int>^ mipmapSizes, const array<int>^ mipmapPitches, int multiSampleMultiple, REIGN_DXGI_FORMAT surfaceFormat, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage, bool isRenderTarget);
		public: virtual ~Texture2DCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		/*public: virtual void Update(array<byte>^ data);
		public: virtual void UpdateDynamic(array<byte>^ data);
		public: virtual void Copy(Texture2DI^ texture);
		public: virtual array<System::Byte>^ Copy();*/
		#pragma endregion
	};
}