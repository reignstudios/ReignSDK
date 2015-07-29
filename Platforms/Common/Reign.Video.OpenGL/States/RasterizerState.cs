using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class RasterizerState : DisposableResource, IRasterizerState
	{
		#region Properties
		private RasterizerStateDesc desc;
		#endregion

		#region Constructors
		public RasterizerState(IDisposableResource parent, IRasterizerStateDesc desc)
		: base(parent)
		{
			this.desc = (RasterizerStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			#if !iOS && !ANDROID && !NaCl && !RPI
			GL.PolygonMode(GL.FRONT_AND_BACK, desc.fillMode);
			#endif
			
			if (desc.cullMode != GL.NONE)
			{
				GL.Enable(GL.CULL_FACE);
				GL.CullFace(desc.cullMode);
			}
			else
			{
				GL.Disable(GL.CULL_FACE);
			}

			#if !iOS && !ANDROID && !NaCl && !RPI
			if (desc.multisampleEnable)
			{
				GL.Enable(GL.MULTISAMPLE);
			}
			else
			{
				GL.Disable(GL.MULTISAMPLE);
			}
			#endif

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}