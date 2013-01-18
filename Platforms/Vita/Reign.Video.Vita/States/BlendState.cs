using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class BlendState : Disposable, BlendStateI
	{
		#region Properties
		private Video video;
		private BlendStateDesc desc;
		#endregion

		#region Constructors
		public static BlendState New(DisposableI parent, BlendStateDescI desc)
		{
			return new BlendState(parent, desc);
		}

		public BlendState(DisposableI parent, BlendStateDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (BlendStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			video.context.SetColorMask(desc.colorMask);

			if (desc.blendEnable)
			{
				video.context.Enable(EnableMode.Blend);
				if (desc.blendEnableAlpha)
				{
					video.context.SetBlendFuncRgb(desc.blendOp, desc.srcBlend, desc.dstBlend);
					video.context.SetBlendFuncAlpha(desc.blendOpAlpha, desc.srcBlendAlpha, desc.dstBlendAlpha);
				}
				else
				{
					video.context.SetBlendFunc(desc.blendOp, desc.srcBlend, desc.dstBlend);
				}
			}
			else
			{
				video.context.Disable(EnableMode.Blend);
			}
		}
		#endregion
	}
}

