using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class RasterizerStateDesc : RasterizerStateDescI
	{
		#region Properties
		internal CullFaceMode cullMode;
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
					cullMode = CullFaceMode.None;
					break;

				case (RasterizerStateTypes.Solid_CullCW):
					cullMode = CullFaceMode.Back;
					break;

				case (RasterizerStateTypes.Solid_CullCCW):
					cullMode = CullFaceMode.Front;
					break;

				default:
					Debug.ThrowError("RasterizerStateDesc", "Unsuported RasterizerStateType");
					break;
			}
		}
		#endregion
	}
}

