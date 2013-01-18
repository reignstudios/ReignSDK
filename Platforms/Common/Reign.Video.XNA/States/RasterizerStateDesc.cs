using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class RasterizerStateDesc : RasterizerStateDescI
	{
		#region Properties
		internal X.FillMode fillMode;
		internal X.CullMode cullMode;
		internal bool multisampleEnable;
		#endregion

		#region Constructors
		public static RasterizerStateDesc New(RasterizerStateTypes type)
		{
			return new RasterizerStateDesc(type);
		}

		public RasterizerStateDesc(RasterizerStateTypes type)
		{
			switch (type)
			{
				case (RasterizerStateTypes.Solid_CullNone):
					fillMode = X.FillMode.Solid;
					cullMode = X.CullMode.None;
					multisampleEnable = false;
					break;

				case (RasterizerStateTypes.Solid_CullCW):
					fillMode = X.FillMode.Solid;
					cullMode = X.CullMode.CullClockwiseFace;
					multisampleEnable = false;
					break;

				case (RasterizerStateTypes.Solid_CullCCW):
					fillMode = X.FillMode.Solid;
					cullMode = X.CullMode.CullCounterClockwiseFace;
					multisampleEnable = false;
					break;

				default:
					Debug.ThrowError("RasterizerStateDesc", "Unsuported RasterizerStateType");
					break;
			}
		}
		#endregion
	}
}