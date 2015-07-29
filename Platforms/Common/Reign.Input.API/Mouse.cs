using Reign.Core;

namespace Reign.Input.Abstraction
{
	public static class MouseAPI
	{
		public static IMouse New(IDisposableResource parent)
		{
			return New(InputAPI.DefaultAPI, parent);
		}

		public static IMouse New(InputTypes inputType, IDisposableResource parent)
		{
			IMouse api = null;

			#if WIN32
			if (inputType == InputTypes.WinForms) api = new WinForms.Mouse(parent);
			#endif

			#if WINRT
			if (inputType == InputTypes.WinRT) api = new WinRT.Mouse(parent);
			#endif
			
			#if OSX
			if (inputType == InputTypes.Cocoa) api = new Cocoa.Mouse(parent);
			#endif
			
			#if LINUX
			if (inputType == InputTypes.X11) api = new X11.Mouse(parent);
			#endif
			
			#if NaCl
			if (inputType == InputTypes.NaCl) api = new NaCl.Mouse(parent);
			#endif

			if (api == null) Debug.ThrowError("MouseAPI", "Unsuported InputType: " + inputType);
			return api;
		}
	}
}
