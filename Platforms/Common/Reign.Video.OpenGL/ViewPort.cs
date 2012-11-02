using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class ViewPort : ViewPortI
	{
		#region Constructors
		public ViewPort(VideoI video)
		{
			
		}

		public ViewPort(VideoI video, int x, int y, int width, int height)
		: base(x, y, width, height)
		{
			
		}

		public ViewPort(VideoI video, Point2 location, Size2 size)
		: base(location, size)
		{
			
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			GL.Viewport((int)Location.X, (int)Location.Y, Size.Width, Size.Height);

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void Apply(RenderTargetI renderTarget)
		{
			GL.Viewport((int)Location.X, (int)Location.Y, Size.Width, Size.Height);

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}