#include "pch.h"
#include "Video.h"
#include "DepthStencil\DepthStencil.h"

#if METRO
#include <client.h>
using namespace Microsoft::WRL;
#endif

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	#if WINDOWS
	VideoError VideoCom::Init(IntPtr handle, bool vSync, int width, int height, bool fullscreen, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel)
	#else
	VideoError VideoCom::Init(IntPtr handle, bool vSync, int width, int height, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel)
	#endif
	{
		null();

		#if WINDOWS
		if (width == 0 || height == 0)
		{
			RECT rect;
			GetClientRect((HWND)handle.ToInt32(), &rect);
			width = rect.right - rect.left;
			height = rect.bottom - rect.top;
		}
		#endif

		lastWidth = width;
		lastHeight = height;

		currentVertexResourceCount = 8;
		currentPixelResourceCount = currentVertexResourceCount;
		currentVertexResources = new ID3D11ShaderResourceView*[currentVertexResourceCount];
		currentPixelResources = new ID3D11ShaderResourceView*[currentPixelResourceCount];
		for (int i = 0; i != currentVertexResourceCount; ++i)
		{
			currentVertexResources[i] = 0;
			currentPixelResources[i] = 0;
		}

		// Create Device and SwapShain
		#if WINDOWS
		DXGI_SWAP_CHAIN_DESC swapChainDesc;
		ZeroMemory(&swapChainDesc, sizeof(swapChainDesc));
		swapChainDesc.BufferDesc.Width = width;
		swapChainDesc.BufferDesc.Height = height;
		swapChainDesc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		swapChainDesc.Windowed = !fullscreen;
		swapChainDesc.OutputWindow = (HWND)handle.ToInt32();
		swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
		#else
		DXGI_SWAP_CHAIN_DESC1 swapChainDesc;
		ZeroMemory(&swapChainDesc, sizeof(swapChainDesc));
		swapChainDesc.Width = width;
		swapChainDesc.Height = height;
		swapChainDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		swapChainDesc.Stereo = false;
		swapChainDesc.Scaling = DXGI_SCALING_NONE;
		swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
		#endif
		swapChainDesc.BufferCount = 2;
		swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		swapChainDesc.SampleDesc.Count = 1;
		swapChainDesc.SampleDesc.Quality = 0;

		#if WINDOWS
		const int featureCount = 6;
		#else
		const int featureCount = 7;
		#endif
		D3D_FEATURE_LEVEL featureLevelTypes[featureCount] =
		{
			#if METRO
			D3D_FEATURE_LEVEL_11_1,
			#endif
			D3D_FEATURE_LEVEL_11_0,
			D3D_FEATURE_LEVEL_10_1,
			D3D_FEATURE_LEVEL_10_0,
			D3D_FEATURE_LEVEL_9_3,
			D3D_FEATURE_LEVEL_9_2,
			D3D_FEATURE_LEVEL_9_1,
		};
		D3D_FEATURE_LEVEL featureLevelType = D3D_FEATURE_LEVEL_9_1;

		#if WINDOWS
		ID3D11Device* deviceTEMP;
		ID3D11DeviceContext* deviceContextTEMP;
		IDXGISwapChain* swapChainTEMP;

		if (FAILED(D3D11CreateDeviceAndSwapChain(0, D3D_DRIVER_TYPE_HARDWARE, 0, 0, featureLevelTypes, featureCount, D3D11_SDK_VERSION, &swapChainDesc, &swapChainTEMP, &deviceTEMP, &featureLevelType, &deviceContextTEMP)))
		{
			return VideoError::DeviceAndSwapChainFailed;
		}
		device = deviceTEMP;
		deviceContext = deviceContextTEMP;
		swapChain = swapChainTEMP;
		#else
		ComPtr<ID3D11Device> deviceTEMP;
		ComPtr<ID3D11DeviceContext> deviceContextTEMP;
		IDXGISwapChain1* swapChainTEMP;

		if (FAILED(D3D11CreateDevice(0, D3D_DRIVER_TYPE_HARDWARE, 0, 0, featureLevelTypes, featureCount, D3D11_SDK_VERSION, &deviceTEMP, &featureLevelType, &deviceContextTEMP)))
		{
			return VideoError::DeviceFailed;
		}
		device = deviceTEMP.Get();
		deviceContext = deviceContextTEMP.Get();

		ComPtr<ID3D11Device1> device1;
		deviceTEMP.As(&device1);
		ComPtr<IDXGIDevice1> dxgiDevice;
		device1.As(&dxgiDevice);
		ComPtr<IDXGIAdapter> dxgiAdapter;
		dxgiDevice->GetAdapter(&dxgiAdapter);
		ComPtr<IDXGIFactory2> dxgiFactory;
		dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory));
		if (FAILED(dxgiFactory->CreateSwapChainForCoreWindow(deviceTEMP.Get(), reinterpret_cast<IUnknown*>(handle.ToPointer()), &swapChainDesc, 0, &swapChainTEMP)))
		{
			return VideoError::SwapChainFailed;
		}
		swapChain = swapChainTEMP;
		#endif

		this->vSync = vSync ? 1 : 0;
		#if WINDOWS
		featureLevel = (REIGN_D3D_FEATURE_LEVEL)featureLevelType;
		#else
		*featureLevel = (REIGN_D3D_FEATURE_LEVEL)featureLevelType;
		#endif

		// create renderTarget and depthStencil views
		auto error = createViews(width, height);
		if (error != VideoError::None) return error;

		// Apply Defaults
		EnableRenderTarget();

		return VideoError::None;
	}

	VideoError VideoCom::createViews(int width, int height)
	{
		// RenterTarget View
		#if WINDOWS
		ID3D11Texture2D* backBuffer = 0;
		if (FAILED(swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (void**)&backBuffer)))
		#else
		ComPtr<ID3D11Texture2D> backBufferPtr;
		if (FAILED(swapChain->GetBuffer(0, IID_PPV_ARGS(&backBufferPtr))))
		#endif
		{
			return VideoError::GetSwapChainFailed;
		}

		#if METRO
		ID3D11Texture2D* backBuffer = backBufferPtr.Get();
		#endif

		ID3D11RenderTargetView* renderTargetTEMP;
		if (FAILED(device->CreateRenderTargetView(backBuffer, 0, &renderTargetTEMP)))
		{
			return VideoError::RenderTargetViewFailed;
		}
		renderTarget = renderTargetTEMP;
		if (backBuffer) backBuffer->Release();
		
		// DepthStencil Texture
		D3D11_TEXTURE2D_DESC descDepth;
		ZeroMemory(&descDepth, sizeof(descDepth));
		descDepth.Width = width;
		descDepth.Height = height;
		descDepth.MipLevels = 1;
		descDepth.ArraySize = 1;
		descDepth.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;//DXGI_FORMAT_D32_FLOAT;
		descDepth.SampleDesc.Count = 1;
		descDepth.SampleDesc.Quality = 0;
		descDepth.Usage = D3D11_USAGE_DEFAULT;
		descDepth.BindFlags = D3D11_BIND_DEPTH_STENCIL;
		descDepth.CPUAccessFlags = 0;
		descDepth.MiscFlags = 0;
		ID3D11Texture2D* depthTextureTEMP = 0;
		if (FAILED(device->CreateTexture2D(&descDepth, 0, &depthTextureTEMP)))
		{
			return VideoError::DepthStencilTextureFailed;
		}
		depthTexture = depthTextureTEMP;
		
		// DepthStencil View
		D3D11_DEPTH_STENCIL_VIEW_DESC descDSV;
		ZeroMemory(&descDSV, sizeof(descDSV));
		descDSV.Format = descDepth.Format;
		if (descDepth.SampleDesc.Count > 1) descDSV.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2DMS;
		else descDSV.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
		descDSV.Texture2D.MipSlice = 0;
		ID3D11DepthStencilView* depthStencilTEMP;
		if (FAILED(device->CreateDepthStencilView(depthTexture, &descDSV, &depthStencilTEMP)))
		{
			return VideoError::DepthStencilViewFailed;
		}
		depthStencil = depthStencilTEMP;
		if (depthTexture) depthTexture->Release();

		return VideoError::None;
	}

	VideoCom::~VideoCom()
	{
		if (deviceContext) deviceContext->ClearState();
		
		if (currentVertexResources) delete currentVertexResources;
		if (currentPixelResources) delete currentPixelResources;
		if (renderTarget) renderTarget->Release();
		if (depthStencil) depthStencil->Release();
		if (depthTexture) depthTexture->Release();
		if (swapChain) swapChain->Release();
		if (deviceContext) deviceContext->Release();
		if (device) device->Release();
		null();
	}

	void VideoCom::null()
	{
		currentVertexResources = 0;
		currentPixelResources = 0;
		renderTarget = 0;
		depthStencil = 0;
		depthTexture = 0;
		swapChain = 0;
		device = 0;
		deviceContext = 0;
	}
	#pragma endregion

	#pragma region Methods
	void VideoCom::Update(int width, int height)
	{
		if (lastWidth != width && lastHeight != height && width != 0 && height != 0)
		{
			renderTarget->Release();
			depthStencil->Release();
			depthTexture->Release();
			swapChain->ResizeBuffers(1, width, height, DXGI_FORMAT_R8G8B8A8_UNORM, 0);
			createViews(width, height);
			lastWidth = width;
			lastHeight = height;
		}
	}

	void VideoCom::EnableRenderTarget()
	{
		ID3D11RenderTargetView* renderTargetTEMP = renderTarget;
		deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, depthStencil);
		currentRenderTarget = renderTarget;
		currentDepthStencil = depthStencil;
	}

	void VideoCom::EnableRenderTarget(DepthStencilCom^ depthStencil)
	{
		ID3D11RenderTargetView* renderTargetTEMP = renderTarget;
		currentRenderTarget = renderTarget;

		if (depthStencil)
		{
			currentDepthStencil = depthStencil->surface;
			deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, depthStencil->surface);
		}
		else
		{
			deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, 0);
		}
	}

	void VideoCom::Present()
	{
		swapChain->Present(vSync, 0);
	}

	void VideoCom::Clear(float r, float g, float b, float a)
	{
		float clearColor[4] = {r, g, b, a};
		deviceContext->ClearRenderTargetView(currentRenderTarget, clearColor);
		deviceContext->ClearDepthStencilView(currentDepthStencil, D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);
	}

	void VideoCom::ClearColor(float r, float g, float b, float a)
	{
		float clearColor[4] = {r, g, b, a};
		deviceContext->ClearRenderTargetView(currentRenderTarget, clearColor);
	}

	void VideoCom::ClearDepthStencil()
	{
		deviceContext->ClearDepthStencilView(currentDepthStencil, D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);
	}

	void VideoCom::removeActiveResource(ID3D11ShaderResourceView* shaderResource)
	{
		if (currentVertexResourceCount != 0)
		{
			bool found = false;
			for (int i = 0; i != currentVertexResourceCount; ++i)
			{
				if (currentVertexResources[i] == shaderResource)
				{
					currentVertexResources[i] = 0;
					found = true;
				}
			}
			if(found) deviceContext->VSSetShaderResources(0, currentVertexResourceCount, currentVertexResources);
		}

		if (currentPixelResourceCount != 0)
		{
			bool found = false;
			for (int i = 0; i != currentPixelResourceCount; ++i)
			{
				if (currentPixelResources[i] == shaderResource)
				{
					currentPixelResources[i] = 0;
					found = true;
				}
			}
			if (found) deviceContext->PSSetShaderResources(0, currentPixelResourceCount, currentPixelResources);
		}
	}
	#pragma endregion
}