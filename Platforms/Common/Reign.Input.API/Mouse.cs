using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	public static class Mouse
	{
		public static MouseI Create(InputTypes apiType, params object[] args)
		{
			#if WINDOWS
			if (apiType == InputTypes.WinForms)
			{
				return (MouseI)OS.CreateInstance(Input.WinForms, Input.WinForms, "Mouse", args);
			}
			#endif

			#if METRO
			if (apiType == InputTypes.Metro)
			{
				return (MouseI)OS.CreateInstance(Input.Metro, Input.Metro, "Mouse", args);
			}
			#endif
			
			#if OSX
			if (apiType == InputTypes.Cocoa)
			{
				return (MouseI)OS.CreateInstance(Input.Cocoa, Input.Cocoa, "Mouse", args);
			}
			#endif
			
			#if LINUX
			if (apiType == InputTypes.X11)
			{
				return (MouseI)OS.CreateInstance(Input.X11, Input.X11, "Mouse", args);
			}
			#endif

			return null;
		}
	}
}
