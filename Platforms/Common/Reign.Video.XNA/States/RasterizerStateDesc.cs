using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class RasterizerStateDesc : RasterizerStateDescI
	{
		#region Properties
		public X.FillMode FillMode {get; private set;}
		public X.CullMode CullMode {get; private set;}
		public bool MultisampleEnable {get; private set;}
		#endregion

		#region Constructors
		public RasterizerStateDesc(RasterizerStateTypes type)
		{
			switch (type)
			{
				case (RasterizerStateTypes.Solid_CullNone):
					FillMode = X.FillMode.Solid;
					CullMode = X.CullMode.None;
					MultisampleEnable = false;
					break;

				case (RasterizerStateTypes.Solid_CullCW):
					FillMode = X.FillMode.Solid;
					CullMode = X.CullMode.CullClockwiseFace;
					MultisampleEnable = false;
					break;

				case (RasterizerStateTypes.Solid_CullCCW):
					FillMode = X.FillMode.Solid;
					CullMode = X.CullMode.CullCounterClockwiseFace;
					MultisampleEnable = false;
					break;

				default:
					Debug.ThrowError("RasterizerStateDesc", "Unsuported RasterizerStateType");
					break;
			}
		}
		#endregion
	}
}