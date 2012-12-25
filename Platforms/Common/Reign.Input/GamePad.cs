using Reign.Core;

namespace Reign.Input
{
	public enum GamePadControllers
	{
		All,
		One,
		Two,
		Three,
		Four
	}

	public interface GamePadI : DisposableI
	{
		#region Properties
		Button Back {get;}
		Button Start {get;}
		Button A {get;}
		Button B {get;}
		Button X {get;}
		Button Y {get;}
		Button LeftBumper {get;}
		Button RightBumper {get;}
		Button LeftStickButton {get;}
		Button RightStickButton {get;}
		Button DLeft {get;}
		Button DRight {get;}
		Button DDown {get;}
		Button DUp {get;}

		Trigger LeftTrigger {get;}
		Trigger RightTrigger {get;}

		Vector2 LeftStick {get;}
		Vector2 RightStick {get;}
		#endregion

		#region Methods
		void SetVibration(float startingStrength, float fadeToStrength, float fadeSpeed);
		void AddVibration(float strength, float fadeToStrength, float fadeSpeed);
		void AddVibration(float strength);
		#endregion
	}

	public static class GamePadAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			GamePadAPI.newPtr = newPtr;
		}

		public delegate GamePadI NewPtrMethod(DisposableI parent, GamePadControllers controller);
		internal static NewPtrMethod newPtr;
		public static GamePadI New(DisposableI parent, GamePadControllers controller)
		{
			return newPtr(parent, controller);
		}
	}
}
