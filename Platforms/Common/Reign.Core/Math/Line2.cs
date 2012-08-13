using System.Runtime.InteropServices;

namespace Reign.Core
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Line2
	{
		#region Properties
		public Vector2 P1, P2;
		#endregion

		#region Constructors
		public Line2(Vector2 p1, Vector2 p2)
		{
			P1 = p1;
			P2 = p2;
		}
		#endregion

		#region Methods
		
		#endregion
	}
}