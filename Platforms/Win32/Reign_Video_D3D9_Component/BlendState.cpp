#include "pch.h"
#include "BlendState.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	BlendStateCom::BlendStateCom(VideoCom^ video, BlendStateDescCom^ desc)
	{
		this->video = video;
		this->desc = desc;
	}
	#pragma endregion
	
	#pragma region Methods
	void BlendStateCom::Enable()
	{
		video->device->SetRenderState(D3DRS_COLORWRITEENABLE, desc->renderTargetWriteMask);

		video->device->SetRenderState(D3DRS_ALPHABLENDENABLE, desc->blendEnable);
		if (desc->blendEnable)
		{
			video->device->SetRenderState(D3DRS_BLENDOP, desc->blendOp);
			video->device->SetRenderState(D3DRS_SRCBLEND, desc->srcBlend);
			video->device->SetRenderState(D3DRS_DESTBLEND, desc->dstBlend);
		}

		video->device->SetRenderState(D3DRS_SEPARATEALPHABLENDENABLE, desc->blendEnableAlpha);
		if (desc->blendEnableAlpha)
		{
			video->device->SetRenderState(D3DRS_BLENDOPALPHA, desc->blendOpAlpha);
			video->device->SetRenderState(D3DRS_SRCBLENDALPHA, desc->srcBlendAlpha);
			video->device->SetRenderState(D3DRS_DESTBLENDALPHA, desc->dstBlendAlpha);
		}
	}
	#pragma endregion
}