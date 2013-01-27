using Reign.Core;

namespace Reign.Video.XNA
{
	public class ViewPort : ViewPortI
	{
		#region Properties
		private Video video;
		#endregion

		#region Constructors
		public static ViewPort New(VideoI video, int x, int y, int width, int height)
		{
			return new ViewPort(video, x, y, width, height);
		}

		public static ViewPort New(VideoI video, Point2 location, Size2 size)
		{
			return new ViewPort(video, location, size);
		}

		public ViewPort(VideoI video, int x, int y, int width, int height)
		: base(x, y, width, height)
		{
			this.video = (Video)video;
		}

		public ViewPort(VideoI video, Point2 location, Size2 size)
		: base(location, size)
		{
			this.video = (Video)video;
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			#if SILVERLIGHT
			try {
			#endif
			var viewPort = new Microsoft.Xna.Framework.Graphics.Viewport()
			{
				X = Location.X,
				Y = video.Device.PresentationParameters.BackBufferHeight - Size.Height - Location.Y,
				Width = Size.Width,
				Height = Size.Height,
				MinDepth = 0,
				MaxDepth = 1
			};
			video.Device.Viewport = viewPort;
			#if SILVERLIGHT
			} catch {}
			#endif
		}

		public override void Apply(RenderTargetI renderTarget)
		{
			#if SILVERLIGHT
			try {
			#endif
			var viewPort = new Microsoft.Xna.Framework.Graphics.Viewport()
			{
				X = Location.X,
				Y = renderTarget.Size.Height - Size.Height - Location.Y,
				Width = Size.Width,
				Height = Size.Height,
				MinDepth = 0,
				MaxDepth = 1
			};
			video.Device.Viewport = viewPort;
			#if SILVERLIGHT
			} catch {}
			#endif
		}
		#endregion
	}
}