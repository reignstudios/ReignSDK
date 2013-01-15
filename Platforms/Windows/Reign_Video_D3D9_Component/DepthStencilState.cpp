#include "pch.h"
#include "DepthStencilState.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	DepthStencilStateCom::DepthStencilStateCom(VideoCom^ video, DepthStencilStateDescCom^ desc)
	{
		this->video = video;
		this->desc = desc;
	}
	#pragma endregion
	
	#pragma region Methods
	void DepthStencilStateCom::Enable()
	{
		video->device->SetRenderState(D3DRS_ZENABLE, desc->depthReadEnable);
		if (desc->depthReadEnable == D3DZB_TRUE)
		{
			video->device->SetRenderState(D3DRS_ZFUNC, desc->depthFunc);
		}
		video->device->SetRenderState(D3DRS_ZWRITEENABLE, desc->depthWriteEnable);

		video->device->SetRenderState(D3DRS_STENCILENABLE, desc->stencilEnable);
		if (desc->stencilEnable)
		{
			//D3DRS_TWOSIDEDSTENCILMODE
			video->device->SetRenderState(D3DRS_STENCILFUNC, desc->stencilFunc);
			video->device->SetRenderState(D3DRS_STENCILFAIL, desc->stencilFailOp);
			video->device->SetRenderState(D3DRS_STENCILZFAIL, desc->stencilDepthFailOp);
			video->device->SetRenderState(D3DRS_STENCILPASS, desc->stencilPassOp);

			/*video->device->SetRenderState(D3DRS_CCW_STENCILFUNC, desc->stencilFunc);
			video->device->SetRenderState(D3DRS_CCW_STENCILFAIL, desc->stencilFailOp);
			video->device->SetRenderState(D3DRS_CCW_STENCILZFAIL, desc->stencilDepthFailOp);
			video->device->SetRenderState(D3DRS_CCW_STENCILPASS, desc->stencilPassOp);*/
		}
	}
	#pragma endregion
}