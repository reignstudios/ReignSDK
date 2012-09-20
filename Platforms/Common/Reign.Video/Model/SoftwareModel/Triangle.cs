using System.Collections.Generic;
using Reign.Core;

namespace Reign.Video
{
	public class TriangleNormalComponent
	{
		public Vector3[] Normals;

		public TriangleNormalComponent(Vector3 normal1, Vector3 normal2, Vector3 normal3)
		{
			Normals = new Vector3[3] {normal1, normal2, normal3};
		}
	}

	public class TriangleUVComponent
	{
		public Vector2[] UVs;

		public TriangleUVComponent(Vector2 uv1, Vector2 uv2, Vector2 uv3)
		{
			UVs = new Vector2[3] {uv1, uv2, uv3};
		}
	}

	public class TriangleColorComponent
	{
		public Vector4[] Colors;

		public TriangleColorComponent(Vector4 color1, Vector4 color2, Vector4 color3)
		{
			Colors = new Vector4[3] {color1, color2, color3};
		}
	}

	public class SoftwareTriangle
	{
		public int Index;
		public SoftwareVertex[] Verticies;
		public SoftwareEdge[] Edges;

		public SoftwareTriangle(int index, SoftwareVertex vertex1, SoftwareVertex vertex2, SoftwareVertex vertex3)
		{
			Index = index;
			Verticies = new SoftwareVertex[3] {vertex1, vertex2, vertex3};
			vertex1.Triangles.Add(this);
			vertex2.Triangles.Add(this);
			vertex3.Triangles.Add(this);
			vertex1.TraingleIndex.Add(0);
			vertex2.TraingleIndex.Add(1);
			vertex3.TraingleIndex.Add(2);
		}
	}
}