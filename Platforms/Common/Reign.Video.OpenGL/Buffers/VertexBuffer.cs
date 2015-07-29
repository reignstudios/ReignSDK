using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class VertexBuffer : IVertexBuffer
	{
		#region Properties
		private Video video;
		private uint vertexBuffer;
		private uint primitiveTopology;
		private VertexBuffer instanceBuffer;

		private VertexBufferTopologys topology;
		public override VertexBufferTopologys Topology
		{
			get {return topology;}
			set
			{
				switch (value)
				{
					case VertexBufferTopologys.None: primitiveTopology = 0; break;
					case VertexBufferTopologys.Point: primitiveTopology = GL.POINTS; break;
					case VertexBufferTopologys.Line: primitiveTopology = GL.LINES; break;
					case VertexBufferTopologys.Triangle: primitiveTopology = GL.TRIANGLES; break;
				}
				topology = value;
			}
		}

		private IndexBuffer indexBuffer, currentIndexBuffer;
		public override IIndexBuffer IndexBuffer
		{
			get {return indexBuffer;}
		}
		#endregion

		#region Constructors
		public VertexBuffer(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices)
		: base(parent, bufferLayoutDesc, bufferUsage, vertices)
		{
			init(parent, bufferLayoutDesc, bufferUsage, vertexBufferTopology, vertices, null);
		}

		public VertexBuffer(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices, int[] indices)
		: base(parent, bufferLayoutDesc, bufferUsage, vertices)
		{
			init(parent, bufferLayoutDesc, bufferUsage, vertexBufferTopology, vertices, indices);
		}

		private unsafe void init(IDisposableResource parent, IBufferLayoutDesc bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices, int[] indices)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				Topology = vertexBufferTopology;

				uint vPtr = 0;
				GL.GenBuffers(1, &vPtr);
				vertexBuffer = vPtr;
				if (vertexBuffer == 0) Debug.ThrowError("VertexBuffer", "Failed to create VertexBuffer");

				GL.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);
				fixed (float* verticesPtr = vertices)
				{
					var bufferSize = new IntPtr(vertexByteSize * vertexCount);
					GL.BufferData(GL.ARRAY_BUFFER, bufferSize, verticesPtr, GL.STATIC_DRAW);
				}

				Video.checkForError();
				
				if (indices != null && indices.Length != 0) indexBuffer = new IndexBuffer(this, usage, indices);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		public unsafe override void Dispose()
		{
			disposeChilderen();
			if (vertexBuffer != 0)
			{
				if (!IPlatform.Singleton.AutoDisposedGL)
				{
					uint vertexBufferTEMP = vertexBuffer;
					GL.BindBuffer(GL.ARRAY_BUFFER, 0);
					GL.DeleteBuffers(1, &vertexBufferTEMP);
				}
				vertexBuffer = 0;

				#if DEBUG && !ANDROID
				Video.checkForError();
				#endif
			}
			base.Dispose();
		}
		#endregion

		#region Methods
		public unsafe override void Update(float[] vertices, int updateCount)
		{
			GL.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);
			fixed(float* verticesPtr = vertices)
			{
				IntPtr bufferSize = new IntPtr(vertexByteSize * updateCount);
				GL.BufferSubData(GL.ARRAY_BUFFER, IntPtr.Zero, bufferSize, verticesPtr);
			}

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void Enable()
		{
			video.currentVertexBuffer = this;
			currentIndexBuffer = indexBuffer;
			this.instanceBuffer = null;
		}

		public override void Enable(IIndexBuffer indexBuffer)
		{
			video.currentVertexBuffer = this;
			this.currentIndexBuffer = (IndexBuffer)indexBuffer;
			this.instanceBuffer = null;
		}

		public override void Enable(IVertexBuffer instanceBuffer)
		{
			video.currentVertexBuffer = this;
			this.currentIndexBuffer = indexBuffer;
			this.instanceBuffer = (VertexBuffer)instanceBuffer;
		}

		public override void Enable(IIndexBuffer indexBuffer, IVertexBuffer instanceBuffer)
		{
			video.currentVertexBuffer = this;
			this.currentIndexBuffer = (IndexBuffer)indexBuffer;
			this.instanceBuffer = (VertexBuffer)instanceBuffer;
		}

		private void enable()
		{
			if (video.currentVertexBuffer == this || !BufferLayout.enabled)
			{
				video.currentVertexBuffer = null;
				if (currentIndexBuffer != null) currentIndexBuffer.enable();
				GL.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);

				var bufferLayout = video.currentBufferLayout;
				if (bufferLayout != null) bufferLayout.enable(0);

				if (instanceBuffer != null)
				{
					GL.BindBuffer(GL.ARRAY_BUFFER, instanceBuffer.vertexBuffer);
					if (bufferLayout != null) bufferLayout.enable(1);
				}

				#if DEBUG
				Video.checkForError();
				#endif
			}
		}

		public override void Draw()
		{
			enable();
			if (currentIndexBuffer == null) GL.DrawArrays(primitiveTopology, 0, vertexCount);
			else GL.DrawElements(primitiveTopology, currentIndexBuffer.IndexCount, currentIndexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0));

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void Draw(int drawCount)
		{
			enable();
			if (currentIndexBuffer == null) GL.DrawArrays(primitiveTopology, 0, drawCount);
			else GL.DrawElements(primitiveTopology, drawCount, currentIndexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0));

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void DrawInstanced(int drawCount)
		{
			enable();
			if (currentIndexBuffer == null) GL.DrawArraysInstanced(primitiveTopology, 0, vertexCount, drawCount);
			else GL.DrawElementsInstanced(primitiveTopology, currentIndexBuffer.IndexCount, currentIndexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0), drawCount);

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
		{
			enable();
			if (currentIndexBuffer == null) GL.DrawArrays(primitiveTopology, 0, drawCount * meshVertexCount);
			else GL.DrawElements(primitiveTopology, drawCount * meshIndexCount, currentIndexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0));

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}