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

	public class RMX_ObjectBoneGroup
	{
		[XmlAttribute("Name")] public string Name;
		[XmlAttribute("Index")] public int Index;
	}

	public class RMX_ObjectBoneGroups
	{
		[XmlElement("BoneGroup")] public RMX_ObjectBoneGroup[] BoneGroups;
	}

	public class RMX_ObjectNameLink
	{
		[XmlAttribute("Name")] public string Name;
	}

	public class RMX_Object
	{
		[XmlAttribute("Name")] public string Name;
		[XmlAttribute("Type")] public string Type;
		[XmlAttribute("Parent")] public string Parent;
		[XmlElement("Transform")] public RMX_ObjectTransform Transform;
		[XmlElement("Mesh")] public RMX_ObjectMesh Mesh;
		[XmlElement("ArmatureObject")] public RMX_ObjectNameLink ArmatureObject;
		[XmlElement("Armature")] public RMX_ObjectNameLink Armature;
		[XmlElement("DefaultAction")] public RMX_ObjectNameLink DefaultAction;
		[XmlElement("BoneGroups")] public RMX_ObjectBoneGroups BoneGroups;

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