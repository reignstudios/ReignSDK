using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class DepthStencilStateDesc : DepthStencilStateDescI
	{
		#region Properties
		internal DepthStencilStateDescCom com;
		#endregion

		#region Constructors
		public static DepthStencilStateDesc New(DepthStencilStateTypes type)
		{
			return new DepthStencilStateDesc(type);
		}

		public DepthStencilStateDesc(DepthStencilStateTypes type)
		{
			try
			{
				bool enable = false;
				REIGN_D3D11_DEPTH_WRITE_MASK mask = REIGN_D3D11_DEPTH_WRITE_MASK.ZERO;
				REIGN_D3D11_COMPARISON_FUNC func = REIGN_D3D11_COMPARISON_FUNC.ALWAYS;
				switch (type)
				{
					
					case (DepthStencilStateTypes.None):
						enable = false;
						mask = REIGN_D3D11_DEPTH_WRITE_MASK.ZERO;
						func = REIGN_D3D11_COMPARISON_FUNC.ALWAYS;
						break;

					case (DepthStencilStateTypes.ReadWrite_Less):
						enable = true;
						mask = REIGN_D3D11_DEPTH_WRITE_MASK.ALL;
						func = REIGN_D3D11_COMPARISON_FUNC.LESS;
						break;

					default:
						Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
						break;
				}

				com = new DepthStencilStateDescCom(enable, mask, func);
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