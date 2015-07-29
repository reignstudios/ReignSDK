using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class ViewPort : IViewPort
	{
		#region Constructors
		public ViewPort(IDisposableResource parent, int x, int y, int width, int height)
		: base(x, y, width, height)
		{
			
		}

		public ViewPort(IDisposableResource parent, Point2 location, Size2 size)
		: base(location, size)
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