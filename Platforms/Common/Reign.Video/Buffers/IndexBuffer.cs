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
		protected IndexBufferI(DisposableI parent, BufferUsages usage, int[] indices)
		: base(parent)
		{
			this.usage = usage;
			indexByteSize = sizeof(short);
			indexCount = indices.Length;
			_32BitIndices = indexCount > short.MaxValue;
			indexByteSize = _32BitIndices ? sizeof(int) : sizeof(short);
		}
		#endregion

		#region Methods
		// TODO: Implement contructors for these
		/*public void Init(List<int[]> indices)
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
		}*/

		public abstract void Update(int[] indices, int updateCount);
		#endregion
	}

	public static class IndexBufferAPI
	{
		public static void Init(NewPtrMethod1 newPtr1)
		{
			IndexBufferAPI.newPtr1 = newPtr1;
		}

		public delegate IndexBufferI NewPtrMethod1(DisposableI parent, BufferUsages usage, int[] indices);
		private static NewPtrMethod1 newPtr1;
		public static IndexBufferI New(DisposableI parent, BufferUsages usage, int[] indices)
		{
			return newPtr1(parent, usage, indices);
		}
	}
}