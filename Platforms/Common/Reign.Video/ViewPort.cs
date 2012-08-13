using Reign.Core;

namespace Reign.Video
{
	public abstract class ViewPortI
	{
		#region Properties
		public Point Location {get; set;}
		public Size2 Size {get; set;}
		public float AspectRatio {get{return Size.Width / (float)Size.Height;}}
		#endregion

		#region Constructors
		protected ViewPortI() { }

		protected ViewPortI(int x, int y, int width, int height)
		{
			Set(x, y, width, height);
		}

		protected ViewPortI(Point location, Size2 size)
		{
			Set(location, size);
		}
		#endregion

		#region Methods
		public void Set(int x, int y, int width, int height)
		{
			Location = new Point(x, y);
			Size = new Size2(width, height);
		}

		public void Set(Point location, Size2 size)
		{
			this.Location = location;
			this.Size = size;
		}

		public abstract void Apply(RenderTargetI renderTarget);
		#endregion
	}
}
