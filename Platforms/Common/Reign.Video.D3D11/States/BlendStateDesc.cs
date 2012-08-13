using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
{
	public class BlendStateDesc : BlendStateDescI
	{
		#region Properties
		internal BlendStateDescCom com;
		#endregion

		#region Constructors
		public BlendStateDesc(BlendStateTypes type)
		{
			try
			{
				bool enable = false;
				REIGN_D3D11_BLEND_OP operation = REIGN_D3D11_BLEND_OP.ADD;
				REIGN_D3D11_BLEND srcBlend = REIGN_D3D11_BLEND.ONE, dstBlend = REIGN_D3D11_BLEND.ONE;
				switch (type)
				{
					case (BlendStateTypes.None):
						enable = false;
						operation = REIGN_D3D11_BLEND_OP.ADD;
						srcBlend = REIGN_D3D11_BLEND.ONE;
						dstBlend = REIGN_D3D11_BLEND.ONE;
						break;

					case (BlendStateTypes.Add):
						enable = true;
						operation = REIGN_D3D11_BLEND_OP.ADD;
						srcBlend = REIGN_D3D11_BLEND.ONE;
						dstBlend = REIGN_D3D11_BLEND.ONE;
						break;

					case (BlendStateTypes.Subtract):
						enable = true;
						operation = REIGN_D3D11_BLEND_OP.SUBTRACT;
						srcBlend = REIGN_D3D11_BLEND.ONE;
						dstBlend = REIGN_D3D11_BLEND.ONE;
						break;

					case (BlendStateTypes.RevSubtract):
						enable = true;
						operation = REIGN_D3D11_BLEND_OP.REV_SUBTRACT;
						srcBlend = REIGN_D3D11_BLEND.ONE;
						dstBlend = REIGN_D3D11_BLEND.ONE;
						break;

					case (BlendStateTypes.Alpha):
						enable = true;
						operation = REIGN_D3D11_BLEND_OP.ADD;
						srcBlend = REIGN_D3D11_BLEND.SRC_ALPHA;
						dstBlend = REIGN_D3D11_BLEND.INV_SRC_ALPHA;
						break;

					default:
						Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
						break;
				}

				com = new BlendStateDescCom(enable, operation, srcBlend, dstBlend);
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