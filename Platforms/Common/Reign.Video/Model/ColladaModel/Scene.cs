using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_InstanceVisualScene
	{
		[XmlAttribute("url")] public string URL;
	}

	public class ColladaModel_Scene
	{
		[XmlElement("instance_visual_scene")] public ColladaModel_InstanceVisualScene InstanceVisualScene;
	}
}