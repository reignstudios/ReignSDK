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
		RGBAx8,
		RGBx10_Ax2,
		RGBAx16f,
		RGBAx32f
	}

	public enum DepthStenicFormats
	{
		//Depth24Stencil8
		Depth16
	}

	public enum RenderTargetUsage
	{
		PlatformDefault,
		PreserveContents,
		DiscardContents
	}

	public interface TextureI : DisposableI
	{
		void Update(byte[] data);
		void UpdateDynamic(byte[] data);
		byte[] Copy();
	}

	public interface Texture2DI : TextureI
	{
		bool Loaded {get;}
		Size2 Size {get;}
		Vector2 SizeF {get;}
		Vector2 TexelOffset {get;}
		void Copy(Texture2DI texture);
	}

	public interface Texture3DI : TextureI
	{
		bool Loaded {get;}
		Size3 Size {get;}
		Vector3 SizeF {get;}
		Vector3 TexelOffset {get;}
		void Copy(Texture3DI texture);
	}

	public interface RenderTargetI : Texture2DI
	{
		void Enable();
		void Enable(DepthStencilI depthStencil);
	}
}