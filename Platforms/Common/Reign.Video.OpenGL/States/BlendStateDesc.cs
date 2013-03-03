using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class BlendStateDesc : BlendStateDescI
	{
		#region Properties
		internal bool renderTargetWriteMaskR;
		internal bool renderTargetWriteMaskG;
		internal bool renderTargetWriteMaskB;
		internal bool renderTargetWriteMaskA;

		internal bool blendEnable;
		internal uint blendOp;
		internal uint srcBlend;
		internal uint dstBlend;

		internal bool blendEnableAlpha;
		internal uint blendOpAlpha;
		internal uint srcBlendAlpha;
		internal uint dstBlendAlpha;
		#endregion

		#region Constructors
		public static BlendStateDesc New(BlendStateTypes type)
		{
			return new BlendStateDesc(type);
		}

		public BlendStateDesc(BlendStateTypes type)
		{
			renderTargetWriteMaskR = true;
			renderTargetWriteMaskG = true;
			renderTargetWriteMaskB = true;
			renderTargetWriteMaskA = true;

			switch (type)
			{
				case BlendStateTypes.None:
					blendEnable = false;
					blendOp = GL.FUNC_ADD;
					srcBlend = GL.ONE;
					dstBlend = GL.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = GL.FUNC_ADD;
					srcBlendAlpha = GL.ONE;
					dstBlendAlpha = GL.ONE;
					break;

				case BlendStateTypes.Add:
					blendEnable = true;
					blendOp = GL.FUNC_ADD;
					srcBlend = GL.ONE;
					dstBlend = GL.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = GL.FUNC_ADD;
					srcBlendAlpha = GL.ONE;
					dstBlendAlpha = GL.ONE;
					break;

				case BlendStateTypes.Subtract:
					blendEnable = true;
					blendOp = GL.FUNC_SUBTRACT;
					srcBlend = GL.ONE;
					dstBlend = GL.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = GL.FUNC_SUBTRACT;
					srcBlendAlpha = GL.ONE;
					dstBlendAlpha = GL.ONE;
					break;

				case BlendStateTypes.RevSubtract:
					blendEnable = true;
					blendOp = GL.FUNC_REVERSE_SUBTRACT;
					srcBlend = GL.ONE;
					dstBlend = GL.ONE;

					blendEnableAlpha = false;
					blendOpAlpha = GL.FUNC_REVERSE_SUBTRACT;
					srcBlendAlpha = GL.ONE;
					dstBlendAlpha = GL.ONE;
					break;

				case BlendStateTypes.Alpha:
					blendEnable = true;
					blendOp = GL.FUNC_ADD;
					srcBlend = GL.SRC_ALPHA;
					dstBlend = GL.ONE_MINUS_SRC_ALPHA;

					blendEnableAlpha = false;
					blendOpAlpha = GL.FUNC_ADD;
					srcBlendAlpha = GL.SRC_ALPHA;
					dstBlendAlpha = GL.ONE_MINUS_SRC_ALPHA;
					break;

				default:
					Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
					break;
			}
		}
		#endregion
	}
}