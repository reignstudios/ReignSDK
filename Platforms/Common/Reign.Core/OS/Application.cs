using System;

namespace Reign.Core
{
	public enum ApplicationEventTypes
	{
		Unkown,
		Closed,
		Touch,
		#if WINRT
		KeyDown,
		KeyUp,
		MouseMove,
		LeftMouseDown,
		LeftMouseUp,
		MiddleMouseDown,
		MiddleMouseUp,
		RightMouseDown,
		RightMouseUp,
		ScrollWheel
		#endif
	}

	public class ApplicationEvent
	{
		public ApplicationEventTypes Type;
		public const int TouchCount = 10;
		public bool[] TouchesOn;
		public Vector2[] TouchLocations;
		#if WINRT
		public int KeyCode;
		public float ScrollWheelVelocity;
		public Point2 CursorLocation;
		#endif
		
		public ApplicationEvent()
		{
			TouchesOn = new bool[TouchCount];
			TouchLocations = new Vector2[TouchCount];
		}
	}
	
	#if !XNA && !VITA
	public enum ApplicationOrientations
	{
		Landscape,
		Portrait
	}
	#endif
	
	public enum ApplicationAdGravity
	{
		#if iOS
		Bottom,
		Top
		#endif

		#if ANDROID || WINRT
		BottomLeft,
		BottomRight,
		BottomCenter,
		TopLeft,
		TopRight,
		TopCenter
		#endif
	}

	public enum ApplicationAdSize
	{
		#if WINRT
		Sqaure_250x250,
		Rect_728x90,
		Rect_500x130,
		Rect_300x250,
		Rect_292x60,
		Rect_250x125,
		Rect_160x600
		#endif

		#if iOS
		Landscape,
		Portrait
		#endif

		#if ANDROID
		Typical_320x50
		#endif
	}

	#if WINRT
	public interface ApplicationI
	{
		Size2 Metro_FrameSize {get; set;}
		void Metro_HandleEvent(ApplicationEvent theEvent);
	}
	#endif

	public class Application
	#if iOS
	: GLController
	#elif ANDROID
	: RootActivity
	#elif SILVERLIGHT
	: SilverlightApplication
	#elif XNA
	: XNAGame
	#elif WINRT
	: MetroApplication, ApplicationI
	#elif WP8
	: XAMLApplication
	#elif VITA
	: VitaApplication
	#endif
	{
		#region Properties
		#if !XNA && !VITA
		internal ApplicationOrientations orientation;
		#endif
		
		internal Size2 frameSize;
		public Size2 FrameSize
		{
			get {return frameSize;}
		}

		#if WINRT
		public Size2 Metro_FrameSize
		{
			get {return frameSize;}
			set {frameSize = value;}
		}
		#endif

		public delegate void ApplicationEventMethod();
		public ApplicationEventMethod Closing;

		public delegate void HandleEventMethod(ApplicationEvent theEvent);
		public HandleEventMethod HandleEvent;

		public delegate void StateMethod();
		public static StateMethod PauseCallback, ResumeCallback;
		#endregion

		#region Constructors
		#if iOS
		public Application(ApplicationOrientations orientation, bool enableAds)
		: base(enableAds)
		#elif ANDROID
		public Application(ApplicationOrientations orientation, bool enableAds, string publisherID)
		: base(enableAds, publisherID)
		#elif WINRT || WP8
		public Application(ApplicationOrientations orientation)
		: base(orientation)
		#elif SILVERLIGHT || VITA
		public Application()
		#elif XNA
		public Application(int width, int height)
		#else
		public Application(int width, int height, ApplicationOrientations orientation)
		#endif
		{
			#if !XNA && !VITA
			this.orientation = orientation;
			#endif
			theEvent = new ApplicationEvent();
				
			OS.CurrentApplication = this;
			#if iOS || ANDROID || WINRT || WP8 || SILVERLIGHT
			setApplication(this);
			#elif XNA
			init(this, width, height);
			#elif VITA
			init(this);
			#else
			init(this, width, height, orientation);
			#endif
			
			#if ANDROID || WP8
			OS.time = new Time(0);
			OS.time.Start();
			#endif
		}
		#endregion

		#region Methods
		protected internal virtual void shown()
		{
			
		}
		
		protected internal virtual void closing()
		{
			#if WINRT
			deferral.Complete();
			#endif
		}

		#if XNA && !SILVERLIGHT
		public void Close()
		{
			Exit();
		}
		#endif
		
		protected internal virtual void handleEvent(ApplicationEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
		}

		#if WINRT
		public void Metro_HandleEvent(ApplicationEvent theEvent)
		{
			handleEvent(theEvent);
		}
		#endif
		
		protected internal virtual void update(Time time)
		{
			
		}

		protected internal virtual void render(Time time)
		{
			
		}
		
		#if iOS || ANDROID || WINRT
		protected internal virtual void pause()
		{
			if (PauseCallback != null) PauseCallback();
		}
		
		protected internal virtual void resume()
		{
			if (ResumeCallback != null) ResumeCallback();
		}
		#endif
		#endregion
	}
}