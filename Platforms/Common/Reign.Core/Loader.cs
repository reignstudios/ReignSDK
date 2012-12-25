using System;
using System.Collections.Generic;

namespace Reign.Core
{
	public interface LoadableI
	{
		bool Loaded {get;}
		bool FailedToLoad {get;}

		bool UpdateLoad();
	}

	public class LoadWaiter : LoadableI
	{
		public bool Loaded {get; private set;}
		public bool FailedToLoad {get; private set;}
		public Loader.LoadedCallbackMethod LoadedCallback {get; private set;}
		public Loader.FailedToLoadCallbackMethod FailedToLoadCallback {get; private set;}

		private LoadableI[] loadables;
		public LoadableI[] Loadables {get{return loadables;}}

		public LoadWaiter(LoadableI[] loadables, Loader.LoadedCallbackMethod loadedCallback, Loader.FailedToLoadCallbackMethod failedToLoadCallback)
		{
			this.loadables = loadables;
			LoadedCallback = loadedCallback;
			FailedToLoadCallback = failedToLoadCallback;

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
					if (FailedToLoadCallback != null) FailedToLoadCallback();
					finish();
					return false;
				}
			}

			if (loadables.Length == loadedCount)
			{
				Loaded = true;
				if (LoadedCallback != null) LoadedCallback(this);
				finish();
				return true;
			}
			
			return false;
		}

		private void finish()
		{
			LoadedCallback = null;
			FailedToLoadCallback = null;
		}
	}

	public static class Loader
	{
		public delegate void LoadedCallbackMethod(object sender);
		public delegate void FailedToLoadCallbackMethod();

		public static int ItemsRemainingToLoad {get{return loadables.Count;}}
		private static List<LoadableI> loadables;
		private static List<Exception> loadableExceptions;
		private static Exception asyncLoadingExeception;
		private static bool aysncLoadDone;

		static Loader()
		{
			loadables = new List<LoadableI>();
			loadableExceptions = new List<Exception>();
			aysncLoadDone = true;
		}

		public static void AddLoadable(LoadableI loadable)
		{
			lock (loadables) if (!loadables.Contains(loadable)) loadables.Add(loadable);
		}

		public static void AddLoadableException(Exception e)
		{
			lock (loadableExceptions) loadableExceptions.Add(e);
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
					if (loadable.UpdateLoad() || loadable.FailedToLoad) loadables.Remove(loadable);
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
	}
}