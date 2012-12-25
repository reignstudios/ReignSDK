using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
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
			try
			{
				REIGN_D3D11_FILTER filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_LINEAR;
				REIGN_D3D11_TEXTURE_ADDRESS_MODE address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.WRAP;
				switch (type)
				{
					case (SamplerStateTypes.Point_Wrap):
						filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_POINT;
						address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.WRAP;
						break;

					case (SamplerStateTypes.Point_Clamp):
						filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_POINT;
						address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.CLAMP;
						break;

					case (SamplerStateTypes.Point_Border):
						filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_POINT;
						address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.BORDER;
						break;

					case (SamplerStateTypes.Linear_Wrap):
						filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_LINEAR;
						address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.WRAP;
						break;

					case (SamplerStateTypes.Linear_Clamp):
						filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_LINEAR;
						address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.CLAMP;
						break;

					case (SamplerStateTypes.Linear_Border):
						filter = REIGN_D3D11_FILTER.MIN_MAG_MIP_LINEAR;
						address = REIGN_D3D11_TEXTURE_ADDRESS_MODE.BORDER;
						break;

					default:
						Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
						break;
				}

				com = new SamplerStateDescCom(filter, address);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public void Dispose()
		{
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
		}
		#endregion
	}
}