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
	public class RootActivity : Activity
	{
		class GLView : GLSurfaceView
		{
			class GLRenderer : Java.Lang.Object, GLSurfaceView.IRenderer
			{
				private Application application;
				private static bool surfaceCreated;
			
				public GLRenderer(Application application)
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
					application.shown();
				}
				
				public void OnSurfaceChanged (IGL10 gl, int width, int height)
				{
					
				}
				
				public void OnDrawFrame (IGL10 gl)
				{
					OS.UpdateAndRender();
				}
			}
			
			private Application application;
			private GLRenderer renderer;
			private ApplicationEvent theEvent;
			
			public GLView (RootActivity rootActivity, Application application)
			: base (rootActivity)
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
				application.frameSize = new Size2(w, h);
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
		
		internal Application application;
		protected ApplicationEvent theEvent;
		private View view;
		private PowerManager.WakeLock wakeLock;
		private View adMobView;
		private bool enableAds;
		private string publisherID;
		
		public RootActivity()
		{
			// this is required by MonoDroid
		}
		
		public RootActivity(bool enableAds, string publisherID)
		{
			this.enableAds = enableAds;
			this.publisherID = publisherID;
		}
		
		protected void setApplication(Application application)
		{
			this.application = application;
		}
		
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			OS.AutoDisposedGL = false;
			
			try
			{
				// Screen orientation
				var requiredOrientation = (application.orientation == ApplicationOrientations.Landscape) ? Android.Content.PM.ScreenOrientation.Landscape : Android.Content.PM.ScreenOrientation.Portrait;
				RequestedOrientation = requiredOrientation;
			
				// Remove title bar
				RequestWindowFeature(WindowFeatures.NoTitle);
				Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
				
				// Create gl view
				VolumeControlStream = Android.Media.Stream.Music;
				view = new GLView(this, application);
				SetContentView(view);
				
				// Ads
				if (enableAds)
				{
					adMobView = AdMobWrapper.CreateAdView(this, publisherID);
					var layout = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.WrapContent, FrameLayout.LayoutParams.WrapContent, GravityFlags.Bottom);
					AddContentView(adMobView, layout);
					AdMobWrapper.LoadAd(adMobView, false);
					adMobView.Visibility = Android.Views.ViewStates.Visible;
					adMobView.BringToFront();
				}
				
				// keep screen from locking
				var power = (PowerManager)GetSystemService(Context.PowerService);
				wakeLock = power.NewWakeLock(WakeLockFlags.ScreenDim, "RootActivity");
			}
			catch (Exception e)
			{
				application.closing();
				throw e;
			}
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
			if (application != null) application.resume();
			if (wakeLock != null) wakeLock.Acquire();
		}
		
		protected override void OnStop ()
		{
			OS.AutoDisposedGL = true;
			if (wakeLock != null) wakeLock.Release();
			if (application != null) application.pause();
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
			application.closing();
			base.OnDestroy ();
		}
	}
}

