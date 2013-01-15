using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class SamplerState : Disposable, SamplerStateI
	{
		#region Properties
		private SamplerStateCom com;
		#endregion

		#region Constructors
		public static SamplerState New(DisposableI parent, SamplerStateDescI desc)
		{
			return new SamplerState(parent, desc);
		}

		public SamplerState(DisposableI parent, SamplerStateDescI desc)
		: base(parent)
		{
			var video = parent.FindParentOrSelfWithException<Video>();
			com = new SamplerStateCom(video.com, ((SamplerStateDesc)desc).com);
		}
		#endregion

		#region Methods
		public void Enable(int index)
		{
			com.Enable(index);
		}
		#endregion
	}
}