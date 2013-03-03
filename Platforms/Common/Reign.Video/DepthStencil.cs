using Reign.Core;

namespace Reign.Video
{
	public enum DepthStenicFormats
	{
		None,
		Defualt,
		Depth24Stencil8,
		Depth16,
		Depth32
	}

	public interface DepthStencilI : DisposableI
	{
		#region Properties
		Size2 Size {get;}
		Vector2 SizeF {get;}
		#endregion
	}

	public static class DepthStencilAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			DepthStencilAPI.newPtr = newPtr;
		}

		public delegate DepthStencilI NewPtrMethod(DisposableI parent, int width, int height, DepthStenicFormats depthStencilFormats);
		private static NewPtrMethod newPtr;
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStencilFormats)
		{
			return newPtr(parent, width, height, depthStencilFormats);
		}
	}
}
