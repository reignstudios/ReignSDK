using Reign.Core;

using System;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;

namespace Reign.Input.XNA
{
	public class Input : Disposable, InputI
	{
		#region Properties
		internal delegate void UpdateCallbackMethod();
		internal UpdateCallbackMethod UpdateCallback;
		internal Application application;
		#endregion

		#region Constructors
		public Input(DisposableI parent, Application application)
		: base(parent)
		{
			this.application = application;
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