using System;
using Reign.Core;

namespace Reign.Input.Metro
{
	public class Mouse : Disposable, MouseI
	{
		#region Properties
		private Input input;
		
		public Button Left {get; private set;}
		public Button Middle {get; private set;}
		public Button Right {get; private set;}
		public float ScrollWheelVelocity {get; private set;}
		public Vector2 Velocity {get; private set;}
		public Vector2 Location {get; private set;}
		public Vector2 ScreenLocation {get; private set;}
		
		private bool leftOn, middleOn, rightOn, scollWheelChanged;
		private float scrollWheelVelocity;
		private Vector2 lastLocation, lastScreenLocation;
		private Point cursorLocation;
		#endregion
	
		#region Constructors
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
					cursorLocation = theEvent.CursorLocation;
					break;

				case (ApplicationEventTypes.LeftMouseUp):
					leftOn = false;
					cursorLocation = theEvent.CursorLocation;
					break;

				case (ApplicationEventTypes.MiddleMouseDown):
					middleOn = true;
					cursorLocation = theEvent.CursorLocation;
					break;

				case (ApplicationEventTypes.MiddleMouseUp):
					middleOn = false;
					cursorLocation = theEvent.CursorLocation;
					break;

				case (ApplicationEventTypes.RightMouseDown):
					rightOn = true;
					cursorLocation = theEvent.CursorLocation;
					break;

				case (ApplicationEventTypes.RightMouseUp):
					rightOn = false;
					cursorLocation = theEvent.CursorLocation;
					break;

				case (ApplicationEventTypes.MouseMove):
					cursorLocation = theEvent.CursorLocation;
					break;
				
				case (ApplicationEventTypes.ScrollWheel):
					scrollWheelVelocity = theEvent.ScrollWheelVelocity;
					scollWheelChanged = true;
					cursorLocation = theEvent.CursorLocation;
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
			
			lastLocation = Location;
			Location = new Vector2(cursorLocation.X, input.application.FrameSize.Height - cursorLocation.Y);
			Velocity = Location - lastLocation;
			
			lastScreenLocation = ScreenLocation;
			ScreenLocation = new Vector2(cursorLocation.X, cursorLocation.Y);
		}
		#endregion
	}
}
