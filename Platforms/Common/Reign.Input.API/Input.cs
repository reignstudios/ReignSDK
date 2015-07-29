using System;
using Reign.Core;

namespace Reign.Input.Abstraction
{
	[Flags]
	public enum InputTypes
	{
		None = 0,
		WinForms = 1,
		WinRT = 2,
		XNA = 4,
		Cocoa = 8,
		X11 = 16,
		Android = 32,
		NaCl = 64
	}

	public static class InputAPI
	{
		public static InputTypes DefaultAPI = InputTypes.None;

		public static IInput New(InputTypes inputTypeFlags, out InputTypes inputType, IDisposableResource parent, IApplication application)
		{
			bool winForms = (inputTypeFlags & InputTypes.WinForms) != 0;
			bool cocoa = (inputTypeFlags & InputTypes.Cocoa) != 0;
			bool x11 = (inputTypeFlags & InputTypes.X11) != 0;
			bool nacl = (inputTypeFlags & InputTypes.NaCl) != 0;
			bool metro = (inputTypeFlags & InputTypes.WinRT) != 0;
			bool xna = (inputTypeFlags & InputTypes.XNA) != 0;
			bool android = (inputTypeFlags & InputTypes.Android) != 0;

			inputType = InputTypes.None;
			Exception lastException = null;
			IInput input = null;
			while (true)
			{
				try
				{
					#if WIN32
					if (winForms)
					{
						winForms = false;
						inputType = InputTypes.WinForms;
						input = new WinForms.Input(parent, application);
						break;
					}
					#endif
					
					#if OSX
					if (cocoa)
					{
						cocoa = false;
						inputType = InputTypes.Cocoa;
						input = new Cocoa.Input(parent, application);
						break;
					}
					#endif
					
					#if LINUX
					if (x11)
					{
						x11 = false;
						inputType = InputTypes.X11;
						input = new X11.Input(parent, application);
						break;
					}
					#endif
					
					#if NaCl
					if (nacl)
					{
						nacl = false;
						inputType = InputTypes.NaCl;
						input = new NaCl.Input(parent, application);
						break;
					}
					#endif

					#if WINRT
					if (metro)
					{
						metro = false;
						inputType = InputTypes.WinRT;
						input = new WinRT.Input(parent, application);
						break;
					}
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						inputType = InputTypes.XNA;
						input = new XNA.Input(parent, application);
						break;
					}
					#endif
					
					#if iOS
					if (cocoa)
					{
						cocoa = false;
						inputType = InputTypes.Cocoa;
						input = new Cocoa.Input(parent, application);
						break;
					}
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						inputType = InputTypes.Android;
						input = new Android.Input(parent, application);
						break;
					}
					#endif

					else break;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			// check for error
			if (lastException != null)
			{
				string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
				Debug.ThrowError("InputAPI", "Failed to create Input API" + ex);
				inputType = InputTypes.None;
			}

			if (inputType != InputTypes.None) DefaultAPI = inputType;
			return input;
		}
	}
}
