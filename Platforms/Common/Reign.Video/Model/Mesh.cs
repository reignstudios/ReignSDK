using Reign.Core;
using System.Collections.Generic;
using System;
using System.IO;

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
		public MeshI(BinaryReader reader, ModelI model)
		: base(model)
		{
			try
			{
				// material
				int materialIndex = reader.ReadInt32();
				if (materialIndex != -1) Material = model.Materials[materialIndex];

				// transform
				Location = reader.ReadVector3();
				Scale = reader.ReadVector3();
				Rotation = reader.ReadVector3();

				// elements
				int elementCount = reader.ReadInt32();
				var elements = new List<BufferLayoutElement>();
				for (int i = 0; i != elementCount; ++i)
				{
					elements.Add(new BufferLayoutElement((BufferLayoutElementTypes)reader.ReadInt32(), (BufferLayoutElementUsages)reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()));
				}
				LayoutDesc = createBufferLayoutDesc(elements);

				// vertices
				int vertexCount = reader.ReadInt32();
				var vertices = new float[vertexCount];
				for (int i = 0; i != vertexCount; ++i)
				{
					vertices[i] = reader.ReadSingle();
				}
				vertexBuffer = createVertexBuffer(this, LayoutDesc, BufferUsages.Default, VertexBufferTopologys.Triangle, vertices);

				// indices
				int indexCount = reader.ReadInt32();
				var indices = new int[indexCount];
				for (int i = 0; i != indexCount; ++i)
				{
					indices[i] = reader.ReadInt32();
				}
				indexBuffer = createIndexBuffer(this, BufferUsages.Default, indices, indices.Length > short.MaxValue);
			}
			catch (Exception e)
			{
				Dispose();
				throw e;
			}
		}

		private static void initData(SoftwareMesh softwareMesh, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals, out List<BufferLayoutElement> elements, out float[] vertices, out int[] indices)
		{
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
			elements = new List<BufferLayoutElement>();
			int vertFloatCount = 0, posCount = 0, colorCount = 0, normalCount = 0, uvCount = 0;
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
					case (TriangleComponentKeyTypes.ColorComponents):
						elements.Add(new BufferLayoutElement(BufferLayoutElementTypes.Vector4, BufferLayoutElementUsages.Color, 0, colorCount, vertFloatCount));
						vertFloatCount += 4;
						++colorCount;
						break;

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
						case (BufferLayoutElementUsages.Position):
							vertices[vi] = vertex.Positions[posIndex].X;
							vertices[vi+1] = vertex.Positions[posIndex].Y;
							if (posFloatCount == 3) vertices[vi+2] = vertex.Positions[posIndex].Z;
							vi += posFloatCount;
							++posIndex;
							break;

						case (BufferLayoutElementUsages.Color):
							if (loadColors)
							{
								vertices[vi] = vertex.Colors[uvIndex].X;
								vertices[vi+1] = vertex.Colors[uvIndex].Y;
								vi += 4;
								++colorIndex;
							}
							break;

						case (BufferLayoutElementUsages.Normal):
							if (loadNormals)
							{
								vertices[vi] = vertex.Normals[normIndex].X;
								vertices[vi+1] = vertex.Normals[normIndex].Y;
								vertices[vi+2] = vertex.Normals[normIndex].Z;
								vi += 3;
								++normIndex;
							}
							break;

						case (BufferLayoutElementUsages.UV):
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

		protected abstract BufferLayoutDescI createBufferLayoutDesc(List<BufferLayoutElement> elements);
		protected abstract VertexBufferI createVertexBuffer(DisposableI parent, BufferLayoutDescI layoutDesc, BufferUsages usage, VertexBufferTopologys topology, float[] vertices);
		protected abstract IndexBufferI createIndexBuffer(DisposableI parent, BufferUsages usage, int[] indices, bool _32BitIndices);
		#endregion

		#region Methods
		public static void Write(BinaryWriter writer, SoftwareModel softwareModel, SoftwareMesh softwareMesh, MeshVertexSizes positionSize, bool loadColors, bool loadUVs, bool loadNormals)
		{
			List<BufferLayoutElement> elements;
			float[] vertices;
			int[] indices;
			initData(softwareMesh, positionSize, loadColors, loadUVs, loadNormals, out elements, out vertices, out indices);

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

			// transform
			writer.WriteVector(softwareMesh.Location);
			writer.WriteVector(softwareMesh.Scale);
			writer.WriteVector(softwareMesh.Rotation);

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