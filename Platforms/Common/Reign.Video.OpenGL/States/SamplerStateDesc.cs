using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class SamplerStateDesc : SamplerStateDescI
	{
		#region Properties
		public int FilterMin {get; private set;}
		public int FilterMinMiped {get; private set;}
		public int FilterMag {get; private set;}

		public int AddressU {get; private set;}
		public int AddressV {get; private set;}
		public int AddressW {get; private set;}

		public int BorderColor {get; private set;}
		#endregion

		#region Constructors
		public SamplerStateDesc(SamplerStateTypes type)
		{
			switch (type)
			{
				case (SamplerStateTypes.Point_Wrap):
					FilterMin = GL.NEAREST;
					FilterMinMiped = GL.NEAREST_MIPMAP_NEAREST;
					FilterMag = GL.NEAREST;
					AddressU = GL.REPEAT;
					AddressV = GL.REPEAT;
					AddressW = GL.REPEAT;
					BorderColor = 0;
					break;

				case (SamplerStateTypes.Point_Clamp):
					FilterMin = GL.NEAREST;
					FilterMinMiped = GL.NEAREST_MIPMAP_NEAREST;
					FilterMag = GL.NEAREST;
					AddressU = GL.CLAMP_TO_EDGE;
					AddressV = GL.CLAMP_TO_EDGE;
					AddressW = GL.CLAMP_TO_EDGE;
					BorderColor = 0;
					break;

				case (SamplerStateTypes.Linear_Wrap):
					FilterMin = GL.LINEAR;
					FilterMinMiped = GL.LINEAR_MIPMAP_LINEAR;
					FilterMag = GL.LINEAR;
					AddressU = GL.REPEAT;
					AddressV = GL.REPEAT;
					AddressW = GL.REPEAT;
					BorderColor = 0;
					break;

				case (SamplerStateTypes.Linear_Clamp):
					FilterMin = GL.LINEAR;
					FilterMinMiped = GL.LINEAR_MIPMAP_LINEAR;
					FilterMag = GL.LINEAR;
					AddressU = GL.CLAMP_TO_EDGE;
					AddressV = GL.CLAMP_TO_EDGE;
					AddressW = GL.CLAMP_TO_EDGE;
					BorderColor = 0;
					break;

				default:
					Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
					break;
			}
		}
		#endregion
	}
}