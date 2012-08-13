using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class ViewPort : ViewPortI
	{
		#region Properties
		private Video video;
		private ViewPortCom com;
		#endregion

		#region Constructors
		public ViewPort(VideoI video)
		{
			init(video);
		}

		public ViewPort(VideoI video, int x, int y, int width, int height)
		: base(x, y, width, height)
		{
			init(video);
		}

		public ViewPort(VideoI video, Point location, Size2 size)
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
		public override void Apply(RenderTargetI renderTarget)
		{
			if (renderTarget == null) com.Apply(Location.X, video.windowFrameSize.Height - Size.Height - Location.Y, Size.Width, Size.Height);
			else com.Apply(Location.X, renderTarget.Size.Height - Size.Height - Location.Y, Size.Width, Size.Height);
		}
		#endregion
	}
}