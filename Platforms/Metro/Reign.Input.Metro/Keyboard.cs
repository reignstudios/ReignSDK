using System;
using Reign.Core;

namespace Reign.Input.Metro
{
	public class Keyboard : Disposable, KeyboardI
	{
		#region Properties
		private Input input;
		private Button[] buttons;
		private bool[] keys;
		
		public Button Key(int keyCode) {return null;}
		public Button Esc {get{return buttons[1];}}
		public Button D1 {get{return buttons[2];}}
		public Button D2 {get{return buttons[3];}}
		public Button D3 {get{return buttons[4];}}
		public Button A {get{return buttons[30];}}
		public Button D {get{return buttons[32];}}
		public Button S {get{return buttons[31];}}
		public Button W {get{return buttons[17];}}
		public Button ArrowLeft {get{return buttons[75];}}
		public Button ArrowRight {get{return buttons[77];}}
		public Button ArrowDown {get{return buttons[80];}}
		public Button ArrowUp {get{return buttons[72];}}
		#endregion
	
		#region Constructors
		public Keyboard(DisposableI parent)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateEventCallback += UpdateEvent;
			input.UpdateCallback += Update;
		
			keys = new bool[256];
			buttons = new Button[256];
			for (int i = 0; i != 256; ++i)
			{
				buttons[i] = new Button();
			}
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
				case (ApplicationEventTypes.KeyDown):
					keys[theEvent.KeyCode] = true;
					break;
				
				case (ApplicationEventTypes.KeyUp):
					keys[theEvent.KeyCode] = false;
					break;
			}
		}
		
		public void Update()
		{
			for (int i = 0; i != 256; ++i)
			{
				buttons[i].Update(keys[i]);
			}
		}
		#endregion
	}
}