using Reign.Core;
using F = Microsoft.Xna.Framework;
using I = Microsoft.Xna.Framework.Input;

namespace Reign.Input.XNA
{
	public class GamePad : Disposable, GamePadI
	{
		#region Properties
		private Input input;
		private F.PlayerIndex playerIndex;
		private bool useAllControllers;
		private float vibrationStrength, vibrationFadeToStrength, vibrationFadeSpeed;

		public Button Back {get; private set;}
		public Button Start {get; private set;}
		public Button A {get; private set;}
		public Button B {get; private set;}
		public Button X {get; private set;}
		public Button Y {get; private set;}
		public Button LeftBumper {get; private set;}
		public Button RightBumper {get; private set;}
		public Button LeftStickButton {get; private set;}
		public Button RightStickButton {get; private set;}
		public Button DLeft {get; private set;}
		public Button DRight {get; private set;}
		public Button DDown {get; private set;}
		public Button DUp {get; private set;}

		public Trigger LeftTrigger {get; private set;}
		public Trigger RightTrigger {get; private set;}

		public Vector2 LeftStick {get; private set;}
		public Vector2 RightStick {get; private set;}
		#endregion

		#region Constructors
		public static GamePad New(DisposableI parent, GamePadControllers controller)
		{
			return new GamePad(parent, controller);
		}

		public GamePad(DisposableI parent, GamePadControllers controller)
		: base(parent)
		{
			input = parent.FindParentOrSelfWithException<Input>();
			input.UpdateCallback += Update;

			switch (controller)
			{
				case GamePadControllers.All: useAllControllers = true; break;
				case GamePadControllers.One: playerIndex = F.PlayerIndex.One; break;
				case GamePadControllers.Two: playerIndex = F.PlayerIndex.Two; break;
				case GamePadControllers.Three: playerIndex = F.PlayerIndex.Three; break;
				case GamePadControllers.Four: playerIndex = F.PlayerIndex.Four; break;
				default: Debug.ThrowError("GamePad", "Unsuported controller"); break;
			}

			Back = new Button();
			Start = new Button();
			A = new Button();
			B = new Button();
			X = new Button();
			Y = new Button();
			LeftBumper = new Button();
			RightBumper = new Button();
			LeftStickButton = new Button();
			RightStickButton = new Button();
			DLeft = new Button();
			DRight = new Button();
			DDown = new Button();
			DUp = new Button();
			LeftTrigger = new Trigger();
			RightTrigger = new Trigger();

			LeftStick = new Vector2();
			RightStick = new Vector2();
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (input != null) input.UpdateCallback -= Update;
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Update()
		{
			vibrationStrength += (vibrationFadeToStrength - vibrationStrength) * vibrationFadeSpeed;

			if (!useAllControllers)
			{
				I.GamePad.SetVibration(playerIndex, vibrationStrength, vibrationStrength);

				var state = I.GamePad.GetState(playerIndex);

				var buttons = state.Buttons;
				Back.Update(buttons.Back == I.ButtonState.Pressed);
				Start.Update(buttons.Start == I.ButtonState.Pressed);
				A.Update(buttons.A == I.ButtonState.Pressed);
				B.Update(buttons.B == I.ButtonState.Pressed);
				X.Update(buttons.X == I.ButtonState.Pressed);
				Y.Update(buttons.Y == I.ButtonState.Pressed);
				LeftBumper.Update(buttons.LeftShoulder == I.ButtonState.Pressed);
				RightBumper.Update(buttons.RightShoulder == I.ButtonState.Pressed);
				LeftStickButton.Update(buttons.LeftStick == I.ButtonState.Pressed);
				RightStickButton.Update(buttons.RightStick == I.ButtonState.Pressed);

				var triggers = state.Triggers;
				LeftTrigger.Update(triggers.Left);
				RightTrigger.Update(triggers.Right);

				var dPad = state.DPad;
				DLeft.Update(dPad.Left == I.ButtonState.Pressed);
				DRight.Update(dPad.Right == I.ButtonState.Pressed);
				DDown.Update(dPad.Down == I.ButtonState.Pressed);
				DUp.Update(dPad.Up == I.ButtonState.Pressed);


				var sticks = I.GamePad.GetState(playerIndex).ThumbSticks;
				var leftStick = sticks.Left;
				var rightStick = sticks.Right;
				LeftStick = new Vector2(leftStick.X, leftStick.Y);
				RightStick = new Vector2(rightStick.X, rightStick.Y);
			}
			else
			{
				I.GamePad.SetVibration(F.PlayerIndex.One, vibrationStrength, vibrationStrength);
				I.GamePad.SetVibration(F.PlayerIndex.Two, vibrationStrength, vibrationStrength);
				I.GamePad.SetVibration(F.PlayerIndex.Three, vibrationStrength, vibrationStrength);
				I.GamePad.SetVibration(F.PlayerIndex.Four, vibrationStrength, vibrationStrength);

				var state1 = I.GamePad.GetState(F.PlayerIndex.One);
				var state2 = I.GamePad.GetState(F.PlayerIndex.Two);
				var state3 = I.GamePad.GetState(F.PlayerIndex.Three);
				var state4 = I.GamePad.GetState(F.PlayerIndex.Four);

				var buttons1 = state1.Buttons;
				var buttons2 = state2.Buttons;
				var buttons3 = state3.Buttons;
				var buttons4 = state4.Buttons;
				Back.Update(buttons1.Back == I.ButtonState.Pressed || buttons2.Back == I.ButtonState.Pressed || buttons3.Back == I.ButtonState.Pressed || buttons4.Back == I.ButtonState.Pressed);
				Start.Update(buttons1.Start == I.ButtonState.Pressed || buttons2.Start == I.ButtonState.Pressed || buttons3.Start == I.ButtonState.Pressed || buttons4.Start == I.ButtonState.Pressed);
				A.Update(buttons1.A == I.ButtonState.Pressed || buttons2.A == I.ButtonState.Pressed || buttons3.A == I.ButtonState.Pressed || buttons4.A == I.ButtonState.Pressed);
				B.Update(buttons1.B == I.ButtonState.Pressed || buttons2.B == I.ButtonState.Pressed || buttons3.B == I.ButtonState.Pressed || buttons4.B == I.ButtonState.Pressed);
				X.Update(buttons1.X == I.ButtonState.Pressed || buttons2.X == I.ButtonState.Pressed || buttons3.X == I.ButtonState.Pressed || buttons4.X == I.ButtonState.Pressed);
				Y.Update(buttons1.Y == I.ButtonState.Pressed || buttons2.Y == I.ButtonState.Pressed || buttons3.Y == I.ButtonState.Pressed || buttons4.Y == I.ButtonState.Pressed);
				LeftBumper.Update(buttons1.LeftShoulder == I.ButtonState.Pressed || buttons2.LeftShoulder == I.ButtonState.Pressed || buttons3.LeftShoulder == I.ButtonState.Pressed || buttons4.LeftShoulder == I.ButtonState.Pressed);
				RightBumper.Update(buttons1.RightShoulder == I.ButtonState.Pressed || buttons2.RightShoulder == I.ButtonState.Pressed || buttons3.RightShoulder == I.ButtonState.Pressed || buttons4.RightShoulder == I.ButtonState.Pressed);
				LeftStickButton.Update(buttons1.LeftStick == I.ButtonState.Pressed || buttons2.LeftStick == I.ButtonState.Pressed || buttons3.LeftStick == I.ButtonState.Pressed || buttons4.LeftStick == I.ButtonState.Pressed);
				RightStickButton.Update(buttons1.RightStick == I.ButtonState.Pressed || buttons2.RightStick == I.ButtonState.Pressed || buttons3.RightStick == I.ButtonState.Pressed || buttons4.RightStick == I.ButtonState.Pressed);

				var triggers1 = state1.Triggers;
				var triggers2 = state2.Triggers;
				var triggers3 = state3.Triggers;
				var triggers4 = state4.Triggers;
				LeftTrigger.Update(System.Math.Min(triggers1.Left + triggers2.Left + triggers3.Left + triggers4.Left, 1));
				RightTrigger.Update(System.Math.Min(triggers1.Right + triggers2.Right + triggers3.Right + triggers4.Right, 1));

				var dPad1 = state1.DPad;
				var dPad2 = state2.DPad;
				var dPad3 = state3.DPad;
				var dPad4 = state4.DPad;
				DLeft.Update(dPad1.Left == I.ButtonState.Pressed || dPad2.Left == I.ButtonState.Pressed || dPad3.Left == I.ButtonState.Pressed || dPad4.Left == I.ButtonState.Pressed);
				DRight.Update(dPad1.Right == I.ButtonState.Pressed || dPad2.Right == I.ButtonState.Pressed || dPad3.Right == I.ButtonState.Pressed || dPad4.Right == I.ButtonState.Pressed);
				DDown.Update(dPad1.Down == I.ButtonState.Pressed || dPad2.Down == I.ButtonState.Pressed || dPad3.Down == I.ButtonState.Pressed || dPad4.Down == I.ButtonState.Pressed);
				DUp.Update(dPad1.Up == I.ButtonState.Pressed || dPad2.Up == I.ButtonState.Pressed || dPad3.Up == I.ButtonState.Pressed || dPad4.Up == I.ButtonState.Pressed);

				var sticks1 = I.GamePad.GetState(F.PlayerIndex.One).ThumbSticks;
				var sticks2 = I.GamePad.GetState(F.PlayerIndex.Two).ThumbSticks;
				var sticks3 = I.GamePad.GetState(F.PlayerIndex.Three).ThumbSticks;
				var sticks4 = I.GamePad.GetState(F.PlayerIndex.Four).ThumbSticks;
				var leftStick = (sticks1.Left + sticks2.Left + sticks3.Left + sticks4.Left);
				var rightStick = (sticks1.Right + sticks2.Right + sticks3.Right + sticks4.Right);
				LeftStick = new Vector2(leftStick.X, leftStick.Y).Min(1);
				RightStick = new Vector2(rightStick.X, rightStick.Y).Min(1);
			}
		}

		public void SetVibration(float startingStrength, float fadeToStrength, float fadeSpeed)
		{
			vibrationStrength = System.Math.Min(startingStrength, 1);;
			vibrationFadeToStrength = fadeToStrength;
			vibrationFadeSpeed = fadeSpeed;
		}

		public void AddVibration(float strength, float fadeToStrength, float fadeSpeed)
		{
			vibrationStrength += System.Math.Min(strength, 1);
			vibrationFadeToStrength = fadeToStrength;
			vibrationFadeSpeed = fadeSpeed;
		}

		public void AddVibration(float strength)
		{
			vibrationStrength += System.Math.Min(strength, 1);
		}
		#endregion
	}
}