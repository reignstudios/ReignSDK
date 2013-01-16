using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;
using System.Collections.Generic;

namespace Reign.Video.Vita
{
	public class Shader : ShaderI
	{
		#region Properties
		private Video video;
		private ShaderProgram program;
		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
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
			init(fileName, shaderVersion, ShaderFloatingPointQuality.High, ShaderFloatingPointQuality.Low, loadedCallback);
		}
		
		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(fileName, shaderVersion, vsQuality, psQuality, loadedCallback);
		}

		private void init(string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				video = Parent.FindParentOrSelfWithException<Video>();
				program = new ShaderProgram(fileName);
				
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

		public override void Dispose()
		{
			disposeChilderen();
			if (program != null)
			{
				program.Dispose();
				program = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			video.context.SetShaderProgram(program);
			
			foreach (var variable in variables)
			{
				variable.Apply();
			}

			foreach (var resource in resources)
			{
				resource.Apply();
			}
		}

		public override ShaderVariableI Variable(string name)
		{
			// Try to find existing variable
			foreach (var variable in variables)
			{
				if (variable.Name == name) return variable;
			}

			// Otherwise add a variable instance
			for (int i = 0; i != program.UniformCount; ++i)
			{
				if (program.GetUniformName(i) == name)
				{
					var newVariable = new ShaderVariable(program, i, name);
					variables.Add(newVariable);
					return newVariable;
				}
			}
			
			Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			return null;
		}

		public override ShaderResourceI Resource(string name)
		{
			// Try to find existing variable
			foreach (var resource in resources)
			{
				if (resource.Name == name) return resource;
			}

			// Otherwise add a variable instance
			for (int i = 0; i != program.UniformCount; ++i)
			{
				if (program.GetUniformName(i) == name)
				{
					var newResource = new ShaderResource(video, i, name);
					resources.Add(newResource);
					return newResource;
				}
			}
			
			Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist", name));
			return null;
		}
		#endregion
	}
}