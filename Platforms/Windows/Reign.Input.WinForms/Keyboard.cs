using System;
using Reign.Core;
using System.Windows.Forms;

namespace Reign.Input.WinForms
{
	public class Keyboard : Disposable, KeyboardI
	{
		#region Properties
		private Input input;
		private Button[] buttons;
		private bool[] keys;
		
		public Button Key(int keyCode) {return buttons[keyCode];}

		public Button Esc {get{return buttons[(int)Keys.Escape];}}
		public Button Break {get{return buttons[(int)Keys.Pause];}}
		public Button Insert {get{return buttons[(int)Keys.Insert];}}
		public Button Delete {get{return buttons[(int)Keys.Delete];}}
		public Button Tab {get{return buttons[(int)Keys.Tab];}}
		public Button CapsLock {get{return buttons[(int)Keys.CapsLock];}}
		public Button Shift {get{return buttons[(int)Keys.ShiftKey];}}
		public Button Backspace {get{return buttons[(int)Keys.Back];}}
		public Button Control {get{return buttons[(int)Keys.ControlKey];}}
		public Button Alt {get{return buttons[(int)Keys.Menu];}}

		public Button D0 {get{return buttons[(int)Keys.D0];}}
		public Button D1 {get{return buttons[(int)Keys.D1];}}
		public Button D2 {get{return buttons[(int)Keys.D2];}}
		public Button D3 {get{return buttons[(int)Keys.D3];}}
		public Button D4 {get{return buttons[(int)Keys.D4];}}
		public Button D5 {get{return buttons[(int)Keys.D5];}}
		public Button D6 {get{return buttons[(int)Keys.D6];}}
		public Button D7 {get{return buttons[(int)Keys.D7];}}
		public Button D8 {get{return buttons[(int)Keys.D8];}}
		public Button D9 {get{return buttons[(int)Keys.D9];}}

		public Button NumPad0 {get{return buttons[(int)Keys.NumPad0];}}
		public Button NumPad1 {get{return buttons[(int)Keys.NumPad1];}}
		public Button NumPad2 {get{return buttons[(int)Keys.NumPad2];}}
		public Button NumPad3 {get{return buttons[(int)Keys.NumPad3];}}
		public Button NumPad4 {get{return buttons[(int)Keys.NumPad4];}}
		public Button NumPad5 {get{return buttons[(int)Keys.NumPad5];}}
		public Button NumPad6 {get{return buttons[(int)Keys.NumPad6];}}
		public Button NumPad7 {get{return buttons[(int)Keys.NumPad7];}}
		public Button NumPad8 {get{return buttons[(int)Keys.NumPad8];}}
		public Button NumPad9 {get{return buttons[(int)Keys.NumPad9];}}
		public Button NumPadMultiply {get{return buttons[(int)Keys.Multiply];}}
		public Button NumPadAdd {get{return buttons[(int)Keys.Add];}}
		public Button NumPadSeparator {get{return buttons[(int)Keys.Separator];}}
		public Button NumPadSubtract {get{return buttons[(int)Keys.Subtract];}}
		public Button NumPadDecimal {get{return buttons[(int)Keys.Decimal];}}
		public Button NumPadDivide {get{return buttons[(int)Keys.Divide];}}

		public Button F1 {get{return buttons[(int)Keys.F1];}}
		public Button F2 {get{return buttons[(int)Keys.F2];}}
		public Button F3 {get{return buttons[(int)Keys.F3];}}
		public Button F4 {get{return buttons[(int)Keys.F4];}}
		public Button F5 {get{return buttons[(int)Keys.F5];}}
		public Button F6 {get{return buttons[(int)Keys.F6];}}
		public Button F7 {get{return buttons[(int)Keys.F7];}}
		public Button F8 {get{return buttons[(int)Keys.F8];}}
		public Button F9 {get{return buttons[(int)Keys.F9];}}
		public Button F10 {get{return buttons[(int)Keys.F10];}}
		public Button F11 {get{return buttons[(int)Keys.F11];}}
		public Button F12 {get{return buttons[(int)Keys.F12];}}

		public Button A {get{return buttons[(int)Keys.A];}}
		public Button B {get{return buttons[(int)Keys.B];}}
		public Button C {get{return buttons[(int)Keys.C];}}
		public Button D {get{return buttons[(int)Keys.D];}}
		public Button E {get{return buttons[(int)Keys.E];}}
		public Button F {get{return buttons[(int)Keys.F];}}
		public Button G {get{return buttons[(int)Keys.G];}}
		public Button H {get{return buttons[(int)Keys.H];}}
		public Button I {get{return buttons[(int)Keys.I];}}
		public Button J {get{return buttons[(int)Keys.J];}}
		public Button K {get{return buttons[(int)Keys.K];}}
		public Button L {get{return buttons[(int)Keys.L];}}
		public Button M {get{return buttons[(int)Keys.M];}}
		public Button N {get{return buttons[(int)Keys.N];}}
		public Button O {get{return buttons[(int)Keys.O];}}
		public Button P {get{return buttons[(int)Keys.P];}}
		public Button Q {get{return buttons[(int)Keys.Q];}}
		public Button R {get{return buttons[(int)Keys.R];}}
		public Button S {get{return buttons[(int)Keys.S];}}
		public Button T {get{return buttons[(int)Keys.T];}}
		public Button U {get{return buttons[(int)Keys.U];}}
		public Button V {get{return buttons[(int)Keys.V];}}
		public Button W {get{return buttons[(int)Keys.W];}}
		public Button X {get{return buttons[(int)Keys.X];}}
		public Button Y {get{return buttons[(int)Keys.Y];}}
		public Button Z {get{return buttons[(int)Keys.Z];}}

		public Button Tilde {get{return buttons[(int)Keys.Oemtilde];}}
		public Button Plus {get{return buttons[(int)Keys.Oemplus];}}
		public Button Minus {get{return buttons[(int)Keys.OemMinus];}}
		public Button OpenBrackets {get{return buttons[(int)Keys.OemOpenBrackets];}}
		public Button CloseBrackets {get{return buttons[(int)Keys.OemCloseBrackets];}}
		public Button BackSlash {get{return buttons[(int)Keys.OemBackslash];}}
		public Button ForwardSlash {get{return buttons[(int)Keys.OemQuestion];}}
		public Button Semicolon {get{return buttons[(int)Keys.OemSemicolon];}}
		public Button Quote {get{return buttons[(int)Keys.OemQuotes];}}
		public Button Comma {get{return buttons[(int)Keys.Oemcomma];}}
		public Button Period {get{return buttons[(int)Keys.OemPeriod];}}

		public Button Home {get{return buttons[(int)Keys.Home];}}
		public Button End {get{return buttons[(int)Keys.End];}}
		public Button PageUp {get{return buttons[(int)Keys.PageUp];}}
		public Button PageDown {get{return buttons[(int)Keys.PageDown];}}

		public Button ArrowLeft {get{return buttons[(int)Keys.Left];}}
		public Button ArrowRight {get{return buttons[(int)Keys.Right];}}
		public Button ArrowDown {get{return buttons[(int)Keys.Down];}}
		public Button ArrowUp {get{return buttons[(int)Keys.Up];}}
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