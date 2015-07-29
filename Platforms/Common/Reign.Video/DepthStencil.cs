using Reign.Core;

namespace Reign.Video
{
	public enum DepthStencilFormats
	{
		None,
		Defualt,
		Depth24Stencil8,
		Depth16,
		Depth24,
		Depth32
	}

	public interface IDepthStencil : IDisposableResource
	{
		#region Properties
		Size2 Size {get;}
		Vector2 SizeF {get;}
		#endregion
	}
}
