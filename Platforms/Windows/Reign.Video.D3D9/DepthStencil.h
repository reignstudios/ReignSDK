#pragma once
#include "Video.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class DepthStencil : Disposable, DepthStencilI
	{
		#pragma region Properties
		private: Video^ video;

		private: int width, height;
		private: DepthStenicFormats depthStenicFormats;

		private: IDirect3DSurface9* surface;
		public: property IDirect3DSurface9* Surface {IDirect3DSurface9* get();}
		#pragma endregion

		#pragma region Contructors
		public: DepthStencil(DisposableI^ parent, int width, int height, DepthStenicFormats depthStenicFormats);
		private: void init(DisposableI^ parent, int width, int height, DepthStenicFormats depthStenicFormats);
		public: ~DepthStencil();
		private: void dispose();
		private: void null();
		private: void deviceLost();
		private: void deviceReset();
		#pragma endregion

		#pragma region Methods
		internal: void enable();
		#pragma endregion
	};
}
}
}