using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class RasterizerState : DisposableResource, IRasterizerState
	{
		#region Properties
		private RasterizerStateCom com;
		#endregion

		#region Constructors
		public RasterizerState(IDisposableResource parent, IRasterizerStateDesc desc)
		: base(parent)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();

				com = new RasterizerStateCom();
				var error = com.Init(video.com, ((RasterizerStateDesc)desc).com);

				if (error == RasterizerStateError.RasterizerState) Debug.ThrowError("RasterizerState", "Failed to create RasterizerState");
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