using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class SamplerState : Disposable, SamplerStateI
	{
		#region Properties
		private Video video;
		private SamplerStateDesc desc;
		#endregion

		#region Constructors
		public static SamplerState New(DisposableI parent, SamplerStateDescI desc)
		{
			return new SamplerState(parent, desc);
		}

		public SamplerState(DisposableI parent, SamplerStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (SamplerStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable(int index)
		{
			var samplerState = new X.SamplerState()
			{
				Filter = desc.filter,
				MaxAnisotropy = 1,
				AddressU = desc.addressU,
				AddressV = desc.addressV,
				#if !SILVERLIGHT
				AddressW = desc.addressW
				#endif
			};

			video.Device.SamplerStates[index] = samplerState;
		}
		#endregion
	}
}