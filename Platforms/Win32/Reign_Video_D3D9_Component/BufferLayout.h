#pragma once
#include "Video.h"

namespace Reign_Video_D3D9_Component
{
	public enum class BufferLayoutErrors
	{
		None,
		VertexDeclaration
	};

	public ref class BufferLayoutDescCom sealed
	{
		#pragma region Properties
		internal: D3DVERTEXELEMENT9* desc;
		#pragma endregion

		#pragma region Constructors
		public: BufferLayoutDescCom(int elementCount, array<REIGN_D3DDECLUSAGE>^ usages, array<REIGN_D3DDECLTYPE>^ types, array<int>^ usageInicies, array<int>^ offsets, array<REIGN_D3DDECLMETHOD>^ methods, array<int>^ streams);
		public: ~BufferLayoutDescCom();
		private: void null();
		#pragma endregion
	};

	public ref class BufferLayoutCom sealed
	{
		#pragma region Fields
		private: VideoCom^ video;
		private: IDirect3DVertexDeclaration9* layout;
		#pragma endregion
		
		#pragma region Constructors
		public: BufferLayoutErrors Init(VideoCom^ video, BufferLayoutDescCom^ bufferLayoutDesc);
		public: ~BufferLayoutCom();
		private: void null();
		#pragma endregion
		
		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}