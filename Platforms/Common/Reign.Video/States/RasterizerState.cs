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
		void Enable();
	}
}
