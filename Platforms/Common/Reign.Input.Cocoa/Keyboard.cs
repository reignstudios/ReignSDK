using System;
using MonoMac.AppKit;
using Reign.Core;

namespace Reign.Input.Cocoa
{
	public class Keyboard : Disposable, KeyboardI
	{
		#region Properties
		private Input input;
		private Button[] buttons;
		private bool[] keys;
		
		public Button Key(int keyCode) {return null;}
		public Button Esc {get{return buttons[53];}}
		public Button D1 {get{return buttons[18];}}
		public Button D2 {get{return buttons[19];}}
		public Button D3 {get{return buttons[20];}}
		public Button A {get{return buttons[20];}}
		public Button D {get{return buttons[20];}}
		public Button S {get{return buttons[20];}}
		public Button W {get{return buttons[20];}}
		public Button ArrowLeft {get{return buttons[20];}}
		public Button ArrowRight {get{return buttons[20];}}
		public Button ArrowDown {get{return buttons[20];}}
		public Button ArrowUp {get{return buttons[20];}}
		#endregion
	
		#region Constructors
		public Keyboard(DisposableI parent)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateEventCallback += updateEvent;
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
				input.UpdateEventCallback -= updateEvent;
				input.UpdateCallback -= Update;
			}
			base.Dispose ();
		}
		#endregion
		
		#region Methods
		private void updateEvent(WindowEvent theEvent)
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

