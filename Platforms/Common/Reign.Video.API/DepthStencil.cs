using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class DepthStencil
	{
		public static DepthStencilI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (DepthStencilI)OS.CreateInstance(Video.D3D11, Video.D3D11, "DepthStencil", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (DepthStencilI)OS.CreateInstance(Video.D3D9, Video.D3D9, "DepthStencil", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (DepthStencilI)OS.CreateInstance(Video.XNA, Video.XNA, "DepthStencil", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (DepthStencilI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "DepthStencil", args);
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
