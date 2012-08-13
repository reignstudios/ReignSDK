#include "pch.h"
#include "IndexBuffer.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	IndexBufferCom::IndexBufferCom(VideoCom^ video)
	{
		null();
		this->video = video;
	}

	IndexBufferErrors IndexBufferCom::Init(const array<int>^ indices, int indexCount, int indexByteSize, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage, bool _32BitIndices)
	{
		this->indexCount = indexCount;

		if (indexBuffer)
		{
			indexBuffer->Release();
			indexBuffer = 0;
		}

		uint bufferSize = indexCount * indexByteSize;
		D3D11_BUFFER_DESC desc;
		ZeroMemory(&desc, sizeof(D3D11_BUFFER_DESC));
		desc.Usage = (D3D11_USAGE)usage;
		desc.CPUAccessFlags = (D3D11_CPU_ACCESS_FLAG)cpuUsage;
		desc.ByteWidth = bufferSize;
		desc.BindFlags = D3D11_BIND_INDEX_BUFFER;
		desc.MiscFlags = 0;

		D3D11_SUBRESOURCE_DATA data;
		ZeroMemory(&data, sizeof(D3D11_SUBRESOURCE_DATA));
		if (_32BitIndices)
		{
			PinPtr(int) indicesTEMP = GetDataPtr(indices);
			data.pSysMem = indicesTEMP;
			format = DXGI_FORMAT_R32_UINT;
		}
		else
		{
			short* _16BitIndices = new short[indices->Length];
			for (int i = 0; i != indices->Length; ++i)
			{
				_16BitIndices[i] = indices[i];
			}
			data.pSysMem = _16BitIndices;
			format = DXGI_FORMAT_R16_UINT;
		}

		ID3D11Buffer* indexBufferTEMP;
		if (FAILED(video->device->CreateBuffer(&desc, &data, &indexBufferTEMP)))
		{
			if (data.pSysMem) delete data.pSysMem;
			return IndexBufferErrors::Buffer;
		}
		indexBuffer = indexBufferTEMP;

		if (data.pSysMem) delete data.pSysMem;

		return IndexBufferErrors::None;
	}

	IndexBufferCom::~IndexBufferCom()
	{
		if (indexBuffer) indexBuffer->Release();
		null();
	}

	void IndexBufferCom::null()
	{
		indexBuffer = 0;
	}
	#pragma endregion
	
	#pragma region Methods
	void IndexBufferCom::Update(const array<int>^ indices, int updateCount)
	{
		/*void* data = 0;
		indexBuffer->Map(D3D11_MAP_WRITE_DISCARD, 0, &data);

		if (_32BitIndices)
		{
			pin_ptr<int> indicesTEMP = &indices[0];
			memcpy(data, indicesTEMP, indexByteSize * updateCount);
		}
		else
		{
			short* _16BitIndices = new short[updateCount];
			for (int i = 0; i != updateCount; ++i)
			{
				_16BitIndices[i] = indices[i];
			}
			memcpy(data, _16BitIndices, indexByteSize * updateCount);
		}

		indexBuffer->Unmap();*/
	}

	void IndexBufferCom::enable()
	{
		video->deviceContext->IASetIndexBuffer(indexBuffer, format, 0);
	}
	#pragma endregion
}