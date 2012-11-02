#include "pch.h"
#include "ViewPort.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	ViewPort::ViewPort(VideoI^ video)
	{
		this->video = (Video^)video;
	}

	ViewPort::ViewPort(VideoI^ video, int x, int y, int width, int height)
	: ViewPortI(x, y, width, height)
	{
		this->video = (Video^)video;
	}

	ViewPort::ViewPort(VideoI^ video, Point2 location, Size2 size)
	: ViewPortI(location, size)
	{
		this->video = (Video^)video;
	}
	#pragma endregion

	#pragma region Methods
	void ViewPort::Apply()
	{
		D3DVIEWPORT9 viewPort =
		{
			Location.X, video->BackBufferSize.Height - Size.Height - Location.Y,
			Size.Width, Size.Height,
			0.0f, 1.0f
		};
		video->Device->SetViewport(&viewPort);
	}

	void ViewPort::Apply(RenderTargetI^ renderTarget)
	{
		D3DVIEWPORT9 viewPort =
		{
			Location.X, renderTarget->Size.Height - Size.Height - Location.Y,
			Size.Width, Size.Height,
			0.0f, 1.0f
		};
		video->Device->SetViewport(&viewPort);
	}
	#pragma endregion
}
}
}