using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Rect2
	{
		#region Properties
		public Point2 Position;
		public Size2 Size;

		public int Left {get{return Position.X;}}
		public int Right {get{return Position.X+Size.Width;}}
		public int Bottom {get{return Position.Y;}}
		public int Top {get{return Position.Y+Size.Height;}}

		public static readonly Rect2 Zero = new Rect2();
		#endregion

		#region Constructors
		public Rect2(int x, int y, int width, int height)
		{
			Position.X = x;
			Position.Y = y;
			Size.Width = width;
			Size.Height = height;
		}

		public Rect2(Point2 position, Size2 size)
		{
			Position = position;
			Size = size;
		}
		#endregion

		#region Methods
		
		#endregion
	}
}