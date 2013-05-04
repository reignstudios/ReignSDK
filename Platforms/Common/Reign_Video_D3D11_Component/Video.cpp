#include "pch.h"
#include "Video.h"
#include "DepthStencil\DepthStencil.h"

#if WINRT || WP8
#include <client.h>
using namespace Microsoft::WRL;
#endif

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	#if WIN32
	VideoError VideoCom::Init(IntPtr handle, bool vSync, int width, int height, int depthBit, int stencilBit, bool fullscreen, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel)
	#elif WINRT
	VideoError VideoCom::Init(Windows::UI::Core::CoreWindow^ coreWindow, bool vSync, int width, int height, int depthBit, int stencilBit, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel, Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^ swapChainBackgroundPanel)
	#else
	VideoError VideoCom::Init(bool vSync, int width, int height, int depthBit, int stencilBit, OutType(REIGN_D3D_FEATURE_LEVEL) featureLevel, RenderDelegate^ renderDelegate)
	#endif
	{
		null();

		#if WP8
		RenderDelegateObject::render += renderDelegate;
		#endif

		#if WINRT
		compositionMode = swapChainBackgroundPanel != nullptr;
		#endif
		#if WP8
		compositionMode = true;
		#endif

		lastWidth = width;
		lastHeight = height;
		this->depthBit = depthBit;
		this->stencilBit = stencilBit;

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
		#if WIN32
		DXGI_SWAP_CHAIN_DESC swapChainDesc;
		ZeroMemory(&swapChainDesc, sizeof(DXGI_SWAP_CHAIN_DESC));
		swapChainDesc.BufferDesc.Width = width;
		swapChainDesc.BufferDesc.Height = height;
		swapChainDesc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
		swapChainFromat = DXGI_FORMAT_R8G8B8A8_UNORM;
		swapChainDesc.Windowed = !fullscreen;
		swapChainDesc.OutputWindow = (HWND)handle.ToInt32();
		swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
		#elif WINRT
		DXGI_SWAP_CHAIN_DESC1 swapChainDesc;
		ZeroMemory(&swapChainDesc, sizeof(DXGI_SWAP_CHAIN_DESC1));
		swapChainDesc.Width = width;
		swapChainDesc.Height = height;
		swapChainDesc.Format = compositionMode ? DXGI_FORMAT_B8G8R8X8_UNORM : DXGI_FORMAT_R8G8B8A8_UNORM;
		swapChainFromat = compositionMode ? DXGI_FORMAT_B8G8R8X8_UNORM : DXGI_FORMAT_R8G8B8A8_UNORM;
		swapChainDesc.Stereo = false;
		swapChainDesc.Scaling = compositionMode ? DXGI_SCALING_STRETCH : DXGI_SCALING_NONE;
		swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL;
		#endif
		#if WIN32 || WINRT
		swapChainDesc.BufferCount = 2;
		swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
		swapChainDesc.SampleDesc.Count = 1;
		swapChainDesc.SampleDesc.Quality = 0;
		#endif

		#if WIN32
		const int featureCount = 6;
		#else
		const int featureCount = 7;
		#endif
		D3D_FEATURE_LEVEL featureLevelTypes[featureCount] =
		{
			#if WINRT || WP8
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

		uint flags = 0;
		#if DEBUG
		flags = D3D11_CREATE_DEVICE_DEBUG;
		#endif

		#if WIN32
		// create device and swapchain
		ID3D11Device* deviceTEMP;
		ID3D11DeviceContext* deviceContextTEMP;
		IDXGISwapChain* swapChainTEMP;

		if (FAILED(D3D11CreateDeviceAndSwapChain(0, D3D_DRIVER_TYPE_HARDWARE, 0, flags, featureLevelTypes, featureCount, D3D11_SDK_VERSION, &swapChainDesc, &swapChainTEMP, &deviceTEMP, &featureLevelType, &deviceContextTEMP)))
		{
			return VideoError::DeviceAndSwapChainFailed;
		}
		device = deviceTEMP;
		deviceContext = deviceContextTEMP;
		swapChain = swapChainTEMP;
		#else
		// create device
		ID3D11Device* deviceTEMP;
		ID3D11DeviceContext* deviceContextTEMP;
		IDXGISwapChain1* swapChainTEMP;
		flags |= compositionMode ? D3D11_CREATE_DEVICE_BGRA_SUPPORT : 0;

		if (FAILED(D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, flags, featureLevelTypes, featureCount, D3D11_SDK_VERSION, &deviceTEMP, &featureLevelType, &deviceContextTEMP)))
		{
			return VideoError::DeviceFailed;
		}
		deviceTEMP->QueryInterface(__uuidof(ID3D11Device1), (void**)&device);
		deviceContextTEMP->QueryInterface(__uuidof(ID3D11DeviceContext1), (void**)&deviceContext);

		#if !WP8
		// create swapchain
		IDXGIDevice1* dxgiDevice;
		deviceTEMP->QueryInterface(__uuidof(IDXGIDevice1), (void**)&dxgiDevice);
		dxgiDevice->SetMaximumFrameLatency(vSync ? 1 : 0);
		IDXGIAdapter* dxgiAdapter;
		dxgiDevice->GetAdapter(&dxgiAdapter);
		IDXGIFactory2* dxgiFactory;
		dxgiAdapter->GetParent(IID_PPV_ARGS(&dxgiFactory));
		if (compositionMode)
		{
			if (FAILED(dxgiFactory->CreateSwapChainForComposition(deviceTEMP, &swapChainDesc, nullptr, &swapChainTEMP)))
			{
				return VideoError::SwapChainFailed;
			}
		}
		else
		{
			if (FAILED(dxgiFactory->CreateSwapChainForCoreWindow(deviceTEMP, reinterpret_cast<IUnknown*>(coreWindow), &swapChainDesc, 0, &swapChainTEMP)))
			{
				return VideoError::SwapChainFailed;
			}
		}
		swapChain = swapChainTEMP;

		// create D2D device
		if (compositionMode)
		{
			D2D1_FACTORY_OPTIONS options;
			ZeroMemory(&options, sizeof(D2D1_FACTORY_OPTIONS));
			ComPtr<ID2D1Factory1> factory;
			if (FAILED(D2D1CreateFactory(D2D1_FACTORY_TYPE_SINGLE_THREADED, __uuidof(ID2D1Factory1), &options, &factory)))
			{
				return VideoError::D2DFactoryFailed;
			}
			d2dFactory = factory.Get();

			ID2D1Device* d2dDeviceTEMP = 0;
			if (FAILED(d2dFactory->CreateDevice(dxgiDevice, &d2dDeviceTEMP)))
			{
				return VideoError::D2DDeviceFailed;
			}
			d2dDevice = d2dDeviceTEMP;

			ID2D1DeviceContext* d2dDeviceContextTEMP = 0;
			if (FAILED(d2dDevice->CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS_NONE, &d2dDeviceContextTEMP)))
			{
				return VideoError::D2DDeviceContextFailed;
			}
			d2dDeviceContext = d2dDeviceContextTEMP;
		}
		#endif
		#endif

		this->vSync = vSync ? 1 : 0;
		#if WIN32
		featureLevel = (REIGN_D3D_FEATURE_LEVEL)featureLevelType;
		#else
		*featureLevel = (REIGN_D3D_FEATURE_LEVEL)featureLevelType;
		#endif

		// create renderTarget and depthStencil views
		auto error = createViews(width, height);
		if (error != VideoError::None) return error;

		// Apply Defaults
		EnableRenderTarget();

		#if WINRT
		// get native swap chain panel
		if (compositionMode)
		{
			ComPtr<ISwapChainBackgroundPanelNative>	m_swapChainNative;
			reinterpret_cast<IInspectable*>(swapChainBackgroundPanel)->QueryInterface(__uuidof(ISwapChainBackgroundPanelNative), (void**)&m_swapChainNative);
			swapChainBackgroundPanelNative = m_swapChainNative.Get();
			if (FAILED(swapChainBackgroundPanelNative->SetSwapChain(swapChain)))
			{
				return VideoError::NativeSwapChainPanelFailed;
			}
		}
		#endif

		return VideoError::None;
	}

	VideoError VideoCom::createViews(int width, int height)
	{
		#if !WP8
		// RenterTarget View
		ID3D11Texture2D* backBuffer = 0;
		if (FAILED(swapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (void**)&backBuffer)))
		{
			return VideoError::GetSwapChainFailed;
		}

		ID3D11RenderTargetView* renderTargetTEMP = 0;
		bool createNormalRenderView = true;
		#if WINRT
		if (compositionMode)
		{
			CD3D11_RENDER_TARGET_VIEW_DESC renderTargetViewDesc(D3D11_RTV_DIMENSION_TEXTURE2DARRAY, DXGI_FORMAT_B8G8R8X8_UNORM, 0, 0, 1);
			if (FAILED(device->CreateRenderTargetView(backBuffer, &renderTargetViewDesc, &renderTargetTEMP)))
			{
				backBuffer->Release();
				return VideoError::RenderTargetViewFailed;
			}
			createNormalRenderView = false;
		}
		#endif
		if (createNormalRenderView)
		{
			if (FAILED(device->CreateRenderTargetView(backBuffer, NULL, &renderTargetTEMP)))
			{
				backBuffer->Release();
				return VideoError::RenderTargetViewFailed;
			}
		}
		renderTarget = renderTargetTEMP;
		backBuffer->Release();
		#else
		CD3D11_TEXTURE2D_DESC renderTargetDesc
		(
			DXGI_FORMAT_B8G8R8X8_UNORM,
			static_cast<UINT>(width),
			static_cast<UINT>(height),
			1,
			1,
			D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE
		);
		renderTargetDesc.MiscFlags = D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX | D3D11_RESOURCE_MISC_SHARED_NTHANDLE;
		if (FAILED(device->CreateTexture2D(&renderTargetDesc, nullptr, &renderTexture)))
		{
			return VideoError::RenderTextureFailed;
		}

		if (FAILED(device->CreateRenderTargetView(renderTexture, nullptr, &renderTarget)))
		{
			return VideoError::RenderTargetViewFailed;
		}

		contentProvider = Make<Direct3DContentProvider>(renderTexture);
		#endif
		
		// DepthStencil Texture
		if (depthBit != 0)
		{
			D3D11_TEXTURE2D_DESC descDepth;
			ZeroMemory(&descDepth, sizeof(descDepth));
			descDepth.Width = width;
			descDepth.Height = height;
			descDepth.MipLevels = 1;
			descDepth.ArraySize = 1;

			descDepth.Format = DXGI_FORMAT_R16_UINT;
			if (depthBit == 24 && (stencilBit == 8 || stencilBit == 0)) descDepth.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
			else if (depthBit == 16 && stencilBit == 0) descDepth.Format = DXGI_FORMAT_D16_UNORM;
			else if (depthBit == 32 && stencilBit == 0) descDepth.Format = DXGI_FORMAT_D32_FLOAT;

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
		}

		#if WINRT
		// D2D renderTarget
		if (compositionMode)
		{
			dpi = Windows::Graphics::Display::DisplayProperties::LogicalDpi;

			D2D1_BITMAP_PROPERTIES1 bitmapProperties;
			ZeroMemory(&bitmapProperties, sizeof(D2D1_BITMAP_PROPERTIES1));
			bitmapProperties.bitmapOptions = D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS_CANNOT_DRAW;
			bitmapProperties.pixelFormat.format = DXGI_FORMAT_B8G8R8A8_UNORM;
			bitmapProperties.pixelFormat.alphaMode = D2D1_ALPHA_MODE_PREMULTIPLIED;
			bitmapProperties.dpiX = dpi;
			bitmapProperties.dpiY = dpi;

			ComPtr<IDXGIResource1> dxgiBackBuffer;
			if (FAILED(swapChain->GetBuffer(0, IID_PPV_ARGS(&dxgiBackBuffer))))
			{
				return VideoError::GetDXGIBackBufferFailed;
			}

			if (FAILED(dxgiBackBuffer->CreateSubresourceSurface(0, &dxgiSurface)))
			{
				dxgiBackBuffer.Get()->Release();
				return VideoError::DXGISurfaceFailed;
			}
			dxgiBackBuffer.Get()->Release();

			if (FAILED(d2dDeviceContext->CreateBitmapFromDxgiSurface(dxgiSurface, &bitmapProperties, &d2dRenderTarget)))
			{
				return VideoError::D2DBitmapFailed;
			}

			d2dDeviceContext->SetDpi(dpi, dpi);
			d2dDeviceContext->SetTarget(d2dRenderTarget);
			d2dDeviceContext->SetTextAntialiasMode(D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE);
		}
		#endif

		return VideoError::None;
	}

	#if WP8
	Windows::Phone::Graphics::Interop::IDrawingSurfaceContentProvider^ VideoCom::GetProvider()
	{
		return reinterpret_cast<Windows::Phone::Graphics::Interop::IDrawingSurfaceContentProvider^>(contentProvider.Detach());
	}
	#endif

	VideoCom::~VideoCom()
	{
		#if WINRT
		if (swapChainBackgroundPanelNative) swapChainBackgroundPanelNative->Release();
		if (d2dFactory) d2dFactory->Release();
		if (dxgiSurface) dxgiSurface->Release();
		if (d2dRenderTarget) d2dRenderTarget->Release();
		if (d2dDevice) d2dDevice->Release();
		/*if (d2dDeviceContext)
		{
			d2dDeviceContext->Flush();
			d2dDeviceContext->Release();
		}*/
		#endif

		if (currentVertexResources) delete currentVertexResources;
		if (currentPixelResources) delete currentPixelResources;
		if (renderTarget) renderTarget->Release();
		if (depthStencil) depthStencil->Release();
		if (depthTexture) depthTexture->Release();
		#if !WP8
		if (swapChain) swapChain->Release();
		#endif
		if (device) device->Release();

		if (deviceContext)
		{
			deviceContext->ClearState();
			deviceContext->Flush();
			deviceContext->Release();
		}

		null();
	}

	void VideoCom::null()
	{
		currentVertexResources = 0;
		currentPixelResources = 0;
		renderTarget = 0;
		depthStencil = 0;
		depthTexture = 0;
		#if !WP8
		swapChain = 0;
		#endif
		device = 0;
		deviceContext = 0;

		#if WINRT
		d2dFactory = 0;
		d2dDevice = 0;
		d2dDeviceContext = 0;
		dxgiSurface = 0;
		d2dRenderTarget = 0;
		swapChainBackgroundPanelNative = 0;
		#endif
	}
	#pragma endregion

	#pragma region Methods
	void VideoCom::Update(int width, int height)
	{
		#if WINRT
		if (compositionMode && dpi != Windows::Graphics::Display::DisplayProperties::LogicalDpi)
		{
			dpi = Windows::Graphics::Display::DisplayProperties::LogicalDpi;
			d2dDeviceContext->SetDpi(dpi, dpi);
		}
		#endif

		if ((lastWidth != width || lastHeight != height) && width != 0 && height != 0)
		{
			renderTarget->Release();
			if (depthStencil) depthStencil->Release();
			if (depthTexture) depthTexture->Release();
			#if WINRT
			if (dxgiSurface) dxgiSurface->Release();
			if (d2dRenderTarget) d2dRenderTarget->Release();
			#endif
			#if !WP8
			swapChain->ResizeBuffers(2, width, height, swapChainFromat, 0);
			#endif
			createViews(width, height);
			lastWidth = width;
			lastHeight = height;
		}
	}

	void VideoCom::EnableRenderTarget()
	{
		currentRenderTarget = renderTarget;
		currentDepthStencil = depthStencil;
		ID3D11RenderTargetView* renderTargetTEMP = renderTarget;
		deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, depthStencil);

		#if WINRT
		if (compositionMode) d2dDeviceContext->SetTarget(d2dRenderTarget);
		#endif
	}

	void VideoCom::EnableRenderTarget(DepthStencilCom^ depthStencil)
	{
		currentRenderTarget = renderTarget;
		currentDepthStencil = depthStencil->surface;
		ID3D11RenderTargetView* renderTargetTEMP = renderTarget;
		deviceContext->OMSetRenderTargets(1, &renderTargetTEMP, depthStencil->surface);

		#if WINRT
		if (compositionMode) d2dDeviceContext->SetTarget(d2dRenderTarget);
		#endif
	}

	void VideoCom::Present()
	{
		#if WIN32
		HRESULT error = swapChain->Present(vSync, 0);
		#elif WINRT
		DXGI_PRESENT_PARAMETERS parameters = {0};
		parameters.DirtyRectsCount = 0;
		parameters.pDirtyRects = nullptr;
		parameters.pScrollRect = nullptr;
		parameters.pScrollOffset = nullptr;

		HRESULT error = swapChain->Present1(vSync, 0, &parameters);
		deviceContext->DiscardView(renderTarget);
		deviceContext->DiscardView(depthStencil);
		#endif

		#if WIN32 || WINRT
		if (error == DXGI_ERROR_DEVICE_REMOVED)
		{
			// TODO: Handle lost device...
		}
		#endif
	}

	void VideoCom::ClearAll(float r, float g, float b, float a)
	{
		const float clearColor[4] = {r, g, b, a};
		deviceContext->ClearRenderTargetView(currentRenderTarget, clearColor);
		deviceContext->ClearDepthStencilView(currentDepthStencil, D3D11_CLEAR_DEPTH | D3D11_CLEAR_STENCIL, 1.0f, 0);
	}

	void VideoCom::ClearColor(float r, float g, float b, float a)
	{
		const float clearColor[4] = {r, g, b, a};
		deviceContext->ClearRenderTargetView(currentRenderTarget, clearColor);
	}

	void VideoCom::ClearColorDepth(float r, float g, float b, float a)
	{
		const float clearColor[4] = {r, g, b, a};
		deviceContext->ClearRenderTargetView(currentRenderTarget, clearColor);
		deviceContext->ClearDepthStencilView(currentDepthStencil, D3D11_CLEAR_DEPTH, 1.0f, 0);
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