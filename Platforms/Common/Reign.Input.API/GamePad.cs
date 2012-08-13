using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	public static class GamePad
	{
		public static GamePadI Create(InputTypes apiType, params object[] args)
		{
			#if WINDOWS
			if (apiType == InputTypes.WinForms)
			{
				return (GamePadI)OS.CreateInstance(Input.WinForms, Input.WinForms, "GamePad", args);
			}
			#endif

			#if XNA
			if (apiType == InputTypes.XNA)
			{
				return (GamePadI)OS.CreateInstance(Input.XNA, Input.XNA, "GamePad", args);
			}
			#endif

			return null;
		}
	}
}
