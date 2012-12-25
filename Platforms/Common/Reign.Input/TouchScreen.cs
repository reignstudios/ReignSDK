using Reign.Core;

namespace Reign.Input
{
	public interface TouchScreenI : DisposableI
	{
		Touch[] Touches {get;}
	}

	public static class TouchScreenAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			TouchScreenAPI.newPtr = newPtr;
		}

		public delegate TouchScreenI NewPtrMethod();
		internal static NewPtrMethod newPtr;
		public static TouchScreenI New()
		{
			return newPtr();
		}
	}
}
