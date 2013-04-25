using System;
using Reign.Core;

namespace Reign.Video.API
{
	[Flags]
	public enum VideoTypes
	{
		None = 0,
		D3D11 = 1,
		D3D9 = 4,
		XNA = 8,
		Vita = 16,
		OpenGL = 32
	}

	public static class Video
	{
		public static VideoI Init(VideoTypes typeFlags, out VideoTypes type, DisposableI parent, ApplicationI application, DepthStencilFormats depthStencilFormats, bool vSync)
		{
			bool d3d11 = (typeFlags & VideoTypes.D3D11) != 0;
			bool d3d9 = (typeFlags & VideoTypes.D3D9) != 0;
			bool gl = (typeFlags & VideoTypes.OpenGL) != 0;
			bool xna = (typeFlags & VideoTypes.XNA) != 0;
			bool vita = (typeFlags & VideoTypes.Vita) != 0;

			type = VideoTypes.None;
			Exception lastException = null;
			VideoI video = null;
			while (true)
			{
				try
				{
					#if WIN32 || WINRT || WP8
					if (d3d11)
					{
						d3d11 = false;
						type = VideoTypes.D3D11;
						video = new Reign.Video.D3D11.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					#if WIN32
					else if (d3d9)
					{
					    d3d9 = false;
					    type = VideoTypes.D3D9;
					    video = new Reign.Video.D3D9.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					#if WIN32 || OSX || LINUX || NaCl || iOS || ANDROID
					if (gl)
					{
						gl = false;
						type = VideoTypes.OpenGL;
						video = new Reign.Video.OpenGL.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif

					#if XNA
					if (xna)
					{
						xna = false;
						type = VideoTypes.XNA;
						video = new Reign.Video.XNA.Video(parent, application, depthStencilFormats, vSync);
						break;
					}
					#endif
				
					#if VITA
					if (vita)
					{
						vita = false;
						type = VideoTypes.Vita;
						video = new Reign.Video.Vita.Video(parent, application, depthStencilFormats, vSync);
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
	}
}
