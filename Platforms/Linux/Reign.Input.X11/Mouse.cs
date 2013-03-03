using System;
using Reign.Core;

namespace Reign.Input.X11
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
		private Point2 lastPosition;
		#endregion
	
		#region Constructors
		public static Mouse New (DisposableI parent)
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
				case ApplicationEventTypes.LeftMouseDown: leftOn = true; break;
				case ApplicationEventTypes.LeftMouseUp: leftOn = false; break;
				
				case ApplicationEventTypes.MiddleMouseDown: middleOn = true; break;
				case ApplicationEventTypes.MiddleMouseUp: middleOn = false; break;
				
				case ApplicationEventTypes.RightMouseDown: rightOn = true; break;
				case ApplicationEventTypes.RightMouseUp: rightOn = false; break;
				
				case (ApplicationEventTypes.ScrollWheel):
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
			Reign.Core.X11.XQueryPointer(input.application.DC, input.application.Handle, out w2, out w3, out x, out y, out x2, out y2, out mask);
			
			lastPosition = Position;
			Position = new Point2(x2, input.application.FrameSize.Height - y2);
			PositionVector = Position.ToVector2();

			Velocity = Position - lastPosition;
			VelocityVector = Velocity.ToVector2();
		}
		#endregion
	}
}

