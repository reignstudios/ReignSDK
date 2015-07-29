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
		Defualt,
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

		RGBx565,
		RGBAx4,
		RGBx5_Ax1,
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

	public interface ITexture : IDisposableResource, ILoadable
	{
		int PixelByteSize {get;}

		void Update(byte[] data);
		void WritePixels(byte[] data);
	}

	public interface ITexture2D : IDisposableResource, ITexture
	{
		#region Properties
		Size2 Size {get;}
		Vector2 SizeF {get;}
		Vector2 TexelOffset {get;}
		#endregion

		#region Methods
		void Copy(ITexture2D texture);
		#endregion
	}

	public interface ITexture3D : IDisposableResource, ITexture
	{
		#region Properties
	    Size3 Size {get;}
	    Vector3 SizeF {get;}
	    Vector3 TexelOffset {get;}
		#endregion

		#region Methods
	    void Copy(ITexture3D texture);
		#endregion
	}

	public interface IRenderTarget : ITexture2D
	{
		#region Methods
		void Enable();
		void Enable(IDepthStencil depthStencil);
		void ReadPixels(byte[] data);
		void ReadPixels(Color4[] colors);
		bool ReadPixel(Point2 position, out Color4 color);
		#endregion
	}
}