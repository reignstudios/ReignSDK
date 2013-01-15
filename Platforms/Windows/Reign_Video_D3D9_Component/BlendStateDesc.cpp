#include "pch.h"
#include "BlendState.h"

namespace Reign_Video_D3D9_Component
{
	BlendStateDescCom::BlendStateDescCom(DWORD renderTargetWriteMask, bool blendEnable, REIGN_D3DBLENDOP blendOp, REIGN_D3DBLEND srcBlend, REIGN_D3DBLEND dstBlend, bool blendEnableAlpha, REIGN_D3DBLENDOP blendOpAlpha, REIGN_D3DBLEND srcBlendAlpha, REIGN_D3DBLEND dstBlendAlpha)
	{
		this->renderTargetWriteMask = renderTargetWriteMask;

		this->blendEnable = blendEnable;
		this->blendOp = (D3DBLENDOP)blendOp;
		this->srcBlend = (D3DBLEND)srcBlend;
		this->dstBlend = (D3DBLEND)dstBlend;

		this->blendEnableAlpha = blendEnableAlpha;
		this->blendOpAlpha = (D3DBLENDOP)blendOpAlpha;
		this->srcBlendAlpha = (D3DBLEND)srcBlendAlpha;
		this->dstBlendAlpha = (D3DBLEND)dstBlendAlpha;
	}
}