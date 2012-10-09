using Reign.Core;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

namespace Reign.Video
{
	public abstract class IndexBufferI : Disposable
	{
		#region Properties
		protected BufferUsages usage;
		protected int indexByteSize;
		public bool _32BitIndices {get; protected set;}

		protected int indexCount;
		public int IndexCount {get{return indexCount;}}
		#endregion

		#region Constructors
		protected IndexBufferI(DisposableI parent, BufferUsages usage)
		: base(parent)
		{
			this.usage = usage;
			indexByteSize = sizeof(short);
		}

		protected IndexBufferI(DisposableI parent, BufferUsages usage, bool _32BitIndices)
		: base(parent)
		{
			this.usage = usage;
			this._32BitIndices = _32BitIndices;
			indexByteSize = _32BitIndices ? sizeof(int) : sizeof(short);
		}
		#endregion

		#region Methods
		public virtual void Init(int[] indices)
		{
			indexCount = indices.Length;
		}

		public void Init(List<int[]> indices)
		{
			int[] i = new int[indices.Count];
			int indexSeg = 0;
			foreach (var index in indices)
			{
				foreach (var seg in index)
				{
					i[indexSeg] = seg;
					++indexSeg;
				}
			}

			Init(i);
		}

		public abstract void Update(int[] indices, int updateCount);
		#endregion
	}
}