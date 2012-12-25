using Reign.Core;

namespace Reign.Video
{
	public enum MultiSampleTypes
	{
		None,
		x2,
		x4,
		x8,
		x16
	}

	public enum SurfaceFormats
	{
		Unknown,
		DXT1,
		DXT3,
		DXT5,
		ATC_RGB,
		ATC_RGBA_Explicit,
		ATC_RGBA_Interpolated,
		PVR_RGB_2,
		PVR_RGBA_2,
		PVR_RGB_4,
		PVR_RGBA_4,
		RGBAx8,
		RGBx10_Ax2,
		RGBAx16f,
		RGBAx32f
	}

	public enum RenderTargetUsage
	{
		PlatformDefault,
		PreserveContents,
		DiscardContents
	}

	public interface TextureI : DisposableI, LoadableI
	{
		int PixelByteSize {get;}

		void Update(byte[] data);
		void WritePixels(byte[] data);
	}

	public interface Texture2DI : DisposableI, TextureI
	{
		#region Properties
		Size2 Size {get;}
		Vector2 SizeF {get;}
		Vector2 TexelOffset {get;}
		#endregion

		#region Methods
		void Copy(Texture2DI texture);
		#endregion
	}

	public interface Texture3DI : DisposableI, TextureI
	{
		#region Properties
	    Size3 Size {get;}
	    Vector3 SizeF {get;}
	    Vector3 TexelOffset {get;}
		#endregion

		#region Methods
	    void Copy(Texture3DI texture);
		#endregion
	}

	public interface RenderTargetI : Texture2DI
	{
		#region Methods
		void Enable();
		void Enable(DepthStencilI depthStencil);
		void ReadPixels(byte[] data);
		void ReadPixels(Color4[] colors);
		bool ReadPixel(Point2 position, out Color4 color);
		#endregion
	}

	public static class Texture2DAPI
	{
		public static void Init(NewReferencePtrMethod newReferencePtr, NewPtrMethod1 newPtr1, NewPtrMethod2 newPtr2, NewPtrMethod3 newPtr3)
		{
			Texture2DAPI.newReferencePtr = newReferencePtr;
			Texture2DAPI.newPtr1 = newPtr1;
			Texture2DAPI.newPtr2 = newPtr2;
			Texture2DAPI.newPtr3 = newPtr3;
		}

		public delegate Texture2DI NewReferencePtrMethod(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);
		private static NewReferencePtrMethod newReferencePtr;
		public static Texture2DI NewReference(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return newReferencePtr(parent, fileName, loadedCallback, failedToLoadCallback);
		}

		public delegate Texture2DI NewPtrMethod1(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);
		private static NewPtrMethod1 newPtr1;
		public static Texture2DI New(DisposableI parent, string fileName, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return newPtr1(parent, fileName, loadedCallback, failedToLoadCallback);
		}

		public delegate Texture2DI NewPtrMethod2(DisposableI parent, string fileName, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);
		private static NewPtrMethod2 newPtr2;
		public static Texture2DI New(DisposableI parent, string fileName, bool generateMipmaps, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return newPtr2(parent, fileName, generateMipmaps, usage, loadedCallback, failedToLoadCallback);
		}

		public delegate Texture2DI NewPtrMethod3(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);
		private static NewPtrMethod3 newPtr3;
		public static Texture2DI New(DisposableI parent, int width, int height, SurfaceFormats surfaceFormat, BufferUsages usage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return newPtr3(parent, width, height, surfaceFormat, usage, loadedCallback, failedToLoadCallback);
		}
	}

	public static class RenderTargetAPI
	{
		public static void Init(NewPtrMethod1 newPtr1, NewPtrMethod2 newPtr2)
		{
			RenderTargetAPI.newPtr1 = newPtr1;
			RenderTargetAPI.newPtr2 = newPtr2;
		}

		public delegate RenderTargetI NewPtrMethod1(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);
		private static NewPtrMethod1 newPtr1;
		public static RenderTargetI New(DisposableI parent, int width, int height, MultiSampleTypes multiSampleType, SurfaceFormats surfaceFormat, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return newPtr1(parent, width, height, multiSampleType, surfaceFormat, usage, renderTargetUsage, loadedCallback, failedToLoadCallback);
		}

		public delegate RenderTargetI NewPtrMethod2(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback);
		private static NewPtrMethod2 newPtr2;
		public static RenderTargetI New(DisposableI parent, string fileName, MultiSampleTypes multiSampleType, BufferUsages usage, RenderTargetUsage renderTargetUsage, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			return newPtr2(parent, fileName, multiSampleType, usage, renderTargetUsage, loadedCallback, failedToLoadCallback);
		}
	}
}