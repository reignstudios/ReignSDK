#include "pch.h"
#include "BlendState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	BlendStateDescCom::BlendStateDescCom(bool enable, REIGN_D3D11_BLEND_OP operation, REIGN_D3D11_BLEND srcBlend, REIGN_D3D11_BLEND dstBlend)
	{
		null();

		desc = new D3D11_BLEND_DESC();
		ZeroMemory(desc, sizeof(D3D11_BLEND_DESC));
		desc->AlphaToCoverageEnable = false;
		desc->IndependentBlendEnable = false;

		for (int i = 0; i != 8; ++i)
		{
			desc->RenderTarget[i].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
			desc->RenderTarget[i].BlendEnable = enable;

			desc->RenderTarget[i].BlendOp = (D3D11_BLEND_OP)operation;
			desc->RenderTarget[i].SrcBlend = (D3D11_BLEND)srcBlend;
			desc->RenderTarget[i].DestBlend = (D3D11_BLEND)dstBlend;

			desc->RenderTarget[i].BlendOpAlpha = (D3D11_BLEND_OP)operation;
			desc->RenderTarget[i].SrcBlendAlpha = (D3D11_BLEND)srcBlend;
			desc->RenderTarget[i].DestBlendAlpha = (D3D11_BLEND)dstBlend;
		}
	}
	#pragma endregion

	#pragma region Methods
	BlendStateDescCom::~BlendStateDescCom()
	{
		if (desc) delete desc;
		null();
	}

	void BlendStateDescCom::null()
	{
		desc = 0;
	}
	#pragma endregion
}