#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class DepthStencilErrors
	{
		None,
		Textrue,
		DepthStencilView
	};

	public ref class DepthStencilCom sealed
	{
		#pragma region Properties
		private: ID3D11Texture2D* texture;
		internal: ID3D11DepthStencilView* surface;
		#pragma endregion
		
		#pragma region Constructors
		public: DepthStencilErrors Init(VideoCom^ video, int width, int height);
		public: ~DepthStencilCom();
		private: void null();
		#pragma endregion
	};
}