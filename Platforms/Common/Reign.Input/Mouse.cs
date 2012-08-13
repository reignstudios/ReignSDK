using Reign.Core;

namespace Reign.Input
{
	public interface MouseI : DisposableI
	{
		Vector2 Velocity {get;}
		Button Left {get;}
		Button Middle {get;}
		Button Right {get;}
		float ScrollWheelVelocity {get;}
		Vector2 Location {get;}
		Vector2 ScreenLocation {get;}
	
		void Update();
	}
}
