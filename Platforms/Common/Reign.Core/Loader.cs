using System;
using System.Collections.Generic;

namespace Reign.Core
{
	public interface ILoadable
	{
		bool Loaded {get;}
		bool FailedToLoad {get;}

		bool UpdateLoad();
	}

	public class LoadWaiter : ILoadable
	{
		#region Properties
		public bool Loaded {get; private set;}
		public bool FailedToLoad {get; private set;}
		public Loader.LoadedCallbackMethod LoadedCallback {get; private set;}

		private ILoadable[] loadables;
		public ILoadable[] Loadables {get{return loadables;}}
		#endregion

		#region Constructors
		public LoadWaiter(ILoadable[] loadables, Loader.LoadedCallbackMethod loadedCallback)
		{
			this.loadables = loadables;
			LoadedCallback = loadedCallback;

			Loader.AddLoadable(this);
		}

		public bool UpdateLoad()
		{
			int loadedCount = 0;
			foreach (var loadable in loadables)
			{
				if (loadable.Loaded) ++loadedCount;
				else if (loadable.UpdateLoad()) ++loadedCount;
				if (loadable.FailedToLoad)
				{
					FailedToLoad = true;
					if (LoadedCallback != null) LoadedCallback(this, false);
					finish();
					return false;
				}
			}

			if (loadables.Length == loadedCount)
			{
				Loaded = true;
				if (LoadedCallback != null) LoadedCallback(this, true);
				finish();
				return true;
			}
			
			return false;
		}

		private void finish()
		{
			LoadedCallback = null;
		}
		#endregion
	}

	public static class Loader
	{
		#region Properties
		public delegate void LoadedCallbackMethod(object sender, bool succeeded);

		public static int ItemsRemainingToLoad {get{return loadables.Count;}}
		private static List<ILoadable> loadables;
		private static List<Exception> loadableExceptions;
		private static Exception asyncLoadingExeception;
		private static bool aysncLoadDone;
		#endregion

		#region Constructors
		static Loader()
		{
			loadables = new List<ILoadable>();
			loadableExceptions = new List<Exception>();
			aysncLoadDone = true;
		}
		#endregion

		#region Methods
		public static void AddLoadable(ILoadable loadable)
		{
			lock (loadables) if (!loadables.Contains(loadable)) loadables.Add(loadable);
		}

		public static void AddLoadableException(Exception e)
		{
			lock (loadableExceptions)
			{
				loadableExceptions.Add(e);
				Debug.PrintError("Loadable Error", e.Message);
			}
		}

		public static Exception UpdateLoad()
		{
			if (loadableExceptions.Count != 0)
			{
				var e = loadableExceptions[0];
				loadableExceptions.Remove(e);
				return e;
			}

			if (aysncLoadDone)
			{
				if (asyncLoadingExeception != null)
				{
					var e = asyncLoadingExeception;
					asyncLoadingExeception = null;
					return e;
				}

				if (loadables.Count != 0)
				{
					aysncLoadDone = false;
					asyncLoadingExeception = null;
					asyncUpdateLoad();
					return asyncLoadingExeception;
				}
			}

			return null;
		}

		private static void asyncUpdateLoad()
		{
			if (ItemsRemainingToLoad == 0)
			{
				aysncLoadDone = true;
				return;
			}

			foreach (var loadable in loadables.ToArray())
			{
				try
				{
					if (loadable.UpdateLoad() || loadable.FailedToLoad)
					{
						loadables.Remove(loadable);
						if (loadable.FailedToLoad) Debug.ThrowError("Loader", "Failed to load object");
					}
				}
				catch (Exception e)
				{
					loadables.Remove(loadable);
					asyncLoadingExeception = e;
					break;
				}
			}

			aysncLoadDone = true;
		}

		public static void Clear()
		{
			aysncLoadDone = false;
			asyncLoadingExeception = null;
			loadables.Clear();
			loadableExceptions.Clear();
		}
		#endregion
	}
}