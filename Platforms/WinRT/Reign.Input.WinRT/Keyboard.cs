using System;
using Reign.Core;
using Windows.System;

namespace Reign.Input.WinRT
{
	public class Keyboard : Disposable, KeyboardI
	{
		#region Properties
		private Input input;
		private Button[] buttons;
		private bool[] keys;
		
		public Button Key(int keyCode) {return buttons[keyCode];}

		public Button Esc {get{return buttons[(int)VirtualKey.Escape];}}
		public Button Delete {get{return buttons[(int)VirtualKey.Delete];}}
		public Button Tab {get{return buttons[(int)VirtualKey.Tab];}}
		public Button Shift {get{return buttons[(int)VirtualKey.Shift];}}
		public Button Backspace {get{return buttons[(int)VirtualKey.Back];}}
		public Button Control {get{return buttons[(int)VirtualKey.Control];}}
		public Button Alt {get{return buttons[(int)VirtualKey.Menu];}}

		public Button D0 {get{return buttons[(int)VirtualKey.Number0];}}
		public Button D1 {get{return buttons[(int)VirtualKey.Number1];}}
		public Button D2 {get{return buttons[(int)VirtualKey.Number2];}}
		public Button D3 {get{return buttons[(int)VirtualKey.Number3];}}
		public Button D4 {get{return buttons[(int)VirtualKey.Number4];}}
		public Button D5 {get{return buttons[(int)VirtualKey.Number5];}}
		public Button D6 {get{return buttons[(int)VirtualKey.Number6];}}
		public Button D7 {get{return buttons[(int)VirtualKey.Number7];}}
		public Button D8 {get{return buttons[(int)VirtualKey.Number8];}}
		public Button D9 {get{return buttons[(int)VirtualKey.Number9];}}

		public Button NumPad0 {get{return buttons[(int)VirtualKey.NumberPad0];}}
		public Button NumPad1 {get{return buttons[(int)VirtualKey.NumberPad1];}}
		public Button NumPad2 {get{return buttons[(int)VirtualKey.NumberPad2];}}
		public Button NumPad3 {get{return buttons[(int)VirtualKey.NumberPad3];}}
		public Button NumPad4 {get{return buttons[(int)VirtualKey.NumberPad4];}}
		public Button NumPad5 {get{return buttons[(int)VirtualKey.NumberPad5];}}
		public Button NumPad6 {get{return buttons[(int)VirtualKey.NumberPad6];}}
		public Button NumPad7 {get{return buttons[(int)VirtualKey.NumberPad7];}}
		public Button NumPad8 {get{return buttons[(int)VirtualKey.NumberPad8];}}
		public Button NumPad9 {get{return buttons[(int)VirtualKey.NumberPad9];}}
		public Button NumPadMultiply {get{return buttons[(int)VirtualKey.Multiply];}}
		public Button NumPadAdd {get{return buttons[(int)VirtualKey.Add];}}
		public Button NumPadSubtract {get{return buttons[(int)VirtualKey.Subtract];}}
		public Button NumPadDecimal {get{return buttons[(int)VirtualKey.Decimal];}}
		public Button NumPadDivide {get{return buttons[(int)VirtualKey.Divide];}}
		public Button NumPadEnter {get{return buttons[(int)VirtualKey.Enter];}}

		public Button A {get{return buttons[(int)VirtualKey.A];}}
		public Button B {get{return buttons[(int)VirtualKey.B];}}
		public Button C {get{return buttons[(int)VirtualKey.C];}}
		public Button D {get{return buttons[(int)VirtualKey.D];}}
		public Button E {get{return buttons[(int)VirtualKey.E];}}
		public Button F {get{return buttons[(int)VirtualKey.F];}}
		public Button G {get{return buttons[(int)VirtualKey.G];}}
		public Button H {get{return buttons[(int)VirtualKey.H];}}
		public Button I {get{return buttons[(int)VirtualKey.I];}}
		public Button J {get{return buttons[(int)VirtualKey.J];}}
		public Button K {get{return buttons[(int)VirtualKey.K];}}
		public Button L {get{return buttons[(int)VirtualKey.L];}}
		public Button M {get{return buttons[(int)VirtualKey.M];}}
		public Button N {get{return buttons[(int)VirtualKey.N];}}
		public Button O {get{return buttons[(int)VirtualKey.O];}}
		public Button P {get{return buttons[(int)VirtualKey.P];}}
		public Button Q {get{return buttons[(int)VirtualKey.Q];}}
		public Button R {get{return buttons[(int)VirtualKey.R];}}
		public Button S {get{return buttons[(int)VirtualKey.S];}}
		public Button T {get{return buttons[(int)VirtualKey.T];}}
		public Button U {get{return buttons[(int)VirtualKey.U];}}
		public Button V {get{return buttons[(int)VirtualKey.V];}}
		public Button W {get{return buttons[(int)VirtualKey.W];}}
		public Button X {get{return buttons[(int)VirtualKey.X];}}
		public Button Y {get{return buttons[(int)VirtualKey.Y];}}
		public Button Z {get{return buttons[(int)VirtualKey.Z];}}

		public Button Tilde {get{return buttons[192];}}
		public Button Plus {get{return buttons[187];}}
		public Button Minus {get{return buttons[189];}}
		public Button OpenBrackets {get{return buttons[219];}}
		public Button CloseBrackets {get{return buttons[221];}}
		public Button BackSlash {get{return buttons[220];}}
		public Button ForwardSlash {get{return buttons[191];}}
		public Button Semicolon {get{return buttons[186];}}
		public Button Quote {get{return buttons[222];}}
		public Button Comma {get{return buttons[188];}}
		public Button Period {get{return buttons[190];}}

		public Button Home {get{return buttons[(int)VirtualKey.Home];}}
		public Button End {get{return buttons[(int)VirtualKey.Back];}}
		public Button PageUp {get{return buttons[(int)VirtualKey.PageUp];}}
		public Button PageDown {get{return buttons[(int)VirtualKey.PageDown];}}

		public Button ArrowLeft {get{return buttons[(int)VirtualKey.Left];}}
		public Button ArrowRight {get{return buttons[(int)VirtualKey.Right];}}
		public Button ArrowDown {get{return buttons[(int)VirtualKey.Down];}}
		public Button ArrowUp {get{return buttons[(int)VirtualKey.Up];}}
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
				case ApplicationEventTypes.KeyDown:
					keys[theEvent.KeyCode] = true;
					break;
				
				case ApplicationEventTypes.KeyUp:
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