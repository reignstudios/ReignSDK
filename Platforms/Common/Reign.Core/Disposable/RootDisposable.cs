using System.Collections.Generic;

#if XNA
using Microsoft.Xna.Framework.Content;
#endif

namespace Reign.Core
{
	public sealed class RootDisposable : Disposable
	{
		#region Properties
		#if XNA
		public ContentManager Content {get; private set;}
		#endif
		#endregion

		#region Constructors
		public RootDisposable()
		: base(null)
		{
			#if XNA
			init(OS.CurrentApplication.Content);
			#endif
		}

		public RootDisposable(DisposableI disposable)
		: base(disposable)
		{
			#if XNA
			var parent = disposable.FindParentOrSelfWithException<RootDisposable>();
			init(parent.Content);
			#endif
		}

		#if XNA
		public RootDisposable(ContentManager rootContent)
		: base(null)
		{
			init(rootContent);
		}

		private void init(ContentManager rootContent)
		{
			Content = new ContentManager(rootContent.ServiceProvider, rootContent.RootDirectory);
		}
		#endif

		public override void Dispose()
		{
			disposeChilderen();
			#if XNA
			if (Content != null)
			{
				Content.Dispose();
				Content = null;
			}
			#endif
			base.Dispose();
		}
		#endregion
	}
}