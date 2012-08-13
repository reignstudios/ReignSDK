#include "pch.h"
#include "RasterizerState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	D3DFILLMODE RasterizerStateDesc::FillMode::get() {return fillMode;}
	D3DCULL RasterizerStateDesc::CullMode::get() {return cullMode;}
	bool RasterizerStateDesc::MultisampleEnable::get() {return multisampleEnable;}
	#pragma endregion

	#pragma region Constructors
	RasterizerStateDesc::RasterizerStateDesc(RasterizerStateTypes type)
	{
		switch (type)
		{
			case (RasterizerStateTypes::Solid_CullNone):
				fillMode = D3DFILL_SOLID;
				cullMode = D3DCULL_NONE;
				multisampleEnable = false;
				break;

			default:
				Debug::ThrowError(L"RasterizerStateDesc", L"Unsuported RasterizerStateType");
				break;
		}
	}
	#pragma endregion
}
}
}