#include "pch.h"
#include "DepthStencilState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	DepthStencilState::DepthStencilState(DisposableI^ parent, DepthStencilStateDescI^ desc)
	: Disposable(parent)
	{
		video = parent->FindParentOrSelfWithException<Video^>();
		this->desc = (DepthStencilStateDesc^)desc;
	}

	DepthStencilState::~DepthStencilState()
	{
		disposeChilderen();
	}
	#pragma endregion
	
	#pragma region Methods
	void DepthStencilState::Enable()
	{
		video->Device->SetRenderState(D3DRS_ZENABLE, desc->DepthReadEnable);
		if (desc->DepthReadEnable == D3DZB_TRUE)
		{
			video->Device->SetRenderState(D3DRS_ZFUNC, desc->DepthFunc);
		}
		video->Device->SetRenderState(D3DRS_ZWRITEENABLE, desc->DepthWriteEnable);

		video->Device->SetRenderState(D3DRS_STENCILENABLE, desc->StencilEnable);
		if (desc->StencilEnable)
		{
			//D3DRS_TWOSIDEDSTENCILMODE
			video->Device->SetRenderState(D3DRS_STENCILFUNC, desc->StencilFunc);
			video->Device->SetRenderState(D3DRS_STENCILFAIL, desc->StencilFailOp);
			video->Device->SetRenderState(D3DRS_STENCILZFAIL, desc->StencilDepthFailOp);
			video->Device->SetRenderState(D3DRS_STENCILPASS, desc->StencilPassOp);

			/*video->Device->SetRenderState(D3DRS_CCW_STENCILFUNC, desc->StencilFunc);
			video->Device->SetRenderState(D3DRS_CCW_STENCILFAIL, desc->StencilFailOp);
			video->Device->SetRenderState(D3DRS_CCW_STENCILZFAIL, desc->StencilDepthFailOp);
			video->Device->SetRenderState(D3DRS_CCW_STENCILPASS, desc->StencilPassOp);*/
		}
	}
	#pragma endregion
}
}
}