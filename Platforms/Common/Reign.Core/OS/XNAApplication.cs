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

			if (desc.DepthBit == -1) graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
			else if (desc.DepthBit == 24 && desc.StencilBit == 8) graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
			else if (desc.DepthBit == 24 && desc.StencilBit == 0) graphics.PreferredDepthStencilFormat = DepthFormat.Depth24;
			else if (desc.DepthBit == 16) graphics.PreferredDepthStencilFormat = DepthFormat.Depth16;
			else if (desc.DepthBit == 0) graphics.PreferredDepthStencilFormat = DepthFormat.None;
			else Debug.ThrowError("XNAApplication", string.Format("Unsuported DepthBit: {0} or StencilBit: {1}", desc.DepthBit, desc.StencilBit));

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
			if (frame.Width == 0 || frame.Height == 0) frame = (OS.ScreenSize.ToVector2() / 1.5f).ToSize2();
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
