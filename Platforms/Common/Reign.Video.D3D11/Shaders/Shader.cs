using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

#if WINRT || WP8
using System.Threading.Tasks;
#endif

namespace Reign.Video.D3D11
{
	public class Shader : IShader
	{
		#region Properties
		private Video video;
		internal VertexShader vertex;
		private PixelShader pixel;

		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		#if WINRT || WP8
		internal int vsVariableBufferSize, vsResourceCount, psVariableBufferSize, psResourceCount;
		internal List<string> vsVariableNames, vsResourceNames, psVariableNames, psResourceNames;
		internal List<int> vsVariableByteOffsets, vsResourceIndices, psVariableByteOffsets, psResourceIndices;
		#endif
		#endregion

		#region Constructors
		public Shader(IDisposableResource parent, string filename, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			#if WINRT || WP8
			Loader.AddLoadable(this);
			filename = Streams.StripFileExt(filename) + ".mrs";
			#endif
			new StreamLoader(filename,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(filename, ((StreamLoader)sender).LoadedStream, shaderVersion, ShaderFloatingPointQuality.High, ShaderFloatingPointQuality.Low, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		public Shader(IDisposableResource parent, string filename, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			#if WINRT || WP8
			Loader.AddLoadable(this);
			filename = Streams.StripFileExt(filename) + ".mrs";
			#endif
			new StreamLoader(filename,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(filename, ((StreamLoader)sender).LoadedStream, shaderVersion, vsQuality, psQuality, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		#if WINRT || WP8
		private async void init(string filename, Stream stream, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		#else
		private void init(string filename, Stream stream, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		#endif
		{
			try
			{
				video = Parent.FindParentOrSelfWithException<Video>();

				#if WIN32
				shaderVersion = (shaderVersion == ShaderVersions.Max) ? video.Cap.MaxShaderVersion : shaderVersion;
				var code = getShaders(stream);
				vertex = new VertexShader(this, code[0], shaderVersion);
				pixel = new PixelShader(this, code[1], shaderVersion);
				#else
				await getReflections(filename);
				var code = getShaders(stream);
				vertex = new VertexShader(this, code[0]);
				pixel = new PixelShader(this, code[1]);
				#endif

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return;
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}

		#if WINRT || WP8
		private async Task getReflections(string filename)
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

			using (var file = await Streams.OpenFile(Streams.StripFileExt(filename) + ".ref"))
			using (var reader = new System.IO.StreamReader(file))
			{
				var value = reader.ReadLine();
				while (value != null)
				{
					var values = value.Split(' ');

					bool vsVar = false, psVar = false, vsRes = false, psRes = false;
					switch (values[0])
					{
						case "gVar":
							vsVar = true;
							psVar = true;
							break;

						case "gRes":
							vsRes = true;
							psRes = true;
							break;

						case "vsVar": vsVar = true; break;
						case "psVar": psVar = true; break;
						case "vsRes": vsRes = true; break;
						case "psRes": psRes = true; break;
					}

					if (vsVar)
					{
						vsVariableNames.Add(values[1]);
						vsVariableByteOffsets.Add(int.Parse(values[2]));
						vsVariableBufferSize += int.Parse(values[3]);
					}

					if (psVar)
					{
						psVariableNames.Add(values[1]);
						psVariableByteOffsets.Add(int.Parse(values[2]));
						psVariableBufferSize += int.Parse(values[3]);
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

					value = reader.ReadLine();
				}
			}

			// make sure padding is a multiple of 16
			int percent = vsVariableBufferSize % 16;
			if (percent != 0) vsVariableBufferSize += 16 - percent;
			percent = psVariableBufferSize % 16;
			if (percent != 0) psVariableBufferSize += 16 - percent;
		}
		#endif
		#endregion

		#region Methods
		public override void Apply()
		{
			vertex.Apply();
			pixel.Apply();
		}

		public override IShaderVariable Variable(string name)
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

		public override IShaderResource Resource(string name)
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
