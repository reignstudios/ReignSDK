using Reign.Core;

namespace Reign.Input.API
{
	public static class Mouse
	{
		public static void Init(InputTypes type)
		{
			#if WINDOWS
			if (type == InputTypes.WinForms) MouseAPI.Init(Reign.Input.WinForms.Mouse.New);
			#endif

			#if METRO
			if (type == InputTypes.Metro) MouseAPI.Init(Reign.Input.Metro.Mouse.New);
			#endif
		}
	}
}
