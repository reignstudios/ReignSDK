using System.Collections.Generic;

namespace Reign.Video.Abstraction
{
	public class SoftwareEdge
	{
		public int Index;
		public SoftwareVertex[] Verticies;
		public List<SoftwareTriangle> Triangles;

		public SoftwareEdge(int index, SoftwareVertex vertex1, SoftwareVertex vertex2)
		{
			Index = index;
			Verticies = new SoftwareVertex[2] {vertex1, vertex2};
			Triangles = new List<SoftwareTriangle>();
		}
	}
}