#pragma once
#include "../Video.h"
#include "../Shaders/ShaderModel.h"

namespace Reign_Video_D3D11_Component
{
	public enum class BufferLayoutErrors
	{
		None,
		InputLayout
	};

	public ref class BufferLayoutDescCom sealed
	{
		#pragma region Properties
		internal: D3D11_INPUT_ELEMENT_DESC* desc;
		internal: int elementCount;
		#pragma endregion

		#pragma region Constructors
		public: BufferLayoutDescCom(int elementCount, const array<string^>^ semanticNames, const array<REIGN_DXGI_FORMAT>^ formats, const array<uint>^ semanticIndices, const array<uint>^ inputSlots, const array<uint>^ alignedByteOffsets);
		public: virtual ~BufferLayoutDescCom();
		private: void null();
		#pragma endregion
	};

	public ref class BufferLayoutCom sealed
	{
		#pragma region Properties
		private: VideoCom^ video;
		private: ID3D11InputLayout* layout;
		#pragma endregion
		
		#pragma region Constructors
		public: BufferLayoutErrors Init(VideoCom^ video, ShaderModelCom^ vertexShader, BufferLayoutDescCom^ desc);
		public: virtual ~BufferLayoutCom();
		private: void null();
		#pragma endregion
		
		#pragma region Methods
		public: void Enable();
		#pragma endregion
	};
}