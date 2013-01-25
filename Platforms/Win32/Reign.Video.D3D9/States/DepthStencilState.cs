using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class DepthStencilState : Disposable, DepthStencilStateI
	{
		#region Properties
		private DepthStencilStateCom com;
		#endregion

		#region Constructors
		public static DepthStencilState New(DisposableI parent, DepthStencilStateDescI desc)
		{
			return new DepthStencilState(parent, desc);
		}

		public DepthStencilState(DisposableI parent, DepthStencilStateDescI desc)
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