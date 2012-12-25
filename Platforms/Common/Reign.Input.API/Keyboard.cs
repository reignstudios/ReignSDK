using Reign.Core;

namespace Reign.Input.API
{
	public static class Keyboard
	{
		public static void Init(InputTypes type)
		{
			#if WINDOWS
			if (type == InputTypes.WinForms) KeyboardAPI.Init(Reign.Input.WinForms.Keyboard.New);
			#endif

			#if METRO
			if (type == InputTypes.Metro) KeyboardAPI.Init(Reign.Input.Metro.Keyboard.New);
			#endif
			
			#if OSX
			if (type == InputTypes.Cocoa) KeyboardAPI.Init(Reign.Input.Cocoa.Keyboard.New);
			#endif
		}
	}
}
