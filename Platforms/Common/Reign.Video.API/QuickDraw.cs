using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class QuickDraw
	{
		public static QuickDrawI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D11)
				{
					return (QuickDrawI)OS.CreateInstance(Video.D3D11, Video.D3D11, "QuickDraw", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (QuickDrawI)OS.CreateInstance(Video.D3D9, Video.D3D9, "QuickDraw", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (QuickDrawI)OS.CreateInstance(Video.XNA, Video.XNA, "QuickDraw", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (QuickDrawI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "QuickDraw", args);
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
