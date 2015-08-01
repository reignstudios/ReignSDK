using Reign.Core;

namespace Reign.Video.Abstraction
{
	public static class RenderTargetAPI
	{
		public static IRenderTarget New(VideoTypes videoType, IDisposableResource parent, string filename, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			IRenderTarget api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.RenderTarget(parent, filename, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.RenderTarget(parent, filename, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.RenderTarget(parent, filename, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.RenderTarget(parent, filename, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.RenderTarget(parent, filename, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			if (api == null) Debug.ThrowError("RenderTargetAPI", "Unsuported InputType: " + videoType);
			return api;
		}

		public static IRenderTarget New(IDisposableResource parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return New(VideoAPI.DefaultAPI, parent, (Image)null, width, height, false, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
		}

		public static IRenderTarget New(VideoTypes videoType, IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, DepthStencilFormats depthStencilFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			IRenderTarget api = null;

			#if WIN32
			if (videoType == VideoTypes.D3D9) api = new D3D9.RenderTarget(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			#if WIN32 || WINRT || WP8
			if (videoType == VideoTypes.D3D11) api = new D3D11.RenderTarget(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			#if WIN32 || OSX || LINUX || iOS || ANDROID || NaCl
			if (videoType == VideoTypes.OpenGL) api = new OpenGL.RenderTarget(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			#if XNA
			if (videoType == VideoTypes.XNA) api = new XNA.RenderTarget(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif
			
			#if VITA
			if (videoType == VideoTypes.Vita) api = new Vita.RenderTarget(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, depthStencilFormat, renderTargetUsage, usage, loadedCallback);
			#endif

			if (api == null) Debug.ThrowError("RenderTargetAPI", "Unsuported InputType: " + videoType);
			return api;
		}
	}
}
