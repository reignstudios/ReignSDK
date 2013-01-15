#include "pch.h"
#include "RasterizerState.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructor
	RasterizerStateCom::RasterizerStateCom(VideoCom^ video, RasterizerStateDescCom^ desc)
	{
		this->video = video;
		this->desc = desc;
	}
	#pragma endregion
	
	#pragma region Methods
	void RasterizerStateCom::Enable()
	{
		video->device->SetRenderState(D3DRS_FILLMODE, desc->fillMode);
		video->device->SetRenderState(D3DRS_CULLMODE, desc->cullMode);
		video->device->SetRenderState(D3DRS_MULTISAMPLEANTIALIAS, desc->multisampleEnable);
	}
	#pragma endregion
}