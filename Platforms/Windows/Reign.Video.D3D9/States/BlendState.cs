using Reign.Video;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class BlendState : Disposable, BlendStateI
	{
		#region Properties
		private BlendStateCom com;
		#endregion

		#region Constructors
		public static BlendState New(DisposableI parent, BlendStateDescI desc)
		{
			return new BlendState(parent, desc);
		}

		public BlendState(DisposableI parent, BlendStateDescI desc)
		: base(parent)
		{
			var video = parent.FindParentOrSelfWithException<Video>();
			com = new BlendStateCom(video.com, ((BlendStateDesc)desc).com);
		}
		#endregion

		#region Methdos
		public void Enable()
		{
			com.Enable();
		}
		#endregion
	}
}
