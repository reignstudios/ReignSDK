using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.OpenGL
{
	public class Shader : ShaderI
	{
		#region Properties
		private Video video;
		private VertexShader vertex;
		private PixelShader pixel;
		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		public uint Program {get; private set;}
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
					init(((StreamLoader)sender).LoadedStream, shaderVersion, ShaderFloatingPointQuality.High, ShaderFloatingPointQuality.Low, loadedCallback);
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
					init(((StreamLoader)sender).LoadedStream, shaderVersion, vsQuality, psQuality, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		private void init(Stream stream, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				video = Parent.FindParentOrSelfWithException<Video>();
				shaderVersion = (shaderVersion == ShaderVersions.Max) ? this.video.Caps.MaxShaderVersion : shaderVersion;

				var code = getShaders(stream);
				vertex = new VertexShader(this, code[0], shaderVersion, vsQuality);
				pixel = new PixelShader(this, code[1], shaderVersion, psQuality);

				Program = GL.CreateProgram();
				if (Program == 0) Debug.ThrowError("Shader", "Failed to create shader program");
				GL.AttachShader(Program, vertex.Shader);
				GL.AttachShader(Program, pixel.Shader);
				GL.LinkProgram(Program);

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();

				Video.checkForError();
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
			if (Program != 0)
			{
				if (!OS.AutoDisposedGL)
				{
					GL.UseProgram(0);
					GL.DeleteProgram(Program);
				}
				Program = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			GL.UseProgram(Program);
			
			foreach (var variable in variables)
			{
				variable.Apply();
			}

			#if DEBUG
			Video.checkForError();
			#endif

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
			int uniform = GL.GetUniformLocation(Program, name);
			if (uniform == -1)
			{
				Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			}

			var variableOut = new ShaderVariable(uniform, name);
			variables.Add(variableOut);
			return variableOut;
		}

		public override ShaderResourceI Resource(string name)
		{
			// Try to find existing variable
			foreach (var resource in resources)
			{
				if (resource.Name == name) return resource;
			}

			// Otherwise add a variable instance
			int uniform = GL.GetUniformLocation(Program, name);
			if (uniform == -1)
			{
				Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist", name));
			}

			var resourceOut = new ShaderResource(video, uniform, resources.Count, name);
			resources.Add(resourceOut);
			return resourceOut;
		}
		#endregion
	}
}