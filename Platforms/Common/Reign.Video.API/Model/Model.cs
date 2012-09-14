using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class Model
	{
		public static ModelI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (ModelI)OS.CreateInstance(Video.D3D11, Video.D3D11, "Model", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (ModelI)OS.CreateInstance(Video.D3D9, Video.D3D9, "Model", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (ModelI)OS.CreateInstance(Video.XNA, Video.XNA, "Model", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (ModelI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "Model", args);
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
