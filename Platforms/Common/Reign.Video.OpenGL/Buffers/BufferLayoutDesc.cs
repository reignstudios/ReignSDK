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
		internal GLBufferElement[] Desc {get; private set;}
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
			Desc = new GLBufferElement[ElementCount];
			int i = 0;
			foreach (BufferLayoutElement element in Elements)
			{
				Desc[i] = new GLBufferElement();
				string usageIndex = element.UsageIndex.ToString();
			    switch (element.Usage)
			    {
					case (BufferLayoutElementUsages.Position):
						Desc[i].Usage = GLBufferElementUsages.Position;
						Desc[i].Name = "Position" + usageIndex;
						break;

					case (BufferLayoutElementUsages.Color):
						Desc[i].Usage = GLBufferElementUsages.Color;
						Desc[i].Name = "Color" + usageIndex;
						break;

					case (BufferLayoutElementUsages.UV):
						Desc[i].Usage = GLBufferElementUsages.UV;
						Desc[i].Name = "Texcoord" + usageIndex;
						break;

					case (BufferLayoutElementUsages.Normal):
						Desc[i].Usage = GLBufferElementUsages.Normal;
						Desc[i].Name = "Normal" + usageIndex;
						break;

					case (BufferLayoutElementUsages.Index):
						Desc[i].Usage = GLBufferElementUsages.Index;
						Desc[i].Name = "BlendIndex" + usageIndex;
						break;

					case (BufferLayoutElementUsages.IndexClassic):
						Desc[i].Usage = GLBufferElementUsages.IndexClassic;
						Desc[i].Name = "BlendIndex" + usageIndex;
						break;

					default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementUsage"); break;
			    }

			    switch (element.Type)
			    {
			        case (BufferLayoutElementTypes.Float): Desc[i].Format = GLBufferElementFormats.Single; break;
			        case (BufferLayoutElementTypes.Vector2): Desc[i].Format = GLBufferElementFormats.Vector2f; break;
			        case (BufferLayoutElementTypes.Vector3): Desc[i].Format = GLBufferElementFormats.Vector3f; break;
			        case (BufferLayoutElementTypes.Vector4): Desc[i].Format = GLBufferElementFormats.Vector4f; break;
			        case (BufferLayoutElementTypes.RGBAx8): Desc[i].Format = GLBufferElementFormats.Color; break;
					default: Debug.ThrowError("BufferLayoutDesc", "Unsuported ElementType"); break;
			    }

			    Desc[i].UsageIndex = element.UsageIndex;
			    Desc[i].Offset = new IntPtr(element.ByteOffset);
				Desc[i].FloatCount = element.FloatCount;
				Desc[i].StreamIndex = (uint)element.StreamIndex;

			    ++i;
			}
		}
		#endregion
	}
}