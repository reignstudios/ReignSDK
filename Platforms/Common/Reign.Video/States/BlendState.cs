using Reign.Core;

namespace Reign.Video
{
	public enum BlendStateTypes
	{
		None,
		Add,
		Subtract,
		RevSubtract,
		Alpha
	}

	public enum BlendStateColorMaskTypes : byte
	{
		Red = 1,
		Green = 2,
		Blue = 4,
		Alpha = 8,
		All = Red | Green | Blue | Alpha
	}

	public enum BlendStateValue
	{
		Zero,
		One,
		SrcColor,
		InvSrcColor,
		SrcAlpha,
		InvSrcAlpha,
		DestAlpha,
		InvDestAlpha,
		DestColor,
		InvDestColor,
		SrcAlphaSat,
		BlendFact,
		InvBlendFact,
		Src1Color,
		InvSrc1Color,
		Src1Alpha,
		InvSrc1Alpha
	}

	public enum BlendStateOperation
	{
		Add,
		Sub,
		RevSub,
		Min,
		Max
	}

	public struct BlendStateRenderTargetDesc
	{
		public bool[] TargetEnable;
		public BlendStateColorMaskTypes[] ColorMask;

		public BlendStateRenderTargetDesc(int renderTargetCount)
		{
			TargetEnable = new bool[renderTargetCount];
			ColorMask = new BlendStateColorMaskTypes[renderTargetCount];
			for (int i = 0; i != renderTargetCount; ++i)
			{
				ColorMask[i] = BlendStateColorMaskTypes.All;
			}
		}
	}

	public interface BlendStateDescI
	{
		
	}

	public interface BlendStateI : DisposableI
	{
		#region Methods
		void Enable();
		#endregion
	}

	public static class BlendStateAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			BlendStateAPI.newPtr = newPtr;
		}

		public delegate BlendStateI NewPtrMethod(DisposableI parent, BlendStateDescI desc);
		private static NewPtrMethod newPtr;
		public static BlendStateI New(DisposableI parent, BlendStateDescI desc)
		{
			return newPtr(parent, desc);
		}
	}

	public static class BlendStateDescAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			BlendStateDescAPI.newPtr = newPtr;
		}

		public delegate BlendStateDescI NewPtrMethod(BlendStateTypes type);
		private static NewPtrMethod newPtr;
		public static BlendStateDescI New(BlendStateTypes type)
		{
			return newPtr(type);
		}
	}
}
