using Reign.Core;
using Reign_Video_D3D9_Component;
using System;

namespace Reign.Video.D3D9
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
		internal Texture2DCom com;

		// LostDevice properties
		private Image LostDevice_image;
		private int LostDevice_width, LostDevice_height;
		private bool LostDevice_generateMipmaps;
		private MultiSampleTypes LostDevice_multiSampleType;
		private SurfaceFormats LostDevice_surfaceFormat;
		private RenderTargetUsage LostDevice_renderTargetUsage;
		private BufferUsages LostDevice_usage;
		private bool LostDevice_isRenderTarget;
		private bool LostDevice_lockable;
		private REIGN_D3DPOOL LostDevice_pool;
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
			Image.New(fileName, false,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					var image = (Image)sender;
					init(parent, image, image.Size.Width, image.Size.Height, false, MultiSampleTypes.None, image.SurfaceFormat, RenderTargetUsage.PlatformDefault, BufferUsages.Default, false, false, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		public Texture2D(DisposableI parent, string fileName, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			Image.New(fileName, false,
			delegate(object sender, bool succeeded)
			{
				if (succeeded)
				{
					var image = (Image)sender;
					init(parent, image, image.Size.Width, image.Size.Height, generateMipmaps, MultiSampleTypes.None, image.SurfaceFormat, RenderTargetUsage.PlatformDefault, usage, false, false, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		public Texture2D(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(parent, null, width, height, false, MultiSampleTypes.None, surfaceFormat, RenderTargetUsage.PlatformDefault, usage, false, false, loadedCallback);
		}

		protected virtual bool init(DisposableI parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, bool lockable, Loader.LoadedCallbackMethod loadedCallback)
		{
			byte[][] mipmaps = null;
			int[] mipmapSizes = null, mipmapPitches = null;
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				if (isRenderTarget) generateMipmaps = false;

				// load image data
				if (image != null)
				{
					mipmaps = new byte[image.Mipmaps.Length][];
					mipmapSizes = new int[image.Mipmaps.Length];
					mipmapPitches = new int[image.Mipmaps.Length];
					for (int i = 0; i != mipmaps.Length; ++i)
					{
						var imageMipmap = image.Mipmaps[i];
						mipmaps[i] = image.Compressed ? imageMipmap.Data : imageMipmap.SwapRBColorChannels();
						mipmapSizes[i] = imageMipmap.Data.Length;
						mipmapPitches[i] = imageMipmap.Pitch;
					}

					Size = image.Size;
					surfaceFormat = image.SurfaceFormat;
					PixelByteSize = image.CalculatePixelByteSize();
				}
				else
				{
					if (width == 0 || height == 0) Debug.ThrowError("Texture2D", "Width or Height cannot be 0");
					Size = new Size2(width, height);
					PixelByteSize = Image.CalculatePixelByteSize(surfaceFormat, width, height);
				}
				TexelOffset = (1 / Size.ToVector2()) * .5f;
				SizeF = Size.ToVector2();

				// init texture
				REIGN_D3DUSAGE nativeUsage = isRenderTarget ? REIGN_D3DUSAGE.RENDERTARGET : REIGN_D3DUSAGE.NONE;
				REIGN_D3DPOOL nativePool = (mipmaps != null && !video.Caps.ExDevice) ? REIGN_D3DPOOL.MANAGED : REIGN_D3DPOOL.DEFAULT;
				if (usage == BufferUsages.Read)
				{
					if (!isRenderTarget) Debug.ThrowError("Texture2D", "Only RenderTargets may be readable");
					// NOTE: Staging texture and states will be created in the RenderTarget
				}
				if (usage == BufferUsages.Write)
				{
					if (mipmaps != null)
					{
						if (video.Caps.ExDevice) nativeUsage |= REIGN_D3DUSAGE.DYNAMIC;
					}
					else
					{
						nativeUsage |= REIGN_D3DUSAGE.DYNAMIC;
					}
				}
				com = new Texture2DCom();
				var error = com.Init(video.com, Size.Width, Size.Height, generateMipmaps, mipmaps, mipmapSizes, mipmapPitches, 0, nativePool, nativeUsage, video.surfaceFormat(surfaceFormat), isRenderTarget);

				switch (error)
				{
					case (TextureError.Texture): Debug.ThrowError("Texture2D", "Failed to create Texture2D"); break;
					case (TextureError.SystemTexture): Debug.ThrowError("Texture2D", "Failed to create System Texture2D"); break;
				}

				if (!video.Caps.ExDevice && nativePool != REIGN_D3DPOOL.MANAGED)
				{
					LostDevice_image = image;
					LostDevice_width = width;
					LostDevice_height = height;
					LostDevice_generateMipmaps = generateMipmaps;
					LostDevice_multiSampleType = multiSampleType;
					LostDevice_surfaceFormat = surfaceFormat;
					LostDevice_renderTargetUsage = renderTargetUsage;
					LostDevice_usage = usage;
					LostDevice_isRenderTarget = isRenderTarget;
					LostDevice_lockable = lockable;
					LostDevice_pool = nativePool;
				}
				if (nativePool == REIGN_D3DPOOL.DEFAULT && !video.Caps.ExDevice && !video.deviceReseting)
				{
					video.DeviceLost += deviceLost;
					video.DeviceReset += deviceReset;
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
			if (video != null)
			{
				video.removeActiveTexture(this);
				if (!video.Caps.ExDevice && !video.deviceReseting)
				{
					video.DeviceLost -= deviceLost;
					video.DeviceReset -= deviceReset;
				}
				video = null;
			}
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}

		private void deviceLost()
		{
			if (!video.Caps.ExDevice && (LostDevice_pool != REIGN_D3DPOOL.MANAGED || LostDevice_isRenderTarget)) Dispose();
		}

		private void deviceReset()
		{
			if (!video.Caps.ExDevice && (LostDevice_pool != REIGN_D3DPOOL.MANAGED || LostDevice_isRenderTarget)) init(Parent, LostDevice_image, LostDevice_width, LostDevice_height, LostDevice_generateMipmaps, LostDevice_multiSampleType, LostDevice_surfaceFormat, LostDevice_renderTargetUsage, LostDevice_usage, LostDevice_isRenderTarget, LostDevice_lockable, null);
		}
		#endregion

		#region Methods
		public void Copy(Texture2DI texture)
		{
			com.Copy(((Texture2D)texture).com);
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
