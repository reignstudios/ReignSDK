using System.Collections.Generic;

namespace Reign.Video
{
	public class SoftwareVertex
	{
		public int Index;
		public List<SoftwareTriangle> Triangles;
		public List<int> TraingleIndex;
		public List<SoftwareEdge> Edges;

		public SoftwareVertex(int index)
		{
			Index = index;
			Triangles = new List<SoftwareTriangle>();
			TraingleIndex = new List<int>();
		}
	}
}