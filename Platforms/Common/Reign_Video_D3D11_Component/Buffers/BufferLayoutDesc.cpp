#include "pch.h"
#include "BufferLayout.h"

namespace Reign_Video_D3D11_Component
{
	#pragma region Constructors
	BufferLayoutDescCom::BufferLayoutDescCom(int elementCount, const array<string^>^ semanticNames, const array<REIGN_DXGI_FORMAT>^ formats, const array<uint>^ semanticIndices, const array<uint>^ inputSlots, const array<uint>^ alignedByteOffsets)
	{
		null();

		this->elementCount = elementCount;
		desc = new D3D11_INPUT_ELEMENT_DESC[elementCount];
		for (int i = 0; i != elementCount; ++i)
		{
			ZeroMemory(&desc[i], sizeof(D3D11_INPUT_ELEMENT_DESC));
			desc[i].SemanticName = StringToAscii(semanticNames[i]);
			desc[i].Format = (DXGI_FORMAT)formats[i];
			desc[i].SemanticIndex = semanticIndices[i];
			desc[i].InputSlot = inputSlots[i];
			desc[i].AlignedByteOffset = alignedByteOffsets[i];
			desc[i].InputSlotClass = inputSlots[i] == 0 ? D3D11_INPUT_PER_VERTEX_DATA : D3D11_INPUT_PER_INSTANCE_DATA;
			desc[i].InstanceDataStepRate = inputSlots[i] == 0 ? 0 : 1;
		}
	}

	BufferLayoutDescCom::~BufferLayoutDescCom()
	{
		if (desc)
		{
			for (int i = 0; i != elementCount; ++i)
			{
				if (desc[i].SemanticName) delete desc[i].SemanticName;
			}
			delete desc;
		}
		null();
	}

	void BufferLayoutDescCom::null()
	{
		desc = 0;
	}
	#pragma endregion
}