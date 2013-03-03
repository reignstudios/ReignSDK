using Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using System;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class BufferLayoutDesc : BufferLayoutDescI
	{
		#region Properties
		internal VertexElement[] desc;
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

		public BufferLayoutDesc(BufferLayoutTypes bufferLayoutType)
		: base(bufferLayoutType)
		{
			init();
		}

		private void init()
		{
			desc = new VertexElement[ElementCount];
			int i = 0;
			foreach (BufferLayoutElement element in Elements)
			{
				desc[i] = new VertexElement();
				switch (element.Usage)
				{
					case BufferLayoutElementUsages.Position: desc[i].VertexElementUsage = VertexElementUsage.Position; break;
					case BufferLayoutElementUsages.Color: desc[i].VertexElementUsage = VertexElementUsage.Color; break;
					case BufferLayoutElementUsages.UV: desc[i].VertexElementUsage = VertexElementUsage.TextureCoordinate; break;
					case BufferLayoutElementUsages.Normal: desc[i].VertexElementUsage = VertexElementUsage.Normal; break;
					case BufferLayoutElementUsages.Index: desc[i].VertexElementUsage = VertexElementUsage.BlendIndices; break;
					case BufferLayoutElementUsages.IndexClassic: desc[i].VertexElementUsage = VertexElementUsage.BlendIndices; break;
				}
				switch (element.Type)
				{
					case BufferLayoutElementTypes.Float: desc[i].VertexElementFormat = VertexElementFormat.Single; break;
					case BufferLayoutElementTypes.Vector2: desc[i].VertexElementFormat = VertexElementFormat.Vector2; break;
					case BufferLayoutElementTypes.Vector3: desc[i].VertexElementFormat = VertexElementFormat.Vector3; break;
					case BufferLayoutElementTypes.Vector4: desc[i].VertexElementFormat = VertexElementFormat.Vector4; break;
					case BufferLayoutElementTypes.RGBAx8: desc[i].VertexElementFormat = VertexElementFormat.Color; break;
				}
				desc[i].UsageIndex = (int)element.UsageIndex;
				desc[i].Offset = element.ByteOffset;
				++i;
			}
		}
		#endregion
	}
}