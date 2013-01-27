using System;
using Sce.PlayStation.Core;

namespace Reign.Core
{
	public abstract class VitaApplication : ApplicationI
	{
		#region Properties
		public ApplicationOrientations Orientation {get; private set;}

		public Size2 FrameSize {get; private set;}
		public bool Closed {get; private set;}

		public event ApplicationHandleEventMethod HandleEvent;
		public event ApplicationStateMethod PauseCallback, ResumeCallback;

		private ApplicationEvent theEvent;
		#endregion
	
		#region Constructors
		public void Init(ApplicationDesc desc)
		{
			Shown();
		}
		#endregion
		
		#region Method Events
		public void Vita_SetFrameSize(int width, int height)
		{
			FrameSize = new Size2(width, height);
		}
		#endregion
		
		#region Methods
		public virtual void Shown()
		{
			
		}

		public virtual void Closing()
		{
			Closed = true;
		}

		public void Close()
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

