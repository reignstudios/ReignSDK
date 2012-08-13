#include "pch.h"
#include "BufferLayout.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	BufferLayout::BufferLayout(DisposableI^ parent, ShaderI^ shader, BufferLayoutDescI^ bufferLayoutDesc)
	: Disposable(video)
	{
		null();
		try
		{
			video = parent->FindParentOrSelfWithException<Video^>();

			IDirect3DVertexDeclaration9* layoutTEMP;
			BufferLayoutDesc^ bufferLayoutDescTEMP = (BufferLayoutDesc^)bufferLayoutDesc;
			if (FAILED(video->Device->CreateVertexDeclaration(bufferLayoutDescTEMP->Desc, &layoutTEMP)))
			{
				layout = 0;
				Debug::ThrowError(L"BufferLayout", L"Failed to create VertexDeclaration");
			}
			layout = layoutTEMP;
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	BufferLayout::~BufferLayout()
	{
		disposeChilderen();
		if (layout) layout->Release();
		null();
	}

	void BufferLayout::null()
	{
		layout = 0;
		video = nullptr;
	}
	#pragma endregion
	
	#pragma region Methods
	void BufferLayout::Enable()
	{
		video->Device->SetVertexDeclaration(layout);
	}
	#pragma endregion
}
}
}