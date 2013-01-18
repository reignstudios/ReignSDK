using System;
using Reign.Core;
using Reign.Video;
using Sce.PlayStation.Core.Graphics;

namespace Reign.Video.Vita
{
	public class SamplerStateDesc : SamplerStateDescI
	{
		#region Properties
		internal TextureFilterMode filterMin;
		internal TextureFilterMode filterMinMiped;
		internal TextureFilterMode filterMag;

		internal TextureWrapMode addressU;
		internal TextureWrapMode addressV;
		internal TextureWrapMode addressW;
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
				case (SamplerStateTypes.Point_Wrap):
					filterMin = TextureFilterMode.Nearest;
					filterMinMiped = TextureFilterMode.Nearest;
					filterMag = TextureFilterMode.Nearest;
					addressU = TextureWrapMode.Repeat;
					addressV = TextureWrapMode.Repeat;
					addressW = TextureWrapMode.Repeat;
					break;

				case (SamplerStateTypes.Point_Clamp):
					filterMin = TextureFilterMode.Nearest;
					filterMinMiped = TextureFilterMode.Nearest;
					filterMag = TextureFilterMode.Nearest;
					addressU = TextureWrapMode.ClampToEdge;
					addressV = TextureWrapMode.ClampToEdge;
					addressW = TextureWrapMode.ClampToEdge;
					break;

				case (SamplerStateTypes.Linear_Wrap):
					filterMin = TextureFilterMode.Linear;
					filterMinMiped = TextureFilterMode.Linear;
					filterMag = TextureFilterMode.Linear;
					addressU = TextureWrapMode.Repeat;
					addressV = TextureWrapMode.Repeat;
					addressW = TextureWrapMode.Repeat;
					break;

				case (SamplerStateTypes.Linear_Clamp):
					filterMin = TextureFilterMode.Linear;
					filterMinMiped = TextureFilterMode.Linear;
					filterMag = TextureFilterMode.Linear;
					addressU = TextureWrapMode.ClampToEdge;
					addressV = TextureWrapMode.ClampToEdge;
					addressW = TextureWrapMode.ClampToEdge;
					break;

				default:
					Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
					break;
			}
		}
		#endregion
	}
}

