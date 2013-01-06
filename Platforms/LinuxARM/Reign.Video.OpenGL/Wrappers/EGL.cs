using System;
using System.Runtime.InteropServices;

namespace Reign.Video.OpenGL
{
	public static class RaspberryPi
	{
		public const string DLL = "libbcm_host";
		
		[StructLayout(LayoutKind.Sequential)]
		public struct VC_RECT_T
		{
			public int x, y, width, height;
		}
		
		public const int DISPMANX_PROTECTION_NONE = 0;
		
		[DllImport(DLL, EntryPoint = "bcm_host_init")]
		public static extern void bcm_host_init();
		
		[DllImport(DLL, EntryPoint = "bcm_host_deinit")]
		public static extern void bcm_host_deinit();
		
		[DllImport(DLL, EntryPoint = "graphics_get_display_size")]
		public unsafe static extern int graphics_get_display_size(ushort display_number, [OutAttribute] uint *width, [OutAttribute] uint *height);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_display_open")]
		public static extern IntPtr vc_dispmanx_display_open(uint device);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_update_start")]
		public static extern IntPtr vc_dispmanx_update_start(int priority);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_update_submit_sync")]
		public static extern int vc_dispmanx_update_submit_sync(IntPtr update);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_element_add")]
		public unsafe static extern IntPtr vc_dispmanx_element_add(IntPtr update, IntPtr display, int layer, VC_RECT_T *dest_rect, IntPtr src, VC_RECT_T *src_rect, uint protection, IntPtr alpha, IntPtr clamp, int transform);
	}

	public static class EGL
	{
		public const string DLL = "libEGL";
		
		[StructLayout(LayoutKind.Sequential)]
		public struct DISPMANX_WINDOW_T
		{
			public IntPtr element;
			public int width;
			public int height;
		}
		
		public const int NONE = 12344;
		public const int CONTEXT_CLIENT_VERSION = 12440;
		public const int OPENGL_ES2_BIT = 4;
		public const int RENDERABLE_TYPE = 12352;
		public const int SAMPLES = 12337;
		public const int SAMPLE_BUFFERS = 12338;
		
		public const int SURFACE_TYPE = 12339;
		public const int PBUFFER_BIT = 1;
		public const int PIXMAP_BIT = 2;
		public const int WINDOW_BIT = 4;
		
		public const int BUFFER_SIZE = 12320;
		public const int ALPHA_SIZE = 12321;
		public const int BLUE_SIZE = 12322;
		public const int GREEN_SIZE = 12323;
		public const int RED_SIZE = 12324;
		public const int DEPTH_SIZE = 12325;
		public const int STENCIL_SIZE = 12326;
		
		public const int MIN_SWAP_INTERVAL = 12347;
		public const int MAX_SWAP_INTERVAL = 12348;
		
		[DllImport(DLL, EntryPoint = "eglGetError")]
		public static extern int GetError();
		
		[DllImport(DLL, EntryPoint = "eglGetDisplay")]
		public static extern IntPtr GetDisplay(IntPtr display_id);
		
		[DllImport(DLL, EntryPoint = "eglInitialize")]
		public static extern bool Initialize(IntPtr dpy, out int major, out int minor);
		
		[DllImport(DLL, EntryPoint = "eglChooseConfig")]
		[return: MarshalAsAttribute(UnmanagedType.I1)]//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< MAY NOT NEED
		public static extern bool ChooseConfig(IntPtr dpy, int[] attrib_list, [In, Out] IntPtr[] configs, int config_size, out int num_config);//<<<<<<< [in, out]? use 'ref' ??
	
		[DllImport(DLL, EntryPoint = "eglCreateContext")]
		public static extern IntPtr CreateContext(IntPtr dpy, IntPtr config, IntPtr share_context, int[] attrib_list);
		
		[DllImport(DLL, EntryPoint = "eglCreateWindowSurface")]
		public static extern  IntPtr CreateWindowSurface(IntPtr dpy, IntPtr config, IntPtr win, int[] attrib_list);
		
		[DllImport(DLL, EntryPoint = "eglDestroySurface")]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool DestroySurface(IntPtr dpy, IntPtr surface);
		
		[DllImport(DLL, EntryPoint = "eglDestroyContext")]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool DestroyContext(IntPtr dpy, IntPtr ctx);
		
		[DllImport(DLL, EntryPoint = "eglMakeCurrent")]
		[return: MarshalAsAttribute(UnmanagedType.I1)]
		public static extern bool MakeCurrent(IntPtr dpy, IntPtr draw, IntPtr read, IntPtr ctx);
		
		[DllImport(DLL, EntryPoint = "eglTerminate")]
		public static extern bool Terminate(IntPtr display);
		
		[DllImport(DLL, EntryPoint = "eglSwapBuffers")]
		public static extern bool SwapBuffers(IntPtr display, IntPtr surface);
		
		[DllImport(DLL, EntryPoint = "eglSwapInterval")]
		public static extern bool SwapInterval(IntPtr display, int interval);
	}
}

