using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class SamplerStateDesc : SamplerStateDescI
	{
		#region Properties
		internal X.TextureFilter filter;

		internal X.TextureAddressMode addressU;
		internal X.TextureAddressMode addressV;
		internal X.TextureAddressMode addressW;
		#endregion

		#region Constructors
		public static SamplerStateDesc New(SamplerStateTypes type)
		{
			return new SamplerStateDesc(type);
		}

		public SamplerStateDesc(SamplerStateTypes type)
		{
			switch (type)
			{
				case SamplerStateTypes.Point_Wrap:
					filter = X.TextureFilter.Point;
					addressU = X.TextureAddressMode.Wrap;
					addressV = X.TextureAddressMode.Wrap;
					addressW = X.TextureAddressMode.Wrap;
					break;

				case SamplerStateTypes.Point_Clamp:
					filter = X.TextureFilter.Point;
					addressU = X.TextureAddressMode.Clamp;
					addressV = X.TextureAddressMode.Clamp;
					addressW = X.TextureAddressMode.Clamp;
					break;

				case SamplerStateTypes.Linear_Wrap:
					filter = X.TextureFilter.Linear;
					addressU = X.TextureAddressMode.Wrap;
					addressV = X.TextureAddressMode.Wrap;
					addressW = X.TextureAddressMode.Wrap;
					break;

				case SamplerStateTypes.Linear_Clamp:
					filter = X.TextureFilter.Linear;
					addressU = X.TextureAddressMode.Clamp;
					addressV = X.TextureAddressMode.Clamp;
					addressW = X.TextureAddressMode.Clamp;
					break;

				default:
					Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
					break;
			}
		}
		#endregion
	}
}