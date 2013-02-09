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
		public Rect2 Rect {get{return rect;}}
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
	}
}
