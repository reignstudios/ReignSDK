#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class SamplerStateError
	{
		None,
		SampleState
	};

	public enum class REIGN_D3D11_FILTER
	{
		MIN_MAG_MIP_POINT = 0,
		MIN_MAG_MIP_LINEAR = 0x15
	};

	public enum class REIGN_D3D11_TEXTURE_ADDRESS_MODE
	{
		WRAP = 1,
		CLAMP = 3,
		BORDER = 4
	};

	public ref class SamplerStateDescCom sealed
	{
		#pragma region Properties
		internal: D3D11_SAMPLER_DESC* desc;
		#pragma endregion

		#pragma region Constructors
		public: SamplerStateDescCom(REIGN_D3D11_FILTER filter, REIGN_D3D11_TEXTURE_ADDRESS_MODE address);
		public: virtual ~SamplerStateDescCom();
		private: void null();
		#pragma endregion
	};

	public ref class SamplerStateCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11SamplerState* state;
		#pragma endregion

		#pragma region Constructors
		public: SamplerStateError Init(VideoCom^ video, SamplerStateDescCom^ desc);
		public: virtual ~SamplerStateCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Enable(int index);
		#pragma endregion
	};
}