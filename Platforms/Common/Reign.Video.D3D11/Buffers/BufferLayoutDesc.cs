using Reign_Video_D3D11_Component;
using System;
using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video.D3D11
{
	public class BufferLayoutDesc : BufferLayoutDescI
	{
		#region Properties
		internal BufferLayoutDescCom com;
		#endregion

		#region Constructors
		public BufferLayoutDesc(List<BufferLayoutElement> elements)
		: base(elements)
		{
			init();
		}

		public BufferLayoutDesc(BufferLayoutTypes bufferFormatType)
		: base(bufferFormatType)
		{
			init();
		}

		private void init()
		{
			try
			{
				int count = Elements.Count;
				var semanticNames = new string[count];
				var formats = new REIGN_DXGI_FORMAT[count];
				var samanticIndicies = new uint[count];
				var inputSlots = new uint[count];
				var alignedByteOffsets = new uint[count];

				uint i = 0;
				foreach(var element in Elements)
				{
					switch (element.Usage)
					{
						case (BufferLayoutElementUsages.Position): semanticNames[i] = "POSITION"; break;
						case (BufferLayoutElementUsages.Color): semanticNames[i] = "COLOR"; break;
						case (BufferLayoutElementUsages.UV): semanticNames[i] = "TEXCOORD"; break;
						case (BufferLayoutElementUsages.Normal): semanticNames[i] = "NORMAL"; break;
						case (BufferLayoutElementUsages.Index): semanticNames[i] = "BLENDINDICES"; break;
						case (BufferLayoutElementUsages.IndexClassic): semanticNames[i] = "BLENDINDICES"; break;
						default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementUsage"); break;
					}

					switch (element.Type)
					{
						case (BufferLayoutElementTypes.Float): formats[i] =  REIGN_DXGI_FORMAT.R32_FLOAT; break;
						case (BufferLayoutElementTypes.Vector2): formats[i] = REIGN_DXGI_FORMAT.R32G32_FLOAT; break;
						case (BufferLayoutElementTypes.Vector3): formats[i] = REIGN_DXGI_FORMAT.R32G32B32_FLOAT; break;
						case (BufferLayoutElementTypes.Vector4): formats[i] = REIGN_DXGI_FORMAT.R32G32B32A32_FLOAT; break;
						case (BufferLayoutElementTypes.RGBAx8): formats[i] = REIGN_DXGI_FORMAT.R8G8B8A8_UNORM; break;
						default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementType"); break;
					}

					samanticIndicies[i] = (uint)element.UsageIndex;
					inputSlots[i] = (uint)element.StreamIndex;
					alignedByteOffsets[i] = (uint)element.ByteOffset;

					++i;
				}

				com = new BufferLayoutDescCom(count, semanticNames, formats, samanticIndicies, inputSlots, alignedByteOffsets);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public void Dispose()
		{
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
		}
		#endregion
	}
}