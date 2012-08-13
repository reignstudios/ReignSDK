#include "pch.h"
#include "IndexBuffer.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	IndexBuffer::IndexBuffer(DisposableI^ parent, BufferUsages bufferUsage)
	: IndexBufferI(parent, bufferUsage)
	{
		init(parent, nullptr);
	}

	IndexBuffer::IndexBuffer(DisposableI^ parent, BufferUsages bufferUsage, array<int>^ indices)
	: IndexBufferI(parent, bufferUsage)
	{
		init(parent, indices);
	}

	IndexBuffer::IndexBuffer(DisposableI^ parent, BufferUsages bufferUsage, array<int>^ indices, bool _32BitIndices)
	: IndexBufferI(parent, bufferUsage, _32BitIndices)
	{
		init(parent, indices);
	}

	void IndexBuffer::init(DisposableI^ parent, array<int>^ indices)
	{
		null();
		try
		{
			video = parent->FindParentOrSelfWithException<Video^>();
			if (indices) Init(indices);
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	IndexBuffer::~IndexBuffer()
	{
		disposeChilderen();
		if (indexBuffer) indexBuffer->Release();
		null();
	}

	void IndexBuffer::null()
	{
		indexBuffer = 0;
		video = nullptr;
	}
	#pragma endregion

	#pragma region Methods
	void IndexBuffer::Init(array<int>^ indices)
	{
		IndexBufferI::Init(indices);
		if (indexBuffer)
		{
			indexBuffer->Release();
			indexBuffer = 0;
		}

		uint bufferSize = indexCount * indexByteSize;
		IDirect3DIndexBuffer9* indexBufferTEMP;
		if (FAILED(video->Device->CreateIndexBuffer(bufferSize, D3DUSAGE_WRITEONLY, _32BitIndices ? D3DFMT_INDEX32 : D3DFMT_INDEX16, video->IsExDevice ? D3DPOOL_DEFAULT : D3DPOOL_MANAGED, &indexBufferTEMP, 0)))
		{
			Debug::ThrowError(L"IndexBuffer", L"Failed to create IndexBuffer");
		}
		indexBuffer = indexBufferTEMP;
		Update(indices, indexCount);
	}

	void IndexBuffer::Update(array<int>^ indices, int updateCount)
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

	void IndexBuffer::enable()
	{
		video->Device->SetIndices(indexBuffer);
	}
	#pragma endregion
}
}
}