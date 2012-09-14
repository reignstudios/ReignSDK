#include "pch.h"
#include "DepthStencilState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	DepthStencilStateDescCom::DepthStencilStateDescCom(bool enable, REIGN_D3D11_DEPTH_WRITE_MASK mask, REIGN_D3D11_COMPARISON_FUNC func)
	{
		null();

		desc = new D3D11_DEPTH_STENCIL_DESC();
		ZeroMemory(desc, sizeof(D3D11_DEPTH_STENCIL_DESC));

		desc->DepthEnable = enable;
		desc->DepthWriteMask = (D3D11_DEPTH_WRITE_MASK)mask;
		desc->DepthFunc = (D3D11_COMPARISON_FUNC)func;

		desc->StencilEnable = false;
		desc->StencilReadMask = D3D11_DEFAULT_STENCIL_READ_MASK;
		desc->StencilWriteMask = D3D11_DEFAULT_STENCIL_WRITE_MASK;

		desc->FrontFace.StencilFunc = D3D11_COMPARISON_NEVER;
		desc->FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
		desc->FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;
		desc->FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
				
		desc->BackFace.StencilFunc = D3D11_COMPARISON_NEVER;
		desc->BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
		desc->BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;
		desc->BackFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
	}
	#pragma endregion

	#pragma region Methods
	DepthStencilStateDescCom::~DepthStencilStateDescCom()
	{
		if (desc) delete desc;
		null();
	}

	void DepthStencilStateDescCom::null()
	{
		desc = 0;
	}
	#pragma endregion
}