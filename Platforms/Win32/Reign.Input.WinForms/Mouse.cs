using System;
using System.Windows.Forms;
using Reign.Core;

namespace Reign.Input.WinForms
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
		private Point2 currentPosition, lastLocation;
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
			input.UpdateEventCallback += UpdateEvent;
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
				input.UpdateEventCallback -= UpdateEvent;
				input.UpdateCallback -= Update;
			}
			base.Dispose ();
		}
		#endregion
		
		#region Methods
		public void UpdateEvent(ApplicationEvent theEvent)
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
				ScrollWheelVelocity = scrollWheelVelocity / 120f;
				scollWheelChanged = false;
			}
			else
			{
				ScrollWheelVelocity = 0;
			}
			
			Left.Update(leftOn);
			Middle.Update(middleOn);
			Right.Update(rightOn);
			
			lastLocation = Position;
			Position = new Point2(currentPosition.X, input.application.FrameSize.Height - currentPosition.Y);
			PositionVector = Position.ToVector2();

			Velocity = Position - lastLocation;
			VelocityVector = Velocity.ToVector2();
		}
		#endregion
	}
}
