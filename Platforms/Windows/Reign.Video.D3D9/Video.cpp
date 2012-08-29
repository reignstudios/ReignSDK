#include "pch.h"
#include "Video.h"
#include "DepthStencil.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Cap Properties
	bool Caps::D3D9Ex::get() {return d3d9Ex;}
	void Caps::D3D9Ex::set(bool value) {d3d9Ex = value;}

	bool Caps::HardwareInstancing::get() {return hardwareInstancing;}
	void Caps::HardwareInstancing::set(bool value) {hardwareInstancing = value;}

	uint Caps::MaxTextureCount::get() {return maxTextureCount;}
	void Caps::MaxTextureCount::set(uint value) {maxTextureCount = value;}

	ShaderVersions Caps::MaxVertexShaderVersion::get() {return maxVertexShaderVersion;}
	void Caps::MaxVertexShaderVersion::set(ShaderVersions value) {maxVertexShaderVersion = value;}

	ShaderVersions Caps::MaxPixelShaderVersion::get() {return maxPixelShaderVersion;}
	void Caps::MaxPixelShaderVersion::set(ShaderVersions value) {maxPixelShaderVersion = value;}

	ShaderVersions Caps::MaxShaderVersion::get() {return maxShaderVersion;}
	void Caps::MaxShaderVersion::set(ShaderVersions value) {maxShaderVersion = value;}
	#pragma endregion

	#pragma region Properties
	string^ Video::FileTag::get() {return fileTag;}

	bool Video::IsExDevice::get() {return isExDevice;}
	IDirect3DDevice9* Video::Device::get() {return device;}
	bool Video::DeviceIsLost::get() {return deviceIsLost;}

	Reign::Video::D3D9::Caps^ Video::Caps::get() {return caps;}
	void Video::Caps::set(Reign::Video::D3D9::Caps^ value) {caps = value;}

	void Video::PointSize::set(float value) {device->SetRenderState(D3DRS_POINTSIZE, FtoDW(value));}
	void Video::PointSizeMax::set(float value) {device->SetRenderState(D3DRS_POINTSIZE_MAX, FtoDW(value));}
	void Video::PointSizeMin::set(float value) {device->SetRenderState(D3DRS_POINTSIZE_MIN, FtoDW(value));}
	void Video::PointDepthScalable::set(bool value) {device->SetRenderState(D3DRS_POINTSCALEENABLE, value);}
	void Video::PointSpriteEnable::set(bool value) {device->SetRenderState(D3DRS_POINTSPRITEENABLE, value);}
	#pragma endregion

	#pragma region Constructors
	Video::Video(DisposableI^ parent, Window^ window, bool vSync)
	: Disposable(parent)
	{
		this->window = window;
		lastWindowFrameSize = window->FrameSize;
		init(parent, window->Handle, 0, 0, false, vSync, false, SoftwareModes::None);
	}

	Video::Video(DisposableI^ parent, IntPtr handle, int width, int height, bool fullScreen, bool vSync, bool multithreaded, SoftwareModes softwareMode)
	: Disposable(parent)
	{
		init(parent, handle, width, height, fullScreen, vSync, multithreaded, softwareMode);
	}

	IDirect3D9* Video::createD3D9Ex()
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

	void Video::init(DisposableI^ parent, IntPtr handle, int width, int height, bool fullScreen, bool vSync, bool multithreaded, SoftwareModes softwareMode)
	{
		null();
		IDirect3D9* direct3D = 0;

		try
		{
			this->handle = handle;
			this->fullScreen = fullScreen;
			this->vSync = vSync;
			caps = gcnew Reign::Video::D3D9::Caps();

			currentVertexTextures = gcnew array<Texture2D^>(4);
			currentPixelTextures = gcnew array<Texture2D^>(8);
			fileTag = L"D3D9_";

			//Create D3D Object
			direct3D = createD3D9Ex();
			if (direct3D != 0)
			{
				isExDevice = true;
				caps->D3D9Ex = true;
			}
			else
			{
				direct3D = Direct3DCreate9(D3D_SDK_VERSION);
				if (direct3D == 0)
				{
					Debug::ThrowError(L"Video", L"Failed to create Direct3D9 interface.\nInstalling the 'DirectX End-User Runtime Web Installer' from Microsoft may fix the problem.");
				}
				isExDevice = false;
				caps->D3D9Ex = false;
			}

			D3DDISPLAYMODE displayMode;
			if (FAILED(direct3D->GetAdapterDisplayMode(D3DADAPTER_DEFAULT, &displayMode))) throw gcnew Exception("GetAdapterDisplayMode");

			D3DDEVTYPE deviceType = D3DDEVTYPE_HAL;
			if (FAILED(direct3D->CheckDeviceType(0, deviceType, D3DFMT_X8R8G8B8, D3DFMT_X8R8G8B8, !fullScreen)))
			{
				Debug::ThrowError(L"Video", L"Your video hardware is not supported");
			}

			/*if (FAILED(direct3D->CheckDeviceFormat(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, D3DFMT_X8R8G8B8, 0, D3DRTYPE_SURFACE, D3DFMT_D16)))//D3DFMT_D16 D3DFMT_D24FS8 displayMode.Format
			{
				throw gcnew Exception("CheckDeviceFormat");
			}*/

			//Get Caps
			D3DCAPS9 d3dCaps;
			if (FAILED(direct3D->GetDeviceCaps(0, deviceType, &d3dCaps)))
			{
				Debug::ThrowError(L"Video", L"Failed to get Direct3D9 Caps");
			}

			caps->MaxTextureCount = d3dCaps.MaxSimultaneousTextures;

			float maxVertexShaderValue = 3.0f;
			caps->MaxVertexShaderVersion = ShaderVersions::HLSL_3_0;
			if (d3dCaps.VertexShaderVersion >= D3DVS_VERSION(3, 0))
			{
				caps->MaxVertexShaderVersion = ShaderVersions::HLSL_3_0;
				maxVertexShaderValue = 3.0f;
			}
			else if (d3dCaps.VertexShaderVersion >= D3DVS_VERSION(2, 1))
			{
				caps->MaxVertexShaderVersion = ShaderVersions::HLSL_2_a;
				maxVertexShaderValue = 2.1f;
			}
			else if (d3dCaps.VertexShaderVersion >= D3DVS_VERSION(2, 0))
			{
				caps->MaxVertexShaderVersion = ShaderVersions::HLSL_2_0;
				maxVertexShaderValue = 2.0f;
			}

			float maxPixelShaderValue = 3.0f;
			caps->MaxPixelShaderVersion = ShaderVersions::HLSL_3_0;
			if (d3dCaps.PixelShaderVersion >= D3DPS_VERSION(3, 0))
			{
				caps->MaxPixelShaderVersion = ShaderVersions::HLSL_3_0;
				maxPixelShaderValue = 3.0f;
			}
			else if (d3dCaps.PixelShaderVersion >= D3DPS_VERSION(2, 1))
			{
				caps->MaxPixelShaderVersion = ShaderVersions::HLSL_2_a;
				maxPixelShaderValue = 2.1f;
			}
			else if (d3dCaps.PixelShaderVersion >= D3DPS_VERSION(2, 0))
			{
				caps->MaxPixelShaderVersion = ShaderVersions::HLSL_2_0;
				maxPixelShaderValue = 2.0f;
			}

			caps->MaxShaderVersion = (maxVertexShaderValue <= maxPixelShaderValue) ? caps->MaxVertexShaderVersion : caps->MaxPixelShaderVersion;
			caps->HardwareInstancing = (caps->MaxVertexShaderVersion == ShaderVersions::HLSL_3_0);

			//Set Flags
			DWORD flags = 0;
			if ((d3dCaps.DevCaps & D3DDEVCAPS_HWTRANSFORMANDLIGHT) != 0) flags |= D3DCREATE_HARDWARE_VERTEXPROCESSING;
			else flags |= D3DCREATE_SOFTWARE_VERTEXPROCESSING;
			if ((d3dCaps.DevCaps & D3DDEVCAPS_PUREDEVICE) != 0) flags |= D3DCREATE_PUREDEVICE;
			if (multithreaded) flags |= D3DCREATE_MULTITHREADED;

			//Create Device
			IDirect3DDevice9* deviceTEMP;
			if (isExDevice)
			{
				if (FAILED(direct3D->CreateDevice(D3DADAPTER_DEFAULT, deviceType, (HWND)handle.ToPointer(), flags, &createPresentParameters(), &deviceTEMP)))
				{
					device = 0;
					Debug::ThrowError(L"Video", "Failed to Create Direct3D9Ex Device");
				}
			}
			else
			{
				if (FAILED(direct3D->CreateDevice(D3DADAPTER_DEFAULT, deviceType, (HWND)handle.ToPointer(), flags, &createPresentParameters(), &deviceTEMP)))
				{
					device = 0;
					Debug::ThrowError(L"Video", "Failed to Create Direct3D9 Device");
				}
			}
			device = deviceTEMP;

			defualtStates();
			DeviceLost = gcnew DeviceLostMethod(this, &Video::deviceLost);
			DeviceReset = gcnew DeviceLostMethod(this, &Video::deviceReset);
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
		finally
		{
			if (direct3D) direct3D->Release();
		}
	}

	Video::~Video()
	{
		disposeChilderen();
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

	void Video::null()
	{
		backBuffer = 0;
		depthStencilBuffer = 0;
		device = 0;
		LastEffect = 0;
		DeviceLost = nullptr;
		DeviceReset = nullptr;
	}

	void Video::defualtStates()
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
		
		PointSpriteEnable = false;
		PointDepthScalable = false;
		PointSize = 1.0f;
		PointSizeMax = 1.0f;
		PointSizeMin = 0.0f;
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
	D3DPRESENT_PARAMETERS Video::createPresentParameters()
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
		presentParameters.EnableAutoDepthStencil = true;
		presentParameters.AutoDepthStencilFormat = D3DFMT_D24S8;
		presentParameters.PresentationInterval = vSync ? D3DPRESENT_INTERVAL_ONE : D3DPRESENT_INTERVAL_IMMEDIATE;
		presentParameters.Flags = D3DPRESENTFLAG_DISCARD_DEPTHSTENCIL;
		presentParameters.hDeviceWindow = (HWND)handle.ToPointer();
		presentParameters.MultiSampleQuality = 0;
		presentParameters.MultiSampleType = D3DMULTISAMPLE_NONE;

		return presentParameters;
	}

	void Video::Update()
	{
		if (active)
		{
			device->EndScene();
			active = false;
		}

		bool resetDevice = false, screenSizeChanged = false;
		Size2 frame = window->FrameSize;
		if (window != nullptr && lastWindowFrameSize != frame && frame.Width != 0 && frame.Height != 0)
		{
			lastWindowFrameSize = frame;
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
				deviceIsLost = true;
				if (DeviceLost != nullptr) DeviceLost();
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
				deviceIsLost = false;
				deviceReseting = true;
				if (DeviceReset != nullptr) DeviceReset();
				deviceReseting = false;
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

	void Video::deviceLost()
	{
		if (backBuffer) backBuffer->Release();
		if (depthStencilBuffer) depthStencilBuffer->Release();

		backBuffer = 0;
		depthStencilBuffer = 0;
	}

	void Video::deviceReset()
	{
		defualtStates();
	}

	void Video::Present()
	{
		if (active)
		{
			device->EndScene();
			active = false;
		}
		device->Present(0, 0, 0, 0);
	}

	void Video::EnableRenderTarget()
	{
		device->SetDepthStencilSurface(depthStencilBuffer);
		device->SetRenderTarget(0, backBuffer);
	}

	void Video::EnableRenderTarget(DepthStencilI^ depthStencil)
	{
		if (depthStencil != nullptr)
		{
			DepthStencil^ depthStencilTEMP = (DepthStencil^)depthStencil;
			device->SetDepthStencilSurface(depthStencilTEMP->Surface);
		}
		else
		{
			device->SetDepthStencilSurface(0);
		}
		device->SetRenderTarget(0, backBuffer);
	}

	void Video::DisableRenderTarget()
	{
		device->SetDepthStencilSurface(0);
		device->SetRenderTarget(0, 0);
	}

	void Video::Clear(float r, float g, float b, float a)
	{
		device->Clear(0, 0, D3DCLEAR_TARGET | D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, D3DCOLOR_COLORVALUE(r,g,b,a), 1.0f, MB_ICONSTOP);
	}

	void Video::ClearColor(float r, float g, float b, float a)
	{
		device->Clear(0, 0, D3DCLEAR_TARGET, D3DCOLOR_COLORVALUE(r,g,b,a), 1.0f, MB_ICONSTOP);
	}

	void Video::ClearDepthStencil()
	{
		device->Clear(0, 0, D3DCLEAR_ZBUFFER | D3DCLEAR_STENCIL, 0, 1.0f, MB_ICONSTOP);
	}

	void Video::removeActiveTexture(Texture2D^ texture)
	{
		array<Texture2D^>^ textures = currentVertexTextures;
		for (int i = 0; i != textures->Length; ++i)
		{
			if (textures[i] == texture)
			{
				device->SetTexture(D3DVERTEXTEXTURESAMPLER0 + i, 0);
				textures[i] = nullptr;
			}
		}

		textures = currentPixelTextures;
		for (int i = 0; i != textures->Length; ++i)
		{
			if (textures[i] == texture)
			{
				device->SetTexture(i, 0);
				textures[i] = nullptr;
			}
		}
	}

	D3DFORMAT Video::surfaceFormat(SurfaceFormats surfaceFormat)
	{
		switch (surfaceFormat)
		{
			case (SurfaceFormats::RGBAx8): return D3DFMT_A8R8G8B8;
			case (SurfaceFormats::RGBx10_Ax2): return D3DFMT_A2R10G10B10;
			case (SurfaceFormats::RGBAx16f): return D3DFMT_A16B16G16R16F;
			case (SurfaceFormats::RGBAx32f): return D3DFMT_A32B32G32R32F;
			default: 
				Debug::ThrowError(L"Video", L"Unsuported SurfaceFormat.");
				return D3DFMT_A8R8G8B8;
		}
	}
	#pragma endregion
}
}
}