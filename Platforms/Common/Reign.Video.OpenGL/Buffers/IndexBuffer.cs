using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class IndexBuffer : IndexBufferI
	{
		#region Properties
		private Video video;
		private uint indexBuffer;
		#endregion

		#region Constructors
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
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				if (indices != null) Init(indices);
			}
			catch (Exception ex)
			{
				Dispose();
				throw ex;
			}
		}

		public unsafe override void Dispose()
		{
			disposeChilderen();
			if (indexBuffer != 0)
			{
				if (!OS.AutoDisposedGL)
				{
					uint indexBufferTEMP = indexBuffer;
					GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, 0);
					GL.DeleteBuffers(1, &indexBufferTEMP);
				}
				indexBuffer = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public unsafe override void Init(int[] indices)
		{
			base.Init(indices);
			if (indexBuffer != 0)
			{
				uint indexBufferTEMP = indexBuffer;
				GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, 0);
				GL.DeleteBuffers(1, &indexBufferTEMP);
				indexBuffer = 0;
			}

			uint vPtr = 0;
			GL.GenBuffers(1, &vPtr);
			indexBuffer = vPtr;
			if (indexBuffer == 0) Debug.ThrowError("IndexBuffer", "Failed to create IndexBuffer");

			GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, indexBuffer);
			if (_32BitIndices)
			{
				fixed(int* indicesPtr = indices)
				{
					var bufferSize = new IntPtr(indexByteSize * indexCount);
					GL.BufferData(GL.ELEMENT_ARRAY_BUFFER, bufferSize, indicesPtr, GL.STATIC_DRAW);
				}
			}
			else
			{
				var indices16Bit = new short[indices.Length];
				for (int i = 0; i != indices.Length; ++i)
				{
					indices16Bit[i] = (short)indices[i];
				}

				fixed(short* indicesPtr = indices16Bit)
				{
					var bufferSize = new IntPtr(indexByteSize * indexCount);
					GL.BufferData(GL.ELEMENT_ARRAY_BUFFER, bufferSize, indicesPtr, GL.STATIC_DRAW);
				}
			}

			Video.checkForError();
		}

		//glMapBuffer 
		//glUnmapBuffer 
		public unsafe override void Update(int[] indices, int updateCount)
		{
			GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, indexBuffer);

			if (_32BitIndices)
			{
				fixed(int* indicesPtr = indices)
				{
					IntPtr bufferSize = new IntPtr(indexByteSize * updateCount);
					GL.BufferSubData(GL.ELEMENT_ARRAY_BUFFER, IntPtr.Zero, bufferSize, indicesPtr);
				}
			}
			else
			{
				var indices16Bit = new short[updateCount];
				for (int i = 0; i != updateCount; ++i)
				{
					indices16Bit[i] = (short)indices[i];
				}

				fixed(short* indicesPtr = indices16Bit)
				{
					IntPtr bufferSize = new IntPtr(indexByteSize * updateCount);
					GL.BufferSubData(GL.ELEMENT_ARRAY_BUFFER, IntPtr.Zero, bufferSize, indicesPtr);
				}
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}

		internal void enable()
		{
			GL.BindBuffer(GL.ELEMENT_ARRAY_BUFFER, indexBuffer);
			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}