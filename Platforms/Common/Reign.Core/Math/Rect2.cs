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
		public Rect2(int x, int y, int width, int height)
		{
			Location.X = x;
			Location.Y = y;
			Size.Width = width;
			Size.Height = height;
		}

		public Rect2(Point2 location, Size2 size)
		{
			Location = location;
			Size = size;
		}
		#endregion

		#region Methods
		
		#endregion
	}
}