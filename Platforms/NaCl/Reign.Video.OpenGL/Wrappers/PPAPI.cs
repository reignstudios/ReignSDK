using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Reign.Video.OpenGL
{
	public static class PPAPI
	{
		const string DLL = "__Internal";
		
		public enum Graphics3DAttrib
		{
			PP_GRAPHICS3DATTRIB_ALPHA_SIZE = 0x3021,
			PP_GRAPHICS3DATTRIB_BLUE_SIZE = 0x3022,
			PP_GRAPHICS3DATTRIB_GREEN_SIZE = 0x3023,
			PP_GRAPHICS3DATTRIB_RED_SIZE = 0x3024,
			PP_GRAPHICS3DATTRIB_DEPTH_SIZE = 0x3025,
			PP_GRAPHICS3DATTRIB_STENCIL_SIZE = 0x3026,
			PP_GRAPHICS3DATTRIB_SAMPLES = 0x3031,
			PP_GRAPHICS3DATTRIB_SAMPLE_BUFFERS = 0x3032,
			PP_GRAPHICS3DATTRIB_NONE = 0x3038,
			PP_GRAPHICS3DATTRIB_HEIGHT = 0x3056,
			PP_GRAPHICS3DATTRIB_WIDTH = 0x3057,
			PP_GRAPHICS3DATTRIB_SWAP_BEHAVIOR = 0x3093,
			PP_GRAPHICS3DATTRIB_BUFFER_PRESERVED = 0x3094,
			PP_GRAPHICS3DATTRIB_BUFFER_DESTROYED = 0x3095
		}
		
		[DllImport(DLL, EntryPoint = "InitOpenGL", ExactSpelling = true)]
		public static extern int InitOpenGL(int instance, IntPtr graphics, IntPtr pbbInstance, int[] attribs, IntPtr gles);
		
		[DllImport(DLL, EntryPoint = "SetCurrentContextPPAPI", ExactSpelling = true)]
		public static extern void SetCurrentContextPPAPI(int context);
		
		[DllImport(DLL, EntryPoint = "StartSwapBufferLoop", ExactSpelling = true)]
		public static extern void StartSwapBufferLoop(IntPtr graphics, int context);
		
		[DllImport(DLL, EntryPoint="StopSwapBufferLoop", ExactSpelling = true)]
		public extern static void StopSwapBufferLoop();
	}
}

