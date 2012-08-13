using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class BlendStateDesc : BlendStateDescI
	{
		#region Properties
		public bool RenderTargetWriteMaskR {get; private set;}
		public bool RenderTargetWriteMaskG {get; private set;}
		public bool RenderTargetWriteMaskB {get; private set;}
		public bool RenderTargetWriteMaskA {get; private set;}

		public bool BlendEnable {get; private set;}
		public uint BlendOp {get; private set;}
		public uint SrcBlend {get; private set;}
		public uint DstBlend {get; private set;}

		public bool BlendEnableAlpha {get; private set;}
		public uint BlendOpAlpha {get; private set;}
		public uint SrcBlendAlpha {get; private set;}
		public uint DstBlendAlpha {get; private set;}
		#endregion

		#region Constructors
		public BlendStateDesc(BlendStateTypes type)
		{
			RenderTargetWriteMaskR = true;
			RenderTargetWriteMaskG = true;
			RenderTargetWriteMaskB = true;
			RenderTargetWriteMaskA = true;

			switch (type)
			{
				case (BlendStateTypes.None):
					BlendEnable = false;
					BlendOp = GL.FUNC_ADD;
					SrcBlend = GL.ONE;
					DstBlend = GL.ONE;

					BlendEnableAlpha = false;
					BlendOpAlpha = GL.FUNC_ADD;
					SrcBlendAlpha = GL.ONE;
					DstBlendAlpha = GL.ONE;
					break;

				case (BlendStateTypes.Add):
					BlendEnable = true;
					BlendOp = GL.FUNC_ADD;
					SrcBlend = GL.ONE;
					DstBlend = GL.ONE;

					BlendEnableAlpha = false;
					BlendOpAlpha = GL.FUNC_ADD;
					SrcBlendAlpha = GL.ONE;
					DstBlendAlpha = GL.ONE;
					break;

				case (BlendStateTypes.Subtract):
					BlendEnable = true;
					BlendOp = GL.FUNC_SUBTRACT;
					SrcBlend = GL.ONE;
					DstBlend = GL.ONE;

					BlendEnableAlpha = false;
					BlendOpAlpha = GL.FUNC_SUBTRACT;
					SrcBlendAlpha = GL.ONE;
					DstBlendAlpha = GL.ONE;
					break;

				case (BlendStateTypes.RevSubtract):
					BlendEnable = true;
					BlendOp = GL.FUNC_REVERSE_SUBTRACT;
					SrcBlend = GL.ONE;
					DstBlend = GL.ONE;

					BlendEnableAlpha = false;
					BlendOpAlpha = GL.FUNC_REVERSE_SUBTRACT;
					SrcBlendAlpha = GL.ONE;
					DstBlendAlpha = GL.ONE;
					break;

				case (BlendStateTypes.Alpha):
					BlendEnable = true;
					BlendOp = GL.FUNC_ADD;
					SrcBlend = GL.SRC_ALPHA;
					DstBlend = GL.ONE_MINUS_SRC_ALPHA;

					BlendEnableAlpha = false;
					BlendOpAlpha = GL.FUNC_ADD;
					SrcBlendAlpha = GL.SRC_ALPHA;
					DstBlendAlpha = GL.ONE_MINUS_SRC_ALPHA;
					break;

				default:
					Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
					break;
			}
		}
		#endregion
	}
}