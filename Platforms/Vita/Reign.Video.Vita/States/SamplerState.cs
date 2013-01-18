using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
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
			video.currentSamplerStates[index] = this;
		}

		internal void enable(Texture2D texture)
		{
			texture.texture.SetFilter(desc.filterMag, desc.filterMin, desc.filterMinMiped);
			texture.texture.SetWrap(desc.addressU, desc.addressV);
		}
		#endregion
	}
}

