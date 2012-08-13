namespace Reign.Input
{
	public class Trigger : Button
	{
		#region Properties
		public float Value;
		#endregion

		#region Methods
		public void Update(float value)
		{
			Value = value;
			base.Update(value >= .02f);
		}
		#endregion
	}
}
