using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Reign.Video.OpenGL
{
	public static class GLX
	{
		public const string DLL = "libGL.so.1";
		
		public const int RGBA = 4;
		public const int DOUBLEBUFFER = 5;
		public const int RED_SIZE = 8;
		public const int GREEN_SIZE	= 9;
		public const int BLUE_SIZE = 10;
		public const int ALPHA_SIZE	= 11;
		public const int DEPTH_SIZE	= 12;
		public const int NONE = 0;
	
		[DllImport(DLL, EntryPoint = "glXChooseVisual", ExactSpelling = true)]
		public static extern IntPtr ChooseVisual(IntPtr dpy, int screen, int[] attribList);
		
		[DllImport(DLL, EntryPoint = "glXQueryVersion", ExactSpelling = true)]
		public static extern bool QueryVersion(IntPtr dpy, ref int maj, ref int min);
		
		[DllImport(DLL, EntryPoint = "glXCreateContext", ExactSpelling = true)]
		public static extern IntPtr CreateContext(IntPtr dpy, IntPtr vis, IntPtr shareList, bool direct);
		
		[DllImport(DLL, EntryPoint = "glXMakeCurrent", ExactSpelling = true)]
		public static extern bool MakeCurrent(IntPtr dpy, IntPtr drawable, IntPtr ctx);
		
		[DllImport(DLL, EntryPoint = "glXSwapBuffers", ExactSpelling = true)]
		public static extern void SwapBuffers(IntPtr dpy, IntPtr drawable);
		
		[DllImport(DLL, EntryPoint = "glXDestroyContext", ExactSpelling = true)]
		public static extern void DestroyContext(IntPtr dpy, IntPtr drawable);
		
		[DllImport(DLL, EntryPoint = "glXGetCurrentDisplay", ExactSpelling = true)]
		public static extern IntPtr GetCurrentDisplay();
		
		[DllImport(DLL, EntryPoint = "glXGetCurrentDrawable", ExactSpelling = true)]
		public static extern IntPtr GetCurrentDrawable();
		
		[DllImport(DLL, EntryPoint = "glXSwapIntervalSGI", ExactSpelling = true)]
		public static extern int SwapIntervalSGI(int interval);
		
		[DllImport(DLL, EntryPoint = "glXSwapIntervalEXT", ExactSpelling = true)]
		public static extern void SwapIntervalEXT(IntPtr dpy, IntPtr drawable, int interval);
		
		[DllImport(DLL, EntryPoint = "glXGetVideoSyncSGI", ExactSpelling = true)]
		public unsafe static extern int GetVideoSyncSGI(uint* count);
		
		[DllImport(DLL, EntryPoint = "glXWaitVideoSyncSGI", ExactSpelling = true)]
		public unsafe static extern int WaitVideoSyncSGI(int divisor, int remainder, uint* count);
		
		[SuppressUnmanagedCodeSecurity()]
		public delegate uint SwapIntervalMesaFunc(int interval);
		public static SwapIntervalMesaFunc SwapIntervalMesa;
		
		public static void Init()
		{
			SwapIntervalMesa = (SwapIntervalMesaFunc)Marshal.GetDelegateForFunctionPointer(GL.GetProcAddress("glXSwapIntervalMESA"), typeof(SwapIntervalMesaFunc));
		}
	}
}