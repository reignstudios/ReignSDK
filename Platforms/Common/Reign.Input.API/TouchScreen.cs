using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	public static class TouchScreen
	{
		public static void Init(InputTypes type)
		{
			#if iOS
			if (type == InputTypes.Cocoa) TouchScreenAPI.Init(Reign.Input.Cocoa.TouchScreen.New);
			#endif
		}
	}
}
