using Reign.Core;
using Reign.Core.MathF32;
using F = Microsoft.Xna.Framework;
using I = Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Reign.Input.XNA
{
	public class TouchScreen : Disposable, TouchScreenI
	{
		#region Properties
		private Input input;
		public Touch[] Touches {get; private set;}
		private bool[] touchOn;
		private Vector2[] touchLocations;
		#endregion

		#region Constructors
		public TouchScreen(InputI input)
		: base(input)
		{
			this.input = (Input)input;
			this.input.UpdateCallback += Update;

			Touches = new Touch[10];
			touchOn = new bool[10];
			touchLocations = new Vector2[10];
			for (int i = 0; i != Touches.Length; ++i)
			{
				Touches[i] = new Touch();
				touchLocations[i] = new Vector2();
			}
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
			for (int i = 0; i != Touches.Length; ++i)
			{
				touchOn[i] = false;
			}

			var touches = TouchPanel.GetState();
			int touchCount = (Touches.Length <= touches.Count) ? Touches.Length : touches.Count;
			for (int i = 0; i != touchCount; ++i)
			{
				touchOn[i] = true;
				var pos = touches[i].Position;
				touchLocations[i] = new Vector2(pos.X, input.application.FrameSize.Y-pos.Y);
			}

			for (int i = 0; i != Touches.Length; ++i)
			{
				Touches[i].Update(touchOn[i], touchLocations[i]);
			}
		}
		#endregion
	}
}