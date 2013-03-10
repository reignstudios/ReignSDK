using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class RMX_ObjectTransformInput
	{
		[XmlAttribute("Type")] public string Type;
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToFloatArray(Content);
		}
	}

	public class RMX_ObjectTransform
	{
		[XmlElement("Input")] public RMX_ObjectTransformInput[] Inputs;

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

	public class RMX_ObjectMesh
	{
		[XmlAttribute("Name")] public string Name;
	}

	public class RMX_Object
	{
		[XmlAttribute("Name")] public string Name;
		[XmlAttribute("Type")] public string Type;
		[XmlElement("Transform")] public RMX_ObjectTransform Transform;
		[XmlElement("Mesh")] public RMX_ObjectMesh Mesh;

		public void Init()
		{
			if (Transform != null) Transform.Init();
		}
	}

	public class RMX_Objects
	{
		[XmlElement("Object")] public RMX_Object[] Objects;

		public void Init()
		{
			if (Objects != null)
			{
				foreach (var o in Objects)
				{
					o.Init();
				}
			}
		}
	}
}