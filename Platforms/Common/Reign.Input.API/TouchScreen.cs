using Reign.Core;

namespace Reign.Input.Abstraction
{
	public static class TouchScreenAPI
	{
		public static ITouchScreen New(IDisposableResource parent)
		{
			return New(InputAPI.DefaultAPI, parent);
		}

		public static ITouchScreen New(InputTypes inputType, IDisposableResource parent)
		{
			ITouchScreen api = null;

			#if iOS
			if (inputType == InputTypes.Cocoa) TouchScreenAPI.Init(Reign.Input.Cocoa.TouchScreen.New);
			#endif
			
			#if ANDROID
			if (inputType == InputTypes.Android) TouchScreenAPI.Init(Reign.Input.Android.TouchScreen.New);
			#endif

			if (api == null) Debug.ThrowError("TouchScreenAPI", "Unsuported InputType: " + inputType);
			return api;
		}
	}
}
