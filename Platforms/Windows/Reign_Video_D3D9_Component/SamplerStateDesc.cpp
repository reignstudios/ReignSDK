#include "pch.h"
#include "SamplerState.h"

namespace Reign_Video_D3D9_Component
{
	SamplerStateDescCom::SamplerStateDescCom(REIGN_D3DTEXTUREFILTERTYPE filter, REIGN_D3DTEXTUREADDRESS addressU, REIGN_D3DTEXTUREADDRESS addressV, REIGN_D3DTEXTUREADDRESS addressW, byte borderColorR, byte borderColorG, byte borderColorB, byte borderColorA)
	{
		this->filter = (D3DTEXTUREFILTERTYPE)filter;
		this->addressU = (D3DTEXTUREADDRESS)addressU;
		this->addressV = (D3DTEXTUREADDRESS)addressV;
		this->addressW = (D3DTEXTUREADDRESS)addressW;
		this->borderColor = D3DCOLOR_ARGB(borderColorA, borderColorR, borderColorG, borderColorB);
	}
}