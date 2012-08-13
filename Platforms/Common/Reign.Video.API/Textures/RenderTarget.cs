using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class RenderTarget
	{
		public static RenderTargetI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D10)
				{
					return (RenderTargetI)OS.CreateInstance(Video.D3D10, Video.D3D10, "RenderTarget", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (RenderTargetI)OS.CreateInstance(Video.D3D9, Video.D3D9, "RenderTarget", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (RenderTargetI)OS.CreateInstance(Video.XNA, Video.XNA, "RenderTarget", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (RenderTargetI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "RenderTarget", args);
				}
				#endif
			}
			catch (TargetInvocationException e)
			{
				throw (e.InnerException != null) ? e.InnerException : e;
			}
			catch (Exception e)
			{
				throw e;
			}

			return null;
		}
	}
}
