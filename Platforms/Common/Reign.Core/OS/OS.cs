#if WINDOWS
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing;
#endif

#if OSX
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

#if iOS
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

#if ANDROID
using Android.App;
#endif

#if NaCl
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endif

#if METRO
using System.Reflection;
using Windows.ApplicationModel.Core;
#endif

using System;
using System.Collections.Generic;
using System.Threading;

namespace Reign.Core
{
	#if iOS
	[Register ("AppDelegate")]
	class AppDelegate : UIApplicationDelegate
	{
		private UIWindow window;
		
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			app.StatusBarHidden = true;
			app.IdleTimerDisabled = true;
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			window.RootViewController = OS.CurrentApplication;
			window.MakeKeyAndVisible();
			
			return true;
		}
	}
	#endif

	public enum UpdateAndRenderModes
	{
		Stepping,
		Adaptive
	}

	public static class OS
	{
		#region Properites
		public static bool AutoDisposedGL {get; internal set;}
		public static UpdateAndRenderModes UpdateAndRenderMode {get; private set;}
		internal static Time updateTime, renderTime;

		#if WINDOWS || OSX || LINUX || NaCl
		public static Size2 ScreenSize
		{
			get
			{
				#if WINDOWS
				var screen = System.Windows.Forms.Screen.PrimaryScreen;
				return new Size2(screen.Bounds.Width, screen.Bounds.Height);
				#endif
				
				#if OSX
				var screen = NSScreen.Screens[0];
				var frame = screen.VisibleFrame;
				return new Size2((int)frame.Width, (int)frame.Height);
				#endif
				
				#if LINUX
				unsafe
				{
					int screenWidth = 0, screenHeight = 0;
					var dc = X11.XOpenDisplay(IntPtr.Zero);
					if (X11.XineramaIsActive(dc))
					{
						int screenCount = 0;
						var info = X11.XineramaQueryScreens(dc, &screenCount);
						screenWidth = info[0].width;
						screenHeight = info[0].height;
					}
					else
					{
						var screen = X11.XDefaultScreenOfDisplay(dc);
						screenWidth = X11.XWidthOfScreen(screen);
						screenHeight = X11.XHeightOfScreen(screen);
					}
					
					return new Size2(screenWidth, screenHeight);
				}
				#endif
				
				#if NaCl
				return new Size2(512, 512);
				#endif
			}
		}

		public static Window CurrentWindow;
		#else
		public static Application CurrentApplication;
		#endif
		
		#if OSX
		private static NSAutoreleasePool pool;
		#endif
		
		#if NaCl
		private static bool running, finishedRunning, closed;
		#endif
		#endregion
	
		#region Methods
		public static Type GetType(string libName, string nameSpace, string className)
		{
			return Type.GetType(string.Format("{1}.{2}, {0}, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", libName, nameSpace, className));
		}

		public static object CreateInstance(Type type, params object[] args)
		{
			return Activator.CreateInstance(type, args);
		}
		
		public static object CreateInstance(string libName, string nameSpace, string className, params object[] args)
		{
			var type = GetType(libName, nameSpace, className);
			if (type == null) Debug.ThrowError("OS", "Could not find instance of type: " + className);
			return Activator.CreateInstance(type, args);
		}
		
		public static object InvokeStaticMethod(Type type, string methodName, params object[] args)
		{
			#if METRO
			var types = new Type[args.Length];
			for (int i = 0; i != args.Length; ++i) types[i] = args[i].GetType();
			return type.GetRuntimeMethod(methodName, types).Invoke(null, args);
			#else
			return type.InvokeMember(methodName, System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, null, args);
			#endif
		}

		public static object InvokeStaticMethod(string libName, string nameSpace, string className, string methodName, params object[] args)
		{
			var type = GetType(libName, nameSpace, className);
			if (type == null) Debug.ThrowError("OS", string.Format("Could not find static method of type: {0}:{1}", className, methodName));

			#if METRO
			var types = new Type[args.Length];
			for (int i = 0; i != args.Length; ++i) types[i] = args[i].GetType();
			return type.GetRuntimeMethod(methodName, types).Invoke(null, args);
			#else
			return type.InvokeMember(methodName, System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.InvokeMethod, null, null, args);
			#endif
		}

		#if NaCl
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static void PostMessage(string message);
		
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static int GetInstance();
		
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static IntPtr GetGraphics();
		
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static IntPtr GetPBBInstance();
		
		[MethodImplAttribute (MethodImplOptions.InternalCall)]
		public extern static IntPtr GetGlES();
		
		[DllImport("__Internal", EntryPoint="Mono_InvokeMethodOnMainThread", ExactSpelling = true)]
		public extern static void Mono_InvokeMethodOnMainThread(string assemblyName, string method);
		
		private static void init()
		{
			CurrentWindow.Show();
		}
		
		private static void exit()
		{
			if (CurrentWindow == null) return;
			
			running = false;
			while (!finishedRunning) Thread.Sleep(1);
			CurrentWindow.Close();
			closed = true;
		}
		
		private static void updateAndRender()
		{
			CurrentWindow.UpdateAndRender();
		}
		#endif	
		
		#if OSX
		static OS()
		{
			NSApplication.Init();
			pool = new NSAutoreleasePool();
		}
		#endif
		
		#if WINDOWS || OSX || LINUX || NaCl
		public static void Run(Window window, UpdateAndRenderModes updateAndRenderMode, int fps)
		{
			CurrentWindow = window;
			OS.UpdateAndRenderMode = updateAndRenderMode;
			updateTime = new Time(fps);
			updateTime.Start();
			if (updateAndRenderMode == UpdateAndRenderModes.Adaptive)
			{
				renderTime = new Time(fps);
				renderTime.Start();
			}

			#if WINDOWS
			Time.OptimizedMode();
			window.Show();
			Application.Idle += mainLoop;
			Application.Run(window);
			Time.EndOptimizedMode();
			#endif
			
			#if OSX
			window.Show();
			pool.Dispose();
			pool = null;
			NSApplication.SharedApplication.ApplicationShouldTerminateAfterLastWindowClosed = delegate {return true;};
			NSApplication.Main(new string[0]);
			#endif
			
			#if LINUX
			window.Show();
			while (!window.Closed)
			{
				window.updateWindowEvents();
				window.UpdateAndRender();
				Thread.Sleep(1);
			}
			#endif
			
			#if NaCl
			running = true;
			finishedRunning = false;
			Mono_InvokeMethodOnMainThread("Reign.Core", "Reign.Core.OS:init");
			while (running)
			{
				CurrentWindow.monoThreadUpdate();
				Thread.Sleep(1);
			}
			finishedRunning = true;
			while (!closed) Thread.Sleep(1);
			#endif
		}
		#endif
		
		#if iOS || XNA || METRO
		public static void Run(Application application, UpdateAndRenderModes updateAndRenderMode, int fps)
		{
			CurrentApplication = application;
			OS.UpdateAndRenderMode = updateAndRenderMode;
			updateTime = new Time(fps);
			updateTime.Start();
			#if XNA
			renderTime = new Time(fps);
			renderTime.Start();
			#else
			if (updateAndRenderMode == UpdateAndRenderModes.Adaptive)
			{
				renderTime = new Time(fps);
				renderTime.Start();
			}
			#endif

			#if METRO
			CoreApplication.Run(application.source);
			#endif

			#if iOS
			UIApplication.Main(new string[0], null, "AppDelegate");
			#endif

			#if XNA
			#if !XBOX360
			Time.OptimizedMode();
			#endif
			using (var game = application)
			{
				game.Run();
			}
			#if !XBOX360
			Time.EndOptimizedMode();
			#endif
			#endif
		}
		#endif

		#if WINDOWS
		[StructLayout(LayoutKind.Sequential)]
		private struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point p;
		}
		
		[SuppressUnmanagedCodeSecurity]
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		private static void mainLoop(object sender, EventArgs e)
		{
			var msg = new Message();
			while (!PeekMessage(out msg, IntPtr.Zero, 0, 0, 0))
			{
				OS.UpdateAndRender();
			}
		}
		#endif

		#if WINDOWS || OSX || LINUX || NaCl
		public static void UpdateAndRender()
		{
			if (UpdateAndRenderMode == UpdateAndRenderModes.Stepping)
			{
				if (updateTime.Update())
				{
					CurrentWindow.update(updateTime);
					CurrentWindow.render(updateTime);
					updateTime.Sleep();
				}
			}
			else// Adaptive
			{
				if (updateTime.Update())
				{
					CurrentWindow.update(updateTime);
					int loop = (int)System.Math.Max((updateTime.FPSGoal / updateTime.FPS) - 1, 0);
					for (int i = 0; i != loop; ++i)
					{
						updateTime.AdaptiveUpdate();
						CurrentWindow.update(updateTime);
					}

					renderTime.Update();
					CurrentWindow.render(renderTime);
					updateTime.Sleep();
				}
			}
		}
		#else
		public static void UpdateAndRender()
		{
			if (UpdateAndRenderMode == UpdateAndRenderModes.Stepping)
			{
				if (updateTime.Update())
				{
					CurrentApplication.update(updateTime);
					CurrentApplication.render(updateTime);
					updateTime.Sleep();
				}
			}
			else// Adaptive
			{
				if (updateTime.Update())
				{
					CurrentApplication.update(updateTime);
					int loop = (int)System.Math.Max((updateTime.FPSGoal / updateTime.FPS) - 1, 0);
					for (int i = 0; i != loop; ++i)
					{
						updateTime.AdaptiveUpdate();
						CurrentApplication.update(updateTime);
					}

					renderTime.Update();
					CurrentApplication.render(renderTime);
					updateTime.Sleep();
				}
			}
		}
		#endif
		#endregion
	}
}