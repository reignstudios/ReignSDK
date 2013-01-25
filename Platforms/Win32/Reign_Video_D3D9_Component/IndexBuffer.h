#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class IndexBufferErrors
	{
		None,
		IndexBuffer
	};

	public ref class IndexBufferCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: IDirect3DIndexBuffer9* indexBuffer;
		#pragma endregion

		#pragma region Constructors
		public: IndexBufferCom(VideoCom^ video);
		public: ~IndexBufferCom();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: IndexBufferErrors Init(array<int>^ indices, int indexCount, int indexByteSize, REIGN_D3DUSAGE usage, bool _32BitIndices);
		public: void Update(array<int>^ indices, int updateCount, int indexByteSize, bool _32BitIndices);
		internal: void enable();
		#pragma endregion
	};
}