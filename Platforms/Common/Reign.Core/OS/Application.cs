using System;

namespace Reign.Core
{
	public enum ApplicationEventTypes
	{
		Unkown,
		Closed,
		Touch
	}

	public class ApplicationEvent
	{
		public ApplicationEventTypes Type;
		public const int TouchCount = 10;
		public bool[] TouchesOn;
		public Vector2[] TouchLocations;
		
		public ApplicationEvent()
		{
			TouchesOn = new bool[TouchCount];
			TouchLocations = new Vector2[TouchCount];
		}
	}
	
	public enum ApplicationOrientations
	{
		Landscape,
		Portrait
	}
	
	public enum ApplicationAdGravity
	{
		#if WP7 || ANDROID
		BottomLeft,
		BottomRight,
		TopLeft,
		TopRight
		#endif

		#if iOS
		Bottom,
		Top
		#endif
	}

	public enum ApplicationAdSize
	{
		#if WP7
		Large_300x50,
		XLarge_480x80
		#endif

		#if iOS
		Landscape,
		Portrait
		#endif

		#if ANDROID
		Typical_320x50
		#endif
	}

	public class Application
	#if iOS
	: GLController
	#elif ANDROID
	 : RootActivity
	#elif WP7
	: GameDummy
	#elif XNA
	: XNAGame
	#elif METRO
	: MetroApplication
	#endif
	{
		#region Properties
		public delegate void StateMethod();
		public static StateMethod PauseCallback, ResumeCallback;
		
		internal ApplicationEvent theEvent;
		internal ApplicationOrientations orientation;
		
		internal Size2 frameSize;
		public Size2 FrameSize
		{
			get {return frameSize;}
		}

		public delegate void ApplicationEventMethod();
		public ApplicationEventMethod Closing;

		public delegate void HandleEventMethod(ApplicationEvent theEvent);
		public HandleEventMethod HandleEvent;
		#endregion

		#region Constructors
		#if WP7
		public Application(Reign.Core.MathI32.Vector2 frameSize, Microsoft.Xna.Framework.Graphics.GraphicsDevice device)
		{
			this.frameSize = frameSize;
			GraphicsDevice = device;
		}
		#endif

		#if iOS
		public Application(ApplicationOrientations orientation, bool enableAds)
		: base(enableAds)
		#elif ANDROID
		public Application(ApplicationOrientations orientation, bool enableAds, string publisherID)
		: base(enableAds, publisherID)
		#elif METRO
		public Application(ApplicationOrientations orientation)
		#else
		public Application(int width, int height, ApplicationOrientations orientation)
		#endif
		{
			this.orientation = orientation;
			theEvent = new ApplicationEvent();
				
			OS.CurrentApplication = this;
			#if iOS || ANDROID || METRO
			setApplication(this);
			#else
			init(this, width, height, orientation);
			#endif
		}
		#endregion

		#region Methods
		protected internal virtual void shown()
		{
			
		}
		
		protected internal virtual void closing()
		{
			
		}

		public void Close()
		{
			closing();
		}
		
		protected internal virtual void handleEvent(ApplicationEvent theEvent)
		{
			if (HandleEvent != null) HandleEvent(theEvent);
		}

		public void UpdateAndRender()
		{
			update();
			render();
		}
		
		protected internal virtual void update()
		{
			
		}

		protected internal virtual void render()
		{
			
		}
		
		#if iOS || ANDROID || METRO
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