using Reign.Core;

namespace Reign.Input.API
{
	public static class Mouse
	{
		public static void Init(InputTypes type)
		{
			#if WIN32
			if (type == InputTypes.WinForms) MouseAPI.Init(Reign.Input.WinForms.Mouse.New);
			#endif

			#if WINRT
			if (type == InputTypes.WinRT) MouseAPI.Init(Reign.Input.WinRT.Mouse.New);
			#endif
			
			#if OSX
			if (type == InputTypes.Cocoa) MouseAPI.Init(Reign.Input.Cocoa.Mouse.New);
			#endif
			
			#if LINUX
			if (type == InputTypes.X11) MouseAPI.Init(Reign.Input.X11.Mouse.New);
			#endif
			
			#if NaCl
			if (type == InputTypes.NaCl) MouseAPI.Init(Reign.Input.NaCl.Mouse.New);
			#endif
		}
	}
}
