#pragma once
#include "../Video.h"
#include "IndexBuffer.h"

namespace Reign_Video_D3D11_Component
{
	public enum class VertexBufferErrors
	{
		None,
		Buffer
	};

	public enum class REIGN_D3D_PRIMITIVE_TOPOLOGY
	{
		TRIANGLELIST = D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST,
		LINELIST = D3D11_PRIMITIVE_TOPOLOGY_LINELIST,
		POINTLIST = D3D11_PRIMITIVE_TOPOLOGY_POINTLIST
	};

	public ref class VertexBufferCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11Buffer* vertexBuffer;
		private: D3D_PRIMITIVE_TOPOLOGY primitiveTopology;
		private: IndexBufferCom^ indexBuffer;
		private: uint vertexCount, vertexByteSize;
		#pragma endregion
		
		#pragma region Constructors
		public: VertexBufferCom(VideoCom^ video, REIGN_D3D_PRIMITIVE_TOPOLOGY topology);
		public: VertexBufferErrors Init(const array<float>^ vertices, int vertexCount, int vertexByteSize, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage);
		public: virtual ~VertexBufferCom();
		private: void null();
		#pragma endregion
		
		#pragma region Methods
		public: void SetTopology(REIGN_D3D_PRIMITIVE_TOPOLOGY topology);
		public: void Update(const array<float>^ vertices, int updateCount);
		public: void Enable(IndexBufferCom^ indexBuffer, VertexBufferCom^ instanceBuffer);
		public: void Draw();
		public: void Draw(int drawCount);
		public: void DrawInstanced(int drawCount);
		public: void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount);
		#pragma endregion
	};
}