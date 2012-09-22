#include "pch.h"
#include "Mesh.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	Mesh::Mesh(BinaryReader^ reader, ModelI^ model)
	: MeshI(reader, model)
	{
		
	}

	BufferLayoutDescI^ Mesh::createBufferLayoutDesc(List<BufferLayoutElement>^ elements)
	{
		return gcnew BufferLayoutDesc(elements);
	}

	VertexBufferI^ Mesh::createVertexBuffer(DisposableI^ parent, BufferLayoutDescI^ layoutDesc, BufferUsages usage, VertexBufferTopologys topology, array<float>^ vertices)
	{
		return gcnew VertexBuffer(parent, layoutDesc, usage, topology, vertices);
	}

	IndexBufferI^ Mesh::createIndexBuffer(DisposableI^ parent, BufferUsages usage, array<int>^ indices, bool _32BitIndices)
	{
		return gcnew IndexBuffer(parent, usage, indices, _32BitIndices);
	}
	#pragma endregion
}
}
}