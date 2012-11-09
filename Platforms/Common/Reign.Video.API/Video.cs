using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	[Flags]
	public enum VideoTypes
	{
		None = 0,
		D3D11 = 1,
		D3D9 = 2,
		XNA = 4,
		OpenGL = 8
	}

	public static class Video
	{
		internal const string D3D11 = "Reign.Video.D3D11";
		internal const string D3D9 = "Reign.Video.D3D9";
		internal const string XNA = "Reign.Video.XNA";
		internal const string OpenGL = "Reign.Video.OpenGL";

		#if XNA || iOS || ANDROID
		public static VideoI Create(VideoTypes typeFlags, out VideoTypes type, params object[] args)
		{
			try
			{
				#if XNA
				type = VideoTypes.XNA;
				return (VideoI)OS.CreateInstance(XNA, XNA, "Video", args);
				#endif

				#if iOS || ANDROID
				type = VideoTypes.OpenGL;
				return (VideoI)OS.CreateInstance(typeof(Reign.Video.OpenGL.Video), args);
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
		}
		#endif

		#if WINDOWS || METRO || OSX || LINUX
		public static VideoI Create(VideoTypes typeFlags, out VideoTypes type, params object[] args)
		{
			#if WINDOWS || METRO
			bool d3d11 = (typeFlags & VideoTypes.D3D11) != 0;
			#endif

			#if WINDOWS
			bool d3d9 = (typeFlags & VideoTypes.D3D9) != 0;
			#endif

			#if WINDOWS || OSX || LINUX
			bool gl = (typeFlags & VideoTypes.OpenGL) != 0;
			#endif

			Exception lastException = null;
			while (true)
			{
				try
				{
					#if WINDOWS || METRO
					if (d3d11)
					{
						d3d11 = false;
						type = VideoTypes.D3D11;
						return (VideoI)OS.CreateInstance(typeof(Reign.Video.D3D11.Video), args);
					}
					#endif

					#if WINDOWS
					else if (d3d9)
					{
						d3d9 = false;
						type = VideoTypes.D3D9;
						return (VideoI)OS.CreateInstance(typeof(Reign.Video.D3D9.Video), args);
					}
					#endif

					#if WINDOWS || OSX || LINUX
					if (gl)
					{
						gl = false;
						type = VideoTypes.OpenGL;
						#if WINDOWS
						return (VideoI)OS.CreateInstance(typeof(Reign.Video.OpenGL.Video), args);
						#else
						return (VideoI)OS.CreateInstance(typeof(Reign.Video.OpenGL.Video), args);
						#endif
					}
					#endif

					else break;
				}
				catch (TargetInvocationException e)
				{
					if (e.InnerException != null) lastException = e.InnerException;
					else lastException = e;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
			Debug.ThrowError("Video", "Failed to create Video API" + ex);
			type = VideoTypes.None;
			return null;
		}
		#endif
	}
}
