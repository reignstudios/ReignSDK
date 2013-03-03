using Reign.Core;
using System;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
{
	enum GLBufferElementUsages
	{
		Position = 0,
		Color = 1,
		UV = 2,
		Normal = 3,
		Index = 4,
		IndexClassic = 5
	}

	enum GLBufferElementFormats
	{
		Single = 0,
		Vector2f = 1,
		Vector3f = 2,
		Vector4f = 3,
		Color = 4
	}

	class GLBufferElement
	{
		public GLBufferElementUsages Usage;
		public GLBufferElementFormats Format;
		public int UsageIndex, FloatCount;
		public uint StreamIndex;
		public IntPtr Offset;
		public string Name;
	}

	public class BufferLayoutDesc : BufferLayoutDescI
	{
		#region Properties
		internal GLBufferElement[] desc;
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

		public BufferLayoutDesc(BufferLayoutTypes type)
		: base(type)
		{
			init();
		}

		private void init()
		{
			desc = new GLBufferElement[ElementCount];
			int i = 0;
			foreach (BufferLayoutElement element in Elements)
			{
				desc[i] = new GLBufferElement();
				string usageIndex = element.UsageIndex.ToString();
			    switch (element.Usage)
			    {
					case BufferLayoutElementUsages.Position:
						desc[i].Usage = GLBufferElementUsages.Position;
						desc[i].Name = "Position" + usageIndex;
						break;

					case BufferLayoutElementUsages.Color:
						desc[i].Usage = GLBufferElementUsages.Color;
						desc[i].Name = "Color" + usageIndex;
						break;

					case BufferLayoutElementUsages.UV:
						desc[i].Usage = GLBufferElementUsages.UV;
						desc[i].Name = "Texcoord" + usageIndex;
						break;

					case BufferLayoutElementUsages.Normal:
						desc[i].Usage = GLBufferElementUsages.Normal;
						desc[i].Name = "Normal" + usageIndex;
						break;

					case BufferLayoutElementUsages.Index:
						desc[i].Usage = GLBufferElementUsages.Index;
						desc[i].Name = "BlendIndex" + usageIndex;
						break;

					case BufferLayoutElementUsages.IndexClassic:
						desc[i].Usage = GLBufferElementUsages.IndexClassic;
						desc[i].Name = "BlendIndex" + usageIndex;
						break;

					default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementUsage"); break;
			    }

			    switch (element.Type)
			    {
			        case BufferLayoutElementTypes.Float: desc[i].Format = GLBufferElementFormats.Single; break;
			        case BufferLayoutElementTypes.Vector2: desc[i].Format = GLBufferElementFormats.Vector2f; break;
			        case BufferLayoutElementTypes.Vector3: desc[i].Format = GLBufferElementFormats.Vector3f; break;
			        case BufferLayoutElementTypes.Vector4: desc[i].Format = GLBufferElementFormats.Vector4f; break;
			        case BufferLayoutElementTypes.RGBAx8: desc[i].Format = GLBufferElementFormats.Color; break;
					default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementType"); break;
			    }

			    desc[i].UsageIndex = element.UsageIndex;
			    desc[i].Offset = new IntPtr(element.ByteOffset);
				desc[i].FloatCount = element.FloatCount;
				desc[i].StreamIndex = (uint)element.StreamIndex;

			    ++i;
			}
		}
		#endregion
	}
}