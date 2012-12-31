using X = Microsoft.Xna.Framework.Graphics;
using System;
using Reign.Core;

namespace Reign.Video.XNA
{
	public class IndexBuffer : IndexBufferI
	{
		#region Properties
		private Video video;
		private X.IndexBuffer indexBuffer;
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

		public IndexBuffer(DisposableI parent, BufferUsages bufferUsage, int[] indices, bool _32BitIndices)
		: base(parent, bufferUsage, _32BitIndices)
		{
			init(parent, indices);
		}

		void init(DisposableI parent, int[] indices)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				if (indices != null) Init(indices);
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
			if (indexBuffer != null)
			{
				indexBuffer.Dispose();
				indexBuffer = null;
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public override void Init(int[] indices)
		{
			base.Init(indices);
			if (indexBuffer != null)
			{
				indexBuffer.Dispose();
				indexBuffer = null;
			}

			#if SILVERLIGHT
			X.IndexElementSize elementSize = X.IndexElementSize.SixteenBits;
			#else
			X.IndexElementSize elementSize = _32BitIndices ? X.IndexElementSize.ThirtyTwoBits : X.IndexElementSize.SixteenBits;
			#endif
			indexBuffer = new X.IndexBuffer(video.Device, elementSize, indices.Length, X.BufferUsage.WriteOnly);
			Update(indices, indexCount);
		}

		public override void Update(int[] indices, int updateCount)
		{
			if (_32BitIndices)
			{
				indexBuffer.SetData<int>(indices, 0, updateCount);
			}
			else
			{
				var indicesTEMP = new short[updateCount];
				for (int i = 0; i != updateCount; ++i)
				{
					indicesTEMP[i] = (short)indices[i];
				}
				indexBuffer.SetData<short>(indicesTEMP, 0, updateCount);
			}
		}

		internal void enable()
		{
			video.Device.Indices = indexBuffer;
		}
		#endregion
	}
}