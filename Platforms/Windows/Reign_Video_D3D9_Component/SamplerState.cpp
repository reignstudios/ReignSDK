#include "pch.h"
#include "SamplerState.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructor
	SamplerStateCom::SamplerStateCom(VideoCom^ video, SamplerStateDescCom^ desc)
	{
		this->video = video;
		this->desc = desc;
	}
	#pragma endregion
	
	#pragma region Methods
	void SamplerStateCom::Enable(int index)
	{
		video->device->SetSamplerState(index, D3DSAMP_MINFILTER, desc->filter);
		video->device->SetSamplerState(index, D3DSAMP_MAGFILTER, desc->filter);
		video->device->SetSamplerState(index, D3DSAMP_MIPFILTER, desc->filter);

		video->device->SetSamplerState(index, D3DSAMP_ADDRESSU, desc->addressU);
		video->device->SetSamplerState(index, D3DSAMP_ADDRESSV, desc->addressV);
		video->device->SetSamplerState(index, D3DSAMP_ADDRESSW, desc->addressW);

		video->device->SetSamplerState(index, D3DSAMP_BORDERCOLOR, desc->borderColor);
	}
	#pragma endregion
}