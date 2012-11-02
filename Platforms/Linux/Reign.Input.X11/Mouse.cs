using System;
using Reign.Core;

namespace Reign.Input.X11
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
		public Vector2 Position {get; private set;}
		public Vector2 Velocity {get; private set;}
		
		private bool leftOn, middleOn, rightOn, scollWheelChanged;
		private float scrollWheelVelocity;
		private Vector2 lastLocation;
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
			
			IntPtr w2, w3;
			int x, y, x2, y2;
			uint mask;
			Reign.Core.X11.XQueryPointer(input.window.DC, input.window.Handle, out w2, out w3, out x, out y, out x2, out y2, out mask);
			
			lastLocation = Position;
			Position = new Vector2(x2, input.window.FrameSize.Height - y2);
			Velecity = Position - lastLocation;
		}
		#endregion
	}
}

