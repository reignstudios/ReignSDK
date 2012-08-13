using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class SamplerStateDesc
	{
		public static SamplerStateDescI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D10)
				{
					return (SamplerStateDescI)OS.CreateInstance(Video.D3D10, Video.D3D10, "SamplerStateDesc", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (SamplerStateDescI)OS.CreateInstance(Video.D3D9, Video.D3D9, "SamplerStateDesc", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (SamplerStateDescI)OS.CreateInstance(Video.XNA, Video.XNA, "SamplerStateDesc", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (SamplerStateDescI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "SamplerStateDesc", args);
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

	public static class SamplerState
	{
		public static SamplerStateI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D10)
				{
					return (SamplerStateI)OS.CreateInstance(Video.D3D10, Video.D3D10, "SamplerState", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (SamplerStateI)OS.CreateInstance(Video.D3D9, Video.D3D9, "SamplerState", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (SamplerStateI)OS.CreateInstance(Video.XNA, Video.XNA, "SamplerState", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (SamplerStateI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "SamplerState", args);
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
