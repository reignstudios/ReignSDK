using Reign.Core;
using System.Collections.Generic;
using System.IO;
using System;

namespace Reign.Video.D3D9
{
	public class Shader : ShaderI
	{
		#region Properties
		private Video video;
		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;

		private VertexShader vertex;
		private PixelShader pixel;
		#endregion

		#region Constructors
		public static Shader New(DisposableI parent, string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new Shader(parent, fileName, shaderVersion, loadedCallback);
		}

		public static Shader New(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new Shader(parent, fileName, shaderVersion, vsQuality, psQuality, loadedCallback);
		}

		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			new StreamLoader(fileName,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(fileName, ((StreamLoader)sender).LoadedStream, shaderVersion, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			new StreamLoader(fileName,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					init(fileName, ((StreamLoader)sender).LoadedStream, shaderVersion, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		private void init(string fileName, Stream stream, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				video = Parent.FindParentOrSelfWithException<Video>();

				var code = getShaders(stream);
				vertex = new VertexShader(this, code[0], (shaderVersion == ShaderVersions.Max) ? video.Caps.MaxVertexShaderVersion : shaderVersion);
				pixel = new PixelShader(this, code[1], (shaderVersion == ShaderVersions.Max) ? video.Caps.MaxPixelShaderVersion : shaderVersion);

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
		#endregion

		#region Methods
		public override void Apply()
		{
			vertex.Apply();
			pixel.Apply();
		
			foreach (ShaderVariable variable in variables)
			{
			    variable.Apply();
			}

			foreach (ShaderResource resource in resources)
			{
			    resource.Apply();
			}
		}

		public override ShaderVariableI Variable(string name)
		{
			// Try to find existing variable
			foreach (ShaderVariable variable in variables)
			{
			    if (variable.Name == name) return variable;
			}
		
			// Otherwise add a variable instance
			var vertexVariable = vertex.Variable(name);
			var pixelVariable = pixel.Variable(name);

			if (vertexVariable == IntPtr.Zero && pixelVariable == IntPtr.Zero)
			{
			    Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			}

			ShaderVariable newVariable = new ShaderVariable(video.com, name, vertexVariable, pixelVariable, vertex.com, pixel.com);
			variables.Add(newVariable);
			return newVariable;
		}

		public override ShaderResourceI Resource(string name)
		{
			// Try to find existing variable
			foreach (ShaderResource resource in resources)
			{
			    if (resource.Name == name) return resource;
			}

			// Otherwise add a variable instance
			int vertexIndex = vertex.Resource(name);
			int pixelIndex = pixel.Resource(name);

			if (vertexIndex == -1 && pixelIndex == -1)
			{
			    Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist.", name));
			}

			ShaderResource newResource = new ShaderResource(video.com, name, vertexIndex, pixelIndex);
			resources.Add(newResource);
			return newResource;
		}
		#endregion
	}
}
