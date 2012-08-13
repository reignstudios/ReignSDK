#pragma once
#include <d3d9.h>
#include "Video.h"
#include "Shader.h"

using namespace System::Collections::Generic;

namespace Reign
{namespace Video
{namespace D3D9
{
	public ref class BufferLayoutDesc : BufferLayoutDescI
	{
		#pragma region Properties
		private: D3DVERTEXELEMENT9* desc;
		public: property D3DVERTEXELEMENT9* Desc {D3DVERTEXELEMENT9* get();}
		#pragma endregion

		#pragma region Constructors
		public: BufferLayoutDesc(List<BufferLayoutElement>^ elements);
		public: BufferLayoutDesc(BufferLayoutTypes bufferLayoutType);
		public: ~BufferLayoutDesc();
		private: void init();
		private: void null();
		#pragma endregion
	};

	public ref class BufferLayout : Disposable, BufferLayoutI
	{
		#pragma region Fields
		private: Video^ video;
		private: IDirect3DVertexDeclaration9* layout;
		#pragma endregion
		
		#pragma region Constructors
		public: BufferLayout(DisposableI^ parent, ShaderI^ shader, BufferLayoutDescI^ bufferLayoutDesc);
		public: ~BufferLayout();
		private: void null();
		#pragma endregion
		
		#pragma region Methods
		public: virtual void Enable();
		#pragma endregion
	};
}
}
}