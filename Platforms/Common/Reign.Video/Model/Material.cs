using Reign.Core;

namespace Reign.Video
{
	public interface MaterialI : DisposableI
	{
		Texture2DI[] DiffuseTextures {get;}
	}
}