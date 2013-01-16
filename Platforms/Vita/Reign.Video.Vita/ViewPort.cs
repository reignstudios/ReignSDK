using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
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
			video.context.SetViewport(Location.X, Location.Y, Size.Width, Size.Height);
		}

		public override void Apply(RenderTargetI renderTarget)
		{
			video.context.SetViewport(Location.X, Location.Y, Size.Width, Size.Height);
		}
		#endregion
	}
}

