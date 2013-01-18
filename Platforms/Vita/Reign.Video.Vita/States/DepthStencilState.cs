using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class DepthStencilState : Disposable, DepthStencilStateI
	{
		#region Properties
		private Video video;
		private DepthStencilStateDesc desc;
		#endregion

		#region Constructors
		public static DepthStencilState New(DisposableI parent, DepthStencilStateDescI desc)
		{
			return new DepthStencilState(parent, desc);
		}

		public DepthStencilState(DisposableI parent, DepthStencilStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (DepthStencilStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (desc.depthReadEnable)
			{
				video.context.Enable(EnableMode.DepthTest);
				video.context.SetDepthFunc(desc.depthFunc, desc.depthWriteEnable);
			}
			else
			{
				video.context.Disable(EnableMode.DepthTest);
			}

			if (desc.stencilEnable)
			{
				video.context.Enable(EnableMode.StencilTest);
				video.context.SetStencilFunc(desc.stencilFunc, 0, 0, 0);
				video.context.SetStencilOp(desc.stencilFailOp, desc.stencilDepthFailOp, desc.stencilPassOp);
			}
			else
			{
				video.context.Disable(EnableMode.StencilTest);
			}
		}
		#endregion
	}
}

