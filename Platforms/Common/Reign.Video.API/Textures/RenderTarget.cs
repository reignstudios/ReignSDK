using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class RenderTargetAPI
	{
		public static IRenderTarget New(IDisposableResource parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
		}

		public static IRenderTarget New(VideoTypes videoType, IDisposableResource parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback)
		{
			IRenderTarget api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.RenderTarget(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.RenderTarget(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.RenderTarget(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.RenderTarget(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.RenderTarget(parent, width, height, multiSampleType, surfaceFormat, depthStencilFormat, usage, renderTargetUsage, loadedCallback);
			#endif

			if (api == null) Debug.ThrowError("RenderTargetAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
