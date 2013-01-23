#pragma once
#include <wrl/module.h>
#include <Windows.Phone.Graphics.Interop.h>
#include <DrawingSurfaceNative.h>

using namespace Microsoft::WRL;
using namespace ABI::Windows::Phone::Graphics::Interop;

namespace Reign_Video_D3D11_Component
{
	class Direct3DContentProvider : public RuntimeClass<RuntimeClassFlags<WinRtClassicComMix>, IDrawingSurfaceContentProvider, IDrawingSurfaceContentProviderNative>
	{
		#pragma region Properties
		public: ComPtr<IDrawingSurfaceRuntimeHostNative> Host;
		public: ComPtr<IDrawingSurfaceSynchronizedTextureNative> SynchronizedTexture;
		private: ID3D11Texture2D* renderTexture;
		#pragma endregion

		#pragma region Constructors
		public: Direct3DContentProvider(ID3D11Texture2D* renderTexture);
		public: void CreateSynchronizedTexture();
		public: void ReleaseD3DResources();
		#pragma endregion

		#pragma region Methods
		public: HRESULT STDMETHODCALLTYPE Connect(_In_ IDrawingSurfaceRuntimeHostNative* host);
		public: void STDMETHODCALLTYPE Disconnect();

		public: HRESULT STDMETHODCALLTYPE PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Out_ BOOL* contentDirty);
		public: HRESULT STDMETHODCALLTYPE GetTexture(_In_ const DrawingSurfaceSizeF* size, _Out_ IDrawingSurfaceSynchronizedTextureNative** synchronizedTexture, _Out_ DrawingSurfaceRectF* textureSubRectangle);
		#pragma endregion
	};
}