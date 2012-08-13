#include "pch.h"
#include "SamplerState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	SamplerStateError SamplerStateCom::Init(VideoCom^ video, SamplerStateDescCom^ desc)
	{
		null();
		this->video = video;

		ID3D11SamplerState* stateTEMP;
		if (FAILED(video->device->CreateSamplerState(desc->desc, &stateTEMP)))
		{
			return SamplerStateError::SampleState;
		}
		state = stateTEMP;

		return SamplerStateError::None;
	}

	SamplerStateCom::~SamplerStateCom()
	{
		if (state) state->Release();
		null();
	}

	void SamplerStateCom::null()
	{
		state = 0;
	}
	#pragma endregion

	#pragma region Methods
	void SamplerStateCom::Enable(int index)
	{
		ID3D11SamplerState* stateTEMP = state;
		video->deviceContext->PSSetSamplers(index, 1, &stateTEMP);
	}
	#pragma endregion
}