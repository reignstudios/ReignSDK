#include "pch.h"
#include "DepthStencilState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	D3DZBUFFERTYPE DepthStencilStateDesc::DepthReadEnable::get() {return depthReadEnable;}
	bool DepthStencilStateDesc::DepthWriteEnable::get() {return depthWriteEnable;}
	D3DCMPFUNC DepthStencilStateDesc::DepthFunc::get() {return depthFunc;}

	bool DepthStencilStateDesc::StencilEnable::get() {return stencilEnable;}
	D3DCMPFUNC DepthStencilStateDesc::StencilFunc::get() {return stencilFunc;}
	D3DSTENCILOP DepthStencilStateDesc::StencilFailOp::get() {return stencilFailOp;}
	D3DSTENCILOP DepthStencilStateDesc::StencilDepthFailOp::get() {return stencilDepthFailOp;}
	D3DSTENCILOP DepthStencilStateDesc::StencilPassOp::get() {return stencilPassOp;}
	#pragma endregion

	#pragma region Constructors
	DepthStencilStateDesc::DepthStencilStateDesc(DepthStencilStateTypes type)
	{
		switch (type)
		{
			case (DepthStencilStateTypes::None):
				depthReadEnable = D3DZB_FALSE;
				depthWriteEnable = false;
				depthFunc = D3DCMP_LESS;

				stencilEnable = false;
				stencilFunc = D3DCMP_NEVER;
				stencilFailOp = D3DSTENCILOP_KEEP;
				stencilDepthFailOp = D3DSTENCILOP_KEEP;
				stencilPassOp = D3DSTENCILOP_KEEP;
				break;

			case (DepthStencilStateTypes::ReadWrite_Less):
				depthReadEnable = D3DZB_TRUE;
				depthWriteEnable = true;
				depthFunc = D3DCMP_LESS;

				stencilEnable = false;
				stencilFunc = D3DCMP_NEVER;
				stencilFailOp = D3DSTENCILOP_KEEP;
				stencilDepthFailOp = D3DSTENCILOP_KEEP;
				stencilPassOp = D3DSTENCILOP_KEEP;
				break;

			default:
				Debug::ThrowError(L"DepthStencilStateDesc", L"Unsuported DepthStencilStateType");
				break;
		}
	}
	#pragma endregion
}
}
}