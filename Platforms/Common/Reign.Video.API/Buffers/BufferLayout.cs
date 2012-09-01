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
	public static class BufferLayoutDesc
	{
		public static BufferLayoutDescI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (BufferLayoutDescI)OS.CreateInstance(Video.D3D11, Video.D3D11, "BufferLayoutDesc", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (BufferLayoutDescI)OS.CreateInstance(Video.D3D9, Video.D3D9, "BufferLayoutDesc", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (BufferLayoutDescI)OS.CreateInstance(Video.XNA, Video.XNA, "BufferLayoutDesc", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (BufferLayoutDescI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "BufferLayoutDesc", args);
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

	public static class BufferLayout
	{
		public static BufferLayoutI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (BufferLayoutI)OS.CreateInstance(Video.D3D11, Video.D3D11, "BufferLayout", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (BufferLayoutI)OS.CreateInstance(Video.D3D9, Video.D3D9, "BufferLayout", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (BufferLayoutI)OS.CreateInstance(Video.XNA, Video.XNA, "BufferLayout", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (BufferLayoutI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "BufferLayout", args);
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
