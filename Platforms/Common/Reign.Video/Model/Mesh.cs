using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;

namespace Reign.Video
{
	public class Mesh : Disposable
	{
		#region Properties
		public string Name {get; private set;}
		public MaterialI Material;

		public VertexBufferI VertexBuffer {get; private set;}
		public IndexBufferI IndexBuffer {get; private set;}
		public BufferLayoutDescI LayoutDesc {get; private set;}

		public VertexBufferI InstancingVertexBuffer {get; private set;}
		public IndexBufferI InstancingIndexBuffer {get; private set;}
		public BufferLayoutDescI InstancingLayoutDesc {get; private set;}
		public int ClassicInstanceCount {get; private set;}
		#endregion

		#region Constructors
		public Mesh(BinaryReader reader, Model model, int classicInstanceCount)
		: base(model)
		{
			try
			{
				Name = reader.ReadString();

				// material
				int materialIndex = reader.ReadInt32();
				if (materialIndex != -1) Material = model.Materials[materialIndex];

				// elements
				int elementCount = reader.ReadInt32();
				var elements = new List<BufferLayoutElement>();
				for (int i = 0; i != elementCount; ++i)
				{
					elements.Add(new BufferLayoutElement((BufferLayoutElementTypes)reader.ReadInt32(), (BufferLayoutElementUsages)reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
				}
				LayoutDesc = BufferLayoutDescAPI.New(elements);

				// vertices
				int vertexFloatCount = reader.ReadInt32();
				var vertices = new float[vertexFloatCount];
				for (int i = 0; i != vertexFloatCount; ++i)
				{
					vertices[i] = reader.ReadSingle();
				}
				VertexBuffer = VertexBufferAPI.New(this, LayoutDesc, BufferUsages.Default, VertexBufferTopologys.Triangle, vertices);

				// indices
				int indexCount = reader.ReadInt32();
				var indices = new int[indexCount];
				for (int i = 0; i != indexCount; ++i)
				{
					indices[i] = reader.ReadInt32();
				}
				IndexBuffer = IndexBufferAPI.New(this, BufferUsages.Default, indices);

				// create instancing buffers
				ClassicInstanceCount = classicInstanceCount;
				if (classicInstanceCount > 0)
				{
					var intancingElements = new List<BufferLayoutElement>();
					foreach (var element in elements) intancingElements.Add(element);
					intancingElements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Float, BufferLayoutElementUsages.IndexClassic, 0, 0, LayoutDesc.FloatCount));
					InstancingLayoutDesc = BufferLayoutDescAPI.New(intancingElements);

					int instanceVertexFloatCount = (vertexFloatCount * classicInstanceCount) + (VertexBuffer.VertexCount * classicInstanceCount);
					var instancingVertices = new float[instanceVertexFloatCount];
					int vi = 0;
					for (int i = 0; i != classicInstanceCount; ++i)
					{
						int vOffset = 0;
						for (int i2 = 0; i2 != VertexBuffer.VertexCount; ++i2)
						{
							for (int i3 = 0; i3 != VertexBuffer.VertexFloatArraySize; ++i3)
							{
								instancingVertices[vi] = vertices[vOffset];
								++vi;
								++vOffset;
							}

							instancingVertices[vi] = i;
							++vi;
						}
					}
					InstancingVertexBuffer = VertexBufferAPI.New(this, InstancingLayoutDesc, BufferUsages.Default, VertexBuffer.Topology, instancingVertices);

					int instanceIndexCount = (indexCount * classicInstanceCount);
					var instancingIndices = new int[instanceIndexCount];
					int ii = 0, iOffset = 0;
					for (int i = 0; i != classicInstanceCount; ++i)
					{
						for (int i2 = 0; i2 != indexCount; ++i2)
						{
							instancingIndices[ii] = indices[i2] + iOffset;
							++ii;
						}

						iOffset += VertexBuffer.VertexCount;
					}
					InstancingIndexBuffer = IndexBufferAPI.New(this, BufferUsages.Default, instancingIndices);
				}
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		private static void initData(SoftwareMesh softwareMesh, bool loadColors, bool loadUVs, bool loadNormals, out List<BufferLayoutElement> elements, out float[] vertices, out int[] indices)
		{
			// get position float size
			int posFloatCount = 0;
			var posElementType = BufferLayoutElementTypes.Vector3;
			switch (softwareMesh.Dimensions)
			{
				case 2:
					posElementType = BufferLayoutElementTypes.Vector2;
					posFloatCount = 2;
					break;

				case 3:
					posElementType = BufferLayoutElementTypes.Vector3;
					posFloatCount = 3;
					break;
			}

			// get vertex float size and create layout
			elements = new List<BufferLayoutElement>();
			int vertFloatCount = 0, posCount = 0, colorCount = 0, normalCount = 0, uvCount = 0;
			foreach (var key in softwareMesh.VertexComponentKeys)
			{
				switch (key.Key)
				{
					case VertexComponentKeyTypes.Positions:
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
					case TriangleComponentKeyTypes.ColorComponents:
						if (loadColors)
						{
							elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector4, BufferLayoutElementUsages.Color, 0, colorCount, vertFloatCount));
							vertFloatCount += 4;
							++colorCount;
						}
						break;

					case TriangleComponentKeyTypes.NormalComponents:
						if (loadNormals)
						{
							elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector3, BufferLayoutElementUsages.Normal, 0, normalCount, vertFloatCount));
							vertFloatCount += 3;
							++normalCount;
						}
						break;

					case TriangleComponentKeyTypes.UVComponents:
						if (loadUVs)
						{
							elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector2, BufferLayoutElementUsages.UV, 0, uvCount, vertFloatCount));
							vertFloatCount += 2;
							++uvCount;
						}
						break;
				}
			}

			// create vertex buffer
			var meshProcessor = new HardwareMeshProcessor(softwareMesh, loadColors, loadUVs, loadNormals);
			vertices = new float[meshProcessor.Verticies.Count * vertFloatCount];
			int vi = 0;
			foreach (var vertex in meshProcessor.Verticies)
			{
				int posIndex = 0, colorIndex = 0, normIndex = 0, uvIndex = 0;
				foreach (var element in elements)
				{
					switch (element.Usage)
					{
						case BufferLayoutElementUsages.Position:
							vertices[vi] = vertex.Positions[posIndex].X;
							vertices[vi+1] = vertex.Positions[posIndex].Y;
							if (posFloatCount == 3) vertices[vi+2] = vertex.Positions[posIndex].Z;
							vi += posFloatCount;
							++posIndex;
							break;

						case BufferLayoutElementUsages.Color:
							if (loadColors)
							{
								vertices[vi] = vertex.Colors[uvIndex].X;
								vertices[vi+1] = vertex.Colors[uvIndex].Y;
								vi += 4;
								++colorIndex;
							}
							break;

						case BufferLayoutElementUsages.Normal:
							if (loadNormals)
							{
								vertices[vi] = vertex.Normals[normIndex].X;
								vertices[vi+1] = vertex.Normals[normIndex].Y;
								vertices[vi+2] = vertex.Normals[normIndex].Z;
								vi += 3;
								++normIndex;
							}
							break;

						case BufferLayoutElementUsages.UV:
							if (loadUVs)
							{
								vertices[vi] = vertex.UVs[uvIndex].X;
								vertices[vi+1] = vertex.UVs[uvIndex].Y;
								vi += 2;
								++uvIndex;
							}
							break;
					}
				}
			}

			// create index buffer
			indices = new int[meshProcessor.Triangles.Count * 3];
			int ti = 0;
			foreach (var triangle in meshProcessor.Triangles)
			{
				indices[ti] = triangle.Verticies[0].Index;
				indices[ti+1] = triangle.Verticies[1].Index;
				indices[ti+2] = triangle.Verticies[2].Index;
				ti += 3;
			}
		}
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareModel softwareModel, SoftwareMesh softwareMesh, bool loadColors, bool loadUVs, bool loadNormals)
		{
			List<BufferLayoutElement> elements;
			float[] vertices;
			int[] indices;
			initData(softwareMesh, loadColors, loadUVs, loadNormals, out elements, out vertices, out indices);

			// name
			writer.Write(softwareMesh.Name);

			// material
			int materialIndex = -1;
			if (softwareMesh.Material != null)
			{
				for (int i = 0; i != softwareModel.Materials.Count; ++i)
				{
					if (softwareMesh.Material == softwareModel.Materials[i])
					{
						materialIndex = i;
						break;
					}
				}
			}
			writer.Write(materialIndex);

			// elements
			writer.Write(elements.Count);
			foreach (var element in elements)
			{
				writer.Write((int)element.Type);
				writer.Write((int)element.Usage);
				writer.Write(element.StreamIndex);
				writer.Write(element.UsageIndex);
				writer.Write(element.FloatOffset);
			}

			// vertices
			writer.Write(vertices.Length);
			foreach (var vertex in vertices)
			{
				writer.Write(vertex);
			}

			// indices
			writer.Write(indices.Length);
			foreach (var index in indices)
			{
				writer.Write(index);
			}
		}

		public void Enable(ObjectMesh objectMesh)
		{
			if (Material != null)
			{
				Material.Enable();
				Material.Apply(objectMesh);
			}
			VertexBuffer.Enable(IndexBuffer);
		}

		public void Draw()
		{
			VertexBuffer.Draw();
		}
		#endregion
	}
}