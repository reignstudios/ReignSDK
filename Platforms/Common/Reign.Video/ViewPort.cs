using Reign.Core;

namespace Reign.Video
{
	public abstract class ViewPortI
	{
		#region Properties
		public Point2 Position {get; set;}
		public Size2 Size {get; set;}
		public float AspectRatio {get{return Size.Width / (float)Size.Height;}}
		#endregion

		#region Constructors
		protected ViewPortI(int x, int y, int width, int height)
		{
			Set(x, y, width, height);
		}

		protected ViewPortI(Point2 location, Size2 size)
		{
			Set(location, size);
		}
		#endregion

		#region Methods
		public void Set(int x, int y, int width, int height)
		{
			Position = new Point2(x, y);
			Size = new Size2(width, height);
		}

		public void Set(Point2 location, Size2 size)
		{
			this.Position = location;
			this.Size = size;
		}

		public abstract void Apply();
		public abstract void Apply(RenderTargetI renderTarget);
		#endregion
	}

	public static class ViewPortAPI
	{
		public static void Init(NewPtrMethod1 newPtr1, NewPtrMethod2 newPtr2)
		{
			ViewPortAPI.newPtr1 = newPtr1;
			ViewPortAPI.newPtr2 = newPtr2;
		}

		public delegate ViewPortI NewPtrMethod1(VideoI video, int x, int y, int width, int height);
		private static NewPtrMethod1 newPtr1;
		public static ViewPortI New(VideoI video, int x, int y, int width, int height)
		{
			return newPtr1(video, x, y, width, height);
		}

		public delegate ViewPortI NewPtrMethod2(VideoI video, Point2 location, Size2 size);
		private static NewPtrMethod2 newPtr2;
		public static ViewPortI New(VideoI video, Point2 location, Size2 size)
		{
			return newPtr2(video, location, size);
		}
	}
}
