using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.GLKit;
using MonoTouch.OpenGLES;
using MonoTouch.iAd;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Reign.Core
{
	public class GLController : GLKViewController
	{
		class GLRenderer : GLKViewControllerDelegate
		{
			private GLController glController;
			private Application application;
			private bool shown;
			
			private const uint RENDERBUFFER = 0x8D41u;
			private const uint RENDERBUFFER_WIDTH = 0x8D42u;
			private const uint RENDERBUFFER_HEIGHT = 0x8D43u;
			[DllImport("/System/Library/Frameworks/OpenGLES.framework/OpenGLES", EntryPoint = "glGetRenderbufferParameteriv", ExactSpelling = true)]
			private unsafe static extern void GetRenderbufferParameteriv(uint target, uint pname, int* @params);
		
			public GLRenderer(GLController glController, Application application)
			{
				this.glController = glController;
				this.application = application;
			}
			
			public override void Update(GLKViewController controller)
			{
				if (!shown)
				{
					unsafe
					{
						int width = 0, height = 0;
						GetRenderbufferParameteriv(RENDERBUFFER, RENDERBUFFER_WIDTH, &width);
						GetRenderbufferParameteriv(RENDERBUFFER, RENDERBUFFER_HEIGHT, &height);
						application.frameSize = new Size2(width, height);
						glController.frameRatio = application.frameSize.ToVector2() / glController.frameRatio;
					}
					application.shown();
					shown = true;
				}
				OS.UpdateAndRender();
			}
		}
	
		#region Properties
		private Application application;
		protected ApplicationEvent theEvent;
		private EAGLContext context;
		private GLRenderer renderer;
		private Vector2 frameRatio;
		private bool enableAds;
		private ADBannerView iAdView;
		#endregion
		
		#region Constructors
		public GLController(bool enableAds)
		{
			this.enableAds = enableAds;
		}
	
		protected void setApplication(Application application)
		{
			this.application = application;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			try
			{
				View.MultipleTouchEnabled = true;
			
				// set up GL
				PreferredFramesPerSecond = 60;
				context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
				var view = (GLKView)this.View;
				view.Context = context;
				view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format16;
				EAGLContext.SetCurrentContext(context);
			
				renderer = new GLRenderer(this, application);
				Delegate = renderer;
				
				// set view stuff
				View.MultipleTouchEnabled = true;
				if (application.orientation == ApplicationOrientations.Landscape) frameRatio = new Vector2(view.Frame.Height, view.Frame.Width);
				else frameRatio = new Vector2(view.Frame.Width, view.Frame.Height);
			
				// iAd
				if (enableAds)
				{
					iAdView = new ADBannerView();
					var nsM = new NSMutableSet();
					nsM.Add(ADBannerView.SizeIdentifierLandscape);
					iAdView.RequiredContentSizeIdentifiers = nsM;
					iAdView.AdLoaded += new EventHandler(iAdLoaded);
					iAdView.FailedToReceiveAd += new EventHandler<AdErrorEventArgs>(iAdFailedToReceiveAd);
					var adSize = ADBannerView.SizeFromContentSizeIdentifier(ADBannerView.SizeIdentifierLandscape);
					iAdView.Frame = new RectangleF(0, frameRatio.Y-adSize.Height, 1, 1);
					View.AddSubview(iAdView);
					iAdView.Hidden = true;
				}
			}
			catch (Exception e)
			{
				application.Close();
				throw e;
			}
		}
		#endregion
		
		#region Methods
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			var requiredOrientation = (application.orientation == ApplicationOrientations.Landscape) ? UIInterfaceOrientation.LandscapeRight : UIInterfaceOrientation.Portrait;
			return toInterfaceOrientation == requiredOrientation;
		}
		
		private void iAdLoaded(object sender, EventArgs e)
		{
			iAdView.Hidden = false;
		}
		
		private void iAdFailedToReceiveAd(object sender, AdErrorEventArgs e)
		{
			iAdView.Hidden = true;
		}
		
		public override void ViewDidUnload ()
		{
			EAGLContext.SetCurrentContext(context);
			application.Close();
			EAGLContext.SetCurrentContext(null);
			base.ViewDidUnload ();
		}
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			application.resume();
		}
		
		public override void ViewDidDisappear (bool animated)
		{
			application.pause();
			base.ViewDidDisappear (animated);
		}
		
		private void manageTouches(NSSet touches, UIEvent evt, bool isUpEvent)
		{
			for (int i = 0; i != ApplicationEvent.TouchCount; ++i)
			{
				theEvent.TouchesOn[i] = false;
			}
			
			var theTouches = touches.ToArray<UITouch>();
			for (int i = 0; i != theTouches.Length; ++i)
			{
				var touch = theTouches[i];
			
				var loc = touch.LocationInView(View);
				theEvent.TouchLocations[i] = new Vector2(loc.X, loc.Y) * frameRatio;
				theEvent.TouchesOn[i] = !isUpEvent;
			}
			
			theEvent.Type = ApplicationEventTypes.Touch;
			application.handleEvent(theEvent);
		}
		
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			manageTouches(touches, evt, false);
		}
		
		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			manageTouches(touches, evt, true);
		}
		
		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			manageTouches(touches, evt, true);
		}
		
		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			manageTouches(touches, evt, false);
		}
		#endregion
	}
}

