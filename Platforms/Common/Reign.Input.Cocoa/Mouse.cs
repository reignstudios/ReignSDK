using System;
using MonoMac.AppKit;
using Reign.Core;

namespace Reign.Input.Cocoa
{
	public class Mouse : Disposable, MouseI
	{
		#region Properties
		private Input input;
		
		public Vector2 Velecity {get; private set;}
		public Button Left {get; private set;}
		public Button Middle {get; private set;}
		public Button Right {get; private set;}
		public float ScrollWheelVelocity {get; private set;}
		public Vector2 Velocity {get; private set;}
		public Vector2 Position {get; private set;}
		public Vector2 ScreenLocation {get; private set;}
		
		private bool leftOn, middleOn, rightOn, scollWheelChanged;
		private float scrollWheelVelocity;
		private Vector2 lastLocation;
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
			
			var loc = NSEvent.CurrentMouseLocation;
			var viewLoc = input.window.ViewLocation;
			
			lastLocation = Position;
			Position = new Vector2(loc.X, loc.Y) - new Vector2(viewLoc.X, viewLoc.Y);
			Velecity = Position - lastLocation;
			
			ScreenLocation = new Vector2(loc.X, loc.Y);
		}
		#endregion
	}
}

