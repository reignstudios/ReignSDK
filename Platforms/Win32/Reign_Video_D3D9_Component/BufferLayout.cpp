#include "pch.h"
#include "BufferLayout.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	BufferLayoutErrors BufferLayoutCom::Init(VideoCom^ video, BufferLayoutDescCom^ bufferLayoutDesc)
	{
		null();
		this->video = video;

		IDirect3DVertexDeclaration9* layoutTEMP;
		if (FAILED(video->device->CreateVertexDeclaration(bufferLayoutDesc->desc, &layoutTEMP)))
		{
			return BufferLayoutErrors::VertexDeclaration;
		}
		layout = layoutTEMP;

		return BufferLayoutErrors::None;
	}

	BufferLayoutCom::~BufferLayoutCom()
	{
		if (layout) layout->Release();
		null();
	}

	void BufferLayoutCom::null()
	{
		layout = 0;
	}
	#pragma endregion

	#pragma region Methods
	void BufferLayoutCom::Enable()
	{
		video->device->SetVertexDeclaration(layout);
	}
	#pragma endregion
}