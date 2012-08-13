#pragma once
#include "Video.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class RasterizerStateDesc : RasterizerStateDescI
	{
		#pragma region Properties
		private: D3DFILLMODE fillMode;
		public: property D3DFILLMODE FillMode {D3DFILLMODE get();}

		private: D3DCULL cullMode;
		public: property D3DCULL CullMode {D3DCULL get();}

		private: bool multisampleEnable;
		public: property bool MultisampleEnable {bool get();}
		#pragma endregion

		#pragma region Constructors
		public: RasterizerStateDesc(RasterizerStateTypes type);
		#pragma endregion
	};

	public ref class RasterizerState : Disposable, RasterizerStateI
	{
		#pragma region Fields
		private: Video^ video;
		private: RasterizerStateDesc^ desc;
		#pragma endregion

		#pragma region Constructors
		public: RasterizerState(DisposableI^ parent, RasterizerStateDescI^ desc);
		public: ~RasterizerState();
		#pragma endregion
		
		#pragma region Methods
		public: virtual void Enable();
		#pragma endregion
	};
}
}
}