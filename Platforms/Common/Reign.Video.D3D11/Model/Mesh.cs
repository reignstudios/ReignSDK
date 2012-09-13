using System;
using Reign.Core;
using System.Collections.Generic;

namespace Reign.Video.D3D11
{
	public class Mesh : MeshI
	{
		#region Constructors
		public Mesh(ModelI model, SoftwareModel softwareModel, SoftwareMesh softwareMesh, MeshVertexSizes positionSize)
		: base(model, softwareModel, softwareMesh, positionSize)
		{
			
		}

		protected override BufferLayoutDescI createBufferLayoutDesc(List<BufferLayoutElement> elements)
		{
			return new BufferLayoutDesc(elements);
		}

		protected override VertexBufferI createVertexBuffer(DisposableI parent, BufferLayoutDescI layoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices)
		{
			return new VertexBuffer(parent, layoutDesc, usage, topology, vertices);
		}

		protected override IndexBufferI createIndexBuffer(DisposableI parent, BufferUsages usage, int[] indices, bool _32BitIndices)
		{
			return new IndexBuffer(parent, usage, indices, _32BitIndices);
		}
		#endregion
	}
}