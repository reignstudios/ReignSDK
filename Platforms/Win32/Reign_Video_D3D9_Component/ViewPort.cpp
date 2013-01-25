#include "pch.h"
#include "ViewPort.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	ViewPortCom::ViewPortCom(VideoCom^ video)
	{
		this->video = video;
	}
	#pragma endregion

	#pragma region Methods
	void ViewPortCom::Apply(int x, int y, int width, int height)
	{
		D3DVIEWPORT9 viewPort =
		{
			x, y,
			width, height,
			0.0f, 1.0f
		};
		video->device->SetViewport(&viewPort);
	}
	#pragma endregion
}