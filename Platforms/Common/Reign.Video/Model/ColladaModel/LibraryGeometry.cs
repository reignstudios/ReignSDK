using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_FloatArray
	{
		[XmlAttribute("id")] public string ID;
		[XmlAttribute("count")] public int Count;
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			Values = ColladaModel.ConvertToFloatArray(Content);
		}
	}

	public class ColladaModel_Param
	{
		[XmlAttribute("name")] public string Name;
		[XmlAttribute("type")] public string Type;
	}

	public class ColladaModel_Accessor
	{
		[XmlAttribute("source")] public string Source;
		[XmlAttribute("count")] public int Count;
		[XmlAttribute("stride")] public int Stride;
		[XmlElement("param")] public ColladaModel_Param[] Params;
	}

	public class ColladaModel_TechniqueCommon
	{
		[XmlElement("accessor")] public ColladaModel_Accessor Accessor;
	}

	public class ColladaModel_Source
	{
		[XmlAttribute("id")] public string ID;
		[XmlElement("float_array")] public ColladaModel_FloatArray FloatArray;
		[XmlElement("technique_common")] public ColladaModel_TechniqueCommon TechniqueCommon;

		public void Init()
		{
			FloatArray.Init();
		}
	}

	public class ColladaModel_VertexInput
	{
		[XmlAttribute("semantic")] public string Semantic;
		[XmlAttribute("source")] public string Source;
	}

	public class ColladaModel_Vertex
	{
		[XmlAttribute("id")] public string ID;
		[XmlElement("input")] public ColladaModel_VertexInput[] Inputs;
	}

	public class ColladaModel_PolyInput
	{
		[XmlAttribute("semantic")] public string Semantic;
		[XmlAttribute("source")] public string Source;
		[XmlAttribute("offset")] public int Offset;
	}

	public class ColladaModel_Polylist
	{
		[XmlAttribute("count")] public int Count;
		[XmlElement("input")] public ColladaModel_PolyInput[] Inputs;
		[XmlElement("vcount")] public string VCounts;
		[XmlElement("p")] public string P;
		public int[] VertexCounts, IndexData;

		public void Init()
		{
			VertexCounts = ColladaModel.ConvertToIntArray(VCounts);
			IndexData = ColladaModel.ConvertToIntArray(P);
		}

		public ColladaModel_PolyInput FindInput(string semantic)
		{
			foreach (var input in Inputs)
			{
				if (input.Semantic == semantic) return input;
			}

			return null;
		}
	}

	public class ColladaModel_Mesh
	{
		[XmlElement("source")] public ColladaModel_Source[] Sources;
		[XmlElement("vertices")] public ColladaModel_Vertex Vertices;
		[XmlElement("polylist")] public ColladaModel_Polylist Polylist;

		public void Init()
		{
			foreach (var source in Sources)
			{
				source.Init();
			}

			Polylist.Init();
		}

		public ColladaModel_Source FindSource(string id)
		{
			id = id.Replace("#", "");
			foreach (var source in Sources)
			{
				if (source.ID == id) return source;
			}

			return null;
		}
	}

	public class ColladaModel_Geometry
	{
		[XmlAttribute("id")] public string ID;
		[XmlAttribute("name")] public string Name;
		[XmlElement("mesh")] public ColladaModel_Mesh Mesh;
		[XmlElement("extra")] public ColladaModel_Extra Extra;

		public void Init()
		{
			Mesh.Init();
		}
	}

	public class ColladaModel_LibraryGeometry
	{
		[XmlElement("geometry")] public ColladaModel_Geometry[] Geometies;

		public void Init()
		{
			foreach (var geometry in Geometies)
			{
				geometry.Init();
			}
		}
	}
}