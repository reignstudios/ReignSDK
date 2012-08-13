using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class BlendState : Disposable, BlendStateI
	{
		#region Properties
		private BlendStateCom com;
		#endregion

		#region Constructors
		public BlendState(DisposableI parent, BlendStateDescI desc)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				com = new BlendStateCom();
				var error = com.Init(video.com, ((BlendStateDesc)desc).com);

				if (error == BlendStateError.BlendState) Debug.ThrowError("BlendState", "Failed to create BlendState");
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
		public void Enable()
		{
			com.Enable();
		}
		#endregion
	}
}