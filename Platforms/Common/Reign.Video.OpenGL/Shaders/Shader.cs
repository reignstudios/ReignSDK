using System;
using Reign.Core;
using System.Collections.Generic;

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
		public static Shader New(DisposableI parent, string fileName, ShaderVersions shaderVersion)
		{
			Shader shader = parent.FindChild<Shader>("New",
				new ConstructorParam(typeof(DisposableI), parent),
				new ConstructorParam(typeof(string), fileName),
				new ConstructorParam(typeof(ShaderVersions), shaderVersion));

			if (shader != null)
			{
				++shader.referenceCount;
				return shader;
			}

			return new Shader(parent, fileName, shaderVersion);
		}

		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				shaderVersion = (shaderVersion == ShaderVersions.Max) ? this.video.Caps.MaxShaderVersion : shaderVersion;

				var code = getShaders(fileName);
				vertex = new VertexShader(this, code[0], shaderVersion);
				pixel = new PixelShader(this, code[1], shaderVersion);

				Program = GL.CreateProgram();
				if (Program == 0) Debug.ThrowError("Shader", "Failed to create shader program");
				GL.AttachShader(Program, vertex.Shader);
				GL.AttachShader(Program, pixel.Shader);
				GL.LinkProgram(Program);

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();

				Video.checkForError();
			}
			catch (Exception ex)
			{
				Dispose();
				throw new Exception(ex.Message + " - File: " + fileName);
			}
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