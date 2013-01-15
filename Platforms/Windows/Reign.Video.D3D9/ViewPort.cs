using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class ViewPort : ViewPortI
	{
		#region Properties
		private Video video;
		private ViewPortCom com;
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
			init(video);
		}

		public ViewPort(VideoI video, Point2 location, Size2 size)
		: base(location, size)
		{
			init(video);
		}

		private void init(VideoI video)
		{
			this.video = (Video)video;
			com = new ViewPortCom(this.video.com);
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			com.Apply(Location.X, video.BackBufferSize.Height - Size.Height - Location.Y, Size.Width, Size.Height);
		}

		public override void Apply(RenderTargetI renderTarget)
		{
			com.Apply(Location.X, renderTarget.Size.Height - Size.Height - Location.Y, Size.Width, Size.Height);
		}
		#endregion
	}
}