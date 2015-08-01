using Reign.Core;
using Reign_Video_D3D11_Component;
using System;
using System.Runtime.InteropServices;

namespace Reign.Video.D3D11
{
	public class Texture2D : DisposableResource, ITexture2D
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
		#endregion

		#region Constructors
		public Texture2D(IDisposableResource parent, string filename, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			Image.New(filename, false,
			delegate (object sender, bool succeeded)
			{
				if (succeeded)
				{
					var image = (Image)sender;
					init(parent, image, image.Size.Width, image.Size.Height, generateMipmaps, MultiSampleTypes.None, image.SurfaceFormat, RenderTargetUsage.PlatformDefault, usage, false, loadedCallback);
				}
				else
				{
					FailedToLoad = true;
					Dispose();
					if (loadedCallback != null) loadedCallback(this, false);
				}
			});
		}

		public Texture2D(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback)
		: base(parent)
		{
			init(parent, image, width, height, generateMipmaps, multiSampleType, surfaceFormat, renderTargetUsage, usage, false, loadedCallback);
		}

		protected virtual bool init(IDisposableResource parent, Image image, int width, int height, bool generateMipmaps, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, RenderTargetUsage renderTargetUsage, BufferUsages usage, bool isRenderTarget, Loader.LoadedCallbackMethod loadedCallback)
		{
			long[] mipmaps = null;
			int[] mipmapSizes = null, mipmapPitches = null;
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				if (isRenderTarget) generateMipmaps = false;

				// load image data
				if (image != null)
				{
					mipmaps = new long[image.Mipmaps.Length];
					mipmapSizes = new int[image.Mipmaps.Length];
					mipmapPitches = new int[image.Mipmaps.Length];
					for (int i = 0; i != mipmaps.Length; ++i)
					{
						var imageMipmap = image.Mipmaps[i];
						IntPtr mipmapPtr = Marshal.AllocHGlobal(imageMipmap.Data.Length);
						Marshal.Copy(imageMipmap.Data, 0, mipmapPtr, imageMipmap.Data.Length);
						mipmapSizes[i] = imageMipmap.Data.Length;
						mipmapPitches[i] = imageMipmap.Pitch;
						mipmaps[i] = mipmapPtr.ToInt64();
					}

					Size = image.Size;
					surfaceFormat = image.SurfaceFormat;
					PixelByteSize = image.CalculatePixelByteSize();
				}
				else
				{
					if (width == 0 || height == 0) Debug.ThrowError("Texture2D", "Width or Height cannot be 0");
					Size = new Size2(width, height);
					PixelByteSize = Image.CalculatePixelByteSize((surfaceFormat == SurfaceFormats.Defualt ? Video.DefaultSurfaceFormat() : surfaceFormat), width, height);
				}
				SizeF = Size.ToVector2();

				// init texture
				REIGN_D3D11_USAGE usageType = REIGN_D3D11_USAGE.DEFAULT;
				REIGN_D3D11_CPU_ACCESS_FLAG cpuAccessFlags = (REIGN_D3D11_CPU_ACCESS_FLAG)0;
				if (usage == BufferUsages.Read)
				{
					if (!isRenderTarget) Debug.ThrowError("Texture2D", "Only RenderTargets may be readable");
					// NOTE: Staging texture and states will be created in the RenderTarget
					//usageType = REIGN_D3D11_USAGE.STAGING;
					//cpuAccessFlags = REIGN_D3D11_CPU_ACCESS_FLAG.READ;
				}
				if (usage == BufferUsages.Write)
				{
					usageType = REIGN_D3D11_USAGE.DYNAMIC;
					cpuAccessFlags = REIGN_D3D11_CPU_ACCESS_FLAG.WRITE;
				}
				com = new Texture2DCom();
				var error = com.Init(video.com, Size.Width, Size.Height, generateMipmaps, mipmaps != null, mipmaps, mipmapSizes, mipmapPitches, 0, video.surfaceFormat(surfaceFormat), usageType, cpuAccessFlags, isRenderTarget);

				switch (error)
				{
					case TextureError.Texture: Debug.ThrowError("Texture2D", "Failed to create Texture2D"); break;
					case TextureError.ShaderResourceView: Debug.ThrowError("Texture2D", "Failed to create ShaderResourceView"); break;
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
			finally
			{
				if (mipmaps != null)
				{
					for (int i = 0; i != mipmaps.Length; ++i)
					{
						if (mipmaps[i] != 0) Marshal.FreeHGlobal(new IntPtr(mipmaps[i]));
					}
				}
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
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public void Copy(ITexture2D texture)
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