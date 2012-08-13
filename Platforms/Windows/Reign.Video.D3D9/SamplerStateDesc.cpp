#include "pch.h"
#include "SamplerState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	D3DTEXTUREFILTERTYPE SamplerStateDesc::Filter::get() {return filter;}

	D3DTEXTUREADDRESS SamplerStateDesc::AddressU::get() {return addressU;}
	D3DTEXTUREADDRESS SamplerStateDesc::AddressV::get() {return addressV;}
	D3DTEXTUREADDRESS SamplerStateDesc::AddressW::get() {return addressW;}

	D3DCOLOR SamplerStateDesc::BorderColor::get() {return borderColor;}
	#pragma endregion

	#pragma region Constructors
	SamplerStateDesc::SamplerStateDesc(SamplerStateTypes type)
	{
		switch (type)
		{
			case (SamplerStateTypes::Point_Wrap):
				filter = D3DTEXF_POINT;
				addressU = D3DTADDRESS_WRAP;
				addressV = D3DTADDRESS_WRAP;
				addressW = D3DTADDRESS_WRAP;
				borderColor = D3DCOLOR_ARGB(0, 0, 0, 0);
				break;

			case (SamplerStateTypes::Point_Clamp):
				filter = D3DTEXF_POINT;
				addressU = D3DTADDRESS_CLAMP;
				addressV = D3DTADDRESS_CLAMP;
				addressW = D3DTADDRESS_CLAMP;
				borderColor = D3DCOLOR_ARGB(0, 0, 0, 0);
				break;

			case (SamplerStateTypes::Linear_Wrap):
				filter = D3DTEXF_LINEAR;
				addressU = D3DTADDRESS_WRAP;
				addressV = D3DTADDRESS_WRAP;
				addressW = D3DTADDRESS_WRAP;
				borderColor = D3DCOLOR_ARGB(0, 0, 0, 0);
				break;

			case (SamplerStateTypes::Linear_Clamp):
				filter = D3DTEXF_LINEAR;
				addressU = D3DTADDRESS_CLAMP;
				addressV = D3DTADDRESS_CLAMP;
				addressW = D3DTADDRESS_CLAMP;
				borderColor = D3DCOLOR_ARGB(0, 0, 0, 0);
				break;

			default:
				Debug::ThrowError(L"SamplerStateDesc", L"Unsuported SamplerStateType");
				break;
		}
	}
	#pragma endregion
}
}
}