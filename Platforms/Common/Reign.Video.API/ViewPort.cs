﻿using System;
using Reign.Core;
using System.Reflection;

namespace Reign.Video.API
{
	public static class ViewPort
	{
		public static ViewPortI Create(VideoTypes apiType, params object[] args)
		{
			try
			{
				#if WINDOWS || METRO
				if (apiType == VideoTypes.D3D11)
				{
					return (ViewPortI)OS.CreateInstance(Video.D3D11, Video.D3D11, "ViewPort", args);
				}
				#endif

				#if WINDOWS
				if (apiType == VideoTypes.D3D9)
				{
					return (ViewPortI)OS.CreateInstance(Video.D3D9, Video.D3D9, "ViewPort", args);
				}
				#endif

				#if XNA
				if (apiType == VideoTypes.XNA)
				{
					return (ViewPortI)OS.CreateInstance(Video.XNA, Video.XNA, "ViewPort", args);
				}
				#endif

				#if WINDOWS || OSX || LINUX || iOS || ANDROID
				if (apiType == VideoTypes.OpenGL)
				{
					return (ViewPortI)OS.CreateInstance(Video.OpenGL, Video.OpenGL, "ViewPort", args);
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
