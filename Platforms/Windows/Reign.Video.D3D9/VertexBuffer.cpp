#include "pch.h"
#include "VertexBuffer.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	VertexBufferTopologys VertexBuffer::Topology::get() {return topology;}
	void VertexBuffer::Topology::set(VertexBufferTopologys value)
	{
		switch (value)
		{
			case (VertexBufferTopologys::None): primitiveTopology = D3DPT_POINTLIST; primitiveVertexCount = 0; break;
			case (VertexBufferTopologys::Point): primitiveTopology = D3DPT_POINTLIST; primitiveVertexCount = 1; break;
			case (VertexBufferTopologys::Line): primitiveTopology = D3DPT_LINELIST; primitiveVertexCount = 2; break;
			case (VertexBufferTopologys::Triangle): primitiveTopology = D3DPT_TRIANGLELIST; primitiveVertexCount = 3; break;
		}
		topology = value;
	}
	#pragma endregion

	#pragma region Constructors
	VertexBuffer::VertexBuffer(DisposableI^ parent, BufferLayoutDescI^ bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology)
	: VertexBufferI(parent, bufferLayoutDesc, bufferUsage)
	{
		init(parent, vertexBufferTopology, nullptr);
	}

	VertexBuffer::VertexBuffer(DisposableI^ parent, BufferLayoutDescI^ bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, array<float>^ vertices)
	: VertexBufferI(parent, bufferLayoutDesc, bufferUsage)
	{
		init(parent, vertexBufferTopology, vertices);
	}

	void VertexBuffer::init(DisposableI^ parent, VertexBufferTopologys vertexBufferTopology, array<float>^ vertices)
	{
		null();
		try
		{
			video = parent->FindParentOrSelfWithException<Video^>();
			Topology = vertexBufferTopology;
			
			if (vertices) Init(vertices);
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	VertexBuffer::~VertexBuffer()
	{
		disposeChilderen();
		if (vertexBuffer) vertexBuffer->Release();
		null();
	}

	void VertexBuffer::null()
	{
		vertexBuffer = 0;
		video = nullptr;
	}
	#pragma endregion

	#pragma region Methods
	void VertexBuffer::Init(array<float>^ vertices)
	{
		VertexBufferI::Init(vertices);
		if (vertexBuffer)
		{
			vertexBuffer->Release();
			vertexBuffer = 0;
		}

		uint bufferSize = vertexCount * vertexByteSize;
		IDirect3DVertexBuffer9* vertexBufferTEMP;
		if (FAILED(video->Device->CreateVertexBuffer(bufferSize, D3DUSAGE_WRITEONLY, 0, video->IsExDevice ? D3DPOOL_DEFAULT : D3DPOOL_MANAGED, &vertexBufferTEMP, 0)))
		{
			Debug::ThrowError(L"VertexBuffer", L"Failed to create VertexBuffer");
		}
		vertexBuffer = vertexBufferTEMP;
		Update(vertices, vertexCount);
	}

	void VertexBuffer::Update(array<float>^ vertices, int updateCount)
	{
		void *data = 0;
		uint bufferSize = vertexByteSize * updateCount;
		vertexBuffer->Lock(0, bufferSize, (void**)&data, 0);
		pin_ptr<float>verticesTEMP = &vertices[0];
		memcpy(data, verticesTEMP, bufferSize);
		vertexBuffer->Unlock();
	}

	void VertexBuffer::enable(IndexBufferI^ indexBuffer, VertexBufferI^ instanceBuffer)
	{
		video->Device->SetStreamSource(0, vertexBuffer, 0, vertexByteSize);

		if (instanceBuffer)
		{
			VertexBuffer^ ib = (VertexBuffer^)instanceBuffer;
			video->Device->SetStreamSourceFreq(1, D3DSTREAMSOURCE_INSTANCEDATA | 1);
			video->Device->SetStreamSource(1, ib->vertexBuffer, 0, ib->vertexByteSize);
		}
		else
		{
			video->Device->SetStreamSourceFreq(0, 1);
			video->Device->SetStreamSourceFreq(1, 1);
		}

		if (indexBuffer)
		{
			this->indexBuffer = (IndexBuffer^)indexBuffer;
			this->indexBuffer->enable();
		}
		else
		{
			this->indexBuffer = nullptr;
		}
	}

	void VertexBuffer::Enable()
	{
		enable(nullptr, nullptr);
	}

	void VertexBuffer::Enable(IndexBufferI^ indexBuffer)
	{
		enable(indexBuffer, nullptr);
	}

	void VertexBuffer::Enable(VertexBufferI^ instanceBuffer)
	{
		enable(nullptr, instanceBuffer);
	}

	void VertexBuffer::Enable(IndexBufferI^ indexBuffer, VertexBufferI^ instanceBuffer)
	{
		enable(indexBuffer, instanceBuffer);
	}

	void VertexBuffer::Draw()
	{
		if (indexBuffer == nullptr) video->Device->DrawPrimitive(primitiveTopology, 0, (vertexCount/primitiveVertexCount));
		else video->Device->DrawIndexedPrimitive(primitiveTopology, 0, 0, vertexCount, 0, (indexBuffer->IndexCount/primitiveVertexCount));
	}

	void VertexBuffer::Draw(int drawCount)
	{
		if (indexBuffer == nullptr) video->Device->DrawPrimitive(primitiveTopology, 0, (drawCount/primitiveVertexCount));
		else video->Device->DrawIndexedPrimitive(primitiveTopology, 0, 0, vertexCount, 0, (drawCount/primitiveVertexCount));
	}

	void VertexBuffer::DrawInstanced(int drawCount)
	{
		video->Device->SetStreamSourceFreq(0, D3DSTREAMSOURCE_INDEXEDDATA | drawCount);
		Draw();
	}

	void VertexBuffer::DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
	{
		if (indexBuffer == nullptr) video->Device->DrawPrimitive(primitiveTopology, 0, drawCount * (meshVertexCount/primitiveVertexCount));
		else video->Device->DrawIndexedPrimitive(primitiveTopology, 0, 0, drawCount * meshVertexCount, 0, drawCount * (meshIndexCount/primitiveVertexCount));
	}
	#pragma endregion
}
}
}