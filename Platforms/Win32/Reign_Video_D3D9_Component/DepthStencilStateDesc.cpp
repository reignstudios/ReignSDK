#include "pch.h"
#include "DepthStencilState.h"

namespace Reign_Video_D3D9_Component
{
	DepthStencilStateDescCom::DepthStencilStateDescCom(REIGN_D3DZBUFFERTYPE depthReadEnable, bool depthWriteEnable, REIGN_D3DCMPFUNC depthFunc, bool stencilEnable, REIGN_D3DCMPFUNC stencilFunc, REIGN_D3DSTENCILOP stencilFailOp, REIGN_D3DSTENCILOP stencilDepthFailOp, REIGN_D3DSTENCILOP stencilPassOp)
	{
		this->depthReadEnable = (D3DZBUFFERTYPE)depthReadEnable;
		this->depthWriteEnable = depthWriteEnable;
		this->depthFunc = (D3DCMPFUNC)depthFunc;

		this->stencilEnable = stencilEnable;
		this->stencilFunc = (D3DCMPFUNC)stencilFunc;
		this->stencilFailOp = (D3DSTENCILOP)stencilFailOp;
		this->stencilDepthFailOp = (D3DSTENCILOP)stencilDepthFailOp;
		this->stencilPassOp = (D3DSTENCILOP)stencilPassOp;
	}
}