using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public class VertexProcessor
	{
		public Vector3[] Positions, Normals;
		public Vector2[] UVs;
		public int Index;

		public VertexProcessor(SoftwareVertex vertex, Vector3[] normals, Vector2[] uvs, HardwareMeshProcessor mesh)
		{
			Positions = new Vector3[mesh.positions.Count];
			for (int i = 0; i != Positions.Length; ++i)
			{
				Positions[i] = mesh.positions[i][vertex.Index];
			}

			Normals = normals;
			UVs = uvs;

			mesh.Verticies.AddLast(this);
		}
	}

	public class TriangleProcessor
	{
		public VertexProcessor[] Verticies;

		public TriangleProcessor(SoftwareTriangle triangle, HardwareMeshProcessor mesh)
		{
			// get normal components
			int componentCount = mesh.normalComponents.Count;
			var normals = new Vector3[3][];
			for (int i = 0; i != normals.Length; ++i)
			{
				normals[i] = new Vector3[componentCount];
			}

			for (int i = 0; i != componentCount; ++i)
			{
				var normalComponent = mesh.normalComponents[i][triangle.Index].Normals;
				normals[0][i] = normalComponent[0];
				normals[1][i] = normalComponent[1];
				normals[2][i] = normalComponent[2];
			}

			// get uv components
			componentCount = mesh.uvComponents.Count;
			var uvs = new Vector2[3][];
			for (int i = 0; i != uvs.Length; ++i)
			{
				uvs[i] = new Vector2[componentCount];
			}

			for (int i = 0; i != componentCount; ++i)
			{
				var normalComponent = mesh.uvComponents[i][triangle.Index].UVs;
				uvs[0][i] = normalComponent[0];
				uvs[1][i] = normalComponent[1];
				uvs[2][i] = normalComponent[2];
			}

			// add verticies
			var vertex1 = new VertexProcessor(triangle.Verticies[0], normals[0], uvs[0], mesh);
			var vertex2 = new VertexProcessor(triangle.Verticies[1], normals[1], uvs[1], mesh);
			var vertex3 = new VertexProcessor(triangle.Verticies[2], normals[2], uvs[2], mesh);
			Verticies = new VertexProcessor[3] {vertex1, vertex2, vertex3};

			mesh.Triangles.Add(this);
		}
	}

	public class HardwareMeshProcessor
	{
		public LinkedList<VertexProcessor> Verticies;
		public List<TriangleProcessor> Triangles;
		internal SoftwareMesh mesh;
		internal List<Vector3[]> positions;
		internal List<TriangleNormalComponent[]> normalComponents;
		internal List<TriangleUVComponent[]> uvComponents;

		public HardwareMeshProcessor(SoftwareMesh mesh)
		{
			this.mesh = mesh;

			// get vertex component types
			positions = new List<Vector3[]>();
			foreach (var key in mesh.VertexComponentKeys)
			{
				switch (key.Key)
				{
					case (VertexComponentKeyTypes.Positions): positions.Add((Vector3[])mesh.VetexComponents[key.Value]); break;
				}
			}

			// get triangle component types
			normalComponents = new List<TriangleNormalComponent[]>();
			uvComponents = new List<TriangleUVComponent[]>();
			foreach (var key in mesh.TriangleComponentKeys)
			{
				switch (key.Key)
				{
					case (TriangleComponentKeyTypes.NormalComponents): normalComponents.Add((TriangleNormalComponent[])mesh.TriangleComponents[key.Value]); break;
					case (TriangleComponentKeyTypes.UVComponents): uvComponents.Add((TriangleUVComponent[])mesh.TriangleComponents[key.Value]); break;
				}
			}

			// create triangles with there own verticies
			Verticies = new LinkedList<VertexProcessor>();
			Triangles = new List<TriangleProcessor>();
			foreach (var triangle in mesh.Triangles)
			{
				var newTriangle = new TriangleProcessor(triangle, this);
			}

			// process (remove duplicate verticies from triangles)
			const float tolerance = .002f;
			int count = Triangles.Count-1, count2 = Triangles.Count;
			for (int i = 0; i != count; ++i)
			{
				for (int vi = 0; vi != 3; ++vi)
				{
					var vertex = Triangles[i].Verticies[vi];
					for (int i2 = i+1; i2 != count2; ++i2)
					{
						for (int vi2 = 0; vi2 != 3; ++vi2)
						{
							var vertex2 = Triangles[i2].Verticies[vi2];
							if (vertex == vertex2) continue;

							// position tolerance
							bool pass = true;
							for (int pi = 0; pi != vertex.Positions.Length; ++pi)
							{
								if (!vertex.Positions[pi].AproxEqualsBox(vertex2.Positions[pi], tolerance))
								{
									pass = false;
									break;
								}
							}

							// normal tolerance
							if (pass)
							{
								for (int pi = 0; pi != vertex.Normals.Length; ++pi)
								{
									if (!vertex.Normals[pi].AproxEqualsBox(vertex2.Normals[pi], tolerance))
									{
										pass = false;
										break;
									}
								}
							}

							// uv tolerance
							if (pass)
							{
								for (int pi = 0; pi != vertex.UVs.Length; ++pi)
								{
									if (!vertex.UVs[pi].AproxEqualsBox(vertex2.UVs[pi], tolerance))
									{
										pass = false;
										break;
									}
								}
							}

							// remove vertex
							if (pass)
							{
								Verticies.Remove(vertex2);
								Triangles[i2].Verticies[vi2] = vertex;
							}
						}
					}
				}
			}

			// process (set vertex indicies)
			int index = 0;
			foreach (var vertex in Verticies)
			{
				vertex.Index = index;
				++index;
			}
		}
	}
}