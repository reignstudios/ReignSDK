using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.XNA
{
	public class Shader : ShaderI
	{
		#region Properties
		private Effect effect;
		private EffectPass pass;
		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		private bool loadedFromContentManager;
		#endregion

		#region Constructors
		public static Shader New(DisposableI parent, string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return new Shader(parent, fileName, shaderVersion, loadedCallback, failedToLoadCallback);
		}

		public static Shader New(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return new Shader(parent, fileName, shaderVersion, vsQuality, psQuality, loadedCallback, failedToLoadCallback);
		}

		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(parent)
		{
			init(fileName, shaderVersion, loadedCallback, failedToLoadCallback);
		}

		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		: base(parent)
		{
			init(fileName, shaderVersion, loadedCallback, failedToLoadCallback);
		}

		private void init(string fileName, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			try
			{
				effect = Parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<Effect>(Streams.StripFileExt(fileName));
				loadedFromContentManager = true;
				pass = effect.CurrentTechnique.Passes[0];

				variables = new List<ShaderVariable>();
				resources = new List<ShaderResource>();
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (failedToLoadCallback != null) failedToLoadCallback();
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this);
		}

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
			pass.Apply();
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
