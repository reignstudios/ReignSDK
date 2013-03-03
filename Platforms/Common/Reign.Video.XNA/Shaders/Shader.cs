using Microsoft.Xna.Framework.Graphics;
#if !SILVERLIGHT
using Microsoft.Xna.Framework.Content;
#endif
using System;
using Reign.Core;
using System.Collections.Generic;
using System.IO;

namespace Reign.Video.XNA
{
	public class Shader : ShaderI
	{
		#region Properties
		#if SILVERLIGHT
		private Video video;
		private VertexShader vertex;
		private PixelShader pixel;
		#else
		private Effect effect;
		private EffectPass pass;
		#endif
		private List<ShaderVariable> variables;
		private List<ShaderResource> resources;
		#if SILVERLIGHT
		internal List<string> vsVariableNames, vsResourceNames, psVariableNames, psResourceNames;
		internal List<int> vsVariableRegistries, vsResourceIndices, psVariableRegistries, psResourceIndices;
		#else
		private bool loadedFromContentManager;
		#endif
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
			#if SILVERLIGHT
			new StreamLoader(Streams.StripFileExt(fileName) + ".mrs",
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
			#else
			init(fileName, null, shaderVersion, loadedCallback);
			#endif
		}

		public Shader(DisposableI parent, string fileName, ShaderVersions shaderVersion, ShaderFloatingPointQuality vsQuality, ShaderFloatingPointQuality psQuality, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			#if SILVERLIGHT
			new StreamLoader(Streams.StripFileExt(fileName) + ".mrs",
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
			#else
			init(fileName, null, shaderVersion, loadedCallback);
			#endif
		}

		private void init(string fileName, Stream stream, ShaderVersions shaderVersion, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				#if SILVERLIGHT
				video = Parent.FindParentOrSelfWithException<Video>();

				getReflections(fileName);
				var code = getShaders(stream);
				vertex = new VertexShader(video.Device, code[0]);
				pixel = new PixelShader(video.Device, code[1]);
				#else
				effect = Parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<Effect>(Streams.StripFileExt(fileName));
				loadedFromContentManager = true;
				pass = effect.CurrentTechnique.Passes[0];
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
			}

			Loaded = true;
			if (loadedCallback != null) loadedCallback(this, true);
		}

		#if SILVERLIGHT
		private void getReflections(string fileName)
		{
			vsVariableNames = new List<string>();
			vsVariableRegistries = new List<int>();
			vsResourceNames = new List<string>();
			vsResourceIndices = new List<int>();

			psVariableNames = new List<string>();
			psVariableRegistries = new List<int>();
			psResourceNames = new List<string>();
			psResourceIndices = new List<int>();

			using (var file = Streams.OpenFile(Streams.StripFileExt(fileName) + ".ref"))
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
						vsVariableRegistries.Add(int.Parse(values[2]));
					}

					if (psVar)
					{
						psVariableNames.Add(values[1]);
						psVariableRegistries.Add(int.Parse(values[2]));
					}

					if (vsRes)
					{
						Debug.ThrowError("Shader", "Vertex resources are not supported in Silverlight");
						vsResourceNames.Add(values[1]);
						vsResourceIndices.Add(int.Parse(values[2]));
					}
				
					if (psRes)
					{
						psResourceNames.Add(values[1]);
						psResourceIndices.Add(int.Parse(values[2]));
					}

					value = reader.ReadLine();
				}
			}
		}
		#endif

		public override void Dispose()
		{
			disposeChilderen();
			#if SILVERLIGHT
			if (vertex != null)
			{
				vertex.Dispose();
				vertex = null;
			}
			if (pixel != null)
			{
				pixel.Dispose();
				pixel = null;
			}
			#else
			if (effect != null && !loadedFromContentManager)
			{
				effect.Dispose();
				effect = null;
			}
			#endif
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Apply()
		{
			#if SILVERLIGHT
			video.Device.SetVertexShader(vertex);
			video.Device.SetPixelShader(pixel);
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
			#if SILVERLIGHT
			int vsRegisterIndex = -1, psRegisterIndex = -1;
			for (int i = 0; i != vsVariableNames.Count; ++i)
			{
				if (vsVariableNames[i] == name)
				{
					vsRegisterIndex = vsVariableRegistries[i];
					break;
				}
			}

			for (int i = 0; i != psVariableNames.Count; ++i)
			{
				if (psVariableNames[i] == name)
				{
					psRegisterIndex = psVariableRegistries[i];
					break;
				}
			}

			if (vsRegisterIndex == -1 && psRegisterIndex == -1)
			{
				Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			}

			var variableOut = new ShaderVariable(video, vsRegisterIndex, psRegisterIndex, name);
			variables.Add(variableOut);
			return variableOut;
			#else
			var parameter = effect.Parameters[name];
			if (parameter == null)
			{
				Debug.ThrowError("Shader", string.Format("Shader variable '{0}' does not exist", name));
			}

			var variableOut = new ShaderVariable(parameter, name);
			variables.Add(variableOut);
			return variableOut;
			#endif
		}

		public override ShaderResourceI Resource(string name)
		{
			// Try to find existing variable
			foreach (var resource in resources)
			{
				if (resource.Name == name) return resource;
			}

			// Otherwise add a variable instance
			#if SILVERLIGHT
			int resourceIndex = -1;
			for (int i = 0; i != psResourceNames.Count; ++i)
			{
				if (psResourceNames[i] == name)
				{
					resourceIndex = psResourceIndices[i];
					break;
				}
			}

			if (resourceIndex == -1)
			{
				Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist", name));
			}

			var resourceOut = new ShaderResource(video, resourceIndex, name);
			resources.Add(resourceOut);
			return resourceOut;
			#else
			var parameter = effect.Parameters[name];
			if (parameter == null)
			{
				Debug.ThrowError("Shader", string.Format("Shader resource '{0}' does not exist", name));
			}

			var resourceOut = new ShaderResource(parameter, name);
			resources.Add(resourceOut);
			return resourceOut;
			#endif
		}
		#endregion
	}
}
