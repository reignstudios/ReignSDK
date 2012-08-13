#include "pch.h"
#include "BlendState.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	DWORD BlendStateDesc::RenderTargetWriteMask::get() {return renderTargetWriteMask;}

	bool BlendStateDesc::BlendEnable::get() {return blendEnable;}
	D3DBLENDOP BlendStateDesc::BlendOp::get() {return blendOp;}
	D3DBLEND BlendStateDesc::SrcBlend::get() {return srcBlend;}
	D3DBLEND BlendStateDesc::DestBlend::get() {return destBlend;}

	bool BlendStateDesc::BlendEnableAlpha::get() {return blendEnableAlpha;}
	D3DBLENDOP BlendStateDesc::BlendOpAlpha::get() {return blendOpAlpha;}
	D3DBLEND BlendStateDesc::SrcBlendAlpha::get() {return srcBlendAlpha;}
	D3DBLEND BlendStateDesc::DestBlendAlpha::get() {return destBlendAlpha;}
	#pragma endregion

	#pragma region Constructors
	BlendStateDesc::BlendStateDesc(BlendStateTypes type)
	{
		renderTargetWriteMask = 0xFFFFFFFF;

		switch (type)
		{
			case (BlendStateTypes::None):
				blendEnable = false;
				blendOp = D3DBLENDOP_ADD;
				srcBlend = D3DBLEND_ONE;
				destBlend = D3DBLEND_ZERO;

				blendEnableAlpha = false;
				blendOpAlpha = D3DBLENDOP_ADD;
				srcBlendAlpha = D3DBLEND_ONE;
				destBlendAlpha = D3DBLEND_ZERO;
				break;

			case (BlendStateTypes::Add):
				blendEnable = true;
				blendOp = D3DBLENDOP_ADD;
				srcBlend = D3DBLEND_ONE;
				destBlend = D3DBLEND_ONE;

				blendEnableAlpha = false;
				blendOpAlpha = D3DBLENDOP_ADD;
				srcBlendAlpha = D3DBLEND_ONE;
				destBlendAlpha = D3DBLEND_ONE;
				break;

			case (BlendStateTypes::Subtract):
				blendEnable = true;
				blendOp = D3DBLENDOP_SUBTRACT;
				srcBlend = D3DBLEND_ONE;
				destBlend = D3DBLEND_ONE;

				blendEnableAlpha = false;
				blendOpAlpha = D3DBLENDOP_SUBTRACT;
				srcBlendAlpha = D3DBLEND_ONE;
				destBlendAlpha = D3DBLEND_ONE;
				break;

			case (BlendStateTypes::RevSubtract):
				blendEnable = true;
				blendOp = D3DBLENDOP_REVSUBTRACT;
				srcBlend = D3DBLEND_ONE;
				destBlend = D3DBLEND_ONE;

				blendEnableAlpha = false;
				blendOpAlpha = D3DBLENDOP_REVSUBTRACT;
				srcBlendAlpha = D3DBLEND_ONE;
				destBlendAlpha = D3DBLEND_ONE;
				break;

			case (BlendStateTypes::Alpha):
				blendEnable = true;
				blendOp = D3DBLENDOP_ADD;
				srcBlend = D3DBLEND_SRCALPHA;
				destBlend = D3DBLEND_INVSRCALPHA;

				blendEnableAlpha = false;
				blendOpAlpha = D3DBLENDOP_ADD;
				srcBlendAlpha = D3DBLEND_SRCALPHA;
				destBlendAlpha = D3DBLEND_INVSRCALPHA;
				break;

			default:
				Debug::ThrowError(L"BlendStateDesc", L"Unsuported BlendStateType");
				break;
		}
	}
	#pragma endregion
}
}
}