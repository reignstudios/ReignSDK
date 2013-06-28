using Reign.Core;

namespace Reign.Input
{
	public class PositionButton : Button
	{
		#region Properties
		public Point2 PositionDown {get; private set;}
		public Vector2 PositionDownf {get; private set;}
		public Point2 PositionUp {get; private set;}
		public Vector2 PositionUpf {get; private set;}
		#endregion

		#region Methods
		public void Update(bool on, Point2 position, Vector2 positionf)
		{
			base.Update(on);

			if (Down)
			{
				PositionDown = position;
				PositionDownf = positionf;
			}

			if (Up)
			{
				PositionUp = position;
				PositionUpf = positionf;
			}
		}

		public override void Flush()
		{
			base.Flush();

			PositionDown = Point2.Zero;
			PositionDownf = Vector2.Zero;
			PositionUp = Point2.Zero;
			PositionUpf = Vector2.Zero;
		}
		#endregion
	}
}
