using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class DepthStencilStateDesc : DepthStencilStateDescI
	{
		#region Properties
		internal bool depthReadEnable;
		internal bool depthWriteEnable;
		internal DepthFuncMode depthFunc;

		internal bool stencilEnable;
		internal StencilFuncMode stencilFunc;
		internal StencilOpMode stencilFailOp;
		internal StencilOpMode stencilDepthFailOp;
		internal StencilOpMode stencilPassOp;
		#endregion

		#region Constructors
		public static DepthStencilStateDesc New(DepthStencilStateTypes type)
		{
			return new DepthStencilStateDesc(type);
		}

		public DepthStencilStateDesc(DepthStencilStateTypes type)
		{
			switch (type)
			{
				case (DepthStencilStateTypes.None):
					depthReadEnable = false;
					depthWriteEnable = false;
					depthFunc = DepthFuncMode.Less;

					stencilEnable = false;
					stencilFunc = StencilFuncMode.Never;
					stencilFailOp = StencilOpMode.Keep;
					stencilDepthFailOp = StencilOpMode.Keep;
					stencilPassOp = StencilOpMode.Keep;
					break;

				case (DepthStencilStateTypes.ReadWrite_Less):
					depthReadEnable = true;
					depthWriteEnable = true;
					depthFunc = DepthFuncMode.Less;

					stencilEnable = false;
					stencilFunc = StencilFuncMode.Never;
					stencilFailOp = StencilOpMode.Keep;
					stencilDepthFailOp = StencilOpMode.Keep;
					stencilPassOp = StencilOpMode.Keep;
					break;

				default:
					Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
					break;
			}
		}
		#endregion
	}
}

