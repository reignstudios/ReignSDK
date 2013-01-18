using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class BlendStateDesc : BlendStateDescI
	{
		#region Properties
		internal ColorMask colorMask;
		
		internal bool blendEnable;
		internal BlendFuncMode blendOp;
		internal BlendFuncFactor srcBlend, dstBlend;
		
		internal bool blendEnableAlpha;
		internal BlendFuncMode blendOpAlpha;
		internal BlendFuncFactor srcBlendAlpha, dstBlendAlpha;
		#endregion
	
		#region Constructors
		public static BlendStateDesc New(BlendStateTypes type)
		{
			return new BlendStateDesc(type);
		}

		public BlendStateDesc(BlendStateTypes type)
		{
			colorMask = ColorMask.Rgba;

			switch (type)
			{
				case (BlendStateTypes.None):
					blendEnable = false;
					blendOp = BlendFuncMode.Add;
					srcBlend = BlendFuncFactor.One;
					dstBlend = BlendFuncFactor.One;

					blendEnableAlpha = false;
					blendOpAlpha = BlendFuncMode.Add;
					srcBlendAlpha = BlendFuncFactor.One;
					dstBlendAlpha = BlendFuncFactor.One;
					break;

				case (BlendStateTypes.Add):
					blendEnable = true;
					blendOp = BlendFuncMode.Add;
					srcBlend = BlendFuncFactor.One;
					dstBlend = BlendFuncFactor.One;

					blendEnableAlpha = false;
					blendOpAlpha = BlendFuncMode.Add;
					srcBlendAlpha = BlendFuncFactor.One;
					dstBlendAlpha = BlendFuncFactor.One;
					break;

				case (BlendStateTypes.Subtract):
					blendEnable = true;
					blendOp = BlendFuncMode.Subtract;
					srcBlend = BlendFuncFactor.One;
					dstBlend = BlendFuncFactor.One;

					blendEnableAlpha = false;
					blendOpAlpha = BlendFuncMode.Subtract;
					srcBlendAlpha = BlendFuncFactor.One;
					dstBlendAlpha = BlendFuncFactor.One;
					break;

				case (BlendStateTypes.RevSubtract):
					blendEnable = true;
					blendOp = BlendFuncMode.Subtract;
					srcBlend = BlendFuncFactor.One;
					dstBlend = BlendFuncFactor.One;

					blendEnableAlpha = false;
					blendOpAlpha = BlendFuncMode.Subtract;
					srcBlendAlpha = BlendFuncFactor.One;
					dstBlendAlpha = BlendFuncFactor.One;
					break;

				case (BlendStateTypes.Alpha):
					blendEnable = true;
					blendOp = BlendFuncMode.Add;
					srcBlend = BlendFuncFactor.SrcAlpha;
					dstBlend = BlendFuncFactor.OneMinusSrcAlpha;

					blendEnableAlpha = false;
					blendOpAlpha = BlendFuncMode.Add;
					srcBlendAlpha = BlendFuncFactor.SrcAlpha;
					dstBlendAlpha = BlendFuncFactor.OneMinusSrcAlpha;
					break;

				default:
					Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
					break;
			}
		}
		#endregion
	}
}

