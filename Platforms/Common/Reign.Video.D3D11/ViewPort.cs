using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class ViewPort : IViewPort
	{
		#region Properties
		private Video video;
		private ViewPortCom com;
		#endregion

		#region Constructors
		public ViewPort(IDisposableResource parent, Point2 position, Size2 size)
		: base(position, size)
		{
			init(parent);
		}

		private void init(IDisposableResource parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			com = new ViewPortCom(this.video.com);
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			com.Apply(Position.X, video.BackBufferSize.Height - Size.Height - Position.Y, Size.Width, Size.Height);
		}

		public override void Apply(IRenderTarget renderTarget)
		{
			com.Apply(Position.X, renderTarget.Size.Height - Size.Height - Position.Y, Size.Width, Size.Height);
		}
		#endregion
	}
}