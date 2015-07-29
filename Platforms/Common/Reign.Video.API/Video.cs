using System;
using Reign.Core;

namespace Reign.Video.Abstraction
{
	[Flags]
	public enum VideoTypes
	{
		None = 0,
		D3D11 = 1,
		D3D9 = 4,
		XNA = 8,
		Vita = 16,
		OpenGL = 32
	}

	public static class VideoAPI
	{
		public static VideoTypes DefaultAPI = VideoTypes.None;

		public static IVideo New(VideoTypes videoTypeFlags, out VideoTypes videoType, IDisposableResource parent, IApplication application, DepthStencilFormats depthStencilFormats, bool vSync)
		{
			bool d3d11 = (videoTypeFlags & VideoTypes.D3D11) != 0;
			bool d3d9 = (videoTypeFlags & VideoTypes.D3D9) != 0;
			bool gl = (videoTypeFlags & VideoTypes.OpenGL) != 0;
			bool xna = (videoTypeFlags & VideoTypes.XNA) != 0;
			bool vita = (videoTypeFlags & VideoTypes.Vita) != 0;

			videoType = VideoTypes.None;
			Exception lastException = null;
			IVideo video = null;
			while (true)
			{
				try
				{
					#if WIN32 || WINRT || WP8
					if (d3d11)
					{
						d3d11 = false;
						videoType = VideoTypes.D3D11;
						video = new Reign.Video.D3D11.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					#if WIN32
					else if (d3d9)
					{
					    d3d9 = false;
					    videoType = VideoTypes.D3D9;
					    video = new Reign.Video.D3D9.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					#if WIN32 || OSX || LINUX || NaCl || iOS || ANDROID
					if (gl)
					{
						gl = false;
						videoType = VideoTypes.OpenGL;
						video = new Reign.Video.OpenGL.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						videoType = VideoTypes.XNA;
						video = new Reign.Video.XNA.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif
				
					#if VITA
					if (vita)
					{
						vita = false;
						videoType = VideoTypes.Vita;
						video = new Reign.Video.Vita.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					else break;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			// check for error
			if (lastException != null)
			{
				string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
				Debug.ThrowError("VideoAPI", "Failed to create Video API" + ex);
				videoType = VideoTypes.None;
			}

			if (videoType != VideoTypes.None) DefaultAPI = videoType;
			return video;
		}
	}
}
