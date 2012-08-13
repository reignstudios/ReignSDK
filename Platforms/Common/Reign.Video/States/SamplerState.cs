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
		void Enable(int index);
	}
}

