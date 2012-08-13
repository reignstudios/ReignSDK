using Reign.Core;

namespace Reign.Input
{
	public class Touch : Button
	{
		#region Properties
		public Vector2 Location;
		#endregion

		#region Methods
		public void Update(bool on, Vector2 location)
		{
			base.Update(on);
			if (on) Location = location;
		}
		#endregion
	}
}
