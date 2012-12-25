using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class DepthStencilStateDesc : DepthStencilStateDescI
	{
		#region Properties
		public bool DepthReadEnable {get; private set;}
		public bool DepthWriteEnable {get; private set;}
		public X.CompareFunction DepthFunc {get; private set;}

		public bool StencilEnable {get; private set;}
		public X.CompareFunction StencilFunc {get; private set;}
		public X.StencilOperation StencilFailOp {get; private set;}
		public X.StencilOperation StencilDepthFailOp {get; private set;}
		public X.StencilOperation StencilPassOp {get; private set;}
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
			        DepthReadEnable = false;
			        DepthWriteEnable = false;
			        DepthFunc = X.CompareFunction.Less;

			        StencilEnable = false;
			        StencilFunc = X.CompareFunction.Never;
			        StencilFailOp = X.StencilOperation.Keep;
			        StencilDepthFailOp = X.StencilOperation.Keep;
			        StencilPassOp = X.StencilOperation.Keep;
			        break;

			    case (DepthStencilStateTypes.ReadWrite_Less):
			        DepthReadEnable = true;
			        DepthWriteEnable = true;
			        DepthFunc = X.CompareFunction.Less;

			        StencilEnable = false;
			        StencilFunc = X.CompareFunction.Never;
			        StencilFailOp = X.StencilOperation.Keep;
			        StencilDepthFailOp = X.StencilOperation.Keep;
			        StencilPassOp = X.StencilOperation.Keep;
			        break;

				default:
					Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
					break;
			}
		}
		#endregion
	}
}