using Reign.Core;

namespace Reign.Input.Abstraction
{
	public static class GamePadAPI
	{
		public static IGamePad New(IDisposableResource disposable)
		{
			return New(InputAPI.DefaultAPI, disposable);
		}

		public static IGamePad New(InputTypes inputType, IDisposableResource parent)
		{
			IGamePad api = null;

			#if XNA
			if (inputType == InputTypes.XNA) api = new XNA.GamePad(disposable);
			#endif

			if (api == null) Debug.ThrowError("GamePadAPI", "Unsuported InputType: " + inputType);
			return api;
		}
	}
}
