using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class BlendState : Disposable, BlendStateI
	{
		#region Properties
		private Video video;
		private BlendStateDesc desc;
		#endregion

		#region Constructors
		public static BlendState New(DisposableI parent, BlendStateDescI desc)
		{
			return new BlendState(parent, desc);
		}

		public BlendState(DisposableI parent, BlendStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (BlendStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (desc.BlendEnable)
			{
				var blendState = new X.BlendState()
				{
					ColorWriteChannels = desc.RenderTargetWriteMask,
					
					ColorBlendFunction = desc.BlendOp,
					ColorSourceBlend = desc.SrcBlend,
					ColorDestinationBlend = desc.DstBlend,

					AlphaBlendFunction = desc.BlendOpAlpha,
					AlphaSourceBlend = desc.SrcBlendAlpha,
					AlphaDestinationBlend = desc.DstBlendAlpha
				};
			
				video.Device.BlendState = blendState;
			}
			else
			{
				video.Device.BlendState = X.BlendState.Opaque;
			}
		}
		#endregion
	}
}