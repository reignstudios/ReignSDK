using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_InstanceEffect
	{
		[XmlAttribute("url")] public string URL;
	}

	public class ColladaModel_Material
	{
		[XmlAttribute("id")] public string ID;
		[XmlAttribute("name")] public string Name;
		[XmlElement("instance_effect")] public ColladaModel_InstanceEffect InstanceEffect;
	}

	public class ColladaModel_LibraryMaterial
	{
		[XmlElement("material")] public ColladaModel_Material[] Materials;
	}
}