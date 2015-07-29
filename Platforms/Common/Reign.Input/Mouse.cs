using Reign.Core;

namespace Reign.Input
{
	public interface IMouse : IDisposableResource
	{
		PositionButton Left {get;}
		PositionButton Middle {get;}
		PositionButton Right {get;}
		float ScrollWheelVelocity {get;}
		Point2 Velocity {get;}
		Vector2 Velocityf {get;}
		Point2 Position {get;}
		Vector2 Positionf {get;}
	
		void Update();
	}
}
