using Reign.Video;
using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	public class BlendStateDesc : IBlendStateDesc
	{
		#region Properties
		internal BlendStateDescCom com;
		#endregion

		#region Constructors
		public BlendStateDesc(BlendStateTypes type)
		{
			uint renderTargetWriteMask = 0xFFFFFFFF;
			bool blendEnable = false, blendEnableAlpha = false;
			REIGN_D3DBLENDOP blendOp = REIGN_D3DBLENDOP.ADD, blendOpAlpha = REIGN_D3DBLENDOP.ADD;
			REIGN_D3DBLEND srcBlend = REIGN_D3DBLEND.ONE, dstBlend = REIGN_D3DBLEND.ONE, srcBlendAlpha = REIGN_D3DBLEND.ONE, dstBlendAlpha = REIGN_D3DBLEND.ONE;

			switch (type)
			{
				case BlendStateTypes.None:
					blendEnable = false;
					blendOp = REIGN_D3DBLENDOP.ADD;
					srcBlend = REIGN_D3DBLEND.ONE;
					dstBlend = REIGN_D3DBLEND.ZERO;

					blendEnableAlpha = false;
					blendOpAlpha = REIGN_D3DBLENDOP.ADD;
					srcBlendAlpha = REIGN_D3DBLEND.ONE;
					dstBlendAlpha = REIGN_D3DBLEND.ZERO;
					break;

				case BlendStateTypes.Add:
					blendEnable = true;
					blendOp = REIGN_D3DBLENDOP.ADD;
					srcBlend = REIGN_D3DBLEND.ONE;
					dstBlend = REIGN_D3DBLEND.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = REIGN_D3DBLENDOP.ADD;
					srcBlendAlpha = REIGN_D3DBLEND.ONE;
					dstBlendAlpha = REIGN_D3DBLEND.ONE;
					break;

				case BlendStateTypes.Subtract:
					blendEnable = true;
					blendOp = REIGN_D3DBLENDOP.SUBTRACT;
					srcBlend = REIGN_D3DBLEND.ONE;
					dstBlend = REIGN_D3DBLEND.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = REIGN_D3DBLENDOP.SUBTRACT;
					srcBlendAlpha = REIGN_D3DBLEND.ONE;
					dstBlendAlpha = REIGN_D3DBLEND.ONE;
					break;

				case BlendStateTypes.RevSubtract:
					blendEnable = true;
					blendOp = REIGN_D3DBLENDOP.REVSUBTRACT;
					srcBlend = REIGN_D3DBLEND.ONE;
					dstBlend = REIGN_D3DBLEND.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = REIGN_D3DBLENDOP.REVSUBTRACT;
					srcBlendAlpha = REIGN_D3DBLEND.ONE;
					dstBlendAlpha = REIGN_D3DBLEND.ONE;
					break;

				case BlendStateTypes.Alpha:
					blendEnable = true;
					blendOp = REIGN_D3DBLENDOP.ADD;
					srcBlend = REIGN_D3DBLEND.SRCALPHA;
					dstBlend = REIGN_D3DBLEND.INVSRCALPHA;

					blendEnableAlpha = false;
					blendOpAlpha = REIGN_D3DBLENDOP.ADD;
					srcBlendAlpha = REIGN_D3DBLEND.SRCALPHA;
					dstBlendAlpha = REIGN_D3DBLEND.INVSRCALPHA;
					break;

				default:
					Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
					break;
			}

			com = new BlendStateDescCom(renderTargetWriteMask, blendEnable, blendOp, srcBlend, dstBlend, blendEnableAlpha, blendOpAlpha, srcBlendAlpha, dstBlendAlpha);
		}
		#endregion
	}
}
