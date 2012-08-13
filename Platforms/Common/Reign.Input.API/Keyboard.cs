using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	public static class Keyboard
	{
		public static KeyboardI Create(InputTypes apiType, params object[] args)
		{
			#if WINDOWS
			if (apiType == InputTypes.WinForms)
			{
				return (KeyboardI)OS.CreateInstance(Input.WinForms, Input.WinForms, "Keyboard", args);
			}
			#endif
			
			#if OSX
			if (apiType == InputTypes.Cocoa)
			{
				return (KeyboardI)OS.CreateInstance(Input.Cocoa, Input.Cocoa, "Keyboard", args);
			}
			#endif
			
			#if LINUX
			if (apiType == InputTypes.X11)
			{
				return (KeyboardI)OS.CreateInstance(Input.X11, Input.X11, "Keyboard", args);
			}
			#endif

			return null;
		}
	}
}
