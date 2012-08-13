using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	public static class TouchScreen
	{
		public static TouchScreenI Create(InputTypes apiType, params object[] args)
		{
			#if WP7
			if (apiType == InputTypes.XNA)
			{
				return (TouchScreenI)OS.CreateInstance(Input.XNA, Input.XNA, "TouchScreen", args);
			}
			#endif

			#if iOS
			if (apiType == InputTypes.Cocoa)
			{
				return (TouchScreenI)OS.CreateInstance(Input.Cocoa, Input.Cocoa, "TouchScreen", args);
			}
			#endif
			
			#if ANDROID
			if (apiType == InputTypes.Android)
			{
				return (TouchScreenI)OS.CreateInstance(Input.Android, Input.Android, "TouchScreen", args);
			}
			#endif

			return null;
		}
	}
}
