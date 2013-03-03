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
		
		public Button Key(int keyCode) {return buttons[keyCode];}
		
		public Button Esc {get{return buttons[53];}}
		public Button Delete {get{return buttons[117];}}
		public Button Tab {get{return buttons[48];}}
		public Button Shift {get{return buttons[56];}}
		public Button Backspace {get{return buttons[51];}}
		public Button Control {get{return buttons[59];}}
		public Button Alt {get{return buttons[58];}}
		
		public Button D0 {get{return buttons[29];}}
		public Button D1 {get{return buttons[18];}}
		public Button D2 {get{return buttons[19];}}
		public Button D3 {get{return buttons[20];}}
		public Button D4 {get{return buttons[21];}}
		public Button D5 {get{return buttons[23];}}
		public Button D6 {get{return buttons[22];}}
		public Button D7 {get{return buttons[26];}}
		public Button D8 {get{return buttons[28];}}
		public Button D9 {get{return buttons[25];}}
		
		public Button NumPad0 {get{return buttons[82];}}
		public Button NumPad1 {get{return buttons[83];}}
		public Button NumPad2 {get{return buttons[84];}}
		public Button NumPad3 {get{return buttons[85];}}
		public Button NumPad4 {get{return buttons[86];}}
		public Button NumPad5 {get{return buttons[87];}}
		public Button NumPad6 {get{return buttons[88];}}
		public Button NumPad7 {get{return buttons[89];}}
		public Button NumPad8 {get{return buttons[91];}}
		public Button NumPad9 {get{return buttons[92];}}
		public Button NumPadMultiply {get{return buttons[67];}}
		public Button NumPadAdd {get{return buttons[69];}}
		public Button NumPadSubtract {get{return buttons[78];}}
		public Button NumPadDecimal {get{return buttons[65];}}
		public Button NumPadDivide {get{return buttons[75];}}
		public Button NumPadEnter {get{return buttons[76];}}
		
		public Button A {get{return buttons[0];}}
		public Button B {get{return buttons[11];}}
		public Button C {get{return buttons[8];}}
		public Button D {get{return buttons[2];}}
		public Button E {get{return buttons[14];}}
		public Button F {get{return buttons[3];}}
		public Button G {get{return buttons[5];}}
		public Button H {get{return buttons[4];}}
		public Button I {get{return buttons[34];}}
		public Button J {get{return buttons[38];}}
		public Button K {get{return buttons[40];}}
		public Button L {get{return buttons[37];}}
		public Button M {get{return buttons[46];}}
		public Button N {get{return buttons[45];}}
		public Button O {get{return buttons[31];}}
		public Button P {get{return buttons[35];}}
		public Button Q {get{return buttons[12];}}
		public Button R {get{return buttons[15];}}
		public Button S {get{return buttons[1];}}
		public Button T {get{return buttons[17];}}
		public Button U {get{return buttons[32];}}
		public Button V {get{return buttons[9];}}
		public Button W {get{return buttons[13];}}
		public Button X {get{return buttons[7];}}
		public Button Y {get{return buttons[16];}}
		public Button Z {get{return buttons[6];}}
		
		public Button Tilde {get{return buttons[50];}}
		public Button Plus {get{return buttons[24];}}
		public Button Minus {get{return buttons[27];}}
		public Button OpenBrackets {get{return buttons[33];}}
		public Button CloseBrackets {get{return buttons[30];}}
		public Button BackSlash {get{return buttons[42];}}
		public Button ForwardSlash {get{return buttons[44];}}
		public Button Semicolon {get{return buttons[41];}}
		public Button Quote {get{return buttons[39];}}
		public Button Comma {get{return buttons[43];}}
		public Button Period {get{return buttons[47];}}
		
		public Button Home {get{return buttons[115];}}
		public Button End {get{return buttons[119];}}
		public Button PageUp {get{return buttons[116];}}
		public Button PageDown {get{return buttons[121];}}
		
		public Button ArrowLeft {get{return buttons[123];}}
		public Button ArrowRight {get{return buttons[124];}}
		public Button ArrowDown {get{return buttons[125];}}
		public Button ArrowUp {get{return buttons[126];}}
		#endregion
	
		#region Constructors
		public static Keyboard New(DisposableI parent)
		{
			return new Keyboard(parent);
		}
		
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
		private void updateEvent(ApplicationEvent theEvent)
		{
			switch (theEvent.Type)
			{
				case ApplicationEventTypes.KeyDown:
					keys[theEvent.KeyCode] = true;
					break;
				
				case ApplicationEventTypes.KeyUp:
					keys[theEvent.KeyCode] = false;
					break;
					
				case ApplicationEventTypes.Unkown:
					keys[theEvent.KeyCode] = !keys[theEvent.KeyCode];
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

