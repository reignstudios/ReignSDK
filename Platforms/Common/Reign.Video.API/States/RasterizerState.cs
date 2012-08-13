using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class RasterizerStateDesc
	{
		public static RasterizerStateDescI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D10)
				{
					return (RasterizerStateDescI)OS.CreateInstance(Video.D3D10, Video.D3D10, "RasterizerStateDesc", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (RasterizerStateDescI)OS.CreateInstance(Video.D3D9, Video.D3D9, "RasterizerStateDesc", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (RasterizerStateDescI)OS.CreateInstance(Video.XNA, Video.XNA, "RasterizerStateDesc", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (RasterizerStateDescI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "RasterizerStateDesc", args);
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

	public static class RasterizerState
	{
		public static RasterizerStateI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D10)
				{
					return (RasterizerStateI)OS.CreateInstance(Video.D3D10, Video.D3D10, "RasterizerState", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (RasterizerStateI)OS.CreateInstance(Video.D3D9, Video.D3D9, "RasterizerState", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (RasterizerStateI)OS.CreateInstance(Video.XNA, Video.XNA, "RasterizerState", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (RasterizerStateI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "RasterizerState", args);
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
