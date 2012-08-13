using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	public class Shader : ShaderI
	{
		#region Properties
		private Video video;
		internal VertexShader vertex;
		private PixelShader pixel;

		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		#endregion

		#region Constructors
		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				shaderVersion = (shaderVersion == ShaderVersions.Max) ? video.Cap.MaxShaderVersion : shaderVersion;

				var code = getShaders(fileName);
				vertex = new VertexShader(this, code[0], shaderVersion);
				pixel = new PixelShader(this, code[1], shaderVersion);

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			vertex.Apply();
			pixel.Apply();
		}

		public override ShaderVariableI Variable(string name)
		{
			// Try to find existing variable
			foreach (var variable in variables)
			{
				if (variable.Name == name) return variable;
			}

			// Otherwise add a variable instance
			int vertexOffset = vertex.Variable(name);
			int pixelOffset = pixel.Variable(name);

			if (vertexOffset == -1 && pixelOffset == -1)
			{
				Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			}

			var newVariable = new ShaderVariable(name, vertex.com, pixel.com, vertexOffset, pixelOffset);
			variables.Add(newVariable);
			return newVariable;
		}

		public override ShaderResourceI Resource(string name)
		{
			// Try to find existing resource
			foreach (var resource in resources)
			{
				if (resource.Name == name) return resource;
			}

			// Otherwise add a resource instance
			int vertexIndex = vertex.Resource(name);
			int pixelIndex = pixel.Resource(name);

			if (pixelIndex == -1 && pixelIndex == -1)
			{
				Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist.", name));
			}

			var newResource = new ShaderResource(name, video.com, vertex.com, pixel.com, vertexIndex, pixelIndex);
			resources.Add(newResource);
			return newResource;
		}
		#endregion
	}
}
