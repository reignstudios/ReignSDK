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

	ViewPort::ViewPort(VideoI^ video, Point location, Size2 size)
	: ViewPortI(location, size)
	{
		this->video = (Video^)video;
	}
	#pragma endregion

	#pragma region Methods
	void ViewPort::Apply(RenderTargetI^ renderTarget)
	{
		if (renderTarget == nullptr)
		{
			D3DVIEWPORT9 viewPort =
			{
				Location.X, video->lastWindowFrameSize.Height - Size.Height - Location.Y,
				Size.Width, Size.Height,
				0.0f, 1.0f
			};
			video->Device->SetViewport(&viewPort);
		}
		else
		{
			D3DVIEWPORT9 viewPort =
			{
				Location.X, renderTarget->Size.Height - Size.Height - Location.Y,
				Size.Width, Size.Height,
				0.0f, 1.0f
			};
			video->Device->SetViewport(&viewPort);
		}
	}
	#pragma endregion
}
}
}