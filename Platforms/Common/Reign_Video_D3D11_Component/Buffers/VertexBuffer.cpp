#include "pch.h"
#include "VertexBuffer.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	VertexBufferCom::VertexBufferCom(VideoCom^ video, REIGN_D3D_PRIMITIVE_TOPOLOGY topology)
	{
		null();
		this->video = video;
		SetTopology(topology);
	}

	VertexBufferErrors VertexBufferCom::Init(const array<float>^ vertices, int vertexCount, int vertexByteSize, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage)
	{
		this->vertexCount = vertexCount;
		this->vertexByteSize = vertexByteSize;

		if (vertexBuffer)
		{
			vertexBuffer->Release();
			vertexBuffer = 0;
		}

		uint bufferSize = vertexCount * vertexByteSize;
		D3D11_BUFFER_DESC desc;
		ZeroMemory(&desc, sizeof(D3D11_BUFFER_DESC));
		desc.Usage = (D3D11_USAGE)usage;
		desc.CPUAccessFlags = (D3D11_CPU_ACCESS_FLAG)cpuUsage;
		desc.ByteWidth = bufferSize;
		desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
		desc.MiscFlags = 0;

		D3D11_SUBRESOURCE_DATA data;
		ZeroMemory(&data, sizeof(D3D11_SUBRESOURCE_DATA));
		PinPtr(float) verticesTEMP = GetDataPtr(vertices);
		data.pSysMem = verticesTEMP;
		ID3D11Buffer* vertexBufferTEMP;
		if (FAILED(video->device->CreateBuffer(&desc, &data, &vertexBufferTEMP)))
		{
			return VertexBufferErrors::Buffer;
		}
		vertexBuffer = vertexBufferTEMP;

		return VertexBufferErrors::None;
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
	void VertexBufferCom::SetTopology(REIGN_D3D_PRIMITIVE_TOPOLOGY topology)
	{
		primitiveTopology = (D3D_PRIMITIVE_TOPOLOGY)topology;
	}

	void VertexBufferCom::Update(const array<float>^ vertices, int updateCount)
	{
		D3D11_MAPPED_SUBRESOURCE source;
		video->deviceContext->Map(vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, NULL, &source);
		PinPtr(float) verticesTEMP = GetDataPtr(vertices);
		memcpy(source.pData, verticesTEMP, vertexByteSize * updateCount);
		video->deviceContext->Unmap(vertexBuffer, 0);
	}

	void VertexBufferCom::enable(IndexBufferCom^ indexBuffer, VertexBufferCom^ instanceBuffer)
	{
		video->deviceContext->IASetPrimitiveTopology(primitiveTopology);

		if (instanceBuffer)
		{
			uint offset[2] = {0, 0};
			uint byteSizeTEMP[2] = {vertexByteSize, instanceBuffer->vertexByteSize};
			ID3D11Buffer* bufferTEMP[2] = {vertexBuffer, instanceBuffer->vertexBuffer};
			video->deviceContext->IASetVertexBuffers(0, 2, &*bufferTEMP, byteSizeTEMP, offset);
		}
		else
		{
			const uint offset = 0, byteSizeTEMP = vertexByteSize;
			ID3D11Buffer* bufferTEMP = vertexBuffer;
			video->deviceContext->IASetVertexBuffers(0, 1, &bufferTEMP, &byteSizeTEMP, &offset);
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

	void VertexBufferCom::Enable()
	{
		enable(nullptr, nullptr);
	}

	void VertexBufferCom::Enable(IndexBufferCom^ indexBuffer)
	{
		enable(indexBuffer, nullptr);
	}

	void VertexBufferCom::Enable(VertexBufferCom^ instanceBuffer)
	{
		enable(nullptr, instanceBuffer);
	}

	void VertexBufferCom::Enable(IndexBufferCom^ indexBuffer, VertexBufferCom^ instanceBuffer)
	{
		enable(indexBuffer, instanceBuffer);
	}

	void VertexBufferCom::Draw()
	{
		if (indexBuffer == nullptr) video->deviceContext->Draw(vertexCount, 0);
		else video->deviceContext->DrawIndexed(indexBuffer->indexCount, 0, 0);
	}

	void VertexBufferCom::Draw(int drawCount)
	{
		if (indexBuffer == nullptr) video->deviceContext->Draw(drawCount, 0);
		else video->deviceContext->DrawIndexed(drawCount, 0, 0);
	}

	void VertexBufferCom::DrawInstanced(int drawCount)
	{
		if (indexBuffer == nullptr) video->deviceContext->DrawInstanced(vertexCount, drawCount, 0, 0);
		else video->deviceContext->DrawIndexedInstanced(indexBuffer->indexCount, drawCount, 0, 0, 0);
	}

	void VertexBufferCom::DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
	{
		if (indexBuffer == nullptr) video->deviceContext->Draw(drawCount * meshVertexCount, 0);
		else video->deviceContext->DrawIndexed(drawCount * meshIndexCount, 0, 0);
	}
	#pragma endregion
}