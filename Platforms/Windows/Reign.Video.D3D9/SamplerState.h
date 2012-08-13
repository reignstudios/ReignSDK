#pragma once
#include "Video.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class SamplerStateDesc : SamplerStateDescI
	{
		#pragma region Properties
		private: D3DTEXTUREFILTERTYPE filter;
		public: property D3DTEXTUREFILTERTYPE Filter {D3DTEXTUREFILTERTYPE get();}

		private: D3DTEXTUREADDRESS addressU;
		public: property D3DTEXTUREADDRESS AddressU {D3DTEXTUREADDRESS get();}

		private: D3DTEXTUREADDRESS addressV;
		public: property D3DTEXTUREADDRESS AddressV {D3DTEXTUREADDRESS get();}

		private: D3DTEXTUREADDRESS addressW;
		public: property D3DTEXTUREADDRESS AddressW {D3DTEXTUREADDRESS get();}

		private: D3DCOLOR borderColor;
		public: property D3DCOLOR BorderColor {D3DCOLOR get();}
		#pragma endregion

		#pragma region Constructors
		public: SamplerStateDesc(SamplerStateTypes type);
		#pragma endregion
	};

	public ref class SamplerState : Disposable, SamplerStateI
	{
		#pragma region Properties
		private: Video^ video;
		private: SamplerStateDesc^ desc;
		#pragma endregion
	
		#pragma region Constructors
		public: SamplerState(DisposableI^ parent, SamplerStateDescI^ desc);
		public: ~SamplerState();
		#pragma endregion
		
		#pragma region Methods
		public: virtual void Enable(int index);
		#pragma endregion
	};
}
}
}