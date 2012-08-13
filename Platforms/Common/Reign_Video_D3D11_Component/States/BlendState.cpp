#include "pch.h"
#include "BlendState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	BlendStateError BlendStateCom::Init(VideoCom^ video, BlendStateDescCom^ desc)
	{
		null();
		this->video = video;

		ID3D11BlendState* stateTEMP;
		if (FAILED(video->device->CreateBlendState(desc->desc, &stateTEMP)))
		{
			return BlendStateError::BlendState;
		}
		state = stateTEMP;

		return BlendStateError::None;
	}

	BlendStateCom::~BlendStateCom()
	{
		if (state) state->Release();
		null();
	}

	void BlendStateCom::null()
	{
		state = 0;
	}
	#pragma endregion

	#pragma region Methods
	void BlendStateCom::Enable()
	{
		ID3D11BlendState* stateTEMP = state;
		video->deviceContext->OMSetBlendState(stateTEMP, 0, 0xffffffff);
	}
	#pragma endregion
}