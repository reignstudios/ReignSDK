#include "pch.h"
#include "BufferLayout.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	BufferLayoutErrors BufferLayoutCom::Init(VideoCom^ video, ShaderModelCom^ vertexShader, BufferLayoutDescCom^ desc)
	{
		null();
		this->video = video;

		ID3D11InputLayout* layoutTEMP;
		#if WINDOWS
		if (FAILED(video->device->CreateInputLayout(desc->desc, desc->elementCount, vertexShader->code->GetBufferPointer(), vertexShader->code->GetBufferSize(), &layoutTEMP)))
		#else
		if (FAILED(video->device->CreateInputLayout(desc->desc, desc->elementCount, vertexShader->code, vertexShader->codeSize, &layoutTEMP)))
		#endif
		{
			return BufferLayoutErrors::InputLayout;
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
		video->deviceContext->IASetInputLayout(layout);
	}
	#pragma endregion
}