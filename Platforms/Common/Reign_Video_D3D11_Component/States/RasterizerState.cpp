#include "pch.h"
#include "RasterizerState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	RasterizerStateError RasterizerStateCom::Init(VideoCom^ video, RasterizerStateDescCom^ desc)
	{
		null();
		this->video = video;

		ID3D11RasterizerState* stateTEMP;
		if (FAILED(video->device->CreateRasterizerState(desc->desc, &stateTEMP)))
		{
			return RasterizerStateError::RasterizerState;
		}
		state = stateTEMP;

		return RasterizerStateError::None;
	}

	RasterizerStateCom::~RasterizerStateCom()
	{
		if (state) state->Release();
		null();
	}

	void RasterizerStateCom::null()
	{
		state = 0;
	}
	#pragma endregion

	#pragma region Methods
	void RasterizerStateCom::Enable()
	{
		video->deviceContext->RSSetState(state);
	}
	#pragma endregion
}