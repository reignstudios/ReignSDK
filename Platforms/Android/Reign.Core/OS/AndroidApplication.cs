using System;

using Android.App;
using Android.Views;
using Android.OS;
using Android.Content.PM;
using Android.Content;
using Android.Opengl;
using Android.Views.InputMethods;
using Android.Widget;
using Javax.Microedition.Khronos.Egl;
using Javax.Microedition.Khronos.Opengles;

namespace Reign.Core
{
	class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
	{
		private AndroidApplication application;
		private static bool surfaceCreated;
		
		public GLRenderer(AndroidApplication application)
		{
			this.application = application;
			surfaceCreated = false;
		}
		
		public void OnSurfaceCreated (IGL10 gl, EGLConfig config)
		{
			if (surfaceCreated)
			{
				surfaceCreated = false;
				return;
			}
			surfaceCreated = true;
			application.Shown();
		}
		
		public void OnSurfaceChanged (IGL10 gl, int width, int height)
		{
			
		}
		
		public void OnDrawFrame (IGL10 gl)
		{
			OS.UpdateAndRender();
		}
	}

	class GLView : GLSurfaceView
	{	
		private AndroidApplication application;
		private GLRenderer renderer;
		private ApplicationEvent theEvent;
		
		public GLView (AndroidApplication application)
		: base (application)
		{
			this.application = application;
			this.theEvent = application.theEvent;
			
			SetEGLContextClientVersion(2);
			SetEGLConfigChooser(true);//8, 8, 8, 8, 16, 0);
			renderer = new GLRenderer(application);
			SetRenderer(renderer);
		}
		
		protected override void OnSizeChanged (int w, int h, int oldw, int oldh)
		{
			application.FrameSize = new Size2(w, h);
			base.OnSizeChanged (w, h, oldw, oldh);
		}
		
		public override bool OnTouchEvent (MotionEvent e)
		{
			for (int i = 0; i != ApplicationEvent.TouchCount; ++i)
			{
				theEvent.TouchesOn[i] = false;
			}
			
			int count = (e.PointerCount <= ApplicationEvent.TouchCount) ? e.PointerCount : ApplicationEvent.TouchCount;
			for (int i = 0; i != count; ++i)
			{
				int id = e.GetPointerId(i);
				theEvent.TouchLocations[id] = new Vector2(e.GetX(i), e.GetY(i));
				theEvent.TouchesOn[id] = (e.Action != MotionEventActions.Up);
			}
			
			theEvent.Type = ApplicationEventTypes.Touch;
			application.handleEvent(theEvent);
			return true;
		}
	}

	public abstract class AndroidApplication : Activity, ApplicationI
	{
		#region Properties
		private View view;
		private PowerManager.WakeLock wakeLock;
		private View adMobView;
		
		private ApplicationDesc desc;
		public ApplicationOrientations Orientation {get; private set;}
		public Size2 FrameSize {get; internal set;}
		public bool Closed {get; private set;}
		
		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;
		
		internal ApplicationEvent theEvent;
		#endregion
		
		#region Constructors
		public AndroidApplication()
		{
			// NOTE: this is required by MonoDroid
		}
		
		public void Init(ApplicationDesc desc)
		{
			OS.CurrentApplication = this;
			this.desc = desc;
			theEvent = new ApplicationEvent();
			OS.time = new Time(0);
			OS.time.Start();
		}
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			OS.AutoDisposedGL = false;
			
			// Screen orientation
			var requiredOrientation = (desc.Orientation == ApplicationOrientations.Landscape) ? Android.Content.PM.ScreenOrientation.Landscape : Android.Content.PM.ScreenOrientation.Portrait;
			RequestedOrientation = requiredOrientation;
		
			// Remove title bar
			RequestWindowFeature(WindowFeatures.NoTitle);
			Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
			
			// Create gl view
			VolumeControlStream = Android.Media.Stream.Music;
			view = new GLView(this);
			SetContentView(view);
			
			// Ads
			if (desc.UseAds)
			{
				adMobView = AdMobWrapper.CreateAdView(this, desc.PublisherID);
				var layout = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent, GravityFlags.Bottom);
				AddContentView(adMobView, layout);
				AdMobWrapper.LoadAd(adMobView, false);
				adMobView.Visibility = Android.Views.ViewStates.Visible;
				adMobView.BringToFront();
			}
			
			// keep screen from locking
			var power = (PowerManager)GetSystemService(Context.PowerService);
			wakeLock = power.NewWakeLock(WakeLockFlags.ScreenDim, "AndroidApplication");
		}
		#endregion
		
		#region Method Events
		internal void handleEvent(ApplicationEvent applicationEvent)
		{
			if (HandleEvent != null) HandleEvent(applicationEvent);
		}
		
		public override bool OnKeyUp (Android.Views.Keycode keyCode, Android.Views.KeyEvent e)
		{
			if (keyCode == Android.Views.Keycode.Back && !BackEvent()) return false;
			return base.OnKeyUp (keyCode, e);
		}
		
		public virtual bool BackEvent()
		{
			return true;
		}
		
		protected override void OnStart ()
		{
			base.OnStart ();
			OS.AutoDisposedGL = false;
			Resume();
			if (wakeLock != null) wakeLock.Acquire();
		}
		
		protected override void OnStop ()
		{
			OS.AutoDisposedGL = true;
			if (wakeLock != null) wakeLock.Release();
			Pause();
			base.OnStop ();
		}
		
		protected override void OnDestroy ()
		{
			OS.AutoDisposedGL = true;
			if (adMobView != null)
			{
				var group = view.Parent as ViewGroup;
				if (group != null) group.RemoveView(adMobView);
				AdMobWrapper.Destroy(adMobView);
			}
			Closing();
			base.OnDestroy ();
		}
		#endregion
		
		#region Methods
		public virtual void Shown()
		{
			
		}
		
		public virtual void Closing()
		{
			
		}
		
		public new virtual void Close()
		{
			
		}
		
		public virtual void Update(Time time)
		{
			
		}
		
		public virtual void Render(Time time)
		{
			
		}
		
		public virtual void Pause()
		{
			if (PauseCallback != null) PauseCallback();
		}
		
		public virtual void Resume()
		{
			if (ResumeCallback != null) ResumeCallback();
		}
		
		public void ShowCursor()
		{
			
		}
		
		public void HideCursor()
		{
			
		}
		#endregion
	}
}

