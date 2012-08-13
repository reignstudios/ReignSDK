#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class DepthStencilStateError
	{
		None,
		DepthStencil
	};

	public enum class REIGN_D3D11_DEPTH_WRITE_MASK
	{
		ZERO = 0,
		ALL = 1
	};

	public enum class REIGN_D3D11_COMPARISON_FUNC
	{
		ALWAYS = 8,
		LESS = 2
	};

	public ref class DepthStencilStateDescCom sealed
	{
		#pragma region Properties
		internal: D3D11_DEPTH_STENCIL_DESC* desc;
		#pragma endregion

		#pragma region Constructors
		public: DepthStencilStateDescCom(bool enable, REIGN_D3D11_DEPTH_WRITE_MASK mask, REIGN_D3D11_COMPARISON_FUNC func);
		public: ~DepthStencilStateDescCom();
		private: void null();
		#pragma endregion
	};

	public ref class DepthStencilStateCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11DepthStencilState* state;
		#pragma endregion

		#pragma region Constructors
		public: DepthStencilStateError Init(VideoCom^ video, DepthStencilStateDescCom^ desc);
		public: ~DepthStencilStateCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}