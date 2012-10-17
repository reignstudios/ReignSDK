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
		
		public Button Key(int keyCode) {return buttons[keyCode];}
		public Button Esc {get{return buttons[27];}}
		public Button PrintScreen {get{return buttons[44];}}
		public Button Break {get{return buttons[19];}}
		public Button Insert {get{return buttons[45];}}
		public Button Delete {get{return buttons[46];}}
		public Button Tab {get{return buttons[9];}}
		public Button CapsLock {get{return buttons[20];}}
		public Button LeftShift {get{return buttons[160];}}
		public Button RightShift {get{return buttons[161];}}
		public Button Backspace {get{return buttons[8];}}
		public Button LeftControl {get{return buttons[162];}}
		public Button RightControl {get{return buttons[163];}}
		public Button LeftAlt {get{return buttons[164];}}
		public Button RightAlt {get{return buttons[165];}}

		public Button D0 {get{return buttons[48];}}
		public Button D1 {get{return buttons[49];}}
		public Button D2 {get{return buttons[50];}}
		public Button D3 {get{return buttons[51];}}
		public Button D4 {get{return buttons[52];}}
		public Button D5 {get{return buttons[53];}}
		public Button D6 {get{return buttons[54];}}
		public Button D7 {get{return buttons[55];}}
		public Button D8 {get{return buttons[56];}}
		public Button D9 {get{return buttons[57];}}

		public Button NumPad0 {get{return buttons[96];}}
		public Button NumPad1 {get{return buttons[97];}}
		public Button NumPad2 {get{return buttons[98];}}
		public Button NumPad3 {get{return buttons[99];}}
		public Button NumPad4 {get{return buttons[100];}}
		public Button NumPad5 {get{return buttons[101];}}
		public Button NumPad6 {get{return buttons[102];}}
		public Button NumPad7 {get{return buttons[103];}}
		public Button NumPad8 {get{return buttons[104];}}
		public Button NumPad9 {get{return buttons[105];}}

		public Button F1 {get{return buttons[112];}}
		public Button F2 {get{return buttons[113];}}
		public Button F3 {get{return buttons[114];}}
		public Button F4 {get{return buttons[115];}}
		public Button F5 {get{return buttons[116];}}
		public Button F6 {get{return buttons[117];}}
		public Button F7 {get{return buttons[118];}}
		public Button F8 {get{return buttons[119];}}
		public Button F9 {get{return buttons[120];}}
		public Button F10 {get{return buttons[121];}}
		public Button F11 {get{return buttons[122];}}
		public Button F12 {get{return buttons[123];}}

		public Button A {get{return buttons[65];}}
		public Button B {get{return buttons[66];}}
		public Button C {get{return buttons[67];}}
		public Button D {get{return buttons[68];}}
		public Button E {get{return buttons[69];}}
		public Button F {get{return buttons[70];}}
		public Button G {get{return buttons[71];}}
		public Button H {get{return buttons[72];}}
		public Button I {get{return buttons[73];}}
		public Button J {get{return buttons[74];}}
		public Button K {get{return buttons[75];}}
		public Button L {get{return buttons[76];}}
		public Button M {get{return buttons[77];}}
		public Button N {get{return buttons[78];}}
		public Button O {get{return buttons[79];}}
		public Button P {get{return buttons[80];}}
		public Button Q {get{return buttons[81];}}
		public Button R {get{return buttons[82];}}
		public Button S {get{return buttons[83];}}
		public Button T {get{return buttons[84];}}
		public Button U {get{return buttons[85];}}
		public Button V {get{return buttons[86];}}
		public Button W {get{return buttons[87];}}
		public Button X {get{return buttons[88];}}
		public Button Y {get{return buttons[89];}}
		public Button Z {get{return buttons[90];}}

		public Button Plus {get{return buttons[187];}}
		public Button Minus {get{return buttons[189];}}

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