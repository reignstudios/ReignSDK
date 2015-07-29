using System;

namespace Reign.Core
{
	public enum ApplicationEventTypes
	{
		Unkown,
		Closed,
		Touch,
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
	}

	public class ApplicationEvent
	{
		public ApplicationEventTypes Type;

		public const int TouchCount = 10;
		public bool[] TouchesOn;
		public Vector2[] TouchLocations;

		public int KeyCode;
		public float ScrollWheelVelocity;
		public Point2 CursorPosition;
		
		public ApplicationEvent()
		{
			TouchesOn = new bool[TouchCount];
			TouchLocations = new Vector2[TouchCount];
		}
	}

	public enum ApplicationTypes
	{
		Box,
		Frame,
		FrameSizable
	}

	public enum ApplicationStartPositions
	{
		Default,
		CenterCurrentScreen
	}
	
	public enum ApplicationOrientations
	{
		Landscape,
		Portrait
	}

	public class ApplicationDesc
	{
		public string Name = "Reign Application";
		public string Win32_IconFileName;
		public ApplicationTypes Type = ApplicationTypes.Frame;
		public ApplicationStartPositions StartPosition = ApplicationStartPositions.CenterCurrentScreen;
		public ApplicationOrientations Orientation = ApplicationOrientations.Landscape;
		public Size2 FrameSize, MinFrameSize, MaxFrameSize;
		public int DepthBit = -1, StencilBit = -1;
	}

	public delegate void ApplicationEventMethod();
	public delegate void ApplicationHandleEventMethod(ApplicationEvent applicationEvent);
	public delegate void ApplicationStateMethod();

	public interface IApplication
	{
		#region Properties
		#if WIN32
		IntPtr Handle {get;}
		#endif
		#if LINUX
		IntPtr Handle {get;}
		IntPtr DC {get;}
		#endif
		#if WINRT
		Windows.UI.Core.CoreWindow CoreWindow {get;}
		bool IsSnapped {get;}
		#endif
		#if XNA
		Microsoft.Xna.Framework.Graphics.GraphicsDevice GraphicsDevice {get;}
		#endif
		#if OSX
		MonoMac.AppKit.NSView View {get;}
		#endif
		#if iOS
		MonoTouch.UIKit.UIView View {get;}
		#endif

		ApplicationOrientations Orientation {get;}
		Size2 FrameSize {get;}
		bool Closed {get;}
		event ApplicationHandleEventMethod HandleEvent;
		event ApplicationStateMethod PauseCallback;
		event ApplicationStateMethod ResumeCallback;
		#endregion

		#region Constructors
		void Init(ApplicationDesc desc);
		#endregion

		#region Methods
		void Shown();
		void Closing();
		void Close();
		void Update(Time time);
		void Render(Time time);
		void Pause();
		void Resume();
		void HideCursor();
		void ShowCursor();
		#endregion
	}
}