using System;
using Reign.Core;

namespace Reign.Video.API
{
	[Flags]
	public enum VideoTypes
	{
		None,
		D3D11,
		D3D9,
		XNA,
		OpenGL
	}

	public static class Video
	{
		#if METRO || XNA || iOS || ANDROID
		public static VideoI Init(VideoTypes typeFlags, out VideoTypes type, DisposableI parent, Application application, bool vSync)
		{
			try
			{
				#if METRO
				type = VideoTypes.D3D11;
				var video = new Reign.Video.D3D11.Video(parent, application, vSync);
				initMethods(type);
				return video;
				#endif

				#if XNA
				type = VideoTypes.XNA;
				var video = new Reign.Video.XNA.Video(parent, application);
				initMethods(type);
				return video;
				#endif

				#if iOS || ANDROID
				type = VideoTypes.OpenGL;
				var video = new Reign.Video.OpenGL.Video(parent, application);
				initMethods(type);
				return video;
				#endif
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		private static void initMethods(VideoTypes type)
		{
			ViewPort.Init(type);
			Shader.Init(type);
			QuickDraw.Init(type);
			DepthStencil.Init(type);
			Texture2D.Init(type);
			RenderTarget.Init(type);
			BlendState.Init(type);
			BlendStateDesc.Init(type);
			DepthStencilState.Init(type);
			DepthStencilStateDesc.Init(type);
			RasterizerState.Init(type);
			RasterizerStateDesc.Init(type);
			SamplerState.Init(type);
			SamplerStateDesc.Init(type);
			BufferLayout.Init(type);
			BufferLayoutDesc.Init(type);
			IndexBuffer.Init(type);
			VertexBuffer.Init(type);
		}
		#else
		public static VideoI Init(VideoTypes typeFlags, out VideoTypes type, DisposableI parent, Window window, bool vSync)
		{
			#if WINDOWS
			bool d3d11 = (typeFlags & VideoTypes.D3D11) != 0;
			#endif

			#if WINDOWS
			bool d3d9 = (typeFlags & VideoTypes.D3D9) != 0;
			#endif

			#if WINDOWS || OSX || LINUX
			bool gl = (typeFlags & VideoTypes.OpenGL) != 0;
			#endif

			type = VideoTypes.None;
			Exception lastException = null;
			VideoI video = null;
			while (true)
			{
				try
				{
					#if WINDOWS || METRO
					if (d3d11)
					{
						d3d11 = false;
						type = VideoTypes.D3D11;
						video = new Reign.Video.D3D11.Video(parent, window, vSync);
						break;
					}
					#endif

					#if WINDOWS
					//else if (d3d9)
					//{
					//    d3d9 = false;
					//    type = VideoTypes.D3D9;
					//    return (VideoI)OS.CreateInstance(typeof(Reign.Video.D3D9.Video), args);
					//break;
					//}
					#endif

					#if WINDOWS || OSX || LINUX
					if (gl)
					{
						gl = false;
						type = VideoTypes.OpenGL;
						video = new Reign.Video.OpenGL.Video(parent, window, vSync);
						break;
					}
					#endif

					else break;
				}
				catch (Exception e)
				{
					lastException = e;
				}
			}

			// check for error
			if (lastException != null)
			{
				string ex = lastException == null ? "" : " - Exception: " + lastException.Message;
				Debug.ThrowError("Video", "Failed to create Video API" + ex);
				type = VideoTypes.None;
			}

			// init api methods
			ViewPort.Init(type);
			Shader.Init(type);
			QuickDraw.Init(type);
			DepthStencil.Init(type);
			Texture2D.Init(type);
			RenderTarget.Init(type);
			BlendState.Init(type);
			BlendStateDesc.Init(type);
			DepthStencilState.Init(type);
			DepthStencilStateDesc.Init(type);
			RasterizerState.Init(type);
			RasterizerStateDesc.Init(type);
			SamplerState.Init(type);
			SamplerStateDesc.Init(type);
			BufferLayout.Init(type);
			BufferLayoutDesc.Init(type);
			IndexBuffer.Init(type);
			VertexBuffer.Init(type);

			return video;
		}
		#endif
	}
}
