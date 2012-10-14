#pragma once
#include <d3d9.h>
#include "Video.h"
#include "BufferLayout.h"
#include "IndexBuffer.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class VertexBuffer : VertexBufferI
	{
		#pragma region Properties
		private: Video^ video;
		private: IDirect3DVertexBuffer9* vertexBuffer;
		private: D3DPRIMITIVETYPE primitiveTopology;
		private: uint primitiveVertexCount;
		private: IndexBuffer^ indexBuffer;

		private: VertexBufferTopologys topology;
		public: property VertexBufferTopologys Topology {virtual VertexBufferTopologys get() override; virtual void set(VertexBufferTopologys value) override;}
		#pragma endregion

		#pragma region Constructors
		public: VertexBuffer(DisposableI^ parent, BufferLayoutDescI^ bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology);
		public: VertexBuffer(DisposableI^ parent, BufferLayoutDescI^ bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, array<float>^ vertices);
		private: void init(DisposableI^ parent, VertexBufferTopologys vertexBufferTopology, array<float>^ vertices);
		public: ~VertexBuffer();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: virtual void Init(array<float>^ vertices) override;
		public: virtual void Update(array<float>^ vertices, int updateCount) override;
		private: void enable(IndexBufferI^ indexBuffer, VertexBufferI^ instanceBuffer);
		public: virtual void Enable() override;
		public: virtual void Enable(IndexBufferI^ indexBuffer) override;
		public: virtual void Enable(VertexBufferI^ instanceBuffer) override;
		public: virtual void Enable(IndexBufferI^ indexBuffer, VertexBufferI^ instanceBuffer) override;
		public: virtual void Draw() override;
		public: virtual void Draw(int drawCount) override;
		public: virtual void DrawInstanced(int drawCount) override;
		public: virtual void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount) override;
		#pragma endregion
	};
}
}
}