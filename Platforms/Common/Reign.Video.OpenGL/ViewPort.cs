using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class ViewPort : IViewPort
	{
		#region Constructors
		public ViewPort(IDisposableResource parent, Point2 position, Size2 size)
		: base(position, size)
		{
			
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			GL.Viewport(Position.X, Position.Y, Size.Width, Size.Height);

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void Apply(IRenderTarget renderTarget)
		{
			GL.Viewport(Position.X, Position.Y, Size.Width, Size.Height);

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}