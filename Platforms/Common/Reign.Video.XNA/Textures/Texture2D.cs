using System;
using X = Microsoft.Xna.Framework.Graphics;
#if !SILVERLIGHT
using Microsoft.Xna.Framework.Content;
#endif
using Reign.Core;
using System.IO;
using System.Collections.Generic;

namespace Reign.Video.XNA
{
	public class Texture2D : Disposable, Texture2DI
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}

		public Size2 Size {get; private set;}
		public Vector2 SizeF {get; private set;}
		public Vector2 TexelOffset {get; private set;}
		public int PixelByteSize {get; private set;}

		protected Video video;
		internal X.Texture2D texture;
		private bool loadedFromContentManager;
		#endregion

		#region Constructors
		public static Texture2D NewReference(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback)
		{
			var texture = parent.FindChild<Texture2D>
			(
				"NewReference",
				new ConstructorParam(typeof(DisposableI), parent),
				new ConstructorParam(typeof(string), fileName),
				new ConstructorParam(typeof(Loader.LoadedCallbackMethod), null)
			);
			if (texture != null)
			{
				++texture.referenceCount;
				return texture;
			}
			return new Texture2D(parent, fileName, loadedCallback);
		}

		public static Texture2D New(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new Texture2D(parent, fileName, loadedCallback);
		}

		public static Texture2D New(DisposableI parent, string fileName, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new Texture2D(parent, fileName, generateMipmaps, usage, loadedCallback);
		}

		public static Texture2D New(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		{
			return new Texture2D(parent, width, height, surfaceFormat, usage, loadedCallback);
		}

		public Texture2D(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			#if SILVERLIGHT
			Image.New(fileName, false,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					var image = (Image)sender;
					init(parent, null, image, image.Size.Width, image.Size.Height, false, MultiSampleTypes.None, image.SurfaceFormat, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
			#else
			init(parent, fileName, null, 0, 0, false, MultiSampleTypes.None, SurfaceFormats.Unknown, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false, loadedCallback);
			#endif
		}

		public Texture2D(DisposableI parent, string fileName, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			#if SILVERLIGHT
			Image.New(fileName, false,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					var image = (Image)sender;
					init(parent, null, image, image.Size.Width, image.Size.Height, generateMipmaps, MultiSampleTypes.None, SurfaceFormats.Unknown, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
			#else
			init(parent, fileName, null, 0, 0, generateMipmaps, MultiSampleTypes.None, SurfaceFormats.Unknown, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
			#endif
		}

		public Texture2D(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(parent, null, null, width, height, false, MultiSampleTypes.None, surfaceFormat, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
		}
		
		protected virtual bool init(DisposableI parent, string fileName, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				if (usage == BufferUsages.Read && !isRenderTarget) Debug.ThrowError("Texture2D", "Only RenderTargets may be readable");

				video = parent.FindParentOrSelfWithException<Video>();

				if (!isRenderTarget)
				{
					if (fileName != null || image != null)
					{
						#if SILVERLIGHT
						texture = new X.Texture2D(video.Device, 256, 256);
						texture.SetData<byte>(image.Mipmaps[0].Data);
						#else
						texture = parent.FindParentOrSelfWithException<RootDisposable>().Content.Load<X.Texture2D>(Streams.StripFileExt(fileName));
						loadedFromContentManager = true;
						#endif
						switch (texture.Format)
						{
							case (X.SurfaceFormat.Color): surfaceFormat = SurfaceFormats.RGBAx8; break;
							#if !SILVERLIGHT
							case (X.SurfaceFormat.Dxt1): surfaceFormat = SurfaceFormats.DXT1; break;
							case (X.SurfaceFormat.Dxt3): surfaceFormat = SurfaceFormats.DXT3; break;
							case (X.SurfaceFormat.Dxt5): surfaceFormat = SurfaceFormats.DXT5; break;
							#endif
							default: Debug.ThrowError("Texture2D", "Unsuported surface format"); break;
						}
					}
					else
					{
						texture = new X.Texture2D(video.Device, width, height, generateMipmaps, Video.surfaceFormat(surfaceFormat));
					}

					Size = new Size2(texture.Width, texture.Height);
					PixelByteSize = Image.CalculatePixelByteSize(surfaceFormat, texture.Width, texture.Height);
				}
				else
				{
					Size = new Size2(width, height);
					PixelByteSize = Image.CalculatePixelByteSize(surfaceFormat, width, height);
				}

				TexelOffset = (1 / Size.ToVector2()) * .5f;
				SizeF = Size.ToVector2();
			}
			catch (Exception e)
			{
				FailedToLoad = true;
				Loader.AddLoadableException(e);
				Dispose();
				if (loadedCallback != null) loadedCallback(this, false);
				return false;
			}

			if (!isRenderTarget)
			{
				Loaded = true;
				if (loadedCallback != null) loadedCallback(this, true);
			}
			return true;
		}

		public bool UpdateLoad()
		{
			return Loaded || IsDisposed;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (texture != null && !loadedFromContentManager)
			{
				texture.Dispose();
				texture = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Copy(Texture2DI texture)
		{
			throw new NotImplementedException();
		}

		public void Update(byte[] data)
		{
			throw new NotImplementedException();
		}

		public void WritePixels(byte[] data)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}