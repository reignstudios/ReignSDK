using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class IndexBuffer
	{
		public static IndexBufferI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D10)
				{
					return (IndexBufferI)OS.CreateInstance(Video.D3D10, Video.D3D10, "IndexBuffer", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (IndexBufferI)OS.CreateInstance(Video.D3D9, Video.D3D9, "IndexBuffer", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (IndexBufferI)OS.CreateInstance(Video.XNA, Video.XNA, "IndexBuffer", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (IndexBufferI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "IndexBuffer", args);
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
