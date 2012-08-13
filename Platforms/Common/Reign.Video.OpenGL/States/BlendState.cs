using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class BlendState : Disposable, BlendStateI
	{
		#region Properties
		private BlendStateDesc desc;
		#endregion

		#region Constructors
		public BlendState(DisposableI parent, BlendStateDescI desc)
		: base(parent)
		{
			this.desc = (BlendStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			GL.ColorMask(desc.RenderTargetWriteMaskR, desc.RenderTargetWriteMaskG, desc.RenderTargetWriteMaskB, desc.RenderTargetWriteMaskA);

			if (desc.BlendEnable)
			{
				GL.Enable(GL.BLEND);
				if (desc.BlendEnableAlpha)
				{
					GL.BlendEquationSeparate(desc.BlendOp, desc.BlendOpAlpha);
					GL.BlendFuncSeparate(desc.SrcBlend, desc.DstBlend, desc.SrcBlendAlpha, desc.DstBlendAlpha);
				}
				else
				{
					GL.BlendEquation(desc.BlendOp);
					GL.BlendFunc(desc.SrcBlend, desc.DstBlend);
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