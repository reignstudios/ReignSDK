using System;
using System.Runtime.InteropServices;

namespace Reign.Video.OpenGL
{
	public static class CGL
	{
		public const string DLL = "/System/Library/Frameworks/ApplicationServices.framework/Versions/A/Frameworks/CoreGraphics.framework/CoreGraphics";
		public const string GLDLL = "/System/Library/Frameworks/OpenGL.framework/OpenGL";
		
		public enum PixelFormatAttribute
		{
			kCGLPFAAllRenderers       =   1,
			kCGLPFADoubleBuffer       =   5,
			kCGLPFAStereo             =   6,
			kCGLPFAAuxBuffers         =   7,
			kCGLPFAColorSize          =   8,
			kCGLPFAAlphaSize          =  11,
			kCGLPFADepthSize          =  12,
			kCGLPFAStencilSize        =  13,
			kCGLPFAAccumSize          =  14,
			kCGLPFAMinimumPolicy      =  51,
			kCGLPFAMaximumPolicy      =  52,
			kCGLPFAOffScreen          =  53,
			kCGLPFAFullScreen         =  54,
			kCGLPFASampleBuffers      =  55,
			kCGLPFASamples            =  56,
			kCGLPFAAuxDepthStencil    =  57,
			kCGLPFAColorFloat         =  58,
			kCGLPFAMultisample        =  59,
			kCGLPFASupersample        =  60,
			kCGLPFASampleAlpha        =  61,
			kCGLPFARendererID         =  70,
			kCGLPFASingleRenderer     =  71,
			kCGLPFANoRecovery         =  72,
			kCGLPFAAccelerated        =  73,
			kCGLPFAClosestPolicy      =  74,
			kCGLPFARobust             =  75,
			kCGLPFABackingStore       =  76,
			kCGLPFAMPSafe             =  78,
			kCGLPFAWindow             =  80,
			kCGLPFAMultiScreen        =  81,
			kCGLPFACompliant          =  83,
			kCGLPFADisplayMask        =  84,
			kCGLPFAPBuffer            =  90,
			kCGLPFARemotePBuffer      =  91,
			kCGLPFAAllowOfflineRenderers = 96,
			kCGLPFAAcceleratedCompute =  97,
			kCGLPFAVirtualScreenCount = 128,
		}
		
		[DllImport(DLL, EntryPoint = "CGMainDisplayID")]
		public static extern uint MainDisplayID();
		
		[DllImport(DLL, EntryPoint = "CGDisplayIDToOpenGLDisplayMask")]
		public static extern IntPtr DisplayIDToOpenGLDisplayMask(uint display);
		
		[DllImport(DLL, EntryPoint = "CGReleaseAllDisplays")]
		public static extern void ReleaseAllDisplays();
		
		[DllImport(GLDLL, EntryPoint = "CGLChoosePixelFormat")]
		public static extern int ChoosePixelFormat(int[] attribs, ref IntPtr pix, ref int npix);
		
		[DllImport(GLDLL, EntryPoint = "CGLCreateContext")]
		public static extern int CreateContext(IntPtr pix, IntPtr share, ref IntPtr ctx);
		
		[DllImport(GLDLL, EntryPoint = "CGLSetCurrentContext")]
		public static extern int SetCurrentContext(IntPtr ctx);
		
		[DllImport(GLDLL, EntryPoint = "CGLDestroyContext")]
		public static extern int DestroyContext(IntPtr ctx);
		
		[DllImport(GLDLL, EntryPoint = "CGLFlushDrawable")]
		public static extern int FlushDrawable(IntPtr ctx);
	}
}