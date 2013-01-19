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
		
		[StructLayout(LayoutKind.Sequential)]
		public struct DISPMANX_WINDOW_T
		{
			public IntPtr element;
			public int width, height;
		}
		
		public const int DISPMANX_PROTECTION_NONE = 0;
		
		[DllImport(DLL, EntryPoint = "bcm_host_init")]
		public static extern void bcm_host_init();
		
		[DllImport(DLL, EntryPoint = "bcm_host_deinit")]
		public static extern void bcm_host_deinit();
		
		[DllImport(DLL, EntryPoint = "graphics_get_display_size")]
		public unsafe static extern int graphics_get_display_size(ushort display_number, uint *width, uint *height);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_display_open")]
		public static extern IntPtr vc_dispmanx_display_open(uint device);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_update_start")]
		public static extern IntPtr vc_dispmanx_update_start(int priority);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_update_submit_sync")]
		public static extern int vc_dispmanx_update_submit_sync(IntPtr update);
		
		[DllImport(DLL, EntryPoint = "vc_dispmanx_element_add")]
		public unsafe static extern IntPtr vc_dispmanx_element_add(IntPtr update, IntPtr display, int layer, VC_RECT_T *dest_rect, IntPtr src, VC_RECT_T *src_rect, uint protection, IntPtr alpha, IntPtr clamp, int transform);
	}
}

