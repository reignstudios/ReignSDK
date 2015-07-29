using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class BlendState : DisposableResource, IBlendState
	{
		#region Properties
		private BlendStateDesc desc;
		#endregion

		#region Constructors
		public BlendState(IDisposableResource parent, IBlendStateDesc desc)
		: base(parent)
		{
			this.desc = (BlendStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			GL.ColorMask(desc.renderTargetWriteMaskR, desc.renderTargetWriteMaskG, desc.renderTargetWriteMaskB, desc.renderTargetWriteMaskA);

			if (desc.blendEnable)
			{
				GL.Enable(GL.BLEND);
				if (desc.blendEnableAlpha)
				{
					GL.BlendEquationSeparate(desc.blendOp, desc.blendOpAlpha);
					GL.BlendFuncSeparate(desc.srcBlend, desc.dstBlend, desc.srcBlendAlpha, desc.dstBlendAlpha);
				}
				else
				{
					GL.BlendEquation(desc.blendOp);
					GL.BlendFunc(desc.srcBlend, desc.dstBlend);
				}
			}
			else
			{
				GL.Disable(GL.BLEND);
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}