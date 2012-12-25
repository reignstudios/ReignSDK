using Reign.Core;
using Reign_Video_D3D11_Component;
using System;

namespace Reign.Video.D3D11
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

		public static IndexBuffer New(DisposableI parent, BufferUsages usage, int[] indices, bool _32BitIndices)
		{
			return new IndexBuffer(parent, usage, indices, _32BitIndices);
		}

		public IndexBuffer(DisposableI parent, BufferUsages usage, int[] indices)
		: base(parent, usage)
		{
			init(parent, indices);
		}

		public IndexBuffer(DisposableI parent, BufferUsages usage, int[] indices, bool _32BitIndices)
		: base(parent, usage, _32BitIndices)
		{
			init(parent, indices);
		}

		private void init(DisposableI parent, int[] indices)
		{
			try
			{
				var video = parent.FindParentOrSelfWithException<Video>();
				com = new IndexBufferCom(video.com);
				Init(indices);
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
		public override void Init(int[] indices)
		{
			base.Init(indices);

			var bufferUsage = (usage == BufferUsages.Write) ? REIGN_D3D11_USAGE.DYNAMIC : REIGN_D3D11_USAGE.DEFAULT;
			var cpuUsage = (usage == BufferUsages.Write) ? REIGN_D3D11_CPU_ACCESS_FLAG.WRITE : (REIGN_D3D11_CPU_ACCESS_FLAG)0;
			var error = com.Init(indices, indexCount, indexByteSize, bufferUsage, cpuUsage, _32BitIndices);

			if (error == IndexBufferErrors.Buffer) Debug.ThrowError("IndexBuffer", "Failed to create Buffer");
		}

		public override void Update(int[] indices, int updateCount)
		{
			com.Update(indices, updateCount);
		}
		#endregion
	}
}