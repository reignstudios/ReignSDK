using Reign.Core;

namespace Reign.Input.API
{
	public static class Keyboard
	{
		public static void Init(InputTypes type)
		{
			#if WIN32
			if (type == InputTypes.WinForms) KeyboardAPI.Init(Reign.Input.WinForms.Keyboard.New);
			#endif

			#if WINRT
			if (type == InputTypes.WinRT) KeyboardAPI.Init(Reign.Input.WinRT.Keyboard.New);
			#endif
			
			#if OSX
			if (type == InputTypes.Cocoa) KeyboardAPI.Init(Reign.Input.Cocoa.Keyboard.New);
			#endif
			
			#if LINUX
			if (type == InputTypes.X11) KeyboardAPI.Init(Reign.Input.X11.Keyboard.New);
			#endif
		}
	}
}
