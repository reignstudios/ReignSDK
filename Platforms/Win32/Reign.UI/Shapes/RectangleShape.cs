using Reign.Core;

namespace Reign.UI
{
	public class RectangleShape : ShapeI
	{
		#region Properties
		private Vector2 location;
		public Vector2 Location
		{
			get {return location;}
			set
			{
				location = value;
				updateBoundingBox();
			}
		}
		
		private Vector2 size;
		public Vector2 Size
		{
			get {return size;}
			set
			{
				size = value;
				updateBoundingBox();
			}
		}

		private BoundingBox2 boundingBox;
		public BoundingBox2 BoundingBox {get {return boundingBox;}}
		#endregion

		#region Constructors
		public RectangleShape(Vector2 location, Vector2 size)
		{
			Location = location;
			Size = size;
			updateBoundingBox();
		}
		#endregion

		#region Methods
		private void updateBoundingBox()
		{
			boundingBox.Min = location;
			boundingBox.Max = location + size;
		}
		#endregion
	}
}
