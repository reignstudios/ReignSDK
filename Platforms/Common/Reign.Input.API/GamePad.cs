using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	public static class GamePad
	{
		public static void Init(InputTypes type)
		{
			#if XNA
			if (type == InputTypes.XNA) GamePadAPI.Init(Reign.Input.XNA.GamePad.New);
			#endif
		}
	}
}
