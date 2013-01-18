using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class RasterizerState : Disposable, RasterizerStateI
	{
		#region Properties
		private Video video;
		private RasterizerStateDesc desc;
		#endregion

		#region Constructors
		public static RasterizerState New(DisposableI parent, RasterizerStateDescI desc)
		{
			return new RasterizerState(parent, desc);
		}

		public RasterizerState(DisposableI parent, RasterizerStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (RasterizerStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (desc.cullMode != CullFaceMode.None)
			{
				video.context.Enable(EnableMode.CullFace);
				video.context.SetCullFace(desc.cullMode, CullFaceDirection.Ccw);
			}
			else
			{
				video.context.Disable(EnableMode.CullFace);
			}
		}
		#endregion
	}
}

