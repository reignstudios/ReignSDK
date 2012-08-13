#include "pch.h"
#include "QuickDraw.h"
#include <d3dx9.h>

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Constructors
	QuickDraw::QuickDraw(DisposableI^ parent, BufferLayoutDescI^ bufferLayoutDesc)
	: QuickDrawI(parent, bufferLayoutDesc)
	{
		try
		{
			vertexBuffer = gcnew VertexBuffer(this, bufferLayoutDesc, BufferUsages::Write, VertexBufferTopologys::Triangle, vertices);
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}
	#pragma endregion
	
	#pragma region Methods
	void QuickDraw::StartTriangles()
	{
		QuickDrawI::StartTriangles();
		vertexBuffer->Topology = VertexBufferTopologys::Triangle;
		vertexBuffer->Enable();
	}

	void QuickDraw::StartLines()
	{
		QuickDrawI::StartLines();
		vertexBuffer->Topology = VertexBufferTopologys::Line;
		vertexBuffer->Enable();
	}

	void QuickDraw::StartPoints()
	{
		QuickDrawI::StartPoints();
		vertexBuffer->Topology = VertexBufferTopologys::Point;
		vertexBuffer->Enable();
	}
	
	void QuickDraw::Draw()
	{
		vertexBuffer->Update(vertices, vertexNext);
		vertexBuffer->Draw(vertexNext);
	}

	void QuickDraw::Color(float r, float g, float b, float a)
	{
		QuickDrawI::Color(b, g, r, a);
	}
	#pragma endregion
}
}
}