using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class BlendStateDesc : BlendStateDescI
	{
		#region Properties
		public X.ColorWriteChannels RenderTargetWriteMask {get; private set;}

		public bool BlendEnable {get; private set;}
		public X.BlendFunction BlendOp {get; private set;}
		public X.Blend SrcBlend {get; private set;}
		public X.Blend DstBlend {get; private set;}

		public X.BlendFunction BlendOpAlpha {get; private set;}
		public X.Blend SrcBlendAlpha {get; private set;}
		public X.Blend DstBlendAlpha {get; private set;}
		#endregion

		#region Constructors
		public BlendStateDesc(BlendStateTypes type)
		{
			RenderTargetWriteMask = X.ColorWriteChannels.All;

			switch (type)
			{
			    case (BlendStateTypes.None):
			        BlendEnable = false;
					BlendOp = X.BlendFunction.Add;
					SrcBlend = X.Blend.One;
					DstBlend = X.Blend.One;

					BlendOpAlpha = X.BlendFunction.Add;
					SrcBlendAlpha = X.Blend.One;
					DstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.Add):
			        BlendEnable = true;
					BlendOp = X.BlendFunction.Add;
					SrcBlend = X.Blend.One;
					DstBlend = X.Blend.One;

					BlendOpAlpha = X.BlendFunction.Add;
					SrcBlendAlpha = X.Blend.One;
					DstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.Subtract):
			        BlendEnable = true;
					BlendOp = X.BlendFunction.Subtract;
					SrcBlend = X.Blend.One;
					DstBlend = X.Blend.One;

					BlendOpAlpha = X.BlendFunction.Subtract;
					SrcBlendAlpha = X.Blend.One;
					DstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.RevSubtract):
			        BlendEnable = true;
					BlendOp = X.BlendFunction.ReverseSubtract;
					SrcBlend = X.Blend.One;
					DstBlend = X.Blend.One;

					BlendOpAlpha = X.BlendFunction.ReverseSubtract;
					SrcBlendAlpha = X.Blend.One;
					DstBlendAlpha = X.Blend.One;
			        break;

				case (BlendStateTypes.Alpha):
			        BlendEnable = true;
					BlendOp = X.BlendFunction.Add;
					SrcBlend = X.Blend.SourceAlpha;
					DstBlend = X.Blend.InverseSourceAlpha;

					BlendOpAlpha = X.BlendFunction.Add;
					SrcBlendAlpha = X.Blend.SourceAlpha;
					DstBlendAlpha = X.Blend.InverseSourceAlpha;
			        break;

				default:
					Debug.ThrowError("BlendStateDesc", "Unsuported BlendStateType");
					break;
			}
		}
		#endregion
	}
}