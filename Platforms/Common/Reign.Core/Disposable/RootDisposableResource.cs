using System.Collections.Generic;

#if XNA && !SILVERLIGHT
using Microsoft.Xna.Framework.Content;
#endif

namespace Reign.Core
{
	public sealed class RootDisposableResource : DisposableResource
	{
		#region Properties
		#if XNA && !SILVERLIGHT
		public ContentManager Content {get; private set;}
		#endif
		#endregion

		#region Constructors
		public RootDisposableResource()
		: base(null)
		{
			#if XNA && !SILVERLIGHT
			init(((XNAApplication)OS.CurrentApplication).Content);
			#endif
		}

		public RootDisposableResource(IDisposableResource disposable)
		: base(disposable)
		{
			#if XNA && !SILVERLIGHT
			var parent = disposable.FindParentOrSelfWithException<RootDisposableResource>();
			init(parent.Content);
			#endif
		}

		#if XNA && !SILVERLIGHT
		public RootDisposableResource(ContentManager rootContent)
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
			#if XNA && !SILVERLIGHT
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