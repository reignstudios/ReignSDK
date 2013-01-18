using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class RasterizerState : Disposable, RasterizerStateI
	{
		#region Properties
		private Video video;
		private RasterizerStateDesc desc;
		#endregion

		#region Constructors
		public static RasterizerState New(DisposableI parent, RasterizerStateDescI desc)
		{
			return new RasterizerState(parent, desc);
		}

		public RasterizerState(DisposableI parent, RasterizerStateDescI desc)
		: base(parent)
		{
			this.video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (RasterizerStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			var rasterizerState = new X.RasterizerState()
			{
				CullMode = desc.cullMode,
				FillMode = desc.fillMode,
				MultiSampleAntiAlias = desc.multisampleEnable
			};

			video.Device.RasterizerState = rasterizerState;
		}
		#endregion
	}
}