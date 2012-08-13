#pragma once
#include "Video.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class BlendStateDesc : BlendStateDescI
	{
		#pragma region Properties
		private: DWORD renderTargetWriteMask;
		public: property DWORD RenderTargetWriteMask {DWORD get();}

		private: bool blendEnable;
		public: property bool BlendEnable {bool get();}

		private: D3DBLENDOP blendOp;
		public: property D3DBLENDOP BlendOp {D3DBLENDOP get();}

		private: D3DBLEND srcBlend;
		public: property D3DBLEND SrcBlend {D3DBLEND get();}

		private: D3DBLEND destBlend;
		public: property D3DBLEND DestBlend {D3DBLEND get();}

		private: bool blendEnableAlpha;
		public: property bool BlendEnableAlpha {bool get();}

		private: D3DBLENDOP blendOpAlpha;
		public: property D3DBLENDOP BlendOpAlpha {D3DBLENDOP get();}

		private: D3DBLEND srcBlendAlpha;
		public: property D3DBLEND SrcBlendAlpha {D3DBLEND get();}

		private: D3DBLEND destBlendAlpha;
		public: property D3DBLEND DestBlendAlpha {D3DBLEND get();}
		#pragma endregion

		#pragma region Constructors
		public: BlendStateDesc(BlendStateTypes type);
		#pragma endregion
	};

	public ref class BlendState : Disposable, BlendStateI
	{
		#pragma region Fields
		private: Video^ video;
		private: BlendStateDesc^ desc;
		#pragma endregion
	
		#pragma region Constructors
		public: BlendState(DisposableI^ parent, BlendStateDescI^ desc);
		public: ~BlendState();
		#pragma endregion
		
		#pragma region Methods
		public: virtual void Enable();
		#pragma endregion
	};
}
}
}