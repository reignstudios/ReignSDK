using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class Mesh
	{
		public static MeshI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (MeshI)OS.CreateInstance(Video.D3D11, Video.D3D11, "Mesh", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (MeshI)OS.CreateInstance(Video.D3D9, Video.D3D9, "Mesh", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (MeshI)OS.CreateInstance(Video.XNA, Video.XNA, "Mesh", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (MeshI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "Mesh", args);
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
