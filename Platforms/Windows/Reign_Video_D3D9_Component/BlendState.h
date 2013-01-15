#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class REIGN_D3DBLENDOP
	{
		ADD = D3DBLENDOP_ADD,
		SUBTRACT = D3DBLENDOP_SUBTRACT,
		REVSUBTRACT = D3DBLENDOP_REVSUBTRACT
	};

	public enum class REIGN_D3DBLEND
	{
		ZERO = D3DBLEND_ZERO,
		ONE = D3DBLEND_ONE,
		SRCALPHA = D3DBLEND_SRCALPHA,
		INVSRCALPHA = D3DBLEND_INVSRCALPHA
	};

	public ref class BlendStateDescCom sealed
	{
		#pragma region Properties
		internal: DWORD renderTargetWriteMask;
		internal: bool blendEnable;
		internal: D3DBLENDOP blendOp;
		internal: D3DBLEND srcBlend;
		internal: D3DBLEND dstBlend;
		internal: bool blendEnableAlpha;
		internal: D3DBLENDOP blendOpAlpha;
		internal: D3DBLEND srcBlendAlpha;
		internal: D3DBLEND dstBlendAlpha;
		#pragma endregion

		#pragma region Constructors
		public: BlendStateDescCom(DWORD renderTargetWriteMask, bool blendEnable, REIGN_D3DBLENDOP blendOp, REIGN_D3DBLEND srcBlend, REIGN_D3DBLEND dstBlend, bool blendEnableAlpha, REIGN_D3DBLENDOP blendOpAlpha, REIGN_D3DBLEND srcBlendAlpha, REIGN_D3DBLEND dstBlendAlpha);
		#pragma endregion
	};

	public ref class BlendStateCom sealed
	{
		#pragma region Fields
		private: VideoCom^ video;
		private: BlendStateDescCom^ desc;
		#pragma endregion
	
		#pragma region Constructors
		public: BlendStateCom(VideoCom^ video, BlendStateDescCom^ desc);
		#pragma endregion
		
		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}