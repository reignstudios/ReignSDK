using Reign.Core;

namespace Reign.Input
{
	public interface KeyboardI : DisposableI
	{
		Button Key(int keyCode);

		Button Esc {get;}
		Button Delete {get;}
		Button Tab {get;}
		Button Shift {get;}
		Button Backspace {get;}
		Button Control {get;}
		Button Alt {get;}

		Button D0 {get;}
		Button D1 {get;}
		Button D2 {get;}
		Button D3 {get;}
		Button D4 {get;}
		Button D5 {get;}
		Button D6 {get;}
		Button D7 {get;}
		Button D8 {get;}
		Button D9 {get;}

		Button NumPad0 {get;}
		Button NumPad1 {get;}
		Button NumPad2 {get;}
		Button NumPad3 {get;}
		Button NumPad4 {get;}
		Button NumPad5 {get;}
		Button NumPad6 {get;}
		Button NumPad7 {get;}
		Button NumPad8 {get;}
		Button NumPad9 {get;}
		Button NumPadMultiply {get;}
		Button NumPadAdd {get;}
		Button NumPadSubtract {get;}
		Button NumPadDecimal {get;}
		Button NumPadDivide {get;}
		Button NumPadEnter {get;}

		Button A {get;}
		Button B {get;}
		Button C {get;}
		Button D {get;}
		Button E {get;}
		Button F {get;}
		Button G {get;}
		Button H {get;}
		Button I {get;}
		Button J {get;}
		Button K {get;}
		Button L {get;}
		Button M {get;}
		Button N {get;}
		Button O {get;}
		Button P {get;}
		Button Q {get;}
		Button R {get;}
		Button S {get;}
		Button T {get;}
		Button U {get;}
		Button V {get;}
		Button W {get;}
		Button X {get;}
		Button Y {get;}
		Button Z {get;}

		Button Tilde {get;}
		Button Plus {get;}
		Button Minus {get;}
		Button OpenBrackets {get;}
		Button CloseBrackets {get;}
		Button BackSlash {get;}
		Button ForwardSlash {get;}
		Button Semicolon {get;}
		Button Quote {get;}
		Button Comma {get;}
		Button Period {get;}

		Button Home {get;}
		Button End {get;}
		Button PageUp {get;}
		Button PageDown {get;}

		Button ArrowLeft {get;}
		Button ArrowRight {get;}
		Button ArrowDown {get;}
		Button ArrowUp {get;}
	
		void Update();
	}

	public static class KeyboardAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			KeyboardAPI.newPtr = newPtr;
		}

		public delegate KeyboardI NewPtrMethod(DisposableI parent);
		internal static NewPtrMethod newPtr;
		public static KeyboardI New(DisposableI parent)
		{
			return newPtr(parent);
		}
	}
}
