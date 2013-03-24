using System.Collections.Generic;

namespace Reign.Video
{
	public class SoftwareVertexBoneGroup
	{
		public int Index;
		public float Weight;

		public SoftwareVertexBoneGroup(int index, float weight)
		{
			Index = index;
			Weight = weight;
		}
	}

	public class SoftwareVertex
	{
		public int Index;
		public List<SoftwareTriangle> Triangles;
		public List<int> TraingleIndex;
		public List<SoftwareEdge> Edges;
		public List<SoftwareVertexBoneGroup> BoneGroups;

		public SoftwareVertex(int index)
		{
			Index = index;
			Triangles = new List<SoftwareTriangle>();
			TraingleIndex = new List<int>();
			Edges = new List<SoftwareEdge>();
			BoneGroups = new List<SoftwareVertexBoneGroup>();
		}
	}
}