namespace Reign.Input
{
	public class Button
	{
		#region Properties
		public bool Down {get; private set;}
		public bool Up {get; private set;}
		public int TimeOn {get; private set;}

		private bool onOld;
		public bool On { get; private set; }
		#endregion

		#region Methods
		public void Update(bool on)
		{
			Down = false;
			Up = false;
			onOld = On;
			On = on;
			if (On == true) TimeOn ++;
			else TimeOn = 0;
		
			if (onOld != On)
			{
				if (On == true) Down = true;
				else Up = true;
			}
		}

		public void Flush()
		{
			Down = false;
			Up = false;
			On = false;
			onOld = false;
			TimeOn = 0;
		}
		#endregion
	}
}
