using Reign.Core;

namespace Reign.Video.API
{
	public static class Texture2D
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS
			if (type == VideoTypes.D3D9) Texture2DAPI.Init(Reign.Video.D3D9.Texture2D.NewReference, Reign.Video.D3D9.Texture2D.New, Reign.Video.D3D9.Texture2D.New, Reign.Video.D3D9.Texture2D.New);
			#endif

			#if WINDOWS || METRO || WP8
			if (type == VideoTypes.D3D11) Texture2DAPI.Init(Reign.Video.D3D11.Texture2D.NewReference, Reign.Video.D3D11.Texture2D.New, Reign.Video.D3D11.Texture2D.New, Reign.Video.D3D11.Texture2D.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) Texture2DAPI.Init(Reign.Video.OpenGL.Texture2D.NewReference, Reign.Video.OpenGL.Texture2D.New, Reign.Video.OpenGL.Texture2D.New, Reign.Video.OpenGL.Texture2D.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) Texture2DAPI.Init(Reign.Video.XNA.Texture2D.NewReference, Reign.Video.XNA.Texture2D.New, Reign.Video.XNA.Texture2D.New, Reign.Video.XNA.Texture2D.New);
			#endif
			
			#if VITA
			if (type == VideoTypes.Vita) Texture2DAPI.Init(Reign.Video.Vita.Texture2D.NewReference, Reign.Video.Vita.Texture2D.New, Reign.Video.Vita.Texture2D.New, Reign.Video.Vita.Texture2D.New);
			#endif
		}
	}
}
