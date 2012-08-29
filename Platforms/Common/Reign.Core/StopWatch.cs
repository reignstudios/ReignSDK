using System;
using S = System.Diagnostics;
using System.Threading;

#if WINDOWS || (XNA && !XBOX360)
using System.Runtime.InteropServices;
#endif

namespace Reign.Core
{
	public class StopWatch
	{
		#region Properties
		private long lapTic;
		private S.Stopwatch stopWatch;
		private int environmentTic;

		public long Milliseconds
		{
			get
			{
				return stopWatch.ElapsedMilliseconds;
			}
		}
		#endregion

		#region Constructors
		public StopWatch()
		{
			stopWatch = new S.Stopwatch();
		}
		#endregion

		#region Methods
		#if WINDOWS || (XNA && !XBOX360)
		[StructLayout(LayoutKind.Sequential)]
		public struct TimeCaps
		{
			public uint wPeriodMin;
			public uint wPeriodMax;
		}

		[DllImport("winmm.dll", EntryPoint="timeGetDevCaps", SetLastError=true)]
		public static extern uint TimeGetDevCaps(ref TimeCaps timeCaps, uint sizeTimeCaps);

		[DllImport("winmm.dll", EntryPoint="timeBeginPeriod", SetLastError=true)]
		public static extern uint TimeBeginPeriod(uint uMilliseconds);

		[DllImport("winmm.dll", EntryPoint="timeEndPeriod", SetLastError=true)]
		public static extern uint TimeEndPeriod(uint uMilliseconds);
		#endif

		public void Start()
		{
			stopWatch.Start();
		}

		public void Lap()
		{
			lapTic = stopWatch.ElapsedTicks;
			environmentTic = Environment.TickCount;
		}

		public void Restart()
		{
			stopWatch.Reset();
			stopWatch.Start();
		}

		public bool FPS(int fps)
		{
			Lap();
			if ((lapTic / (S.Stopwatch.Frequency/fps)) != 0)
			{
				Restart();
				return true;
			}

			return false;
		}

		public void FPSFinishSleep()
		{
			int sleepTime = System.Math.Max(14-(Environment.TickCount - environmentTic), 0);
			#if METRO
			new ManualResetEvent(false).WaitOne(sleepTime);
			#else
			Thread.Sleep(sleepTime);
			#endif
		}
		#endregion
	}
}