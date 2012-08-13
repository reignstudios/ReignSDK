#include "pch.h"
#include "DepthStencilState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	DepthStencilStateError DepthStencilStateCom::Init(VideoCom^ video, DepthStencilStateDescCom^ desc)
	{
		null();
		this->video = video;

		ID3D11DepthStencilState* stateTEMP;
		if (FAILED(video->device->CreateDepthStencilState(desc->desc, &stateTEMP)))
		{
			return DepthStencilStateError::DepthStencil;
		}
		state = stateTEMP;

		return DepthStencilStateError::None;
	}

	DepthStencilStateCom::~DepthStencilStateCom()
	{
		if (state) state->Release();
		null();
	}

	void DepthStencilStateCom::null()
	{
		state = 0;
	}
	#pragma endregion

	#pragma region Methods
	void DepthStencilStateCom::Enable()
	{
		video->deviceContext->OMSetDepthStencilState(state, 0);
	}
	#pragma endregion
}