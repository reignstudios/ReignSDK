using Reign.Core;

namespace Reign.Input
{
	public interface MouseI : DisposableI
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

	public static class MouseAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			MouseAPI.newPtr = newPtr;
		}

		public delegate MouseI NewPtrMethod(DisposableI parent);
		private static NewPtrMethod newPtr;
		public static MouseI New(DisposableI parent)
		{
			return newPtr(parent);
		}
	}
}
