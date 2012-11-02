using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Rect2
	{
		#region Properties
		public Point2 Location;
		public Size2 Size;

		public int Left {get{return Location.X;}}
		public int Right {get{return Location.X+Size.Width;}}
		public int Bottom {get{return Location.Y;}}
		public int Top {get{return Location.Y+Size.Height;}}
		#endregion

		#region Constructors
		
		#endregion

		#region Methods
		
		#endregion
	}
}