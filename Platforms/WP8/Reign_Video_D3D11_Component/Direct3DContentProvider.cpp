#include "pch.h"
#include "Direct3DContentProvider.h"
#include "RenderDelegateObject.h"

namespace Reign_Video_D3D11_Component
{
	Direct3DContentProvider::Direct3DContentProvider(ID3D11Texture2D* renderTexture)
	{
		this->renderTexture = renderTexture;
	}

	void Direct3DContentProvider::CreateSynchronizedTexture()
	{
		
	}

	HRESULT Direct3DContentProvider::Connect(_In_ IDrawingSurfaceRuntimeHostNative* host)
	{
		Host = host;
		return S_OK;
	}

	void Direct3DContentProvider::Disconnect()
	{
		Host = nullptr;
		SynchronizedTexture = nullptr;
		renderTexture = nullptr;
	}

	HRESULT Direct3DContentProvider::PrepareResources(_In_ const LARGE_INTEGER* presentTargetTime, _Out_ BOOL* contentDirty)
	{
		*contentDirty = true;
		return S_OK;
	}

	HRESULT Direct3DContentProvider::GetTexture(_In_ const DrawingSurfaceSizeF* size, _Out_ IDrawingSurfaceSynchronizedTextureNative** synchronizedTexture, _Out_ DrawingSurfaceRectF* textureSubRectangle)
	{
		if (this->SynchronizedTexture == nullptr)
		{
			if (FAILED(Host->CreateSynchronizedTexture(renderTexture, &this->SynchronizedTexture)))
			{
				return S_FALSE;
			}
		}

		// Set output parameters.
		textureSubRectangle->left = 0.0f;
		textureSubRectangle->top = 0.0f;
		textureSubRectangle->right = static_cast<FLOAT>(size->width);
		textureSubRectangle->bottom = static_cast<FLOAT>(size->height);

		// Draw to the texture.
		if (SUCCEEDED(SynchronizedTexture->BeginDraw())) RenderDelegateObject::Render();
		SynchronizedTexture->EndDraw();

		SynchronizedTexture.CopyTo(synchronizedTexture);
		Host->RequestAdditionalFrame();

		return S_OK;
	}
}