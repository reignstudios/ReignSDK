using Reign.Core;

namespace Reign.Video
{
	public enum RasterizerStateTypes
	{
		Solid_CullNone,
		Solid_CullCW,
		Solid_CullCCW
	}

	public interface IRasterizerStateDesc
	{
		
	}

	public interface IRasterizerState : IDisposableResource
	{
		#region Methods
		void Enable();
		#endregion
	}
}
