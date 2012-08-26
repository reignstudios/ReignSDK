using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class Shader
	{
		public static ShaderI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS
				if (apiType == VideoTypes.D3D11)
				{
					return (ShaderI)OS.CreateInstance(Video.D3D11, Video.D3D11, "Shader", args);
				}

				if (apiType == VideoTypes.D3D9)
				{
					return (ShaderI)OS.CreateInstance(Video.D3D9, Video.D3D9, "Shader", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (ShaderI)OS.CreateInstance(Video.XNA, Video.XNA, "Shader", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (ShaderI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "Shader", args);
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
