#pragma once
#include <d3d9.h>
#include "Video.h"
#include "BufferLayout.h"
#include "VertexBuffer.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class QuickDraw : QuickDrawI
	{
		#pragma region Properties
		private: VertexBuffer^ vertexBuffer;
		#pragma endregion

		#pragma region Constructors
		public: QuickDraw(DisposableI^ parent, BufferLayoutDescI^ bufferLayoutDesc);
		#pragma endregion
		
		#pragma region Methods
		public: virtual void StartTriangles() override;
		public: virtual void StartLines() override;
		public: virtual void StartPoints() override;
		public: virtual void Draw() override;
		public: virtual void Color(float r, float g, float b, float a) override;
		#pragma endregion
	};
}
}
}