using Reign.Core;

namespace Reign.Video
{
	public enum DepthStencilStateTypes
	{
		None,
		ReadWrite_Less
	}

	public interface DepthStencilStateDescI
	{
		
	}

	public interface DepthStencilStateI : DisposableI
	{
		#region Methods
		void Enable();
		#endregion
	}

	public static class DepthStencilStateAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			DepthStencilStateAPI.newPtr = newPtr;
		}

		public delegate DepthStencilStateI NewPtrMethod(DisposableI parent, DepthStencilStateDescI desc);
		private static NewPtrMethod newPtr;
		public static DepthStencilStateI New(DisposableI parent, DepthStencilStateDescI desc)
		{
			return newPtr(parent, desc);
		}
	}

	public static class DepthStencilStateDescAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			DepthStencilStateDescAPI.newPtr = newPtr;
		}

		public delegate DepthStencilStateDescI NewPtrMethod(DepthStencilStateTypes type);
		private static NewPtrMethod newPtr;
		public static DepthStencilStateDescI New(DepthStencilStateTypes type)
		{
			return newPtr(type);
		}
	}
}
