using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class DepthStencilStateDesc : IDepthStencilStateDesc
	{
		#region Properties
		internal bool depthReadEnable;
		internal bool depthWriteEnable;
		internal uint depthFunc;

		internal bool stencilEnable;
		internal uint stencilFunc;
		internal uint stencilFailOp;
		internal uint stencilDepthFailOp;
		internal uint stencilPassOp;
		#endregion

		#region Constructors
		public DepthStencilStateDesc(DepthStencilStateTypes type)
		{
			switch (type)
			{
				case DepthStencilStateTypes.None:
					depthReadEnable = false;
					depthWriteEnable = false;
					depthFunc = GL.ALWAYS;

					stencilEnable = false;
					stencilFunc = GL.NEVER;
					stencilFailOp = GL.KEEP;
					stencilDepthFailOp = GL.KEEP;
					stencilPassOp = GL.KEEP;
					break;

				case DepthStencilStateTypes.ReadWrite_Less:
					depthReadEnable = true;
					depthWriteEnable = true;
					depthFunc = GL.LESS;

					stencilEnable = false;
					stencilFunc = GL.NEVER;
					stencilFailOp = GL.KEEP;
					stencilDepthFailOp = GL.KEEP;
					stencilPassOp = GL.KEEP;
					break;

				default:
					Debug.ThrowError("DepthStencilStateDesc", "Unsuported DepthStencilStateType");
					break;
			}
		}
		#endregion
	}
}