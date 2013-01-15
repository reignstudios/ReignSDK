using System;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
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
			REIGN_D3DZBUFFERTYPE depthReadEnable = REIGN_D3DZBUFFERTYPE._FALSE;
			bool depthWriteEnable = false;
			REIGN_D3DCMPFUNC depthFunc = REIGN_D3DCMPFUNC.ALWAYS;
			bool stencilEnable = false;
			REIGN_D3DCMPFUNC stencilFunc = REIGN_D3DCMPFUNC.ALWAYS;
			REIGN_D3DSTENCILOP stencilFailOp = REIGN_D3DSTENCILOP.DECR, stencilDepthFailOp = REIGN_D3DSTENCILOP.DECR, stencilPassOp = REIGN_D3DSTENCILOP.DECR;

			switch (type)
			{
				case (DepthStencilStateTypes.None):
					depthReadEnable = REIGN_D3DZBUFFERTYPE._FALSE;
					depthWriteEnable = false;
					depthFunc = REIGN_D3DCMPFUNC.LESS;

					stencilEnable = false;
					stencilFunc = REIGN_D3DCMPFUNC.NEVER;
					stencilFailOp = REIGN_D3DSTENCILOP.KEEP;
					stencilDepthFailOp = REIGN_D3DSTENCILOP.KEEP;
					stencilPassOp = REIGN_D3DSTENCILOP.KEEP;
					break;

				case (DepthStencilStateTypes.ReadWrite_Less):
					depthReadEnable = REIGN_D3DZBUFFERTYPE._TRUE;
					depthWriteEnable = true;
					depthFunc = REIGN_D3DCMPFUNC.LESS;

					stencilEnable = false;
					stencilFunc = REIGN_D3DCMPFUNC.NEVER;
					stencilFailOp = REIGN_D3DSTENCILOP.KEEP;
					stencilDepthFailOp = REIGN_D3DSTENCILOP.KEEP;
					stencilPassOp = REIGN_D3DSTENCILOP.KEEP;
					break;

				default:
					Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
					break;
			}

			com = new DepthStencilStateDescCom(depthReadEnable, depthWriteEnable, depthFunc, stencilEnable, stencilFunc, stencilFailOp, stencilDepthFailOp, stencilPassOp);
		}
		#endregion
	}
}