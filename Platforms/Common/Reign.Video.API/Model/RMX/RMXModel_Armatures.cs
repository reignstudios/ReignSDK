using System.Xml.Serialization;
using System;

namespace Reign.Video.Abstraction
{
	public class RMX_ArmatureBoneValues
	{
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToFloatArray(Content);
		}
	}

	public class RMX_ArmatureBone
	{
		[XmlAttribute("Name")] public string Name;
		[XmlAttribute("Parent")] public string Parent;

		[XmlElement("InheritScale")] public string InheritScaleContent;
		public bool InheritScale {get{return System.Xml.XmlConvert.ToBoolean(InheritScaleContent.ToLower());}}

		[XmlElement("InheritRotation")] public string InheritRotationContent;
		public bool InheritRotation {get{return System.Xml.XmlConvert.ToBoolean(InheritRotationContent.ToLower());}}

		[XmlElement("Position")] public RMX_ArmatureBoneValues Position;
		[XmlElement("Rotation")] public RMX_ArmatureBoneValues Rotation;

		public void Init()
		{
			Position.Init();
			Rotation.Init();
		}
	}

	public class RMX_ArmatureBones
	{
		[XmlElement("Bone")] public RMX_ArmatureBone[] Bones;

		public void Init()
		{
			if (Bones != null)
			{
				foreach (var bone in Bones)
				{
					bone.Init();
				}
			}
		}
	}

	public class RMX_Armature
	{
		[XmlAttribute("Name")] public string Name;
		[XmlElement("Bones")] public RMX_ArmatureBones Bones;

		public void Init()
		{
			Bones.Init();
		}
	}

	public class RMX_Armatures
	{
		[XmlElement("Armature")] public RMX_Armature[] Armatures;

		public void Init()
		{
			if (Armatures != null)
			{
				foreach (var armature in Armatures)
				{
					armature.Init();
				}
			}
		}
	}
}