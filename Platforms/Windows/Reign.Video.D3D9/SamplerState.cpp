#include "pch.h"
#include "SamplerState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructor
	SamplerState::SamplerState(DisposableI^ parent, SamplerStateDescI^ desc)
	: Disposable(parent)
	{
		video = parent->FindParentOrSelfWithException<Video^>();
		this->desc = (SamplerStateDesc^)desc;
	}

	SamplerState::~SamplerState()
	{
		disposeChilderen();
	}
	#pragma endregion
	
	#pragma region Methods
	void SamplerState::Enable(int index)
	{
		video->Device->SetSamplerState(index, D3DSAMP_MINFILTER, desc->Filter);
		video->Device->SetSamplerState(index, D3DSAMP_MAGFILTER, desc->Filter);
		video->Device->SetSamplerState(index, D3DSAMP_MIPFILTER, desc->Filter);

		video->Device->SetSamplerState(index, D3DSAMP_ADDRESSU, desc->AddressU);
		video->Device->SetSamplerState(index, D3DSAMP_ADDRESSV, desc->AddressV);
		video->Device->SetSamplerState(index, D3DSAMP_ADDRESSW, desc->AddressW);

		video->Device->SetSamplerState(index, D3DSAMP_BORDERCOLOR, desc->BorderColor);
	}
	#pragma endregion
}
}
}