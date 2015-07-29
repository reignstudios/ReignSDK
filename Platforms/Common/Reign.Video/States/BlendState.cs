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

	public interface IBlendStateDesc
	{
		
	}

	public interface IBlendState : IDisposableResource
	{
		#region Methods
		void Enable();
		#endregion
	}
}
