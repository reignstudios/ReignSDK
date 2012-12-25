using Reign.Core;

namespace Reign.Input
{
	public interface MouseI : DisposableI
	{
		Button Left {get;}
		Button Middle {get;}
		Button Right {get;}
		float ScrollWheelVelocity {get;}
		Point2 Velocity {get;}
		Vector2 VelocityVector {get;}
		Point2 Position {get;}
		Vector2 PositionVector {get;}
	
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
