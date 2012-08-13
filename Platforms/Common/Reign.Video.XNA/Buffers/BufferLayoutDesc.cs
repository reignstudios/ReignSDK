using Microsoft.Xna.Framework.Graphics;
using Reign.Core;
using System;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class BufferLayoutDesc : BufferLayoutDescI
	{
		#region Properties
		public VertexElement[] Desc {get; private set;}
		#endregion

		#region Constructors
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
			Desc = new VertexElement[ElementCount];
			int i = 0;
			foreach (BufferLayoutElement element in Elements)
			{
				Desc[i] = new VertexElement();
				switch (element.Usage)
				{
					case (BufferLayoutElementUsages.Position): Desc[i].VertexElementUsage = VertexElementUsage.Position; break;
					case (BufferLayoutElementUsages.Color): Desc[i].VertexElementUsage = VertexElementUsage.Color; break;
					case (BufferLayoutElementUsages.UV): Desc[i].VertexElementUsage = VertexElementUsage.TextureCoordinate; break;
					case (BufferLayoutElementUsages.Index): Desc[i].VertexElementUsage = VertexElementUsage.BlendIndices; break;
					case (BufferLayoutElementUsages.IndexClassic): Desc[i].VertexElementUsage = VertexElementUsage.BlendIndices; break;
				}
				switch (element.Type)
				{
					case (BufferLayoutElementTypes.Float): Desc[i].VertexElementFormat = VertexElementFormat.Single; break;
					case (BufferLayoutElementTypes.Vector2): Desc[i].VertexElementFormat = VertexElementFormat.Vector2; break;
					case (BufferLayoutElementTypes.Vector3): Desc[i].VertexElementFormat = VertexElementFormat.Vector3; break;
					case (BufferLayoutElementTypes.Vector4): Desc[i].VertexElementFormat = VertexElementFormat.Vector4; break;
					case (BufferLayoutElementTypes.RGBAx8): Desc[i].VertexElementFormat = VertexElementFormat.Color; break;
				}
				Desc[i].UsageIndex = (int)element.UsageIndex;
				Desc[i].Offset = element.ByteOffset;
				++i;
			}
		}
		#endregion
	}
}