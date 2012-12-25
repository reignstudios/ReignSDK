using System;
using Reign.Core;
using X = Microsoft.Xna.Framework.Graphics;

namespace Reign.Video.XNA
{
	public class SamplerStateDesc : SamplerStateDescI
	{
		#region Properties
		public X.TextureFilter Filter {get; private set;}

		public X.TextureAddressMode AddressU {get; private set;}
		public X.TextureAddressMode AddressV {get; private set;}
		public X.TextureAddressMode AddressW {get; private set;}
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
					Filter = X.TextureFilter.Point;
					AddressU = X.TextureAddressMode.Wrap;
					AddressV = X.TextureAddressMode.Wrap;
					AddressW = X.TextureAddressMode.Wrap;
					break;

				case (SamplerStateTypes.Point_Clamp):
					Filter = X.TextureFilter.Point;
					AddressU = X.TextureAddressMode.Clamp;
					AddressV = X.TextureAddressMode.Clamp;
					AddressW = X.TextureAddressMode.Clamp;
					break;

				case (SamplerStateTypes.Linear_Wrap):
					Filter = X.TextureFilter.Linear;
					AddressU = X.TextureAddressMode.Wrap;
					AddressV = X.TextureAddressMode.Wrap;
					AddressW = X.TextureAddressMode.Wrap;
					break;

				case (SamplerStateTypes.Linear_Clamp):
					Filter = X.TextureFilter.Linear;
					AddressU = X.TextureAddressMode.Clamp;
					AddressV = X.TextureAddressMode.Clamp;
					AddressW = X.TextureAddressMode.Clamp;
					break;

				default:
					Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
					break;
			}
		}
		#endregion
	}
}