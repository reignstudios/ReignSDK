using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class DepthStencilState : Disposable, DepthStencilStateI
	{
		#region Properties
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
			this.desc = (DepthStencilStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (desc.DepthReadEnable)
			{
				GL.Enable(GL.DEPTH_TEST);
				GL.DepthFunc(desc.DepthFunc);
			}
			else
			{
				GL.Disable(GL.DEPTH_TEST);
			}
			
			if (desc.DepthWriteEnable)
			{
				GL.DepthMask(true);
			}
			else
			{
				GL.DepthMask(false);
			}

			if (desc.StencilEnable)
			{
				GL.Enable(GL.STENCIL_TEST);
				GL.StencilFunc(desc.StencilFunc, 0, 0xFFFFFFFF);
				//GL.StencilFuncSeparate(depthStencilDesc.StencilFunc, 0, 0xFFFFFFFF);
				GL.StencilOp(desc.StencilFailOp, desc.StencilDepthFailOp, desc.StencilPassOp);
				//GL.StencilOpSeparate(depthStencilDesc.StencilFailOp, depthStencilDesc.StencilDepthFailOp, depthStencilDesc.StencilPassOp);
			}
			else
			{
				GL.Disable(GL.STENCIL_TEST);
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}