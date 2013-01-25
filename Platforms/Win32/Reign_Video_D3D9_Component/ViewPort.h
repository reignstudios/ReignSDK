#pragma once
#include "Video.h"
#include "RenderTarget.h"

namespace Reign_Video_D3D9_Component
{
	public ref class ViewPortCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		#pragma endregion

		#pragma region Constructors
		public: ViewPortCom(VideoCom^ video);
		#pragma endregion

		#pragma region Methods
		public: void Apply(int x, int y, int width, int height);
		#pragma endregion
	};
}