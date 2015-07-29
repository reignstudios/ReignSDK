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

	public interface ISamplerStateDesc
	{
		
	}

	public interface ISamplerState : IDisposableResource
	{
		#region Methods
		void Enable(int index);
		#endregion
	}
}

