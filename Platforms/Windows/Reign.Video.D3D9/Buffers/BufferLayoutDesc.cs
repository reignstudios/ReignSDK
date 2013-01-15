using System;
using Reign.Video;
using Reign_Video_D3D9_Component;
using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class BufferLayoutDesc : BufferLayoutDescI
	{
		#region Properties
		internal BufferLayoutDescCom com;
		#endregion

		#region Constructors
		public static BufferLayoutDesc New(List<BufferLayoutElement> elements)
		{
			return new BufferLayoutDesc(elements);
		}

		public static BufferLayoutDesc New(BufferLayoutTypes type)
		{
			return new BufferLayoutDesc(type);
		}

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
				var usages = new REIGN_D3DDECLUSAGE[ElementCount];
				var types = new REIGN_D3DDECLTYPE[ElementCount];
				var usageIndicies = new int[ElementCount];
				var offsets = new int[ElementCount];
				var methods = new REIGN_D3DDECLMETHOD[ElementCount];
				var streams = new int[ElementCount];
				int i = 0;
				foreach(BufferLayoutElement element in Elements)
				{
					switch (element.Usage)
					{
						case (BufferLayoutElementUsages.Position): usages[i] = REIGN_D3DDECLUSAGE.POSITION; break;
						case (BufferLayoutElementUsages.Color): usages[i] = REIGN_D3DDECLUSAGE.COLOR; break;
						case (BufferLayoutElementUsages.UV): usages[i] = REIGN_D3DDECLUSAGE.TEXCOORD; break;
						case (BufferLayoutElementUsages.Normal): usages[i] = REIGN_D3DDECLUSAGE.NORMAL; break;
						case (BufferLayoutElementUsages.Index): usages[i] = REIGN_D3DDECLUSAGE.BLENDINDICES; break;
						case (BufferLayoutElementUsages.IndexClassic): usages[i] = REIGN_D3DDECLUSAGE.BLENDINDICES; break;
						default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementUsage"); break;
					}

					switch (element.Type)
					{
						case (BufferLayoutElementTypes.Float): types[i] = REIGN_D3DDECLTYPE.FLOAT1; break;
						case (BufferLayoutElementTypes.Vector2): types[i] = REIGN_D3DDECLTYPE.FLOAT2; break;
						case (BufferLayoutElementTypes.Vector3): types[i] = REIGN_D3DDECLTYPE.FLOAT3; break;
						case (BufferLayoutElementTypes.Vector4): types[i] = REIGN_D3DDECLTYPE.FLOAT4; break;
						case (BufferLayoutElementTypes.RGBAx8): types[i] = REIGN_D3DDECLTYPE.D3DCOLOR; break;
						default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementType"); break;
					}

					usageIndicies[i] = element.UsageIndex;
					offsets[i] = element.ByteOffset;
					methods[i] = REIGN_D3DDECLMETHOD.DEFAULT;
					streams[i] = element.StreamIndex;

					++i;
				}

				com = new BufferLayoutDescCom(ElementCount, usages, types, usageIndicies, offsets, methods, streams);
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
