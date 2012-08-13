using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.XNA
{
	#if WP7
	public enum SimpleShaderTypes
	{
		Basic,
		DualTexture,
		AlphaTest
	}
	#endif

	public class Shader : ShaderI
	{
		#region Properties
		private Video video;
		private Effect effect;
		#if WP7
		public SimpleShaderTypes Type {get; private set;}
		#else
		private EffectPass pass;
		#endif
		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		private bool loadedFromContentManager;
		#endregion

		#region Constructors
		#if WP7
		public Shader(DisposableI parent, SimpleShaderTypes type, ShaderVersions shaderVersion)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				Type = type;

				switch (type)
				{
					case (SimpleShaderTypes.Basic): effect = new BasicEffect(video.Device); break;
					case (SimpleShaderTypes.DualTexture): effect = new DualTextureEffect(video.Device); break;
					case (SimpleShaderTypes.AlphaTest): effect = new AlphaTestEffect(video.Device); break;
				}

				FX = effect;
				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
		}
		#else
		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion)
		: base(parent)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();

				effect = parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<Effect>(Streams.StripFileExt(fileName));
				loadedFromContentManager = true;
				pass = effect.CurrentTechnique.Passes[0];

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
		}
		#endif

		public override void Dispose()
		{
			disposeChilderen();
			if (effect != null && !loadedFromContentManager)
			{
				effect.Dispose();
				effect = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			#if WP7
			effect.CurrentTechnique.Passes[0].Apply();
			#else
			pass.Apply();
			#endif
		}

		public override ShaderVariableI Variable(string name)
		{
			// Try to find existing variable
			foreach (var variable in variables)
			{
				if (variable.Name == name) return variable;
			}

			// Otherwise add a variable instance
			var parameter = effect.Parameters[name];
			if (parameter == null)
			{
				Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			}

			var variableOut = new ShaderVariable(parameter, name);
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
			var parameter = effect.Parameters[name];
			if (parameter == null)
			{
				Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist", name));
			}

			var resourceOut = new ShaderResource(parameter, name);
			resources.Add(resourceOut);
			return resourceOut;
		}
		#endregion
	}
}
