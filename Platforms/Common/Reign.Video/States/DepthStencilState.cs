using Reign.Core;

namespace Reign.Video
{
	public enum DepthStencilStateTypes
	{
		None,
		ReadWrite_Less
	}

	public interface IDepthStencilStateDesc
	{
		
	}

	public interface IDepthStencilState : IDisposableResource
	{
		#region Methods
		void Enable();
		#endregion
	}
}
