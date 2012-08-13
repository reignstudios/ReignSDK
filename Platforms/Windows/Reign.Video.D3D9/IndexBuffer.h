#pragma once
#include <d3d9.h>
#include "Video.h"
#include "BufferLayout.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class IndexBuffer : IndexBufferI
	{
		#pragma region Properties
		private: Video^ video;
		private: IDirect3DIndexBuffer9* indexBuffer;
		#pragma endregion

		#pragma region Constructors
		public: IndexBuffer(DisposableI^ parent, BufferUsages bufferUsage);
		public: IndexBuffer(DisposableI^ parent, BufferUsages bufferUsage, array<int>^ indices);
		public: IndexBuffer(DisposableI^ parent, BufferUsages bufferUsage, array<int>^ indices, bool _32BitIndices);
		private: void init(DisposableI^ parent, array<int>^ indices);
		public: ~IndexBuffer();
		private: void null();
		#pragma endregion

		#pragma region Methods
		public: virtual void Init(array<int>^ indices) override;
		public: virtual void Update(array<int>^ indices, int updateCount) override;
		internal: void enable();
		#pragma endregion
	};
}
}
}