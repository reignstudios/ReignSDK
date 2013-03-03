using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class DepthStencilStateDesc : DepthStencilStateDescI
	{
		#region Properties
		internal bool depthReadEnable;
		internal bool depthWriteEnable;
		internal X.CompareFunction depthFunc;

		internal bool stencilEnable;
		internal X.CompareFunction stencilFunc;
		internal X.StencilOperation stencilFailOp;
		internal X.StencilOperation stencilDepthFailOp;
		internal X.StencilOperation stencilPassOp;
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
			    case DepthStencilStateTypes.None:
			        depthReadEnable = false;
			        depthWriteEnable = false;
			        depthFunc = X.CompareFunction.Less;

			        stencilEnable = false;
			        stencilFunc = X.CompareFunction.Never;
			        stencilFailOp = X.StencilOperation.Keep;
			        stencilDepthFailOp = X.StencilOperation.Keep;
			        stencilPassOp = X.StencilOperation.Keep;
			        break;

			    case DepthStencilStateTypes.ReadWrite_Less:
			        depthReadEnable = true;
			        depthWriteEnable = true;
			        depthFunc = X.CompareFunction.Less;

			        stencilEnable = false;
			        stencilFunc = X.CompareFunction.Never;
			        stencilFailOp = X.StencilOperation.Keep;
			        stencilDepthFailOp = X.StencilOperation.Keep;
			        stencilPassOp = X.StencilOperation.Keep;
			        break;

				default:
					Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
					break;
			}
		}
		#endregion
	}
}