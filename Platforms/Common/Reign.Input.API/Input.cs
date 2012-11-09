using System;
using Reign.Core;
using Reign.Input;
using System.Reflection;

namespace Reign.Input.API
{
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
		internal const string WinForms = "Reign.Input.WinForms";
		internal const string Metro = "Reign.Input.Metro";
		internal const string XNA = "Reign.Input.XNA";
		internal const string Cocoa = "Reign.Input.Cocoa";
		internal const string X11 = "Reign.Input.X11";
		internal const string Android = "Reign.Input.Android";

		public static InputI Create(InputTypes typeFlags, out InputTypes type, params object[] args)
		{
			#if WINDOWS
			bool winForms = (typeFlags & InputTypes.WinForms) != 0;
			#endif

			#if METRO
			bool metro = (typeFlags & InputTypes.Metro) != 0;
			#endif

			#if XNA
			bool xna = (typeFlags & InputTypes.XNA) != 0;
			#endif

			#if OSX || iOS
			bool cocoa = (typeFlags & InputTypes.Cocoa) != 0;
			#endif

			#if LINUX
			bool x11 = (typeFlags & InputTypes.X11) != 0;
			#endif

			#if ANDROID
			bool android = (typeFlags & InputTypes.Android) != 0;
			#endif

			Exception lastException = null;
			while (true)
			{
				try
				{
					#if WINDOWS
					if (winForms)
					{
						winForms = false;
						type = InputTypes.WinForms;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.WinForms.Input), args);
					}
					#endif

					#if METRO
					if (metro)
					{
						metro = false;
						type = InputTypes.Metro;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.Metro.Input), args);
					}
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						type = InputTypes.XNA;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.XNA.Input), args);
					}
					#endif
					
					#if OSX || iOS
					if (cocoa)
					{
						cocoa = false;
						type = InputTypes.Cocoa;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.Cocoa.Input), args);
					}
					#endif
					
					#if LINUX
					if (x11)
					{
						x11 = false;
						type = InputTypes.X11;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.X11.Input), args);
					}
					#endif
					
					#if ANDROID
					if (android)
					{
						android = false;
						type = InputTypes.Android;
						return (InputI)OS.CreateInstance(typeof(Reign.Input.Android.Input), args);
					}
					#endif

					else break;
				}
				catch (TargetInvocationException e)
				{
					if (e.InnerException != null) lastException = e.InnerException;
					else lastException = e;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
			Debug.ThrowError("Input", "Failed to create Input API" + ex);
			type = InputTypes.None;
			return null;
		}
	}
}
