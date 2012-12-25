using Reign.Core;

namespace Reign.Video
{
	public enum SamplerStateTypes
	{
		Point_Wrap,
		Point_Clamp,
		Point_Border,
		Linear_Wrap,
		Linear_Clamp,
		Linear_Border
	}

	public interface SamplerStateDescI
	{
		
	}

	public interface SamplerStateI : DisposableI
	{
		#region Methods
		void Enable(int index);
		#endregion
	}

	public static class SamplerStateAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			SamplerStateAPI.newPtr = newPtr;
		}

		public delegate SamplerStateI NewPtrMethod(DisposableI parent, SamplerStateDescI desc);
		private static NewPtrMethod newPtr;
		public static SamplerStateI New(DisposableI parent, SamplerStateDescI desc)
		{
			return newPtr(parent, desc);
		}
	}

	public static class SamplerStateDescAPI
	{
		public static void Init(NewPtrMethod newPtr)
		{
			SamplerStateDescAPI.newPtr = newPtr;
		}

		public delegate SamplerStateDescI NewPtrMethod(SamplerStateTypes type);
		private static NewPtrMethod newPtr;
		public static SamplerStateDescI New(SamplerStateTypes type)
		{
			return newPtr(type);
		}
	}
}

