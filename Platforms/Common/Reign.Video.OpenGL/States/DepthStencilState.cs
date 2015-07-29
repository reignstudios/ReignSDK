using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class DepthStencilState : DisposableResource, IDepthStencilState
	{
		#region Properties
		private DepthStencilStateDesc desc;
		#endregion

		#region Constructors
		public DepthStencilState(IDisposableResource parent, IDepthStencilStateDesc desc)
		: base(parent)
		{
			this.desc = (DepthStencilStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			if (desc.depthReadEnable)
			{
				GL.Enable(GL.DEPTH_TEST);
				GL.DepthFunc(desc.depthFunc);
			}
			else
			{
				GL.Disable(GL.DEPTH_TEST);
			}
			
			if (desc.depthWriteEnable)
			{
				GL.DepthMask(true);
			}
			else
			{
				GL.DepthMask(false);
			}

			if (desc.stencilEnable)
			{
				GL.Enable(GL.STENCIL_TEST);
				GL.StencilFunc(desc.stencilFunc, 0, 0xFFFFFFFF);
				//GL.StencilFuncSeparate(depthStencilDesc.StencilFunc, 0, 0xFFFFFFFF);
				GL.StencilOp(desc.stencilFailOp, desc.stencilDepthFailOp, desc.stencilPassOp);
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