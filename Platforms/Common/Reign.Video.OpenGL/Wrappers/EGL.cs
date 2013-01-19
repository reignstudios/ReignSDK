using System;
using System.Runtime.InteropServices;

namespace Reign.Video.OpenGL
{
	public static class EGL
	{
		public const string DLL = "libEGL";
		
		public const int NONE = 12344;
		public const int DONT_CARE = -1;
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
		public const int OPENGL_ES_API = 12448;
		
		public const int SUCCESS = 0x3000;
		public const int NOT_INITIALIZED = 0x3001;
		public const int BAD_ACCESS = 0x3002;
		public const int BAD_ALLOC = 0x3003;
		public const int BAD_ATTRIBUTE = 0x3004;
		public const int BAD_CONFIG = 0x3005;
		public const int BAD_CONTEXT = 0x3006;
		public const int BAD_CURRENT_SURFACE = 0x3007;
		public const int BAD_DISPLAY = 0x3008;
		public const int BAD_MATCH = 0x3009;
		public const int BAD_NATIVE_PIXMAP = 0x300A;
		public const int BAD_NATIVE_WINDOW = 0x300B;
		public const int BAD_PARAMETER = 0x300C;
		public const int BAD_SURFACE = 0x300D;
		public const int CONTEXT_LOST = 0x300E;
		
		[DllImport(DLL, EntryPoint = "eglGetError")]
		public static extern int GetError();
		
		[DllImport(DLL, EntryPoint = "eglGetDisplay")]
		public static extern IntPtr GetDisplay(IntPtr display_id);
		
		[DllImport(DLL, EntryPoint = "eglInitialize")]
		public static extern bool Initialize(IntPtr dpy, out int major, out int minor);
		
		[DllImport(DLL, EntryPoint = "eglChooseConfig")]
		public unsafe static extern int ChooseConfig(IntPtr dpy, int[] attrib_list, IntPtr* configs, int config_size, int* num_config);
		
		[DllImport(DLL, EntryPoint = "eglBindAPI")]
		public static extern int BindAPI(int api);
	
		[DllImport(DLL, EntryPoint = "eglCreateContext")]
		public static extern IntPtr CreateContext(IntPtr dpy, IntPtr config, IntPtr share_context, int[] attrib_list);
		
		[DllImport(DLL, EntryPoint = "eglCreateWindowSurface")]
		public unsafe static extern  IntPtr CreateWindowSurface(IntPtr dpy, IntPtr config, IntPtr win, int* attrib_list);
		
		[DllImport(DLL, EntryPoint = "eglDestroySurface")]
		public static extern int DestroySurface(IntPtr dpy, IntPtr surface);
		
		[DllImport(DLL, EntryPoint = "eglDestroyContext")]
		public static extern int DestroyContext(IntPtr dpy, IntPtr ctx);
		
		[DllImport(DLL, EntryPoint = "eglMakeCurrent")]
		public static extern int MakeCurrent(IntPtr dpy, IntPtr draw, IntPtr read, IntPtr ctx);
		
		[DllImport(DLL, EntryPoint = "eglTerminate")]
		public static extern int Terminate(IntPtr display);
		
		[DllImport(DLL, EntryPoint = "eglSwapBuffers")]
		public static extern int SwapBuffers(IntPtr display, IntPtr surface);
		
		[DllImport(DLL, EntryPoint = "eglSwapInterval")]
		public static extern int SwapInterval(IntPtr display, int interval);
	}
}

