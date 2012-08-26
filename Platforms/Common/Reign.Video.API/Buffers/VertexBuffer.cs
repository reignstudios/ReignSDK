using System;
using Reign.Core;
using System.Reflection;

#if XNA
using VX = Reign.Video.XNA;
#endif

#if WINDOWS || OSX || LINUX || iOS || ANDROID
using VGL = Reign.Video.OpenGL;
#endif

namespace Reign.Video.API
{
	public static class VertexBuffer
	{
		public static VertexBufferI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D11)
				{
					return (VertexBufferI)OS.CreateInstance(Video.D3D11, Video.D3D11, "VertexBuffer", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (VertexBufferI)OS.CreateInstance(Video.D3D9, Video.D3D9, "VertexBuffer", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (VertexBufferI)OS.CreateInstance(Video.XNA, Video.XNA, "VertexBuffer", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (VertexBufferI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "VertexBuffer", args);
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
