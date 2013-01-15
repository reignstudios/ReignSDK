#include "pch.h"
#include "RasterizerState.h"

namespace Reign_Video_D3D9_Component
{
	RasterizerStateDescCom::RasterizerStateDescCom(REIGN_D3DFILLMODE fillMode, REIGN_D3DCULL cullMode, bool multisampleEnable)
	{
		this->fillMode = (D3DFILLMODE)fillMode;
		this->cullMode = (D3DCULL)cullMode;
		this->multisampleEnable = multisampleEnable;
	}
}