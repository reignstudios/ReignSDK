#include "pch.h"
#include "BufferLayout.h"

namespace Reign_Video_D3D9_Component
{
	#pragma region Constructors
	BufferLayoutDescCom::BufferLayoutDescCom(int elementCount, array<REIGN_D3DDECLUSAGE>^ usages, array<REIGN_D3DDECLTYPE>^ types, array<int>^ usageInicies, array<int>^ offsets, array<REIGN_D3DDECLMETHOD>^ methods, array<int>^ streams)
	{
		null();
		
		desc = new D3DVERTEXELEMENT9[elementCount+1];
		for (int i = 0; i != elementCount; ++i)
		{
			desc[i].Usage = (D3DDECLUSAGE)usages[i];
			desc[i].Type = (D3DDECLTYPE)types[i];
			desc[i].UsageIndex = usageInicies[i];
			desc[i].Offset = offsets[i];
			desc[i].Method = (D3DDECLMETHOD)methods[i];
			desc[i].Stream = streams[i];
		}

		desc[elementCount].Stream = 0xFF;
		desc[elementCount].Offset = 0;
		desc[elementCount].Type = D3DDECLTYPE_UNUSED;
		desc[elementCount].Method = 0;
		desc[elementCount].Usage = 0;
		desc[elementCount].UsageIndex = 0;
	}

	BufferLayoutDescCom::~BufferLayoutDescCom()
	{
		if (desc) delete desc;
		null();
	}

	void BufferLayoutDescCom::null()
	{
		desc = 0;
	}
	#pragma endregion
}