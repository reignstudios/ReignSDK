using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class BlendStateDesc
	{
		public static BlendStateDescI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (BlendStateDescI)OS.CreateInstance(Video.D3D11, Video.D3D11, "BlendStateDesc", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (BlendStateDescI)OS.CreateInstance(Video.D3D9, Video.D3D9, "BlendStateDesc", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (BlendStateDescI)OS.CreateInstance(Video.XNA, Video.XNA, "BlendStateDesc", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (BlendStateDescI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "BlendStateDesc", args);
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

	public static class BlendState
	{
		public static BlendStateI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (BlendStateI)OS.CreateInstance(Video.D3D11, Video.D3D11, "BlendState", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (BlendStateI)OS.CreateInstance(Video.D3D9, Video.D3D9, "BlendState", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (BlendStateI)OS.CreateInstance(Video.XNA, Video.XNA, "BlendState", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (BlendStateI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "BlendState", args);
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
