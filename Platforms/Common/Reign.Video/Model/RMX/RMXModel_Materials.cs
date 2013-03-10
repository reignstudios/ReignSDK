using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class RMX_MaterialInput
	{
		[XmlAttribute("ID")] public string ID;
		[XmlAttribute("Type")] public string Type;
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			if (Type == "Value") Values = RMXModel.ConvertToFloatArray(Content);
		}
	}

	public class RMX_Material
	{
		[XmlAttribute("Name")] public string Name;
		[XmlElement("Input")] public RMX_MaterialInput[] Inputs;

		public void Init()
		{
			if (Inputs != null)
			{
				foreach (var input in Inputs)
				{
					input.Init();
				}
			}
		}
	}

	public class RMX_Materials
	{
		[XmlElement("Material")] public RMX_Material[] Materials;

		public void Init()
		{
			if (Materials != null)
			{
				foreach (var material in Materials)
				{
					material.Init();
				}
			}
		}
	}
}