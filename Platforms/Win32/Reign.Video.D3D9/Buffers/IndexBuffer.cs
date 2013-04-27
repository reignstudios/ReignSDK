using System;
using Reign_Video_D3D9_Component;
using Reign.Video;
using Reign.Core;

namespace Reign.Video.D3D9
{
	public class IndexBuffer : IndexBufferI
	{
		#region Properties
		internal IndexBufferCom com;
		#endregion

		#region Constructors
		public static IndexBuffer New(DisposableI parent, BufferUsages usage, int[] indices)
		{
			return new IndexBuffer(parent, usage, indices);
		}

		public IndexBuffer(DisposableI parent, BufferUsages usage, int[] indices)
		: base(parent, usage, indices)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();
				com = new IndexBufferCom(video.com);
				var error = com.Init(indices, indexCount, indexByteSize, REIGN_D3DUSAGE.WRITEONLY, _32BitIndices);
				if (error == IndexBufferErrors.IndexBuffer) Debug.ThrowError("IndexBuffer", "Failed to create IndexBuffer");
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public override void Dispose()
		{
			disposeChilderen();
			if (com != null)
			{
				com.Dispose();
				com = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Update(int[] indices, int updateCount)
		{
			com.Update(indices, updateCount, indexByteSize, _32BitIndices);
		}
		#endregion
	}
}
