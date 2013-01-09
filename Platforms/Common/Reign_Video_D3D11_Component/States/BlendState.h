#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class BlendStateError
	{
		None,
		BlendState
	};

	public enum class REIGN_D3D11_BLEND_OP
	{
		ADD = 1,
		SUBTRACT = 2,
		REV_SUBTRACT = 3
	};

	public enum class REIGN_D3D11_BLEND
	{
		ONE = 2,
		SRC_ALPHA = 5,
		INV_SRC_ALPHA = 6
	};

	public ref class BlendStateDescCom sealed
	{
		#pragma region Properties
		internal: D3D11_BLEND_DESC* desc;
		#pragma endregion

		#pragma region Constructors
		public: BlendStateDescCom(bool enable, REIGN_D3D11_BLEND_OP operation, REIGN_D3D11_BLEND blend, REIGN_D3D11_BLEND dstBlend);
		public: ~BlendStateDescCom();
		private: void null();
		#pragma endregion
	};

	public ref class BlendStateCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11BlendState* state;
		#pragma endregion

		#pragma region Constructors
		public: BlendStateError Init(VideoCom^ video, BlendStateDescCom^ desc);
		public: ~BlendStateCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}