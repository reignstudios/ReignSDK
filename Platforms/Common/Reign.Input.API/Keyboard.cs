using Reign.Core;

namespace Reign.Input.Abstraction
{
	public static class KeyboardAPI
	{
		public static IKeyboard New(IDisposableResource parent)
		{
			return New(InputAPI.DefaultAPI, parent);
		}

		public static IKeyboard New(InputTypes inputType, IDisposableResource parent)
		{
			IKeyboard api = null;

			#if WIN32
			if (inputType == InputTypes.WinForms) api = new WinForms.Keyboard(parent);
			#endif

			#if WINRT
			if (inputType == InputTypes.WinRT) api = new WinRT.Keyboard(parent);
			#endif
			
			#if OSX
			if (inputType == InputTypes.Cocoa) api = new Cocoa.Keyboard(parent);
			#endif
			
			#if LINUX
			if (inputType == InputTypes.X11) api = new X11.Keyboard(parent);
			#endif

			if (api == null) Debug.ThrowError("KeyboardAPI", "Unsuported InputType: " + inputType);
			return api;
		}
	}
}
