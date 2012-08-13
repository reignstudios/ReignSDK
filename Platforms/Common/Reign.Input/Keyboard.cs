using Reign.Core;

namespace Reign.Input
{
	public interface KeyboardI : DisposableI
	{
		Button Key(int keyCode);
		Button Esc {get;}
		Button D1 {get;}
		Button D2 {get;}
		Button D3 {get;}
		Button A {get;}
		Button D {get;}
		Button S {get;}
		Button W {get;}
		Button ArrowLeft {get;}
		Button ArrowRight {get;}
		Button ArrowDown {get;}
		Button ArrowUp {get;}
	
		void Update();
	}
}
