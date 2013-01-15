#include "pch.h"
#include "IndexBuffer.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	IndexBufferCom::IndexBufferCom(VideoCom^ video)
	{
		null();
		this->video = video;
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
	IndexBufferErrors IndexBufferCom::Init(array<int>^ indices, int indexCount, int indexByteSize, REIGN_D3DUSAGE usage, bool _32BitIndices)
	{
		if (indexBuffer)
		{
			indexBuffer->Release();
			indexBuffer = 0;
		}

		uint bufferSize = indexCount * indexByteSize;
		IDirect3DIndexBuffer9* indexBufferTEMP;
		if (FAILED(video->device->CreateIndexBuffer(bufferSize, D3DUSAGE_WRITEONLY, _32BitIndices ? D3DFMT_INDEX32 : D3DFMT_INDEX16, video->caps->ExDevice ? D3DPOOL_DEFAULT : D3DPOOL_MANAGED, &indexBufferTEMP, 0)))
		{
			return IndexBufferErrors::IndexBuffer;
		}
		indexBuffer = indexBufferTEMP;
		Update(indices, indexCount, indexByteSize, _32BitIndices);

		return IndexBufferErrors::None;
	}

	void IndexBufferCom::Update(array<int>^ indices, int updateCount, int indexByteSize, bool _32BitIndices)
	{
		void *data = 0;
		uint bufferSize = indexByteSize * updateCount;
		indexBuffer->Lock(0, bufferSize, (void**)&data, 0);

		void* indicesPtr = 0;
		short* _16BitIndices = 0;
		if (_32BitIndices)
		{
			pin_ptr<int>indicesTEMP = &indices[0];
			indicesPtr = indicesTEMP;
		}
		else
		{
			_16BitIndices = new short[updateCount];
			for (int i = 0; i != updateCount; ++i)
			{
				_16BitIndices[i] = indices[i];
			}
			indicesPtr = _16BitIndices;
		}
		
		memcpy(data, indicesPtr, bufferSize);
		indexBuffer->Unlock();
		if (_16BitIndices) delete _16BitIndices;
	}

	void IndexBufferCom::enable()
	{
		video->device->SetIndices(indexBuffer);
	}
	#pragma endregion
}