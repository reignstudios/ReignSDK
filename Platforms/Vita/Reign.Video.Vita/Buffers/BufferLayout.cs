using Reign.Core;
using System;

namespace Reign.Video.Vita
{
	public class BufferLayout : Disposable, BufferLayoutI
	{
		#region Properties
		private Video video;
		internal BufferLayoutDesc desc;
		#endregion

		#region Constructors
		public static BufferLayout New(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		{
			return new BufferLayout(parent, shader, desc);
		}

		public BufferLayout(DisposableI parent, ShaderI shader, BufferLayoutDescI desc)
		: base(parent)
		{
			video = parent.FindParentOrSelfWithException<Video>();
			this.desc = (BufferLayoutDesc)desc;
		}
		#endregion

		#region Methods
		public void Enable()
		{
			// Not used in Vita
		}
		#endregion
	}
}