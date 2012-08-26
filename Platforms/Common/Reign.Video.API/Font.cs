using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class Font
	{
		public static FontI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D11)
				{
					return (FontI)OS.CreateInstance(Video.D3D11, Video.D3D11, "Font", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (FontI)OS.CreateInstance(Video.D3D9, Video.D3D9, "Font", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (FontI)OS.CreateInstance(Video.XNA, Video.XNA, "Font", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (FontI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "Font", args);
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
