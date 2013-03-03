#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class DepthStencilErrors
	{
		None,
		DepthStencilSurface
	};

	public ref class DepthStencilCom sealed
	{
		#pragma region Properties
		internal: IDirect3DSurface9* surface;
		#pragma endregion
		
		#pragma region Constructors
		public: DepthStencilErrors Init(VideoCom^ video, int width, int height, int depthBit, int stencilBit);
		public: virtual ~DepthStencilCom();
		private: void null();
		#pragma endregion
	};
}