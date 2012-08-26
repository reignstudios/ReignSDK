#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class RasterizerStateError
	{
		None,
		RasterizerState
	};

	public enum class REIGN_D3D11_FILL_MODE
	{
		SOLID = 3,
		WIREFRAME = 2
	};

	public enum class REIGN_D3D11_CULL_NONE
	{
		NONE = 1,
		FRONT = 2,
		BACK = 3
	};

	public ref class RasterizerStateDescCom sealed
	{
		#pragma region Properties
		internal: D3D11_RASTERIZER_DESC* desc;
		#pragma endregion

		#pragma region Constructors
		public: RasterizerStateDescCom(REIGN_D3D11_FILL_MODE fillMode, REIGN_D3D11_CULL_NONE cullMode, bool frontCounterClockwise, int depthBias, float depthBiasClamp, float slopeScaledDepthBias, bool depthClipEnable, bool scissorEnable, bool multisampleEnable, bool antialiasedLineEnable);
		public: virtual ~RasterizerStateDescCom();
		private: void null();
		#pragma endregion
	};

	public ref class RasterizerStateCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11RasterizerState* state;
		#pragma endregion

		#pragma region Constructors
		public: RasterizerStateError Init(VideoCom^ video, RasterizerStateDescCom^ desc);
		public: virtual ~RasterizerStateCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}