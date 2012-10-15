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
		public Vector2 Velocity {get; private set;}
		public Vector2 Position {get; private set;}
		public Vector2 ScreenLocation {get; private set;}
		
		private bool leftOn, middleOn, rightOn, scollWheelChanged;
		private float scrollWheelVelocity;
		private Vector2 lastLocation, lastScreenLocation;
		private Point viewLocation;
		#endregion
	
		#region Constructors
		public Mouse(DisposableI parent)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateEventCallback += UpdateEvent;
			input.UpdateCallback += Update;
			viewLocation = this.input.window.ViewLocation;
			
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
		public void UpdateEvent(WindowEvent theEvent)
		{
			switch (theEvent.Type)
			{
				case (WindowEventTypes.LeftMouseDown): leftOn = true; break;
				case (WindowEventTypes.LeftMouseUp): leftOn = false; break;
				
				case (WindowEventTypes.MiddleMouseDown): middleOn = true; break;
				case (WindowEventTypes.MiddleMouseUp): middleOn = false; break;
				
				case (WindowEventTypes.RightMouseDown): rightOn = true; break;
				case (WindowEventTypes.RightMouseUp): rightOn = false; break;
				
				case (WindowEventTypes.ScrollWheel):
					scrollWheelVelocity = theEvent.ScrollWheelVelocity;
					scollWheelChanged = true;
					break;

				case (WindowEventTypes.Move):
					viewLocation = input.window.ViewLocation;
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
			
			var cursorLoc = Cursor.Position;
			
			lastLocation = Position;
			var loc = new Point(cursorLoc.X, cursorLoc.Y) - new Point(viewLocation.X, viewLocation.Y);
			Position = new Vector2(loc.X, input.window.FrameSize.Height - loc.Y);
			Velocity = Position - lastLocation;
			
			lastScreenLocation = ScreenLocation;
			ScreenLocation = new Vector2(cursorLoc.X, cursorLoc.Y);
		}
		#endregion
	}
}
