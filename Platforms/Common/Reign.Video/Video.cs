using Reign.Core;

namespace Reign.Video
{
	public interface VideoI : DisposableI
	{
		string FileTag {get;}

		void Update();
		void EnableRenderTarget();
		void EnableRenderTarget(DepthStencilI depthStencil);
		void Clear(float r, float g, float b, float a);
		void ClearColor(float r, float g, float b, float a);
		void ClearDepthStencil();
		void Present();
	}
}
