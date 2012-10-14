using Reign.Core;
using System;

namespace Reign.Video.OpenGL
{
	public class VertexBuffer : VertexBufferI
	{
		#region Properties
		private Video video;
		private uint vertexBuffer;
		private uint primitiveTopology;
		private IndexBuffer indexBuffer;
		private VertexBuffer instanceBuffer;

		private VertexBufferTopologys topology;
		public override VertexBufferTopologys Topology
		{
			get {return topology;}
			set
			{
				switch (value)
				{
					case (VertexBufferTopologys.None): primitiveTopology = 0; break;
					case (VertexBufferTopologys.Point): primitiveTopology = GL.POINTS; break;
					case (VertexBufferTopologys.Line): primitiveTopology = GL.LINES; break;
					case (VertexBufferTopologys.Triangle): primitiveTopology = GL.TRIANGLES; break;
				}
				topology = value;
			}
		}
		#endregion

		#region Constructors
		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology)
		: base(parent, bufferLayoutDesc, bufferUsage)
		{
			init(parent, vertexBufferTopology, null);
		}

		public VertexBuffer(DisposableI parent, BufferLayoutDescI bufferLayoutDesc, BufferUsages bufferUsage, VertexBufferTopologys vertexBufferTopology, float[] vertices)
		: base(parent, bufferLayoutDesc, bufferUsage)
		{
			init(parent, vertexBufferTopology, vertices);
		}

		private void init(DisposableI parent, VertexBufferTopologys vertexBufferTopology, float[] vertices)
		{
			try
			{
				video = parent.FindParentOrSelfWithException<Video>();
				Topology = vertexBufferTopology;

				if (vertices != null) Init(vertices);
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
			if (vertexBuffer != 0)
			{
				if (!OS.AutoDisposedGL)
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
		public unsafe override void Init(float[] vertices)
		{
			base.Init(vertices);
			if (vertexBuffer != 0)
			{
				uint vertexBufferTEMP = vertexBuffer;
				GL.BindBuffer(GL.ARRAY_BUFFER, 0);
				GL.DeleteBuffers(1, &vertexBufferTEMP);
				vertexBuffer = 0;
			}

			uint vPtr = 0;
			GL.GenBuffers(1, &vPtr);
			vertexBuffer = vPtr;
			if (vertexBuffer == 0) Debug.ThrowError("VertexBuffer", "Failed to create VertexBuffer");

			GL.BindBuffer(GL.ARRAY_BUFFER, vertexBuffer);
			fixed(float* verticesPtr = vertices)
			{
				var bufferSize = new IntPtr(vertexByteSize * vertexCount);
				GL.BufferData(GL.ARRAY_BUFFER, bufferSize, verticesPtr, GL.STATIC_DRAW);
			}

			Video.checkForError();
		}

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
		}

		public override void Enable(IndexBufferI indexBuffer)
		{
			this.indexBuffer = (IndexBuffer)indexBuffer;
			this.instanceBuffer = null;
			Enable();
		}

		public override void Enable(VertexBufferI instanceBuffer)
		{
			this.indexBuffer = null;
			this.instanceBuffer = (VertexBuffer)instanceBuffer;
			Enable();
		}

		public override void Enable(IndexBufferI indexBuffer, VertexBufferI instanceBuffer)
		{
			this.indexBuffer = (IndexBuffer)indexBuffer;
			this.instanceBuffer = (VertexBuffer)instanceBuffer;
			Enable();
		}

		private void enable()
		{
			if (video.currentVertexBuffer == this || !BufferLayout.enabled)
			{
				video.currentVertexBuffer = null;
				if (indexBuffer != null) indexBuffer.enable();
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
			if (indexBuffer == null) GL.DrawArrays(primitiveTopology, 0, vertexCount);
			else GL.DrawElements(primitiveTopology, indexBuffer.IndexCount, indexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0));

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void Draw(int drawCount)
		{
			enable();
			if (indexBuffer == null) GL.DrawArrays(primitiveTopology, 0, drawCount);
			else GL.DrawElements(primitiveTopology, drawCount, indexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0));

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void DrawInstanced(int drawCount)
		{
			enable();
			if (indexBuffer == null) GL.DrawArraysInstanced(primitiveTopology, 0, vertexCount, drawCount);
			else GL.DrawElementsInstanced(primitiveTopology, indexBuffer.IndexCount, indexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0), drawCount);

			#if DEBUG
			Video.checkForError();
			#endif
		}

		public override void DrawInstancedClassic(int drawCount, int meshVertexCount, int meshIndexCount)
		{
			enable();
			if (indexBuffer == null) GL.DrawArrays(primitiveTopology, 0, drawCount * meshVertexCount);
			else GL.DrawElements(primitiveTopology, drawCount * meshIndexCount, indexBuffer._32BitIndices ? GL.UNSIGNED_INT : GL.UNSIGNED_SHORT, new IntPtr(0));

			#if DEBUG
			Video.checkForError();
			#endif
		}
		#endregion
	}
}