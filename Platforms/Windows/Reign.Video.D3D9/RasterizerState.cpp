#include "pch.h"
#include "RasterizerState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructor
	RasterizerState::RasterizerState(DisposableI^ parent, RasterizerStateDescI^ desc)
	: Disposable(parent)
	{
		video = parent->FindParentOrSelfWithException<Video^>();
		this->desc = (RasterizerStateDesc^)desc;
	}

	RasterizerState::~RasterizerState()
	{
		disposeChilderen();
	}
	#pragma endregion
	
	#pragma region Methods
	void RasterizerState::Enable()
	{
		video->Device->SetRenderState(D3DRS_FILLMODE, desc->FillMode);
		video->Device->SetRenderState(D3DRS_CULLMODE, desc->CullMode);
		video->Device->SetRenderState(D3DRS_MULTISAMPLEANTIALIAS, desc->MultisampleEnable);
	}
	#pragma endregion
}
}
}