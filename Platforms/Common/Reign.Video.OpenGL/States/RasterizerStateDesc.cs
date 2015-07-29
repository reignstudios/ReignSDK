using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class RasterizerStateDesc : IRasterizerStateDesc
	{
		#region Properties
		internal uint fillMode;
		internal uint cullMode;
		internal bool multisampleEnable;
		#endregion

		#region Constructors
		public RasterizerStateDesc(RasterizerStateTypes type)
		{
			switch (type)
			{
				case RasterizerStateTypes.Solid_CullNone:
					fillMode = GL.FILL;
					cullMode = GL.NONE;
					multisampleEnable = false;
					break;

				case RasterizerStateTypes.Solid_CullCW:
					fillMode = GL.FILL;
					cullMode = GL.FRONT;
					multisampleEnable = false;
					break;

				case RasterizerStateTypes.Solid_CullCCW:
					fillMode = GL.FILL;
					cullMode = GL.BACK;
					multisampleEnable = false;
					break;

				default:
					Debug.ThrowError("RasterizerStateDesc", "Unsuported RasterizerStateType");
					break;
			}
		}
		#endregion
	}
}