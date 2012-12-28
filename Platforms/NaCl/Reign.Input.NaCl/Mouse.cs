using System;
using Reign.Core;

namespace Reign.Input.NaCl
{
	public class Mouse : Disposable, MouseI
	{
		#region Properties
		private Input input;
		
		public Button Left {get; private set;}
		public Button Middle {get; private set;}
		public Button Right {get; private set;}
		public float ScrollWheelVelocity {get; private set;}
		public Point2 Velocity {get; private set;}
		public Vector2 VelocityVector {get; private set;}
		public Point2 Position {get; private set;}
		public Vector2 PositionVector {get; private set;}
		
		private bool leftOn, middleOn, rightOn, scollWheelChanged;
		private float scrollWheelVelocity;
		private Point2 currentPosition, lastPosition;
		#endregion
	
		#region Constructors
		public Mouse(DisposableI parent)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateEventCallback += updateEvent;
			input.UpdateCallback += Update;
			
			Left = new Button();
			Middle = new Button();
			Right = new Button();
		}
		
		public override void Dispose ()
		{
			disposeChilderen();
			if (input != null)
			{
				input.UpdateEventCallback -= updateEvent;
				input.UpdateCallback -= Update;
			}
			base.Dispose ();
		}
		#endregion
		
		#region Methods
		public void updateEvent(WindowEvent theEvent)
		{
			switch (theEvent.Type)
			{
				case (WindowEventTypes.LeftMouseDown):
					leftOn = true;
					currentPosition = theEvent.CursorPosition;
					break;

				case (WindowEventTypes.LeftMouseUp):
					leftOn = false;
					currentPosition = theEvent.CursorPosition;
					break;
				
				case (WindowEventTypes.MiddleMouseDown):
					 middleOn = true;
					 currentPosition = theEvent.CursorPosition;
					 break;

				case (WindowEventTypes.MiddleMouseUp):
					middleOn = false;
					currentPosition = theEvent.CursorPosition;
					break;
				
				case (WindowEventTypes.RightMouseDown):
					rightOn = true;
					currentPosition = theEvent.CursorPosition;
					break;

				case (WindowEventTypes.RightMouseUp):
					rightOn = false;
					currentPosition = theEvent.CursorPosition;
					break;

				case (WindowEventTypes.MouseMove):
					currentPosition = theEvent.CursorPosition;
					break;
				
				case (WindowEventTypes.ScrollWheel):
					scrollWheelVelocity = theEvent.ScrollWheelVelocity;
					scollWheelChanged = true;
					currentPosition = theEvent.CursorPosition;
					break;
			}
		}
		
		public void Update()
		{
			if (scollWheelChanged)
			{
				ScrollWheelVelocity = scrollWheelVelocity;
				scollWheelChanged = false;
			}
			else
			{
				ScrollWheelVelocity = 0;
			}
			
			Left.Update(leftOn);
			Middle.Update(middleOn);
			Right.Update(rightOn);
			
			lastPosition = Position;
			Position = new Point2(currentPosition.X, input.window.FrameSize.Height - currentPosition.Y);
			PositionVector = Position.ToVector2();

			Velocity = Position - lastPosition;
			VelocityVector = Velocity.ToVector2();
		}
		#endregion
	}
}

