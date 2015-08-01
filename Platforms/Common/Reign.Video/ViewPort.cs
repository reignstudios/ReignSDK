using Reign.Core;

namespace Reign.Video
{
	public abstract class IViewPort
	{
		#region Properties
		public Point2 Position {get; set;}
		public Size2 Size {get; set;}
		public float AspectRatio {get{return Size.Width / (float)Size.Height;}}
		#endregion

		#region Constructors
		protected IViewPort(Point2 position, Size2 size)
		{
			Set(position, size);
		}
		#endregion

		#region Methods
		public void Set(int x, int y, int width, int height)
		{
			Position = new Point2(x, y);
			Size = new Size2(width, height);
		}

		public void Set(Point2 position, Size2 size)
		{
			this.Position = position;
			this.Size = size;
		}

		public abstract void Apply();
		public abstract void Apply(IRenderTarget renderTarget);
		#endregion
	}
}
