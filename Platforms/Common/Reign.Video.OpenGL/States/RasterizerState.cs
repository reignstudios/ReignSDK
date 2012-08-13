using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class RasterizerState : Disposable, RasterizerStateI
	{
		#region Properties
		private RasterizerStateDesc desc;
		#endregion

		#region Constructors
		public RasterizerState(DisposableI parent, RasterizerStateDescI desc)
		: base(parent)
		{
			this.desc = (RasterizerStateDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			#if !iOS && !ANDROID && !NaCl
			GL.PolygonMode(GL.FRONT_AND_BACK, desc.FillMode);
			#endif
			
			if (desc.CullMode != GL.NONE)
			{
				GL.Enable(GL.CULL_FACE);
				GL.CullFace(desc.CullMode);
			}
			else
			{
				GL.Disable(GL.CULL_FACE);
			}

			#if !iOS && !ANDROID && !NaCl
			if (desc.MultisampleEnable)
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