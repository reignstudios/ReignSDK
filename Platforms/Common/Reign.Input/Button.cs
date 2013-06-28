namespace Reign.Input
{
	public class Button
	{
		#region Properties
		public bool Down {get; private set;}
		public bool Up {get; private set;}
		public int OnTic {get; private set;}

		private bool onOld;
		public bool On {get; private set;}
		#endregion

		#region Methods
		public void Update(bool on)
		{
			Down = false;
			Up = false;
			onOld = On;
			On = on;
			if (On == true) OnTic ++;
			else OnTic = 0;
		
			if (onOld != On)
			{
				if (On == true) Down = true;
				else Up = true;
			}
		}

		public virtual void Flush()
		{
			Down = false;
			Up = false;
			On = false;
			onOld = false;
			OnTic = 0;
		}
		#endregion
	}
}
