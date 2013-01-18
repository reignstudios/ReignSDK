using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class DepthStencilState : Disposable, DepthStencilStateI
	{
		#region Properties
		private Video video;
		private DepthStencilStateDesc desc;
		#endregion

		#region Constructors
		public static DepthStencilState New(DisposableI parent, DepthStencilStateDescI desc)
		{
			return new DepthStencilState(parent, desc);
		}

		public DepthStencilState(DisposableI parent, DepthStencilStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (DepthStencilStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			var depthStencilState = new X.DepthStencilState()
			{
				DepthBufferEnable = desc.depthReadEnable,
				DepthBufferWriteEnable = desc.depthWriteEnable,
				DepthBufferFunction = desc.depthFunc,

				StencilEnable = desc.stencilEnable,
				StencilFunction = desc.stencilFunc,
				StencilFail = desc.stencilFailOp,
				StencilDepthBufferFail = desc.stencilDepthFailOp,
				StencilPass = desc.stencilPassOp,

				TwoSidedStencilMode = false,
				CounterClockwiseStencilFunction = X.CompareFunction.Never,
				CounterClockwiseStencilFail = X.StencilOperation.Keep,
				CounterClockwiseStencilDepthBufferFail = X.StencilOperation.Keep,
				CounterClockwiseStencilPass = X.StencilOperation.Keep
			};

			video.Device.DepthStencilState = depthStencilState;
		}
		#endregion
	}
}