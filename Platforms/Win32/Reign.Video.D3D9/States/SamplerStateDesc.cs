using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class SamplerStateDesc : SamplerStateDescI
	{
		#region Properties
		internal SamplerStateDescCom com;
		#endregion

		#region Constructors
		public static SamplerStateDesc New(SamplerStateTypes type)
		{
			return new SamplerStateDesc(type);
		}

		public SamplerStateDesc(SamplerStateTypes type)
		{
			REIGN_D3DTEXTUREFILTERTYPE filter = REIGN_D3DTEXTUREFILTERTYPE.ANISOTROPIC;
			REIGN_D3DTEXTUREADDRESS addressU = REIGN_D3DTEXTUREADDRESS.BORDER, addressV = REIGN_D3DTEXTUREADDRESS.BORDER, addressW = REIGN_D3DTEXTUREADDRESS.BORDER;
			byte r = 0, g = 0, b = 0, a = 0;

			switch (type)
			{
				case SamplerStateTypes.Point_Wrap:
					filter = REIGN_D3DTEXTUREFILTERTYPE.POINT;
					addressU = REIGN_D3DTEXTUREADDRESS.WRAP;
					addressV = REIGN_D3DTEXTUREADDRESS.WRAP;
					addressW = REIGN_D3DTEXTUREADDRESS.WRAP;
					break;

				case SamplerStateTypes.Point_Clamp:
					filter = REIGN_D3DTEXTUREFILTERTYPE.POINT;
					addressU = REIGN_D3DTEXTUREADDRESS.CLAMP;
					addressV = REIGN_D3DTEXTUREADDRESS.CLAMP;
					addressW = REIGN_D3DTEXTUREADDRESS.CLAMP;
					break;

				case SamplerStateTypes.Linear_Wrap:
					filter = REIGN_D3DTEXTUREFILTERTYPE.LINEAR;
					addressU = REIGN_D3DTEXTUREADDRESS.WRAP;
					addressV = REIGN_D3DTEXTUREADDRESS.WRAP;
					addressW = REIGN_D3DTEXTUREADDRESS.WRAP;
					break;

				case SamplerStateTypes.Linear_Clamp:
					filter = REIGN_D3DTEXTUREFILTERTYPE.LINEAR;
					addressU = REIGN_D3DTEXTUREADDRESS.CLAMP;
					addressV = REIGN_D3DTEXTUREADDRESS.CLAMP;
					addressW = REIGN_D3DTEXTUREADDRESS.CLAMP;
					break;

				default:
					Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
					break;
			}

			com = new SamplerStateDescCom(filter, addressU, addressV, addressW, r, g, b, a);
		}
		#endregion
	}
}