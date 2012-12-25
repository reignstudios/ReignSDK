using Reign.Core;

namespace Reign.Video.API
{
	public static class Shader
	{
		public static void Init(VideoTypes type)
		{
			#if WINDOWS || METRO
			if (type == VideoTypes.D3D11) ShaderAPI.Init(Reign.Video.D3D11.Shader.New, Reign.Video.D3D11.Shader.New);
			#endif

			#if WINDOWS || OSX || LINUX || iOS || ANDROID || NaCl
			if (type == VideoTypes.OpenGL) ShaderAPI.Init(Reign.Video.OpenGL.Shader.New, Reign.Video.OpenGL.Shader.New);
			#endif

			#if XNA
			if (type == VideoTypes.XNA) ShaderAPI.Init(Reign.Video.XNA.Shader.New, Reign.Video.XNA.Shader.New);
			#endif
		}
	}
}
