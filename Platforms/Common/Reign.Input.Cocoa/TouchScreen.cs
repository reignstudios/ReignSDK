using System;
using Reign.Core;
using Reign.Core.MathF32;

namespace Reign.Input.Cocoa
{
	public class TouchScreen : Disposable, TouchScreenI
	{
		#region Properties
		private Input input;

		public Touch[] Touches {get; private set;}
		private bool[] touchOn;
		private Vector2[] touchLocations;
		#endregion
	
		#region Constructors
		public TouchScreen(DisposableI parent)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateEventCallback += UpdateEvent;
			input.UpdateCallback += Update;
			
			Touches = new Touch[ApplicationEvent.TouchCount];
			touchOn = new bool[ApplicationEvent.TouchCount];
			touchLocations = new Vector2[ApplicationEvent.TouchCount];
			for (int i = 0; i != ApplicationEvent.TouchCount; ++i)
			{
				Touches[i] = new Touch();
				touchLocations[i] = new Vector2();
			}
		}
		
		public override void Dispose()
		{
			disposeChilderen();
			if (input != null)
			{
				input.UpdateEventCallback -= UpdateEvent;
				input.UpdateCallback -= Update;
			}
			base.Dispose();
		}
		#endregion
		
		#region Methods
		public void UpdateEvent(ApplicationEvent theEvent)
		{
			if (theEvent.Type == ApplicationEventTypes.Touch)
			{
				for (int i = 0; i != ApplicationEvent.TouchCount; ++i)
				{
					touchOn[i] = theEvent.TouchesOn[i];
					var loc = theEvent.TouchLocations[i];
					touchLocations[i] = new Vector2(loc.X, input.application.FrameSize.Y-loc.Y);
				}
			}
		}
		
		public void Update()
		{
			for (int i = 0; i != ApplicationEvent.TouchCount; ++i)
			{
				Touches[i].Update(touchOn[i], touchLocations[i]);
			}
		}
		#endregion
	}
}

