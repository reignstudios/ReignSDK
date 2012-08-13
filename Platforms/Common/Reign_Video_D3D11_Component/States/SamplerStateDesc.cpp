#include "pch.h"
#include "SamplerState.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	SamplerStateDescCom::SamplerStateDescCom(REIGN_D3D11_FILTER filter, REIGN_D3D11_TEXTURE_ADDRESS_MODE address)
	{
		null();

		desc = new D3D11_SAMPLER_DESC();
		ZeroMemory(desc, sizeof(D3D11_SAMPLER_DESC));

		desc->Filter = (D3D11_FILTER)filter;
		desc->AddressU = (D3D11_TEXTURE_ADDRESS_MODE)address;
		desc->AddressV = (D3D11_TEXTURE_ADDRESS_MODE)address;
		desc->AddressW = (D3D11_TEXTURE_ADDRESS_MODE)address;
		desc->MipLODBias = 0.0f;
		desc->MaxAnisotropy = 1;
		desc->ComparisonFunc = D3D11_COMPARISON_NEVER;
		desc->BorderColor[0] = 0.0f;
		desc->BorderColor[1] = 0.0f;
		desc->BorderColor[2] = 0.0f;
		desc->BorderColor[3] = 0.0f;
		desc->MinLOD = 0.0f;
		desc->MaxLOD = D3D11_FLOAT32_MAX;
	}
	#pragma endregion

	#pragma region Methods
	SamplerStateDescCom::~SamplerStateDescCom()
	{
		if (desc) delete desc;
		null();
	}

	void SamplerStateDescCom::null()
	{
		desc = 0;
	}
	#pragma endregion
}