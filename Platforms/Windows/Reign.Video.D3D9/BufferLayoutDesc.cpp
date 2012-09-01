#include "pch.h"
#include "BufferLayout.h"

namespace Reign
{namespace Video
{namespace D3D9
{
	#pragma region Properties
	D3DVERTEXELEMENT9* BufferLayoutDesc::Desc::get() {return desc;}
	#pragma endregion

	#pragma region Constructors
	BufferLayoutDesc::BufferLayoutDesc(List<BufferLayoutElement>^ elements)
	: BufferLayoutDescI(elements)
	{
		init();
	}

	BufferLayoutDesc::BufferLayoutDesc(BufferLayoutTypes bufferLayoutType)
	: BufferLayoutDescI(bufferLayoutType)
	{
		init();
	}

	void BufferLayoutDesc::init()
	{
		null();
		try
		{
			desc = new D3DVERTEXELEMENT9[ElementCount+1];
			uint i = 0;
			for each(BufferLayoutElement element in Elements)
			{
				switch (element.Usage)
				{
					case (BufferLayoutElementUsages::Position): desc[i].Usage = D3DDECLUSAGE_POSITION; break;
					case (BufferLayoutElementUsages::Color): desc[i].Usage = D3DDECLUSAGE_COLOR; break;
					case (BufferLayoutElementUsages::UV): desc[i].Usage = D3DDECLUSAGE_TEXCOORD; break;
					case (BufferLayoutElementUsages::Normal): desc[i].Usage = D3DDECLUSAGE_NORMAL; break;
					case (BufferLayoutElementUsages::Index): desc[i].Usage = D3DDECLUSAGE_BLENDINDICES; break;
					case (BufferLayoutElementUsages::IndexClassic): desc[i].Usage = D3DDECLUSAGE_BLENDINDICES; break;
					default: Debug::ThrowError("BufferLayoutDesc", "Unsuported ElementUsage"); break;
				}

				switch (element.Type)
				{
					case (BufferLayoutElementTypes::Float): desc[i].Type = D3DDECLTYPE_FLOAT1; break;
					case (BufferLayoutElementTypes::Vector2): desc[i].Type = D3DDECLTYPE_FLOAT2; break;
					case (BufferLayoutElementTypes::Vector3): desc[i].Type = D3DDECLTYPE_FLOAT3; break;
					case (BufferLayoutElementTypes::Vector4): desc[i].Type = D3DDECLTYPE_FLOAT4; break;
					case (BufferLayoutElementTypes::RGBAx8): desc[i].Type = D3DDECLTYPE_D3DCOLOR; break;
					default: Debug::ThrowError("BufferLayoutDesc", "Unsuported ElementType"); break;
				}

				desc[i].UsageIndex = element.UsageIndex;
				desc[i].Offset = element.ByteOffset;
				desc[i].Method = D3DDECLMETHOD_DEFAULT;
				desc[i].Stream = element.StreamIndex;
				++i;
			}
			desc[ElementCount].Stream = 0xFF;
			desc[ElementCount].Offset = 0;
			desc[ElementCount].Type = D3DDECLTYPE_UNUSED;
			desc[ElementCount].Method = 0;
			desc[ElementCount].Usage = 0;
			desc[ElementCount].UsageIndex = 0;
		}
		catch (Exception^ ex)
		{
			delete this;
			throw ex;
		}
	}

	BufferLayoutDesc::~BufferLayoutDesc()
	{
		if (desc) delete desc;
		null();
	}

	void BufferLayoutDesc::null()
	{
		desc = 0;
	}
	#pragma endregion
}
}
}