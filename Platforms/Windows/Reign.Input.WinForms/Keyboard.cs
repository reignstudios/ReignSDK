using System;
using Reign.Core;

namespace Reign.Input.WinForms
{
	public class Keyboard : Disposable, KeyboardI
	{
		#region Properties
		private Input input;
		private Button[] buttons;
		private bool[] keys;
		
		public Button Key(int keyCode) {return null;}
		public Button Esc {get{return buttons[27];}}
		public Button D1 {get{return buttons[49];}}
		public Button D2 {get{return buttons[50];}}
		public Button D3 {get{return buttons[51];}}
		public Button A {get{return buttons[65];}}
		public Button D {get{return buttons[68];}}
		public Button S {get{return buttons[83];}}
		public Button W {get{return buttons[87];}}
		public Button ArrowLeft {get{return buttons[37];}}
		public Button ArrowRight {get{return buttons[39];}}
		public Button ArrowDown {get{return buttons[40];}}
		public Button ArrowUp {get{return buttons[38];}}
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
		public void UpdateEvent(WindowEvent theEvent)
		{
			switch (theEvent.Type)
			{
				case (WindowEventTypes.KeyDown):
					keys[theEvent.KeyCode] = true;
					break;
				
				case (WindowEventTypes.KeyUp):
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