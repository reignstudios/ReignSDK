using Reign.Core;
using Reign_Video_D3D9_Component;

namespace Reign.Video.D3D9
{
	class RenderTarget : Texture2D, RenderTargetI
	{
		#region Properties
		private RenderTargetCom renderTargetCom;
		#endregion

		#region Constructors
		public RenderTarget(DisposableI parent)
		: base(parent)
		{
			
		}
		#endregion

		#region Methods
		public void Enable()
		{
			//renderTargetCom.Enable();
		}

		public void Enable(DepthStencilI depthStencil)
		{
			//renderTargetCom.Enable(((DepthStencil)depthStencil).com);
		}

		public void ReadPixels(byte[] data)
		{
			//fixed (byte* ptr = data)
			//{
			//    renderTargetCom.ReadPixels((int)ptr, data.Length);
			//}
		}

		public void ReadPixels(Color4[] colors)
		{
			//fixed (Color4* ptr = colors)
			//{
			//    renderTargetCom.ReadPixels((int)ptr, colors.Length * 4);
			//}
		}

		public bool ReadPixel(Point2 position, out Color4 color)
		{
			//if (position.X < 0 || position.X >= Size.Width || position.Y < 0 || position.Y >= Size.Height)
			//{
			//    color = new Color4();
			//    return false;
			//}

			//color = new Color4(renderTargetCom.ReadPixel(position.X, position.Y, Size.Height));
			//return true;

			color = new Color4();
			return false;
		}
		#endregion
	}
}
