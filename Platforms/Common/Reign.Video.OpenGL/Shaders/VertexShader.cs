using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.OpenGL
{
	public class VertexShader : ShaderModel
	{
		#region Constructors
		public VertexShader(ShaderI shader, string code, ShaderVersions shaderVersion)
		: base(shader, code, shaderVersion, ShaderTypes.VS)
		{

		}
		#endregion
	}
}