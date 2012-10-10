using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
{
	public class PixelShader : ShaderModel
	{
		#region Constructors
		public PixelShader(ShaderI shader, string code, ShaderVersions shaderVersion)
		: base(shader, code, shaderVersion, ShaderTypes.PS, ShaderFloatingPointQuality.Low)
		{
			
		}
		
		public PixelShader(ShaderI shader, string code, ShaderVersions shaderVersion, ShaderFloatingPointQuality quality)
		: base(shader, code, shaderVersion, ShaderTypes.PS, quality)
		{
			
		}
		#endregion
	}
}