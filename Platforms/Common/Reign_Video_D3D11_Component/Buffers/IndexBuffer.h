#pragma once
#include "../Video.h"

namespace Reign_Video_D3D11_Component
{
	public enum class IndexBufferErrors
	{
		None,
		Buffer
	};

	public ref class IndexBufferCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11Buffer* indexBuffer;
		private: DXGI_FORMAT format;
		internal: uint indexCount;
		#pragma endregion
		
		#pragma region Constructors
		public: IndexBufferCom(VideoCom^ video);
		public: IndexBufferErrors Init(const array<int>^ indices, int indexCount, int indexByteSize, REIGN_D3D11_USAGE usage, REIGN_D3D11_CPU_ACCESS_FLAG cpuUsage, bool _32BitIndices);
		public: ~IndexBufferCom();
		private: void null();
		#pragma endregion
		
		#pragma region Methods
		public: void Update(const array<int>^ indices, int updateCount);
		internal: void enable();
		#pragma endregion
	};
}