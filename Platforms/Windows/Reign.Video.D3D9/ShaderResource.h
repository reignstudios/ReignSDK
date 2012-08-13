#pragma once
#include "Video.h"
#include "Texture2D.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class ShaderResource : ShaderResourceI
	{
		#pragma region Properties
		internal: delegate void ApplyFunc();
		internal: ApplyFunc^ Apply;
		private: IDirect3DTexture9* resource;

		private: Video^ video;
		private: int vertexIndex, pixelIndex;

		private: string^ name;
		public: property string^ Name {string^ get();}
		#pragma endregion

		#pragma region Constructors
		public: ShaderResource(VideoI^ video, int vertexIndex, int pixelIndex, string^ name);
		#pragma endregion

		#pragma region Methods
		private: void setNothing();
		private: void setTexture2D();
		public: virtual void Set(Texture2DI^ resource);
		public: virtual void Set(Texture3DI^ resource);
		#pragma endregion
	};
}
}
}