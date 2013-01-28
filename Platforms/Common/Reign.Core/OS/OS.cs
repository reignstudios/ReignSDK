#if WIN32
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

#if WINRT
using System.Reflection;
using Windows.ApplicationModel.Core;
#endif

#if VITA
using Sce.PlayStation.Core.Environment;
#endif

#if XNA
using Microsoft.Xna.Framework.Graphics;
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
	
	#if OSX
	[Register ("AppDelegate")]
	class AppDelegate : NSApplicationDelegate
	{
		public override void FinishedLaunching (NSObject notification)
		{
			NSApplication.SharedApplication.ApplicationShouldTerminateAfterLastWindowClosed = delegate {return true;};
		}
	}
	#endif

	public static class OS
	{
		#region Properites
		#if WIN32 || OSX || LINUX || NaCl || iOS || ANDROID
		public static bool AutoDisposedGL {get; internal set;}
		#endif

		internal static Time time;
		#if XNA && !SILVERLIGHT
		internal static Time renderTime;
		#endif

		public static Size2 ScreenSize
		{
			get
			{
				#if WIN32
				var screen = System.Windows.Forms.Screen.PrimaryScreen;
				return new Size2(screen.Bounds.Width, screen.Bounds.Height);
				#elif XNA && !SILVERLIGHT
				var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
				return new Size2(display.Width, display.Height);
				#elif OSX
				var screen = NSScreen.Screens[0];
				var frame = screen.VisibleFrame;
				return new Size2((int)frame.Width, (int)frame.Height);
				#elif LINUX
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
				#elif NaCl
				return new Size2(512, 512);
				#else
				return new Size2();
				#endif
			}
		}

		public static ApplicationI CurrentApplication;
		
		#if OSX
		private static NSAutoreleasePool pool;
		public static NSOpenGLContext NSContext;
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
			#if WINRT
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

			#if WINRT
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
			
			CurrentWindow.Close();
			running = false;
			while (!finishedRunning) Thread.Sleep(1);
			closed = true;
		}
		
		private static void updateAndRender()
		{
			UpdateAndRender();
		}
		#endif	
		
		#if OSX
		static OS()
		{
			NSApplication.Init();
			pool = new NSAutoreleasePool();
		}
		#endif
		
		public static void Run(ApplicationI application, int fps)
		{
			CurrentApplication = application;

			#if iOS || ANDROID || XNA || VITA || NaCl
			fps = 0;
			#endif
			time = new Time(fps);
			time.Start();

			#if WIN32
			var form = (Form)application;
			Time.OptimizedMode();
			form.Show();
			Application.Idle += mainLoop;
			Application.Run(form);
			Time.EndOptimizedMode();
			#endif
			
			#if OSX
			((CocoaApplication)application).show();
			pool.Dispose();
			pool = null;
			NSTimer.CreateRepeatingScheduledTimer(0, delegate
			{
				using (var autoPool = new NSAutoreleasePool())
				{
					if (NSContext != null) NSContext.CGLContext.Lock();
					UpdateAndRender();
					if (NSContext != null) NSContext.CGLContext.Unlock();
				}
			});
			//NSApplication.SharedApplication.ApplicationShouldTerminateAfterLastWindowClosed = delegate {return true;};
			NSApplication.Main(new string[0]);
			#endif
			
			#if LINUX
			window.Show();
			while (!window.Closed)
			{
				window.updateWindowEvents();
				UpdateAndRender();
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

			#if WINRT
			CoreApplication.Run(((CoreWindowApplication)application).source);
			#endif

			#if iOS
			UIApplication.Main(new string[0], null, "AppDelegate");
			#endif

			#if XNA && !SILVERLIGHT
			renderTime = new Time(fps);
			renderTime.Start();
			#if !XBOX360
			Time.OptimizedMode();
			#endif
			using (var game = (XNAApplication)application)
			{
				game.Run();
			}
			#if !XBOX360
			Time.EndOptimizedMode();
			#endif
			#endif
			
			#if VITA
			while (true)
			{
				SystemEvents.CheckEvents ();
				UpdateAndRender();
			}
			#endif
		}

		#if WIN32
		[StructLayout(LayoutKind.Sequential)]
		private struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public Point2 p;
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

		public static void UpdateAndRender()
		{
			if (time.FPSGoal != 0) time.Sleep();
			time.Update();

			CurrentApplication.Update(time);
			CurrentApplication.Render(time);
		}
		#endregion
	}
}