using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class RasterizerState : Disposable, RasterizerStateI
	{
		#region Properties
		private RasterizerStateCom com;
		#endregion

		#region Constructors
		public static RasterizerState New(DisposableI parent, RasterizerStateDescI desc)
		{
			return new RasterizerState(parent, desc);
		}

		public RasterizerState(DisposableI parent, RasterizerStateDescI desc)
		: base(parent)
		{
			var video = parent.FindParentOrSelfWithException<Video>();
			com = new RasterizerStateCom(video.com, ((RasterizerStateDesc)desc).com);
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