using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class DepthStencilState : DisposableResource, IDepthStencilState
	{
		#region Properties
		private DepthStencilStateCom com;
		#endregion

		#region Constructors
		public DepthStencilState(IDisposableResource parent, IDepthStencilStateDesc desc)
		: base(parent)
		{
			var video = parent.FindParentOrSelfWithException<Video>();
			com = new DepthStencilStateCom(video.com, ((DepthStencilStateDesc)desc).com);
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