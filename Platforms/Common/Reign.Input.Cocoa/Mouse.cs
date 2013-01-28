using System;
using MonoMac.AppKit;
using Reign.Core;

namespace Reign.Input.Cocoa
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
		public static Mouse New(DisposableI parent)
		{
			return new Mouse(parent);
		}
		
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
		public void updateEvent(ApplicationEvent theEvent)
		{
			switch (theEvent.Type)
			{
				case (ApplicationEventTypes.LeftMouseDown):
					leftOn = true;
					currentPosition = theEvent.CursorPosition;
					break;

				case (ApplicationEventTypes.LeftMouseUp):
					leftOn = false;
					currentPosition = theEvent.CursorPosition;
					break;
				
				case (ApplicationEventTypes.MiddleMouseDown):
					 middleOn = true;
					 currentPosition = theEvent.CursorPosition;
					 break;

				case (ApplicationEventTypes.MiddleMouseUp):
					middleOn = false;
					currentPosition = theEvent.CursorPosition;
					break;
				
				case (ApplicationEventTypes.RightMouseDown):
					rightOn = true;
					currentPosition = theEvent.CursorPosition;
					break;

				case (ApplicationEventTypes.RightMouseUp):
					rightOn = false;
					currentPosition = theEvent.CursorPosition;
					break;

				case (ApplicationEventTypes.MouseMove):
					currentPosition = theEvent.CursorPosition;
					break;
				
				case (ApplicationEventTypes.ScrollWheel):
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
			Position = currentPosition;
			PositionVector = Position.ToVector2();
			
			Velocity = Position - lastPosition;
			VelocityVector = Velocity.ToVector2();
		}
		#endregion
	}
}

