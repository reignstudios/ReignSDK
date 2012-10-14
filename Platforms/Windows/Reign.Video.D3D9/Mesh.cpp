#include "pch.h"
#include "Mesh.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	Mesh::Mesh(BinaryReader^ reader, ModelI^ model, int classicInstanceCount)
	: MeshI(reader, model, classicInstanceCount)
	{
		
	}

	BufferLayoutDescI^ Mesh::createBufferLayoutDesc(List<BufferLayoutElement>^ elements)
	{
		return gcnew BufferLayoutDesc(elements);
	}

	VertexBufferI^ Mesh::createVertexBuffer(DisposableI^ parent, BufferLayoutDescI^ layoutDesc, BufferUsages usage, VertexBufferTopologys topology, array<float>^ vertices)
	{
		return gcnew Reign::Video::D3D9::VertexBuffer(parent, layoutDesc, usage, topology, vertices);
	}

	IndexBufferI^ Mesh::createIndexBuffer(DisposableI^ parent, BufferUsages usage, array<int>^ indices, bool _32BitIndices)
	{
		return gcnew Reign::Video::D3D9::IndexBuffer(parent, usage, indices, _32BitIndices);
	}
	#pragma endregion
}
}
}