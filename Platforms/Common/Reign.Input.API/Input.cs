using System;
using Reign.Core;
using Reign.Input;

namespace Reign.Input.API
{
	[Flags]
	public enum InputTypes
	{
		None,
		WinForms,
		Metro,
		XNA,
		Cocoa,
		X11,
		Android
	}

	public static class Input
	{
		#if METRO || XNA || iOS || ANDROID
		public static InputI Init(InputTypes typeFlags, out InputTypes type, DisposableI parent, Application application)
		{
			#if METRO
			bool metro = (typeFlags & InputTypes.Metro) != 0;
			#endif

			#if XNA
			bool xna = (typeFlags & InputTypes.XNA) != 0;
			#endif

			#if iOS
			bool cocoa = (typeFlags & InputTypes.Cocoa) != 0;
			#endif

			#if ANDROID
			bool android = (typeFlags & InputTypes.Android) != 0;
			#endif

			type = InputTypes.None;
			Exception lastException = null;
			InputI input = null;
			while (true)
			{
				try
				{
					#if METRO
					if (metro)
					{
						metro = false;
						type = InputTypes.Metro;
						input = new Reign.Input.Metro.Input(parent, application);
						break;
					}
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						type = InputTypes.XNA;
						input = new Reign.Input.XNA.Input(parent, application);
						break;
					}
					#endif
					
					#if iOS
					if (cocoa)
					{
						cocoa = false;
						type = InputTypes.Cocoa;
						input = new Reign.Input.Cocoa.Input(parent, application);
						break;
					}
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						type = InputTypes.Android;
						input = new Reign.Input.Android.Input(parent, application);
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
				Debug.ThrowError("Input", "Failed to create Input API" + ex);
				type = InputTypes.None;
			}

			// init api methods
			#if METRO
			Keyboard.Init(type);
			Mouse.Init(type);
			#endif

			#if XNA
			GamePad.Init(type);
			#endif
			
			#if iOS || ANDROID
			TouchScreen.Init(type);
			#endif

			return input;
		}
		#else
		public static InputI Init(InputTypes typeFlags, out InputTypes type, DisposableI parent, Window window)
		{
			#if WINDOWS
			bool winForms = (typeFlags & InputTypes.WinForms) != 0;
			#endif

			#if OSX
			bool cocoa = (typeFlags & InputTypes.Cocoa) != 0;
			#endif

			#if LINUX
			bool x11 = (typeFlags & InputTypes.X11) != 0;
			#endif

			type = InputTypes.None;
			Exception lastException = null;
			InputI input = null;
			while (true)
			{
				try
				{
					#if WINDOWS
					if (winForms)
					{
						winForms = false;
						type = InputTypes.WinForms;
						input = new Reign.Input.WinForms.Input(parent, window);
						break;
					}
					#endif
					
					#if OSX
					if (cocoa)
					{
						cocoa = false;
						type = InputTypes.Cocoa;
						input = new Reign.Input.Cocoa.Input(parent, window);
						break;
					}
					#endif
					
					#if LINUX
					if (x11)
					{
						x11 = false;
						type = InputTypes.X11;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.X11.Input), args);
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
				Debug.ThrowError("Input", "Failed to create Input API" + ex);
				type = InputTypes.None;
			}

			// init api methods
			Keyboard.Init(type);
			Mouse.Init(type);

			return input;
		}
		#endif
	}
}
