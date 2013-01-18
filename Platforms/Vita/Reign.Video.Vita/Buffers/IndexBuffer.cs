using Reign.Core;
using System;

namespace Reign.Video.Vita
{
	public class IndexBuffer : IndexBufferI
	{
		#region Properties
		internal ushort[] indices;
		public bool Used {get; internal set;}
		public bool Updateable;
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

		public IndexBuffer(DisposableI parent, BufferUsages bufferUsage)
		: base(parent, bufferUsage)
		{
			init(parent, null);
		}

		public IndexBuffer(DisposableI parent, BufferUsages bufferUsage, int[] indices)
		: base(parent, bufferUsage)
		{
			init(parent, indices);
		}

		public IndexBuffer(DisposableI parent, BufferUsages bufferUsage, int[] indices, bool _32BitIndice)
		: base(parent, bufferUsage, _32BitIndice)
		{
			init(parent, indices);
		}

		private void init(DisposableI parent, int[] indices)
		{
			Init(indices);
		}

		public override void Dispose()
		{
			disposeChilderen();
			indices = null;
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Init(int[] indices)
		{
			base.Init(indices);
			this.indices = new ushort[indices.Length];
			for (int i = 0; i != indices.Length; ++i)
			{
				this.indices[i] = (ushort)indices[i];
			}
		}

		public override void Update(int[] indices, int updateCount)
		{
			for (int i = 0; i != updateCount; ++i)
			{
				this.indices[i] = (ushort)indices[i];
			}
		}
		#endregion
	}
}