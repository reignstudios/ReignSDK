#include "pch.h"
#include "Video.h"
#include "DepthStencil.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	IDirect3D9* createD3D9Ex()
	{
		IDirect3D9Ex* direct3DEx = 0;
		HMODULE libHandle = 0;

		libHandle = LoadLibrary(L"d3d9.dll");
		if(libHandle != 0)
		{
			typedef HRESULT (WINAPI *LPDIRECT3DCREATE9EX)(UINT, IDirect3D9Ex**);
			LPDIRECT3DCREATE9EX Direct3DCreate9ExPtr = 0;
			Direct3DCreate9ExPtr = (LPDIRECT3DCREATE9EX)GetProcAddress(libHandle, "Direct3DCreate9Ex");

			if (Direct3DCreate9ExPtr != 0)
			{
				if (FAILED(Direct3DCreate9ExPtr(D3D_SDK_VERSION, &direct3DEx)))
				{
					direct3DEx = 0;
				}
			}

			FreeLibrary(libHandle);
		}

		return direct3DEx;
	}

	VideoError VideoCom::Init(IntPtr handle, bool vSync, int width, int height, int depthBit, int stencilBit, bool fullscreen, bool multithreaded, [Out] ComponentCaps^% caps)
	{
		null();
		IDirect3D9* direct3D = 0;

		caps = gcnew ComponentCaps();
		this->caps = caps;
		this->handle = (HWND)handle.ToPointer();
		this->fullScreen = fullScreen;
		this->vSync = vSync;
		lastWidth = width;
		lastHeight = height;
		this->depthBit = depthBit;
		this->stencilBit = stencilBit;

		//Create D3D Object
		direct3D = createD3D9Ex();
		if (direct3D != 0)
		{
			isExDevice = true;
			caps->ExDevice = true;
		}
		else
		{
			direct3D = Direct3DCreate9(D3D_SDK_VERSION);
			if (direct3D == 0) return VideoError::Direct3DInterfaceFailed;
			isExDevice = false;
			caps->ExDevice = false;
		}

		D3DDISPLAYMODE displayMode;
		if (FAILED(direct3D->GetAdapterDisplayMode(D3DADAPTER_DEFAULT, &displayMode))) return VideoError::AdapterDisplayModeFailed;

		D3DDEVTYPE deviceType = D3DDEVTYPE_HAL;
		if (FAILED(direct3D->CheckDeviceType(0, deviceType, D3DFMT_X8R8G8B8, D3DFMT_X8R8G8B8, !fullScreen))) return VideoError::VideoHardwareNotSupported;

		/*if (FAILED(direct3D->CheckDeviceFormat(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, D3DFMT_X8R8G8B8, 0, D3DRTYPE_SURFACE, D3DFMT_D16)))//D3DFMT_D16 D3DFMT_D24FS8 displayMode.Format
		{
			return VideoError::VideoHardwareNotSupported;
		}*/

		//Get Caps
		D3DCAPS9 d3dCaps;
		if (FAILED(direct3D->GetDeviceCaps(0, deviceType, &d3dCaps)))
		{
			return VideoError::GetCapsFailed;
		}

		caps->MaxTextureCount = d3dCaps.MaxSimultaneousTextures;

		caps->MaxVertexShaderVersion = 3.0f;
		if (d3dCaps.VertexShaderVersion >= D3DVS_VERSION(3, 0)) caps->MaxVertexShaderVersion = 3.0f;
		else if (d3dCaps.VertexShaderVersion >= D3DVS_VERSION(2, 1)) caps->MaxVertexShaderVersion = 2.1f;
		else if (d3dCaps.VertexShaderVersion >= D3DVS_VERSION(2, 0)) caps->MaxVertexShaderVersion = 2.0f;

		caps->MaxPixelShaderVersion = 3.0f;
		if (d3dCaps.PixelShaderVersion >= D3DPS_VERSION(3, 0)) caps->MaxPixelShaderVersion = 3.0f;
		else if (d3dCaps.PixelShaderVersion >= D3DPS_VERSION(2, 1)) caps->MaxPixelShaderVersion = 2.1f;
		else if (d3dCaps.PixelShaderVersion >= D3DPS_VERSION(2, 0)) caps->MaxPixelShaderVersion = 2.0f;

		caps->MaxShaderVersion = (d3dCaps.VertexShaderVersion <= caps->MaxPixelShaderVersion) ? caps->MaxVertexShaderVersion : caps->MaxPixelShaderVersion;
		caps->HardwareInstancing = caps->MaxVertexShaderVersion == 3.0f;

		//Set Flags
		DWORD flags = 0;
		if ((d3dCaps.DevCaps & D3DDEVCAPS_HWTRANSFORMANDLIGHT) != 0) flags |= D3DCREATE_HARDWARE_VERTEXPROCESSING;
		else flags |= D3DCREATE_SOFTWARE_VERTEXPROCESSING;
		if ((d3dCaps.DevCaps & D3DDEVCAPS_PUREDEVICE) != 0) flags |= D3DCREATE_PUREDEVICE;
		if (multithreaded) flags |= D3DCREATE_MULTITHREADED;

		//Create Device
		IDirect3DDevice9* deviceTEMP;
		if (FAILED(direct3D->CreateDevice(D3DADAPTER_DEFAULT, deviceType, this->handle, flags, &createPresentParameters(), &deviceTEMP)))
		{
			device = 0;
			return VideoError::DeviceAndSwapChainFailed;
		}
		device = deviceTEMP;
		defualtStates();

		return VideoError::None;
	}

	D3DPRESENT_PARAMETERS VideoCom::createPresentParameters()
	{
		D3DPRESENT_PARAMETERS presentParameters;
		ZeroMemory(&presentParameters, sizeof(D3DPRESENT_PARAMETERS));
		presentParameters.BackBufferFormat = D3DFMT_X8R8G8B8;//D3DFMT_X8R8G8B8;//D3DFMT_UNKNOWN;//displayMode.Format;
		presentParameters.BackBufferCount = 1;
		presentParameters.BackBufferWidth = 0;
		presentParameters.BackBufferHeight = 0;
		presentParameters.SwapEffect = D3DSWAPEFFECT_DISCARD;
		if (fullScreen) presentParameters.FullScreen_RefreshRateInHz = 60;
		presentParameters.Windowed = !fullScreen;

		if (depthBit != 0)
		{
			presentParameters.Flags = D3DPRESENTFLAG_DISCARD_DEPTHSTENCIL;
			presentParameters.EnableAutoDepthStencil = true;
			presentParameters.AutoDepthStencilFormat = D3DFMT_D16;
			if (depthBit == 24 && stencilBit == 8) presentParameters.AutoDepthStencilFormat = D3DFMT_D24S8;
			else if (depthBit == 24 && stencilBit == 0) presentParameters.AutoDepthStencilFormat = D3DFMT_D24X8;
			else if (depthBit == 16 && stencilBit == 0) presentParameters.AutoDepthStencilFormat = D3DFMT_D16;
			else if (depthBit == 32 && stencilBit == 0) presentParameters.AutoDepthStencilFormat = D3DFMT_D32;
		}

		presentParameters.PresentationInterval = vSync ? D3DPRESENT_INTERVAL_ONE : D3DPRESENT_INTERVAL_IMMEDIATE;
		presentParameters.hDeviceWindow = handle;
		presentParameters.MultiSampleQuality = 0;
		presentParameters.MultiSampleType = D3DMULTISAMPLE_NONE;

		return presentParameters;
	}

	VideoCom::~VideoCom()
	{
		if (active)
		{
			device->EndScene();
			active = false;
		}

		if (backBuffer) backBuffer->Release();
		if (depthStencilBuffer) depthStencilBuffer->Release();
		if (device) device->Release();
		null();
	}

	void VideoCom::null()
	{
		backBuffer = 0;
		depthStencilBuffer = 0;
		device = 0;
	}

	void VideoCom::defualtStates()
	{
		IDirect3DSurface9* backBufferTEMP = 0, *depthStencilBufferTEMP = 0;
		device->GetBackBuffer(0, 0, D3DBACKBUFFER_TYPE_MONO, &backBufferTEMP);
		device->GetDepthStencilSurface(&depthStencilBufferTEMP);
		backBuffer = backBufferTEMP;
		depthStencilBuffer = depthStencilBufferTEMP;

		device->SetRenderState(D3DRS_LIGHTING, false);
		device->SetRenderState(D3DRS_FOGENABLE, false);
		device->SetRenderState(D3DRS_DITHERENABLE, false);
		device->SetRenderState(D3DRS_ALPHABLENDENABLE, false);
		device->SetRenderState(D3DRS_SEPARATEALPHABLENDENABLE, false);
		device->SetRenderState(D3DRS_ALPHATESTENABLE, false);
		device->SetRenderState(D3DRS_CULLMODE, D3DCULL_NONE);
		device->SetRenderState(D3DRS_MULTISAMPLEANTIALIAS, false);
		
		device->SetRenderState(D3DRS_POINTSIZE, FtoDW(1.0f));
		device->SetRenderState(D3DRS_POINTSIZE_MAX, FtoDW(1.0f));
		device->SetRenderState(D3DRS_POINTSIZE_MIN, FtoDW(0.0f));
		device->SetRenderState(D3DRS_POINTSCALEENABLE, false);
		device->SetRenderState(D3DRS_POINTSPRITEENABLE, false);
		device->SetRenderState(D3DRS_POINTSCALE_A, FtoDW(0.0f));
		device->SetRenderState(D3DRS_POINTSCALE_B, FtoDW(0.0f));
		device->SetRenderState(D3DRS_POINTSCALE_C, FtoDW(1.0f));

		for (uint i = 0; i != caps->MaxTextureCount; ++i)
		{
			device->SetSamplerState(i, D3DSAMP_MINFILTER, D3DTEXF_LINEAR);
			device->SetSamplerState(i, D3DSAMP_MAGFILTER, D3DTEXF_LINEAR);
			device->SetSamplerState(i, D3DSAMP_MIPFILTER, D3DTEXF_LINEAR);
		}
	}
	#pragma endregion

	#pragma region Methods
	void VideoCom::Update(int width, int height)
	{
		if (active)
		{
			device->EndScene();
			active = false;
		}

		bool resetDevice = false, screenSizeChanged = false;
		if ((lastWidth != width || lastHeight != height) && width != 0 && height != 0)
		{
			lastWidth = width;
			lastHeight = height;
			resetDevice = true;
			screenSizeChanged = true;
		}

		if (device->TestCooperativeLevel() == D3DERR_DEVICELOST)
		{
			resetDevice = true;
			screenSizeChanged = false;
		}

		int i = 0;
		while (resetDevice)
		{
			Sleep(50);
			while (!screenSizeChanged && device->TestCooperativeLevel() != D3DERR_DEVICENOTRESET) Sleep(50);
			
			resetDevice = false;
			try
			{
				deviceLost();
				if (device->Reset(&createPresentParameters()) != D3D_OK) resetDevice = true;
			}
			catch (Exception^ ex)
			{
				resetDevice = true;
			}

			if (!resetDevice)
			{
				i = 0;
				Sleep(50);
				deviceReset();
			}

			Sleep(50);
			++i;
			if (i == 10)
			{
				MessageBox(0, L"The Device has tried to Reset 10 times. Press OK to try again or please force quit the application.", L"Critical Error!", MB_ICONERROR);
				i = 0;
			}
		}

		if (!active)
		{
			device->BeginScene();
			active = true;
		}
	}

	void VideoCom::deviceLost()
	{
		if (backBuffer) backBuffer->Release();
		if (depthStencilBuffer) depthStencilBuffer->Release();

		backBuffer = 0;
		depthStencilBuffer = 0;

		if (DeviceLost != nullptr) DeviceLost();
	}

	void VideoCom::deviceReset()
	{
		defualtStates();
		if (DeviceReset != nullptr) DeviceReset();
	}

	void VideoCom::Present()
	{
		if (active)
		{
			device->EndScene();
			active = false;
		}
		device->Present(0, 0, 0, 0);
	}

	void VideoCom::EnableRenderTarget()
	{
		device->SetDepthStencilSurface(depthStencilBuffer);
		device->SetRenderTarget(0, backBuffer);
	}

	void VideoCom::EnableRenderTarget(DepthStencilCom^ depthStencil)
	{
		device->SetDepthStencilSurface(depthStencil->surface);
		device->SetRenderTarget(0, backBuffer);
	}

	void VideoCom::ClearAll(float r, float g, float b, float a)
	{
		device->Clear(0, 0, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, D3DCOLOR_COLORVALUE(r,g,b,a), 1.0f, MB_ICONSTOP);
	}

	void VideoCom::ClearColor(float r, float g, float b, float a)
	{
		device->Clear(0, 0, D3DCLEAR_TARGET, D3DCOLOR_COLORVALUE(r,g,b,a), 1.0f, MB_ICONSTOP);
	}

	void VideoCom::ClearColorDepth(float r, float g, float b, float a)
	{
		device->Clear(0, 0, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER, D3DCOLOR_COLORVALUE(r,g,b,a), 1.0f, MB_ICONSTOP);
	}

	void VideoCom::ClearDepthStencil()
	{
		device->Clear(0, 0, D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, 0, 1.0f, MB_ICONSTOP);
	}

	void VideoCom::DisableTexture(int index)
	{
		device->SetTexture(index, 0);
	}

	void VideoCom::DisableVertexTexture(int index)
	{
		device->SetTexture(D3DVERTEXTEXTURESAMPLER0 + index, 0);
	}

	void VideoCom::DisableRenderTarget(int index)
	{
		device->SetRenderTarget(index, 0);
	}
	#pragma endregion
}