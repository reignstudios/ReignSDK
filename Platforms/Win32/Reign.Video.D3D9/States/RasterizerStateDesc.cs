using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class RasterizerStateDesc : RasterizerStateDescI
	{
		#region Properties
		internal RasterizerStateDescCom com;
		#endregion

		#region Constructors
		public static RasterizerStateDesc New(RasterizerStateTypes type)
		{
			return new RasterizerStateDesc(type);
		}

		public RasterizerStateDesc(RasterizerStateTypes type)
		{
			REIGN_D3DFILLMODE fillMode = REIGN_D3DFILLMODE.POINT;
			REIGN_D3DCULL cullMode = REIGN_D3DCULL.CCW;
			bool multisampleEnable = false;

			switch (type)
			{
				case RasterizerStateTypes.Solid_CullNone:
					fillMode = REIGN_D3DFILLMODE.SOLID;
					cullMode = REIGN_D3DCULL.NONE;
					multisampleEnable = false;
					break;

				case RasterizerStateTypes.Solid_CullCW:
					fillMode = REIGN_D3DFILLMODE.SOLID;
					cullMode = REIGN_D3DCULL.CW;
					multisampleEnable = false;
					break;

				case RasterizerStateTypes.Solid_CullCCW:
					fillMode = REIGN_D3DFILLMODE.SOLID;
					cullMode = REIGN_D3DCULL.CCW;
					multisampleEnable = false;
					break;

				default:
					Debug.ThrowError("RasterizerStateDesc", "Unsuported RasterizerStateType");
					break;
			}

			com = new RasterizerStateDescCom(fillMode, cullMode, multisampleEnable);
		}
		#endregion
	}
}