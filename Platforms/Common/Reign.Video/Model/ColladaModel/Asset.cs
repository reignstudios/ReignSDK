using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_Contributor
	{
		[XmlElement("author")] public string Author;
		[XmlElement("authoring_tool")] public string AuthoringTool;
	}

	public class ColladaModel_Unit
	{
		[XmlAttribute("name")] public string Name;
		[XmlAttribute("meter")] public int Meter;
	}

	public class ColladaModel_Asset
	{
		[XmlElement("contributor")] public ColladaModel_Contributor Contributor;
		[XmlElement("created")] public DateTime Created;
		[XmlElement("modified")] public DateTime Modified;
		[XmlElement("unit")] public ColladaModel_Unit Unit;
		[XmlElement("up_axis")] public string UpAxis;
	}
}