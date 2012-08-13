using Reign.Core;
using Microsoft.Xna.Framework.Content;

namespace Reign.Audio.XNA
{
	public class Audio : Disposable, AudioI
	{
		#region Properties
		internal delegate void UpdateCallbackFunc();
		internal UpdateCallbackFunc UpdateCallback;
		#endregion

		#region Constructors
		public Audio(DisposableI parent)
		: base(parent)
		{
			
		}
		#endregion

		#region Methods
		public void Update()
		{
			if (UpdateCallback != null) UpdateCallback();
		}
		#endregion
	}
}
