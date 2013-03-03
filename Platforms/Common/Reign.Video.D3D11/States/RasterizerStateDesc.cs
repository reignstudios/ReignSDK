using System;
using Reign.Core;
using Reign_Video_D3D11_Component;

namespace Reign.Video.D3D11
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
			try
			{
				REIGN_D3D11_FILL_MODE fillMode = REIGN_D3D11_FILL_MODE.SOLID;
				REIGN_D3D11_CULL_NONE cullMode = REIGN_D3D11_CULL_NONE.NONE;
				bool frontCounterClockwise = false, depthClipEnable = false, scissorEnable = false, multisampleEnable = false, antialiasedLineEnable = false;
				int depthBias = 0;
				float depthBiasClamp = 0, slopeScaledDepthBias = 0;
				switch (type)
				{
					case RasterizerStateTypes.Solid_CullNone:
						fillMode = REIGN_D3D11_FILL_MODE.SOLID;
						cullMode = REIGN_D3D11_CULL_NONE.NONE;
						frontCounterClockwise = false;
						depthBias = 0;
						depthBiasClamp = 0.0f;
						slopeScaledDepthBias = 0.0f;
						depthClipEnable = true;
						scissorEnable = false;
						multisampleEnable = false;
						antialiasedLineEnable = false;
						break;

					case RasterizerStateTypes.Solid_CullCW:
						fillMode = REIGN_D3D11_FILL_MODE.SOLID;
						cullMode = REIGN_D3D11_CULL_NONE.FRONT;
						frontCounterClockwise = false;
						depthBias = 0;
						depthBiasClamp = 0.0f;
						slopeScaledDepthBias = 0.0f;
						depthClipEnable = true;
						scissorEnable = false;
						multisampleEnable = false;
						antialiasedLineEnable = false;
						break;

					case RasterizerStateTypes.Solid_CullCCW:
						fillMode = REIGN_D3D11_FILL_MODE.SOLID;
						cullMode = REIGN_D3D11_CULL_NONE.BACK;
						frontCounterClockwise = false;
						depthBias = 0;
						depthBiasClamp = 0.0f;
						slopeScaledDepthBias = 0.0f;
						depthClipEnable = true;
						scissorEnable = false;
						multisampleEnable = false;
						antialiasedLineEnable = false;
						break;

					default:
						Debug.ThrowError("RasterizerStateDesc", "Unsuported type: " + type.ToString());
						break;
				}

				com = new RasterizerStateDescCom(fillMode, cullMode, frontCounterClockwise, depthBias, depthBiasClamp, slopeScaledDepthBias, depthClipEnable, scissorEnable, multisampleEnable, antialiasedLineEnable);
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