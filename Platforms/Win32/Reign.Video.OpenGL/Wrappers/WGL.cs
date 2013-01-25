using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Reign.Video.OpenGL
{
	public static class WGL
	{
		[StructLayout(LayoutKind.Sequential)] 
		public struct PIXELFORMATDESCRIPTOR 
		{
			public ushort  nSize; 
			public ushort  nVersion; 
			public uint    dwFlags; 
			public byte    iPixelType; 
			public byte    cColorBits; 
			public byte    cRedBits; 
			public byte    cRedShift; 
			public byte    cGreenBits; 
			public byte    cGreenShift; 
			public byte    cBlueBits; 
			public byte    cBlueShift; 
			public byte    cAlphaBits; 
			public byte    cAlphaShift; 
			public byte    cAccumBits; 
			public byte    cAccumRedBits; 
			public byte    cAccumGreenBits; 
			public byte    cAccumBlueBits; 
			public byte    cAccumAlphaBits; 
			public byte    cDepthBits; 
			public byte    cStencilBits; 
			public byte    cAuxBuffers; 
			public byte    iLayerType; 
			public byte    bReserved; 
			public uint    dwLayerMask; 
			public uint    dwVisibleMask; 
			public uint    dwDamageMask; 
			// 40 bytes total
		}

		public static void ZeroPixelDescriptor(ref PIXELFORMATDESCRIPTOR pfd)
		{
			pfd.nSize           = 40; // sizeof(PIXELFORMATDESCRIPTOR); 
			pfd.nVersion        = 0; 
			pfd.dwFlags         = 0;
			pfd.iPixelType      = 0;
			pfd.cColorBits      = 0; 
			pfd.cRedBits        = 0; 
			pfd.cRedShift       = 0; 
			pfd.cGreenBits      = 0; 
			pfd.cGreenShift     = 0; 
			pfd.cBlueBits       = 0; 
			pfd.cBlueShift      = 0; 
			pfd.cAlphaBits      = 0; 
			pfd.cAlphaShift     = 0; 
			pfd.cAccumBits      = 0; 
			pfd.cAccumRedBits   = 0; 
			pfd.cAccumGreenBits = 0;
			pfd.cAccumBlueBits  = 0; 
			pfd.cAccumAlphaBits = 0;
			pfd.cDepthBits      = 0; 
			pfd.cStencilBits    = 0; 
			pfd.cAuxBuffers     = 0; 
			pfd.iLayerType      = 0;
			pfd.bReserved       = 0; 
			pfd.dwLayerMask     = 0; 
			pfd.dwVisibleMask   = 0; 
			pfd.dwDamageMask    = 0; 
		}

		/* pixel types */
		public const uint  PFD_TYPE_RGBA        = 0;
		public const uint  PFD_TYPE_COLORINDEX  = 1;

		/* layer types */
		public const uint  PFD_MAIN_PLANE       = 0;
		public const uint  PFD_OVERLAY_PLANE    = 1;
		public const uint  PFD_UNDERLAY_PLANE   = 0xff; // (-1)

		/* PIXELFORMATDESCRIPTOR flags */
		public const uint  PFD_DOUBLEBUFFER            = 0x00000001;
		public const uint  PFD_STEREO                  = 0x00000002;
		public const uint  PFD_DRAW_TO_WINDOW          = 0x00000004;
		public const uint  PFD_DRAW_TO_BITMAP          = 0x00000008;
		public const uint  PFD_SUPPORT_GDI             = 0x00000010;
		public const uint  PFD_SUPPORT_OPENGL          = 0x00000020;
		public const uint  PFD_GENERIC_FORMAT          = 0x00000040;
		public const uint  PFD_NEED_PALETTE            = 0x00000080;
		public const uint  PFD_NEED_SYSTEM_PALETTE     = 0x00000100;
		public const uint  PFD_SWAP_EXCHANGE           = 0x00000200;
		public const uint  PFD_SWAP_COPY               = 0x00000400;
		public const uint  PFD_SWAP_LAYER_BUFFERS      = 0x00000800;
		public const uint  PFD_GENERIC_ACCELERATED     = 0x00001000;
		public const uint  PFD_SUPPORT_DIRECTDRAW      = 0x00002000;

		/* PIXELFORMATDESCRIPTOR flags for use in ChoosePixelFormat only */
		public const uint  PFD_DEPTH_DONTCARE          = 0x20000000;
		public const uint  PFD_DOUBLEBUFFER_DONTCARE   = 0x40000000;
		public const uint  PFD_STEREO_DONTCARE         = 0x80000000;

		[DllImport("user32", EntryPoint = "GetDC")]
		public static extern IntPtr GetDC( IntPtr hwnd );

		[DllImport("user32", EntryPoint = "ReleaseDC")]
		public static extern int ReleaseDC( IntPtr hwnd, IntPtr dc );

		[DllImport("gdi32", EntryPoint = "ChoosePixelFormat")]
		public static extern int ChoosePixelFormat(IntPtr hdc, ref PIXELFORMATDESCRIPTOR p_pfd);

		[DllImport("gdi32", EntryPoint = "SetPixelFormat")]
		public static extern uint SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PIXELFORMATDESCRIPTOR p_pfd);

		[DllImport(GL.DLL, EntryPoint = "wglCreateContext")]
		public static extern IntPtr CreateContext(IntPtr hdc);

		[DllImport(GL.DLL, EntryPoint = "wglMakeCurrent")]
		public static extern int MakeCurrent(IntPtr hdc, IntPtr hglrc);

		[DllImport(GL.DLL, EntryPoint = "wglDeleteContext")]
		public static extern int DeleteContext(IntPtr hglrc);

		[DllImport(GL.DLL, EntryPoint = "wglSwapBuffers")]
		public static extern IntPtr SwapBuffers(IntPtr hdc);

		[SuppressUnmanagedCodeSecurity()]
		public delegate uint SwapIntervalFunc(int interval);
		public static SwapIntervalFunc SwapInterval;

		public static void Init()
		{
			SwapInterval = (SwapIntervalFunc)Marshal.GetDelegateForFunctionPointer(GL.GetProcAddress("wglSwapIntervalEXT"), typeof(SwapIntervalFunc));
		}
	}
}