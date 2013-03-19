using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	[XmlRoot("Scene")]
	public class RMXModel
	{
		[XmlAttribute("Version")] public float Version;
		[XmlElement("Materials")] public RMX_Materials Materials;
		[XmlElement("Meshes")] public RMX_Meshes Meshes;
		[XmlElement("Actions")] public RMX_Actions Actions;
		[XmlElement("Armatures")] public RMX_Materials Armatures;
		[XmlElement("Objects")] public RMX_Objects RMXObjects;

		public void Init()
		{
			if (Materials != null) Materials.Init();
			if (Meshes != null) Meshes.Init();
			if (Actions != null) Actions.Init();
			if (Armatures != null) Armatures.Init();
			if (RMXObjects != null) RMXObjects.Init();
		}

		public static float[] ConvertToFloatArray(string content)
		{
			if (string.IsNullOrEmpty(content)) return null;

			var charData = content.Trim().Split(' ');
			var data = new float[charData.Length];
			for (int i = 0; i != charData.Length; ++i)
			{
				data[i] = float.Parse(charData[i]);
			}

			return data;
		}

		public static int[] ConvertToIntArray(string content)
		{
			if (string.IsNullOrEmpty(content)) return null;

			var charData = content.Trim().Split(' ');
			var data = new int[charData.Length];
			for (int i = 0; i != charData.Length; ++i)
			{
				data[i] = int.Parse(charData[i]);
			}

			return data;
		}
	}
}