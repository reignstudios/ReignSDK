#include "pch.h"
#include "BlendState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	BlendState::BlendState(DisposableI^ parent, BlendStateDescI^ desc)
	: Disposable(parent)
	{
		video = parent->FindParentOrSelfWithException<Video^>();
		this->desc = (BlendStateDesc^)desc;
	}

	BlendState::~BlendState()
	{
		disposeChilderen();
	}
	#pragma endregion
	
	#pragma region Methods
	void BlendState::Enable()
	{
		video->Device->SetRenderState(D3DRS_COLORWRITEENABLE, desc->RenderTargetWriteMask);

		video->Device->SetRenderState(D3DRS_ALPHABLENDENABLE, desc->BlendEnable);
		if (desc->BlendEnable)
		{
			video->Device->SetRenderState(D3DRS_BLENDOP, desc->BlendOp);
			video->Device->SetRenderState(D3DRS_SRCBLEND, desc->SrcBlend);
			video->Device->SetRenderState(D3DRS_DESTBLEND, desc->DestBlend);
		}

		video->Device->SetRenderState(D3DRS_SEPARATEALPHABLENDENABLE, desc->BlendEnableAlpha);
		if (desc->BlendEnableAlpha)
		{
			video->Device->SetRenderState(D3DRS_BLENDOPALPHA, desc->BlendOpAlpha);
			video->Device->SetRenderState(D3DRS_SRCBLENDALPHA, desc->SrcBlendAlpha);
			video->Device->SetRenderState(D3DRS_DESTBLENDALPHA, desc->DestBlendAlpha);
		}
	}
	#pragma endregion
}
}
}