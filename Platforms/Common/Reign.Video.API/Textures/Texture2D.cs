using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class Texture2D
	{
		public static Texture2DI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (Texture2DI)OS.CreateInstance(Video.D3D11, Video.D3D11, "Texture2D", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (Texture2DI)OS.CreateInstance(Video.D3D9, Video.D3D9, "Texture2D", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (Texture2DI)OS.CreateInstance(Video.XNA, Video.XNA, "Texture2D", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (Texture2DI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "Texture2D", args);
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
