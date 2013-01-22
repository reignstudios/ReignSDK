#include "pch.h"
#include "Direct3DContentProvider.h"

namespace Reign_Video_D3D11_Component
{
	Direct3DContentProvider::Direct3DContentProvider(ID3D11Texture2D* renderTexture)
	{
		this->renderTexture = renderTexture;
	}

	HRESULT Direct3DContentProvider::Connect(_In_ IDrawingSurfaceRuntimeHostNative* host)
	{
		if (FAILED(Host->CreateSynchronizedTexture(renderTexture, &SynchronizedTexture)))
		{
			// Fail here !!!!
		}

		Host = host;
		//host->RequestAdditionalFrame();
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
		//return m_controller->PrepareResources(presentTargetTime, contentDirty);
		return S_OK;
	}

	HRESULT Direct3DContentProvider::GetTexture(_In_ const DrawingSurfaceSizeF* size, _Out_ IDrawingSurfaceSynchronizedTextureNative** synchronizedTexture, _Out_ DrawingSurfaceRectF* textureSubRectangle)
	{
		HRESULT hr = S_OK;

		/*if (!m_synchronizedTexture)
		{
			hr = m_host->CreateSynchronizedTexture(m_controller->GetTexture(), &m_synchronizedTexture);
		}*/

		// Set output parameters.
		textureSubRectangle->left = 0.0f;
		textureSubRectangle->top = 0.0f;
		textureSubRectangle->right = static_cast<FLOAT>(size->width);
		textureSubRectangle->bottom = static_cast<FLOAT>(size->height);

		SynchronizedTexture.CopyTo(synchronizedTexture);

		// Draw to the texture.
		//if (SUCCEEDED(hr))
		//{
			hr = SynchronizedTexture->BeginDraw();
		
			if (SUCCEEDED(hr))
			{
				//hr = m_controller->GetTexture(size, synchronizedTexture, textureSubRectangle);
				// RENDER HERE!!!
			}

			SynchronizedTexture->EndDraw();
		//}

		//Host->RequestAdditionalFrame();

		return hr;
	}
}