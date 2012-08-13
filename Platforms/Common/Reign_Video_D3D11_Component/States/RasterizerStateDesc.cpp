#include "pch.h"
#include "RasterizerState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	RasterizerStateDescCom::RasterizerStateDescCom(REIGN_D3D11_FILL_MODE fillMode, REIGN_D3D11_CULL_NONE cullMode, bool frontCounterClockwise, int depthBias, float depthBiasClamp, float slopeScaledDepthBias, bool depthClipEnable, bool scissorEnable, bool multisampleEnable, bool antialiasedLineEnable)
	{
		null();

		desc = new D3D11_RASTERIZER_DESC();
		ZeroMemory(desc, sizeof(D3D11_RASTERIZER_DESC));

		desc->FillMode = (D3D11_FILL_MODE)fillMode;
		desc->CullMode = (D3D11_CULL_MODE)cullMode;
		desc->FrontCounterClockwise = frontCounterClockwise;
		desc->DepthBias = depthBias;
		desc->DepthBiasClamp = depthBiasClamp;
		desc->SlopeScaledDepthBias = slopeScaledDepthBias;
		desc->DepthClipEnable = depthClipEnable;
		desc->ScissorEnable = scissorEnable;
		desc->MultisampleEnable = multisampleEnable;
		desc->AntialiasedLineEnable = antialiasedLineEnable;
	}
	#pragma endregion

	#pragma region Methods
	RasterizerStateDescCom::~RasterizerStateDescCom()
	{
		if (desc) delete desc;
		null();
	}

	void RasterizerStateDescCom::null()
	{
		desc = 0;
	}
	#pragma endregion
}