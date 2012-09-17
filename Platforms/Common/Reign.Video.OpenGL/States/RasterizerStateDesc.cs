using System;
using Reign.Core;

namespace Reign.Video.OpenGL
{
	public class RasterizerStateDesc : RasterizerStateDescI
	{
		#region Properties
		public uint FillMode {get; private set;}
		public uint CullMode {get; private set;}
		public bool MultisampleEnable {get; private set;}
		#endregion

		#region Constructors
		public RasterizerStateDesc(RasterizerStateTypes type)
		{
			switch (type)
			{
				case (RasterizerStateTypes.Solid_CullNone):
					FillMode = GL.FILL;
					CullMode = GL.NONE;
					MultisampleEnable = false;
					break;

				case (RasterizerStateTypes.Solid_CullCW):
					FillMode = GL.FILL;
					CullMode = GL.BACK;
					MultisampleEnable = false;
					break;

				case (RasterizerStateTypes.Solid_CullCCW):
					FillMode = GL.FILL;
					CullMode = GL.FRONT;
					MultisampleEnable = false;
					break;

				default:
					Debug.ThrowError("RasterizerStateDesc", "Unsuported RasterizerStateType");
					break;
			}
		}
		#endregion
	}
}