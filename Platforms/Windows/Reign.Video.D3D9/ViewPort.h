#pragma once
#include "Video.h"
#include "RenderTarget.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class ViewPort : ViewPortI
	{
		#pragma region Properties
		private: Video^ video;
		#pragma endregion

		#pragma region Constructors
		public: ViewPort(VideoI^ video);
		public: ViewPort(VideoI^ video, int x, int y, int width, int height);
		public: ViewPort(VideoI^ video, Point2 location, Size2 size);
		#pragma endregion

		#pragma region Methods
		public: virtual void Apply() override;
		public: virtual void Apply(RenderTargetI^ renderTarget) override;
		#pragma endregion
	};
}
}
}