using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class DepthStencilStateDesc
	{
		public static DepthStencilStateDescI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (DepthStencilStateDescI)OS.CreateInstance(Video.D3D11, Video.D3D11, "DepthStencilStateDesc", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (DepthStencilStateDescI)OS.CreateInstance(Video.D3D9, Video.D3D9, "DepthStencilStateDesc", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (DepthStencilStateDescI)OS.CreateInstance(Video.XNA, Video.XNA, "DepthStencilStateDesc", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (DepthStencilStateDescI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "DepthStencilStateDesc", args);
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

	public static class DepthStencilState
	{
		public static DepthStencilStateI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (DepthStencilStateI)OS.CreateInstance(Video.D3D11, Video.D3D11, "DepthStencilState", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (DepthStencilStateI)OS.CreateInstance(Video.D3D9, Video.D3D9, "DepthStencilState", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (DepthStencilStateI)OS.CreateInstance(Video.XNA, Video.XNA, "DepthStencilState", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (DepthStencilStateI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "DepthStencilState", args);
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
