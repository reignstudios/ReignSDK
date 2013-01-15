#include "pch.h"
#include "VertexBuffer.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	VertexBufferCom::VertexBufferCom(VideoCom^ video, REIGN_D3DPRIMITIVETYPE topology)
	{
		this->video = video;
		SetTopology(topology);
	}

	VertexBufferCom::~VertexBufferCom()
	{
		if (vertexBuffer) vertexBuffer->Release();
		null();
	}

	void VertexBufferCom::null()
	{
		vertexBuffer = 0;
	}
	#pragma endregion

	#pragma region Methods
	void VertexBufferCom::SetTopology(REIGN_D3DPRIMITIVETYPE topology)
	{
		this->topology = (D3DPRIMITIVETYPE)topology;
		switch (topology)
		{
			case (REIGN_D3DPRIMITIVETYPE::TRIANGLELIST): primitiveVertexCount = 3; break;
			case (REIGN_D3DPRIMITIVETYPE::LINELIST): primitiveVertexCount = 2; break;
			case (REIGN_D3DPRIMITIVETYPE::POINTLIST): primitiveVertexCount = 1; break;
		}
	}

	VertexBufferErrors VertexBufferCom::Init(array<float>^ vertices, REIGN_D3DUSAGE usage, int vertexCount, int vertexByteSize)
	{
		if (vertexBuffer)
		{
			vertexBuffer->Release();
			vertexBuffer = 0;
		}

		uint bufferSize = vertexCount * vertexByteSize;
		IDirect3DVertexBuffer9* vertexBufferTEMP;
		if (FAILED(video->device->CreateVertexBuffer(bufferSize, D3DUSAGE_WRITEONLY, 0, video->caps->ExDevice ? D3DPOOL_DEFAULT : D3DPOOL_MANAGED, &vertexBufferTEMP, 0)))
		{
			return VertexBufferErrors::VertexBuffer;
		}
		vertexBuffer = vertexBufferTEMP;
		Update(vertices, vertexCount, vertexByteSize);

		return VertexBufferErrors::None;
	}

	void VertexBufferCom::Update(array<float>^ vertices, int updateCount, int vertexByteSize)
	{
		void *data = 0;
		uint bufferSize = vertexByteSize * updateCount;
		vertexBuffer->Lock(0, bufferSize, (void**)&data, 0);
		pin_ptr<float>verticesTEMP = &vertices[0];
		memcpy(data, verticesTEMP, bufferSize);
		vertexBuffer->Unlock();
	}

	void VertexBufferCom::Enable(IndexBufferCom^ indexBuffer, VertexBufferCom^ instanceBuffer, int vertexByteSize, int instanceVertexByteSize)
	{
		video->device->SetStreamSource(0, vertexBuffer, 0, vertexByteSize);

		if (instanceBuffer)
		{
			video->device->SetStreamSourceFreq(1, D3DSTREAMSOURCE_INSTANCEDATA | 1);
			video->device->SetStreamSource(1, instanceBuffer->vertexBuffer, 0, instanceVertexByteSize);
		}
		else
		{
			video->device->SetStreamSourceFreq(0, 1);
			video->device->SetStreamSourceFreq(1, 1);
		}

		if (indexBuffer)
		{
			this->indexBuffer = indexBuffer;
			this->indexBuffer->enable();
		}
		else
		{
			this->indexBuffer = nullptr;
		}
	}

	void VertexBufferCom::Draw(int drawCount, int vertexCount, int indexCount)
	{
		if (indexBuffer == nullptr) video->device->DrawPrimitive(topology, 0, (vertexCount/primitiveVertexCount));
		else video->device->DrawIndexedPrimitive(topology, 0, 0, vertexCount, 0, (indexCount/primitiveVertexCount));
	}

	void VertexBufferCom::Draw(int drawCount, int vertexCount)
	{
		if (indexBuffer == nullptr) video->device->DrawPrimitive(topology, 0, (drawCount/primitiveVertexCount));
		else video->device->DrawIndexedPrimitive(topology, 0, 0, vertexCount, 0, (drawCount/primitiveVertexCount));
	}

	void VertexBufferCom::DrawInstanced(int drawCount, int vertexCount)
	{
		video->device->SetStreamSourceFreq(0, D3DSTREAMSOURCE_INDEXEDDATA | drawCount);
		Draw(drawCount, vertexCount);
	}

	void VertexBufferCom::DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
	{
		if (indexBuffer == nullptr) video->device->DrawPrimitive(topology, 0, drawCount * (meshVertexCount/primitiveVertexCount));
		else video->device->DrawIndexedPrimitive(topology, 0, 0, drawCount * meshVertexCount, 0, drawCount * (meshIndexCount/primitiveVertexCount));
	}
	#pragma endregion
}