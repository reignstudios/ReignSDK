using System;
using System.Diagnostics;
using System.Threading;

#if WINDOWS || (XNA && !XBOX360)
using System.Runtime.InteropServices;
#endif

#if ANDROID
using Java.Lang;
#endif

namespace Reign.Core
{
	public class Time
	{
		#region Properties
		#if ANDROID
		private long millisecond;
		#else
		private Stopwatch stopWatch;
		#endif
		private long fps;

		public int Milliseconds {get; private set;}
		public int FPS {get; private set;}
		public int FPSGoal {get{return (int)fps;}}
		public float Delta {get; private set;}
		#endregion

		#region Constructors
		public Time(int fps)
		{
			this.fps = fps;
			FPS = fps;
			#if !ANDROID
			stopWatch = new Stopwatch();
			#endif
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

		private static TimeCaps caps;

		[DllImport("winmm.dll", EntryPoint="timeGetDevCaps", SetLastError=true)]
		public static extern uint TimeGetDevCaps(ref TimeCaps timeCaps, uint sizeTimeCaps);

		[DllImport("winmm.dll", EntryPoint="timeBeginPeriod", SetLastError=true)]
		public static extern uint TimeBeginPeriod(uint uMilliseconds);

		[DllImport("winmm.dll", EntryPoint="timeEndPeriod", SetLastError=true)]
		public static extern uint TimeEndPeriod(uint uMilliseconds);

		public static void OptimizedMode()
		{
			caps = new TimeCaps();
			if (TimeGetDevCaps(ref caps, (uint)System.Runtime.InteropServices.Marshal.SizeOf(caps)) != 0)
			{
				Debug.ThrowError("StopWatch", "TimeGetDevCaps failed");
			}
			
			if (TimeBeginPeriod(caps.wPeriodMin) != 0)
			{
				Debug.ThrowError("StopWatch", "TimeBeginPeriod failed");
			}
		}

		public static void EndOptimizedMode()
		{
			if (TimeEndPeriod(caps.wPeriodMin) != 0)
			{
				Debug.ThrowError("StopWatch", "TimeEndPeriod failed");
			}
		}
		#endif

		public void Start()
		{
			#if ANDROID
			millisecond = JavaSystem.CurrentTimeMillis();
			#else
			stopWatch.Start();
			#endif
		}

		public bool Update()
		{
			#if ANDROID
			long currentMilli = JavaSystem.CurrentTimeMillis();
			long mili = currentMilli - millisecond;
			long fpsSec = 1000 / fps;
			if (mili >= fpsSec)
			{
				Milliseconds = (int)mili;
				Delta += ((Milliseconds / 1000f) - Delta) * .1f;
				FPS = (int)(fps / (mili / fpsSec));
				millisecond = currentMilli;
				
				return true;
			}
			#else
			long tics = (stopWatch.ElapsedTicks / (Stopwatch.Frequency/fps));
			if (tics != 0)
			{
			    Milliseconds = (int)stopWatch.ElapsedMilliseconds;
				Delta += ((Milliseconds / 1000f) - Delta) * .1f;
				FPS = (int)(fps / tics);

				stopWatch.Restart();
			    return true;
			}
			#endif

			return false;
		}

		public void AdaptiveUpdate()
		{
			Milliseconds = 0;
			Delta = 0;
			FPS = (int)fps;
			#if ANDROID
			millisecond = JavaSystem.CurrentTimeMillis();
			#else
			stopWatch.Restart();
			#endif
		}

		public void ManualUpdate(int milliseconds, float delta, int fps)
		{
			Milliseconds = milliseconds;
			Delta = delta;
			FPS = fps;
		}

		#if !iOS && !ANDROID
		public void Sleep()
		{
			int sleepTime = (int)System.Math.Max((1000/fps) - 5 - stopWatch.ElapsedMilliseconds, 0);
			#if METRO
			new ManualResetEvent(false).WaitOne(sleepTime);
			#else
			Thread.Sleep(sleepTime);
			#endif
		}
		#endif
		#endregion
	}
}