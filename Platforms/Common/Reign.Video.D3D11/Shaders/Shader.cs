using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	class ShaderStreamLoader : StreamLoaderI
	{
		private Shader shader;
		private string fileName;
		private ShaderVersions shaderVersion;

		public ShaderStreamLoader(Shader shader, string fileName, ShaderVersions shaderVersion)
		{
			this.shader = shader;
			this.fileName = fileName;
			this.shaderVersion = shaderVersion;
		}

		public override bool Load()
		{
			shader.load(fileName, shaderVersion);
			return true;
		}
	}

	public class Shader : ShaderI
	{
		#region Properties
		private Video video;
		internal VertexShader vertex;
		private PixelShader pixel;

		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		#if METRO
		internal int vsVariableBufferSize, vsResourceCount, psVariableBufferSize, psResourceCount;
		internal List<string> vsVariableNames, vsResourceNames, psVariableNames, psResourceNames;
		internal List<int> vsVariableByteOffsets, vsResourceIndices, psVariableByteOffsets, psResourceIndices;
		#endif
		#endregion

		#region Constructors
		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			new ShaderStreamLoader(this, fileName, shaderVersion);
		}

		#if METRO
		internal async void load(string fileName, ShaderVersions shaderVersion)
		#else
		internal void load(string fileName, ShaderVersions shaderVersion)
		#endif
		{
			try
			{
				#if WINDOWS
				shaderVersion = (shaderVersion == ShaderVersions.Max) ? video.Cap.MaxShaderVersion : shaderVersion;
				var code = getShaders(fileName);
				vertex = new VertexShader(this, code[0], shaderVersion);
				pixel = new PixelShader(this, code[1], shaderVersion);
				#else
				getReflections(fileName);
				var code = await getShaders(fileName);
				vertex = new VertexShader(this, code[0]);
				pixel = new PixelShader(this, code[1]);
				#endif

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}

			Loaded = true;
		}

		#if METRO
		private async void getReflections(string fileName)
		{
			vsVariableNames = new List<string>();
			vsVariableByteOffsets = new List<int>();
			vsResourceNames = new List<string>();
			vsResourceIndices = new List<int>();

			psVariableNames = new List<string>();
			psVariableByteOffsets = new List<int>();
			psResourceNames = new List<string>();
			psResourceIndices = new List<int>();

			vsVariableBufferSize = 0;
			vsResourceCount = 0;
			psVariableBufferSize = 0;
			psResourceCount = 0;

			using (var file = await Streams.OpenFile(Streams.StripFileExt(fileName) + ".ref"))
			using (var reader = new System.IO.StreamReader(file))
			{
				var values = reader.ReadLine().Split(' ');
				bool vsVar = false, psVar = false, vsRes = false, psRes = false;
				switch (values[0])
				{
					case ("gVar"):
						vsVar = true;
						psVar = true;
						break;

					case ("gRes"):
						vsRes = true;
						psRes = true;
						break;

					case ("vsVar"): vsVar = true; break;
					case ("psVar"): vsVar = true; break;
					case ("vsRes"): vsRes = true; break;
					case ("psRes"): vsRes = true; break;
				}

				if (vsVar)
				{
					vsVariableNames.Add(values[1]);
					int size = int.Parse(values[2]);
					vsVariableByteOffsets.Add(size);
					vsVariableBufferSize += size;
				}

				if (psVar)
				{
					psVariableNames.Add(values[1]);
					int size = int.Parse(values[2]);
					psVariableByteOffsets.Add(size);
					psVariableBufferSize += size;
				}

				if (vsRes)
				{
					vsResourceNames.Add(values[1]);
					vsResourceIndices.Add(int.Parse(values[2]));
					++vsResourceCount;
				}
				
				if (psRes)
				{
					psResourceNames.Add(values[1]);
					psResourceIndices.Add(int.Parse(values[2]));
					++psResourceCount;
				}
			}
		}
		#endif
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
