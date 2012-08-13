#include "pch.h"
#include "ViewPort.h"

namespace Reign_Video_D3D11_Component
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
		D3D11_VIEWPORT viewPort =
		{
		    x, y,
		    width, height,
		    0.0f, 1.0f
		};

		video->deviceContext->RSSetViewports(1, &viewPort);
	}
	#pragma endregion
}