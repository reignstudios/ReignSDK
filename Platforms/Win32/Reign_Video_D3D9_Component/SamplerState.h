#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class REIGN_D3DTEXTUREFILTERTYPE
	{
		NONE = D3DTEXF_NONE,
		POINT = D3DTEXF_POINT,
		LINEAR = D3DTEXF_LINEAR,
		ANISOTROPIC = D3DTEXF_ANISOTROPIC,
		PYRAMIDALQUAD = D3DTEXF_PYRAMIDALQUAD,
		GAUSSIANQUAD = D3DTEXF_GAUSSIANQUAD,
		CONVOLUTIONMONO = D3DTEXF_CONVOLUTIONMONO
	};

	public enum class REIGN_D3DTEXTUREADDRESS
	{
		WRAP = D3DTADDRESS_WRAP,
		MIRROR = D3DTADDRESS_MIRROR,
		CLAMP = D3DTADDRESS_CLAMP,
		BORDER = D3DTADDRESS_BORDER,
		MIRRORONCE = D3DTADDRESS_MIRRORONCE
	};

	public ref class SamplerStateDescCom sealed
	{
		#pragma region Properties
		internal: D3DTEXTUREFILTERTYPE filter;
		internal: D3DTEXTUREADDRESS addressU;
		internal: D3DTEXTUREADDRESS addressV;
		internal: D3DTEXTUREADDRESS addressW;
		internal: D3DCOLOR borderColor;
		#pragma endregion

		#pragma region Constructors
		public: SamplerStateDescCom(REIGN_D3DTEXTUREFILTERTYPE filter, REIGN_D3DTEXTUREADDRESS addressU, REIGN_D3DTEXTUREADDRESS addressV, REIGN_D3DTEXTUREADDRESS addressW, byte borderColorR, byte borderColorG, byte borderColorB, byte borderColorA);
		#pragma endregion
	};

	public ref class SamplerStateCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: SamplerStateDescCom^ desc;
		#pragma endregion
	
		#pragma region Constructors
		public: SamplerStateCom(VideoCom^ video, SamplerStateDescCom^ desc);
		#pragma endregion
		
		#pragma region Methods
		public: void Enable(int index);
		#pragma endregion
	};
}