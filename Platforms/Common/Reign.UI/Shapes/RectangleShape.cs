using Reign.Core;

namespace Reign.UI
{
	public class RectangleShape : ShapeI
	{
		#region Properties
		public Point2 Position
		{
			get {return rect.Position;}
			set {rect.Position = value;}
		}

		public Size2 Size
		{
			get {return rect.Size;}
			set {rect.Size = value;}
		}

		protected Rect2 rect;
		public Rect2 Rect
		{
			get {return rect;}
			set {rect = value;}
		}
		#endregion

		#region Constructors
		public RectangleShape()
		{
			
		}

		public RectangleShape(Point2 position, Size2 size)
		{
			rect = new Rect2(position, size);
		}
		#endregion

		#region Methods
		public bool Intersects(Point2 point)
		{
			return point.Intersects(rect);
		}
		#endregion
	}
}
