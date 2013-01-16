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
		#if ANDROID || NaCl || SILVERLIGHT
		private long millisecond;
		#else
		private Stopwatch stopWatch;
		#endif
		private long fps;
		private int fpsTic;
		private float seconds;

		public int FPS {get; private set;}
		public float Delta {get; private set;}

		public int FPSGoal
		{
			get {return (int)fps;}
			set {fps = value;}
		}
		#endregion

		#region Constructors
		public Time(int fps)
		{
			this.fps = fps;
			FPS = fps;
			#if !ANDROID && !NaCl && !SILVERLIGHT
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
			#elif NaCl || SILVERLIGHT
			millisecond = DateTime.Now.Millisecond;
			#else
			stopWatch.Start();
			#endif
		}

		public void Update()
		{
			#if ANDROID
			long currentMilli = JavaSystem.CurrentTimeMillis();
			if (millisecond <= currentMilli) Delta += (((currentMilli - millisecond) / 1000f) - Delta) * .1f;
			millisecond = currentMilli;
			#elif NaCl || SILVERLIGHT
			long currentMilli = DateTime.Now.Millisecond;
			if (millisecond <= currentMilli) Delta += (((currentMilli - millisecond) / 1000f) - Delta) * .1f;
			millisecond = currentMilli;
			#else
			Delta += ((stopWatch.ElapsedTicks / (float)(Stopwatch.Frequency)) - Delta) * .1f;
			#if XBOX360 || VITA
			stopWatch.Reset();
			stopWatch.Start();
			#else
			stopWatch.Restart();
			#endif
			#endif

			Delta = (Delta > 1) ? 1 : Delta;
			Delta = (Delta < 0) ? 0 : Delta;

			++fpsTic;
			seconds += Delta;
			if (seconds >= 1)
			{
				FPS = fpsTic;
				fpsTic = 0;
				seconds = 0;
			}
		}

		public void ManualUpdate(float delta)
		{
			Delta = delta;
			
			++fpsTic;
			seconds += Delta;
			if (seconds >= 1)
			{
				FPS = fpsTic;
				fpsTic = 0;
				seconds = 0;
			}
		}

		public void Sleep()
		{
			#if !iOS && !ANDROID && !NaCl && !SILVERLIGHT
			int sleepTime = (int)System.Math.Max((1000/fps) - stopWatch.ElapsedMilliseconds, 0);

			#if METRO
			new ManualResetEvent(false).WaitOne(sleepTime);
			#else
			Thread.Sleep(sleepTime);
			#endif
			#endif
		}
		#endregion
	}
}