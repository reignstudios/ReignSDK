using Reign.Core;

namespace Reign.Video
{
	public enum RasterizerStateTypes
	{
		Solid_CullNone,
		Solid_CullCW,
		Solid_CullCCW
	}

	public interface RasterizerStateDescI
	{
		
	}

	public interface RasterizerStateI : DisposableI
	{
		#region Methods
		void Enable();
		#endregion
	}

	public static class RasterizerStateAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			RasterizerStateAPI.newPtr = newPtr;
		}

		public delegate RasterizerStateI NewPtrMethod(DisposableI parent, RasterizerStateDescI desc);
		private static NewPtrMethod newPtr;
		public static RasterizerStateI New(DisposableI parent, RasterizerStateDescI desc)
		{
			return newPtr(parent, desc);
		}
	}

	public static class RasterizerStateDescAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			RasterizerStateDescAPI.newPtr = newPtr;
		}

		public delegate RasterizerStateDescI NewPtrMethod(RasterizerStateTypes type);
		private static NewPtrMethod newPtr;
		public static RasterizerStateDescI New(RasterizerStateTypes type)
		{
			return newPtr(type);
		}
	}
}
