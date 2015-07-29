using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
{
	public class VertexShader : ShaderModel
	{
		#region Constructors
		public VertexShader(IShader shader, string code, ShaderVersions shaderVersion)
		: base(shader, code, shaderVersion, ShaderTypes.VS, ShaderFloatingPointQuality.High)
		{

		}
		
		public VertexShader(IShader shader, string code, ShaderVersions shaderVersion, ShaderFloatingPointQuality quality)
		: base(shader, code, shaderVersion, ShaderTypes.VS, quality)
		{
			
		}
		#endregion
	}
}