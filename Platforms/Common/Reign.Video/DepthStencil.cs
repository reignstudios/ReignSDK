using Reign.Core;

namespace Reign.Video
{
	public enum DepthStenicFormats
	{
		//Depth24Stencil8
		Depth16
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

		public delegate DepthStencilI NewPtrMethod(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats);
		private static NewPtrMethod newPtr;
		public static DepthStencilI New(DisposableI parent, int width, int height, DepthStenicFormats depthStenicFormats)
		{
			return newPtr(parent, width, height, depthStenicFormats);
		}
	}
}
