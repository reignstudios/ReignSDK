using Reign.Core;
using Reign.Video;
using System;
using G = Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class Texture2D : Disposable, Texture2DI
	{
		#region Properties
		public bool Loaded {get; protected set;}
		public bool FailedToLoad {get; protected set;}

		public Size2 Size {get; protected set;}
		public Vector2 SizeF {get; protected set;}
		public Vector2 TexelOffset {get; protected set;}
		public int PixelByteSize {get; protected set;}

		protected Video video;
		protected internal G.Texture2D texture;
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
			init(parent, fileName, 0, 0, false, MultiSampleTypes.None, SurfaceFormats.RGBAx8, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false, loadedCallback);
		}

		public Texture2D(DisposableI parent, string fileName, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(parent, fileName, 0, 0, generateMipmaps, MultiSampleTypes.None, SurfaceFormats.RGBAx8, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
		}

		public Texture2D(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(parent, null, width, height, false, MultiSampleTypes.None, surfaceFormat, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
		}

		protected virtual bool init(DisposableI parent, string fileName, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			try
			{
				if (usage == BufferUsages.Read && !isRenderTarget) Debug.ThrowError("Texture2D", "Only RenderTargets may be readable");

				video = parent.FindParentOrSelfWithException<Video>();
				if (isRenderTarget) generateMipmaps = false;
				
				if (fileName != null)
				{
					texture = new G.Texture2D("/Application/" + fileName, generateMipmaps);
				}
				else
				{
					texture = new G.Texture2D(width, height, generateMipmaps, Video.surfaceFormat(surfaceFormat), isRenderTarget ? G.PixelBufferOption.Renderable : G.PixelBufferOption.None);
				}
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
			return Loaded;
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (texture != null)
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

