using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class DepthStencilStateDesc : DepthStencilStateDescI
	{
		#region Properties
		public bool DepthReadEnable {get; private set;}
		public bool DepthWriteEnable {get; private set;}
		public uint DepthFunc {get; private set;}

		public bool StencilEnable {get; private set;}
		public uint StencilFunc {get; private set;}
		public uint StencilFailOp {get; private set;}
		public uint StencilDepthFailOp {get; private set;}
		public uint StencilPassOp {get; private set;}
		#endregion

		#region Constructors
		public static DepthStencilStateDesc New(DepthStencilStateTypes type)
		{
			return new DepthStencilStateDesc(type);
		}

		public DepthStencilStateDesc(DepthStencilStateTypes type)
		{
			switch (type)
			{
				case (DepthStencilStateTypes.None):
					DepthReadEnable = false;
					DepthWriteEnable = false;
					DepthFunc = GL.LESS;

					StencilEnable = false;
					StencilFunc = GL.NEVER;
					StencilFailOp = GL.KEEP;
					StencilDepthFailOp = GL.KEEP;
					StencilPassOp = GL.KEEP;
					break;

				case (DepthStencilStateTypes.ReadWrite_Less):
					DepthReadEnable = true;
					DepthWriteEnable = true;
					DepthFunc = GL.LESS;

					StencilEnable = false;
					StencilFunc = GL.NEVER;
					StencilFailOp = GL.KEEP;
					StencilDepthFailOp = GL.KEEP;
					StencilPassOp = GL.KEEP;
					break;

				default:
					Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
					break;
			}
		}
		#endregion
	}
}