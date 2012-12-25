using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public enum BufferUsages
	{
		Default,
		Write,
		Read
	}

	public enum BufferLayoutTypes
	{
		Index,
		Position2,
		Position2_UV,
		Position2_IndexClassic,
		Position3,
		Position3_UV,
		Position3_UV2,
		Position3_UV_Index,
		Position3_UV_IndexClassic,
		Position3_Color,
		Position3_Color_UV,
		Position3_Color_UV_Index,
		Position3_Color_UV_IndexClassic,
		Position3_Normal
	}

	public enum BufferLayoutElementTypes
	{
		Float,
		Vector2,
		Vector3,
		Vector4,
		RGBAx8
	}

	public enum BufferLayoutElementUsages
	{
		Position,
		Color,
		UV,
		Normal,
		Index,
		IndexClassic
	}

	public struct BufferLayoutElement
	{
		#region Properties
		public BufferLayoutElementTypes Type;
		public BufferLayoutElementUsages Usage;
		public int StreamIndex, UsageIndex, FloatOffset;
		public int ByteOffset {get {return FloatOffset * 4;}}

		public int TypeSize
		{
			get
			{
				switch (Type)
				{
					case (BufferLayoutElementTypes.Float): return sizeof(float);
					case (BufferLayoutElementTypes.Vector2): return sizeof(float) * 2;
					case (BufferLayoutElementTypes.Vector3): return sizeof(float) * 3;
					case (BufferLayoutElementTypes.Vector4): return sizeof(float) * 4;
					case (BufferLayoutElementTypes.RGBAx8): return sizeof(int);
				}
				return 0;
			}
		}

		public int FloatCount
		{
			get
			{
				switch (Type)
				{
					case (BufferLayoutElementTypes.Float): return 1;
					case (BufferLayoutElementTypes.Vector2): return 2;
					case (BufferLayoutElementTypes.Vector3): return 3;
					case (BufferLayoutElementTypes.Vector4): return 4;
					case (BufferLayoutElementTypes.RGBAx8): return 1;
				}
				return 0;
			}
		}
		#endregion

		#region Constructors
		public BufferLayoutElement(BufferLayoutElementTypes type, BufferLayoutElementUsages usage, int streamIndex, int usageIndex, int floatOffset)
		{
			Type = type;
			Usage = usage;
			StreamIndex = streamIndex;
			UsageIndex = usageIndex;
			FloatOffset = floatOffset;
		}
		#endregion
	}

	public interface BufferLayoutI : DisposableI
	{
		#region Methods
		void Enable();
		#endregion
	}

	public abstract class BufferLayoutDescI
	{
		#region Properties
		public List<BufferLayoutElement> Elements {get; private set;}
		public int ElementCount {get; private set;}
		public int ByteSize {get; private set;}
		public int[] StreamBytesSizes {get; private set;}
		public int FloatCount {get; private set;}
		#endregion

		#region Constructors
		protected BufferLayoutDescI(List<BufferLayoutElement> elements)
		{
			Elements = elements;
			finish();
		}

		public BufferLayoutDescI(BufferLayoutTypes bufferFormatType)
		{
			Elements = new List<BufferLayoutElement>();
			switch (bufferFormatType)
			{
				case (BufferLayoutTypes.Index):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.Index, 1, 0, 0));
					break;

				case (BufferLayoutTypes.Position2):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.Position, 0, 0, 0));
					break;

				case (BufferLayoutTypes.Position2_UV):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 2));
					break;

				case (BufferLayoutTypes.Position2_IndexClassic):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.IndexClassic, 0, 0, 2));
					break;

				case (BufferLayoutTypes.Position3):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					break;

				case (BufferLayoutTypes.Position3_UV):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
					break;

				case (BufferLayoutTypes.Position3_UV2):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 1, 5));
					break;

				case (BufferLayoutTypes.Position3_UV_Index):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.Index, 1, 0, 0));
					break;

				case (BufferLayoutTypes.Position3_UV_IndexClassic):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 3));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.IndexClassic, 0, 0, 5));
					break;

				case (BufferLayoutTypes.Position3_Color):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));
					break;

				case (BufferLayoutTypes.Position3_Color_UV):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 4));
					break;

				case (BufferLayoutTypes.Position3_Color_UV_Index):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 4));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.Index, 1, 0, 0));
					break;

				case (BufferLayoutTypes.Position3_Color_UV_IndexClassic):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.RGBAx8, BufferLayoutElementUsages.Color, 0, 0, 3));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, 0, 4));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.IndexClassic, 0, 0, 6));
					break;

				case (BufferLayoutTypes.Position3_Normal):
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Position, 0, 0, 0));
					Elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Normal, 0, 0, 3));
					break;
			}

			finish();
		}

		private void finish()
		{
			ElementCount = Elements.Count;
			ByteSize = 0;
			FloatCount = 0;
			StreamBytesSizes = new int[2];
			foreach (var element in Elements)
			{
				ByteSize += element.TypeSize;
				FloatCount += element.FloatCount;
				StreamBytesSizes[element.StreamIndex] += element.TypeSize;
			}
		}
		#endregion

		#region Methods
		public List<BufferLayoutElement> ElementsUsages(BufferLayoutElementUsages usage)
		{
			List<BufferLayoutElement> newList = new List<BufferLayoutElement>();
			foreach (var element in Elements)
			{
				if (element.Usage == usage) newList.Add(element);
			}
			return newList;
		}
		#endregion
	}

	public static class BufferLayoutAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			BufferLayoutAPI.newPtr = newPtr;
		}

		public delegate BufferLayoutI NewPtrMethod(DisposableI parent, ShaderI shader, BufferLayoutDescI desc);
		private static NewPtrMethod newPtr;
		public static BufferLayoutI New(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		{
			return newPtr(parent, shader, desc);
		}
	}

	public static class BufferLayoutDescAPI
	{
		public static void Init(NewPtrMethod1 newPtr1, NewPtrMethod2 newPtr2)
		{
			BufferLayoutDescAPI.newPtr1 = newPtr1;
			BufferLayoutDescAPI.newPtr2 = newPtr2;
		}

		public delegate BufferLayoutDescI NewPtrMethod1(List<BufferLayoutElement> elements);
		private static NewPtrMethod1 newPtr1;
		public static BufferLayoutDescI New(List<BufferLayoutElement> elements)
		{
			return newPtr1(elements);
		}

		public delegate BufferLayoutDescI NewPtrMethod2(BufferLayoutTypes bufferFormatType);
		private static NewPtrMethod2 newPtr2;
		public static BufferLayoutDescI New(BufferLayoutTypes bufferFormatType)
		{
			return newPtr2(bufferFormatType);
		}
	}
}