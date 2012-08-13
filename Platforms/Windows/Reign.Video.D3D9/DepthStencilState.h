#pragma once
#include "Video.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class DepthStencilStateDesc : DepthStencilStateDescI
	{
		#pragma region Properties
		private: D3DZBUFFERTYPE depthReadEnable;
		public: property D3DZBUFFERTYPE DepthReadEnable {D3DZBUFFERTYPE get();}

		private: bool depthWriteEnable;
		public: property bool DepthWriteEnable {bool get();}

		private: D3DCMPFUNC depthFunc;
		public: property D3DCMPFUNC DepthFunc {D3DCMPFUNC get();}

		private: bool stencilEnable;
		public: property bool StencilEnable {bool get();}

		private: D3DCMPFUNC stencilFunc;
		public: property D3DCMPFUNC StencilFunc {D3DCMPFUNC get();}

		private: D3DSTENCILOP stencilFailOp;
		public: property D3DSTENCILOP StencilFailOp {D3DSTENCILOP get();}

		private: D3DSTENCILOP stencilDepthFailOp;
		public: property D3DSTENCILOP StencilDepthFailOp {D3DSTENCILOP get();}

		private: D3DSTENCILOP stencilPassOp;
		public: property D3DSTENCILOP StencilPassOp {D3DSTENCILOP get();}
		#pragma endregion

		#pragma region Constructors
		public: DepthStencilStateDesc(DepthStencilStateTypes type);
		#pragma endregion
	};

	public ref class DepthStencilState : Disposable, DepthStencilStateI
	{
		#pragma region Fields
		private: Video^ video;
		private: DepthStencilStateDesc^ desc;
		#pragma endregion
	
		#pragma region Constructors
		public: DepthStencilState(DisposableI^ parent, DepthStencilStateDescI^ desc);
		public: ~DepthStencilState();
		#pragma endregion
		
		#pragma region Methods
		public: virtual void Enable();
		#pragma endregion
	};
}
}
}