using Reign.Core;
using System.Collections.Generic;
using System;

namespace Reign.Video
{
	public enum MeshVertexSizes
	{
		Float2,
		Float3
	}

	public abstract class MeshI : Disposable
	{
		#region Properties
		public Vector3 Location, Rotation, Scale;
		public MaterialI Material;

		private VertexBufferI vertexBuffer;
		private IndexBufferI indexBuffer;
		public BufferLayoutDescI LayoutDesc {get; private set;}
		#endregion

		#region Constructors
		public MeshI(ModelI model, SoftwareModel softwareModel, SoftwareMesh softwareMesh, MeshVertexSizes positionSize)
		: base(model)
		{
			try
			{
				// material
				if (softwareMesh.Material != null)
				{
					for (int i = 0; i != softwareModel.Materials.Count; ++i)
					{
					    if (softwareMesh.Material == softwareModel.Materials[i])
					    {
					        Material = model.Materials[i];
					        break;
					    }
					}

					if (Material == null) Debug.ThrowError("Mesh", "Failed to find material: " + softwareMesh.Material.Name);
				}

				// transformations
				Location = softwareMesh.Location;
				Rotation = softwareMesh.Rotation;
				Scale = softwareMesh.Scale;

				// get position float size
				int posFloatCount = 0;
				var posElementType = BufferLayoutElementTypes.Vector3;
				switch (positionSize)
				{
					case (MeshVertexSizes.Float2):
						posElementType = BufferLayoutElementTypes.Vector2;
						posFloatCount = 2;
						break;
					case (MeshVertexSizes.Float3):
						posElementType = BufferLayoutElementTypes.Vector3;
						posFloatCount = 3;
						break;
				}

				// get vertex float size and create layout
				var elements = new List<BufferLayoutElement>();
				int vertFloatCount = 0, posCount = 0, normalCount = 0, uvCount = 0;
				foreach (var key in softwareMesh.VertexComponentKeys)
				{
					switch (key.Key)
					{
						case (VertexComponentKeyTypes.Positions):
							elements.Add(new BufferLayoutElement(posElementType, BufferLayoutElementUsages.Position, 0, posCount, vertFloatCount));
							vertFloatCount += posFloatCount;
							++posCount;
							break;
					}
				}

				foreach (var key in softwareMesh.TriangleComponentKeys)
				{
					switch (key.Key)
					{
						case (TriangleComponentKeyTypes.NormalComponents):
						    elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Normal, 0, normalCount, vertFloatCount));
						    vertFloatCount += 3;
						    ++normalCount;
						    break;

						case (TriangleComponentKeyTypes.UVComponents):
							elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, uvCount, vertFloatCount));
							vertFloatCount += 2;
							++uvCount;
							break;
					}
				}

				// create vertex buffer
				LayoutDesc = createBufferLayoutDesc(elements);
				var meshProcessor = new HardwareMeshProcessor(softwareMesh);
				var verts = new float[meshProcessor.Verticies.Count * vertFloatCount];
				int vi = 0;
				foreach (var vertex in meshProcessor.Verticies)
				{
					int posIndex = 0, normIndex = 0, uvIndex = 0;
					foreach (var element in elements)
					{
						switch (element.Usage)
						{
							case (BufferLayoutElementUsages.Position):
								verts[vi] = vertex.Positions[posIndex].X;
								verts[vi+1] = vertex.Positions[posIndex].Y;
								if (posFloatCount == 3) verts[vi+2] = vertex.Positions[posIndex].Z;
								vi += posFloatCount;
								++posIndex;
								break;

							case (BufferLayoutElementUsages.Normal):
								verts[vi] = vertex.Normals[normIndex].X;
								verts[vi+1] = vertex.Normals[normIndex].Y;
								verts[vi+2] = vertex.Normals[normIndex].Z;
								vi += 3;
								++normIndex;
								break;

							case (BufferLayoutElementUsages.UV):
								verts[vi] = vertex.UVs[uvIndex].X;
								verts[vi+1] = vertex.UVs[uvIndex].Y;
								vi += 2;
								++uvIndex;
								break;
						}
					}
				}
				vertexBuffer = createVertexBuffer(this, LayoutDesc, BufferUsages.Default, VertexBufferTopologys.Triangle, verts);

				// create index buffer
				var indicies = new int[meshProcessor.Triangles.Count * 3];
				int ti = 0;
				foreach (var triangle in meshProcessor.Triangles)
				{
				    indicies[ti] = triangle.Verticies[0].Index;
				    indicies[ti+1] = triangle.Verticies[1].Index;
				    indicies[ti+2] = triangle.Verticies[2].Index;
				    ti += 3;
				}
				indexBuffer = createIndexBuffer(this, BufferUsages.Default, indicies, indicies.Length > short.MaxValue);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		protected abstract BufferLayoutDescI createBufferLayoutDesc(List<BufferLayoutElement> elements);
		protected abstract VertexBufferI createVertexBuffer(DisposableI parent, BufferLayoutDescI layoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices);
		protected abstract IndexBufferI createIndexBuffer(DisposableI parent, BufferUsages usage, int[] indices, bool _32BitIndices);
		#endregion

		#region Methods
		public void Enable()
		{
			if (Material != null)
			{
				Material.Enable();
				Material.Apply(this);
			}
			vertexBuffer.Enable(indexBuffer);
		}

		public void Draw()
		{
			vertexBuffer.Draw();
		}

		public void Render()
		{
			Enable();
			Draw();
		}
		#endregion
	}
}