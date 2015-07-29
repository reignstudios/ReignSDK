using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class SamplerStateDesc : ISamplerStateDesc
	{
		#region Properties
		internal int filterMin;
		internal int filterMinMiped;
		internal int filterMag;

		internal int addressU;
		internal int addressV;
		internal int addressW;

		internal int borderColor;
		#endregion

		#region Constructors
		public SamplerStateDesc(SamplerStateTypes type)
		{
			switch (type)
			{
				case SamplerStateTypes.Point_Wrap:
					filterMin = GL.NEAREST;
					filterMinMiped = GL.NEAREST_MIPMAP_NEAREST;
					filterMag = GL.NEAREST;
					addressU = GL.REPEAT;
					addressV = GL.REPEAT;
					addressW = GL.REPEAT;
					borderColor = 0;
					break;

				case SamplerStateTypes.Point_Clamp:
					filterMin = GL.NEAREST;
					filterMinMiped = GL.NEAREST_MIPMAP_NEAREST;
					filterMag = GL.NEAREST;
					addressU = GL.CLAMP_TO_EDGE;
					addressV = GL.CLAMP_TO_EDGE;
					addressW = GL.CLAMP_TO_EDGE;
					borderColor = 0;
					break;

				case SamplerStateTypes.Linear_Wrap:
					filterMin = GL.LINEAR;
					filterMinMiped = GL.LINEAR_MIPMAP_LINEAR;
					filterMag = GL.LINEAR;
					addressU = GL.REPEAT;
					addressV = GL.REPEAT;
					addressW = GL.REPEAT;
					borderColor = 0;
					break;

				case SamplerStateTypes.Linear_Clamp:
					filterMin = GL.LINEAR;
					filterMinMiped = GL.LINEAR_MIPMAP_LINEAR;
					filterMag = GL.LINEAR;
					addressU = GL.CLAMP_TO_EDGE;
					addressV = GL.CLAMP_TO_EDGE;
					addressW = GL.CLAMP_TO_EDGE;
					borderColor = 0;
					break;

				default:
					Debug.ThrowError("SamplerStateDesc", "Unsuported SamplerStateType");
					break;
			}
		}
		#endregion
	}
}