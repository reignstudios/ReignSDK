using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class BlendStateDesc : BlendStateDescI
	{
		#region Properties
		internal X.ColorWriteChannels renderTargetWriteMask;

		internal bool blendEnable;
		internal X.BlendFunction blendOp;
		internal X.Blend srcBlend;
		internal X.Blend dstBlend;

		internal X.BlendFunction blendOpAlpha;
		internal X.Blend srcBlendAlpha;
		internal X.Blend dstBlendAlpha;
		#endregion

		#region Constructors
		public static BlendStateDesc New(BlendStateTypes type)
		{
			return new BlendStateDesc(type);
		}

		public BlendStateDesc(BlendStateTypes type)
		{
			renderTargetWriteMask = X.ColorWriteChannels.All;

			switch (type)
			{
			    case (BlendStateTypes.None):
			        blendEnable = false;
					blendOp = X.BlendFunction.Add;
					srcBlend = X.Blend.One;
					dstBlend = X.Blend.One;

					blendOpAlpha = X.BlendFunction.Add;
					srcBlendAlpha = X.Blend.One;
					dstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.Add):
			        blendEnable = true;
					blendOp = X.BlendFunction.Add;
					srcBlend = X.Blend.One;
					dstBlend = X.Blend.One;

					blendOpAlpha = X.BlendFunction.Add;
					srcBlendAlpha = X.Blend.One;
					dstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.Subtract):
			        blendEnable = true;
					blendOp = X.BlendFunction.Subtract;
					srcBlend = X.Blend.One;
					dstBlend = X.Blend.One;

					blendOpAlpha = X.BlendFunction.Subtract;
					srcBlendAlpha = X.Blend.One;
					dstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.RevSubtract):
			        blendEnable = true;
					blendOp = X.BlendFunction.ReverseSubtract;
					srcBlend = X.Blend.One;
					dstBlend = X.Blend.One;

					blendOpAlpha = X.BlendFunction.ReverseSubtract;
					srcBlendAlpha = X.Blend.One;
					dstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.Alpha):
			        blendEnable = true;
					blendOp = X.BlendFunction.Add;
					srcBlend = X.Blend.SourceAlpha;
					dstBlend = X.Blend.InverseSourceAlpha;

					blendOpAlpha = X.BlendFunction.Add;
					srcBlendAlpha = X.Blend.SourceAlpha;
					dstBlendAlpha = X.Blend.InverseSourceAlpha;
			        break;

				default:
					Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
					break;
			}
		}
		#endregion
	}
}