using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace Reign.Core
{
	public class XNAGame : Game
	{
		#region Properties
		private Application application;
		private GraphicsDeviceManager graphics;
		protected ApplicationEvent theEvent;

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
		#endregion

		#region Constructors
		protected void init(Application application, int width, int height)
		{
			this.application = application;

			try
			{
				#if XNA
				graphics = new GraphicsDeviceManager(this);
				graphics.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
				graphics.SynchronizeWithVerticalRetrace = true;
				graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;

				#if XBOX360
				graphics.PreferredBackBufferWidth = width == 0 ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width : width;
				graphics.PreferredBackBufferHeight = height == 0 ? GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height : height;
				#else
				if (width == 0 || height == 0) Debug.ThrowError("Application", "Width or Height can not be 0");
				graphics.PreferredBackBufferWidth = width;
				graphics.PreferredBackBufferHeight = height;
				#endif

				var gsc = new GamerServicesComponent(this); 
				Components.Add(gsc);

				Content.RootDirectory = "";//"Content";
				#endif
			}
			catch (Exception e)
			{
				application.Close();
				throw e;
			}
		}

		private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.One;
		}
		#endregion

		#region Methods
		protected override void LoadContent()
		{
			application.frameSize = new Size2(GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
			application.shown();
		}

		protected override void UnloadContent()
		{
			application.closing();
		}
		
		protected override void Update(GameTime gameTime)
		{
			application.update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			application.render();
			base.Draw(gameTime);
		}
		#endregion
	}
}
