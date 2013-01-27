using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace Reign.Core
{
	public abstract class XNAApplication : Game, ApplicationI
	{
		#region Properties
		private GraphicsDeviceManager graphics;

		public bool SimulateTrailMode
		{
			get
			{
				return Guide.SimulateTrialMode;
			}
			set
			{
				Guide.SimulateTrialMode = value;
			}
		}

		public bool TrailMode
		{
			get {return Guide.IsTrialMode;}
		}

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
			#if XNA
			graphics = new GraphicsDeviceManager(this);
			graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
			graphics.SynchronizeWithVerticalRetrace = true;
			graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

			var frame = desc.FrameSize;
			#if XBOX360
			if (frame.Width == 0 || frame.Height == 0)
			{
				var display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
				frame = new Size2(display.Width, display.Height);
			}
			graphics.PreferredBackBufferWidth = frame.Width;
			graphics.PreferredBackBufferHeight = frame.Height;
			#else
			if (frame.Width == 0 || frame.Height == 0) frame = (OS.ScreenSize.ToVector2() / 1.5f).ToSize();
			graphics.PreferredBackBufferWidth = frame.Width;
			graphics.PreferredBackBufferHeight = frame.Height;
			#endif

			var gsc = new GamerServicesComponent(this); 
			Components.Add(gsc);

			Content.RootDirectory = "";
			#endif
		}

		private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
		}
		#endregion

		#region Method Events
		protected override void LoadContent()
		{
			FrameSize = new Size2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			Shown();
		}

		protected override void UnloadContent()
		{
			Closing();
		}
		
		protected override void Update(GameTime gameTime)
		{
			OS.time.ManualUpdate(gameTime.ElapsedGameTime.Milliseconds / 1000f);
			Update(OS.time);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			OS.renderTime.ManualUpdate(gameTime.ElapsedGameTime.Milliseconds / 1000f);
			Render(OS.renderTime);
			base.Draw(gameTime);
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
			if (Closed) return;
			Closed = true;
			Exit();
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
