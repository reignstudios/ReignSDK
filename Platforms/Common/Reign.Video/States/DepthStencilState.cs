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
		void Enable();
	}
}
