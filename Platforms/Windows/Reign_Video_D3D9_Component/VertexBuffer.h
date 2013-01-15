#pragma once
#include "Video.h"
#include "IndexBuffer.h"

namespace Reign_Video_D3D9_Component
{
	public enum class VertexBufferErrors
	{
		None,
		VertexBuffer
	};

	public ref class VertexBufferCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: IDirect3DVertexBuffer9* vertexBuffer;
		private: D3DPRIMITIVETYPE topology;
		private: uint primitiveVertexCount;
		private: IndexBufferCom^ indexBuffer;
		#pragma endregion

		#pragma region Constructors
		public: VertexBufferCom(VideoCom^ video, REIGN_D3DPRIMITIVETYPE topology);
		public: ~VertexBufferCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: void SetTopology(REIGN_D3DPRIMITIVETYPE topology);
		public: VertexBufferErrors Init(array<float>^ vertices, REIGN_D3DUSAGE usage, int vertexCount, int vertexByteSize);
		public: void Update(array<float>^ vertices, int updateCount, int vertexByteSize);
		public: void Enable(IndexBufferCom^ indexBuffer, VertexBufferCom^ instanceBuffer, int vertexByteSize, int instanceVertexByteSize);
		public: void Draw(int drawCount, int vertexCount, int indexCount);
		public: void Draw(int drawCount, int vertexCount);
		public: void DrawInstanced(int drawCount, int vertexCount);
		public: void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount);
		#pragma endregion
	};
}