using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
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
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				com = new SamplerStateCom();
				var error = com.Init(video.com, ((SamplerStateDesc)desc).com);

				if (error == SamplerStateError.SampleState) Debug.ThrowError("SampleState", "Failed to create SampleState");
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
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