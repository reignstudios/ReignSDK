using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	[XmlRoot("Scene")]
	public class RMXModel
	{
		[XmlAttribute("Version")] public float Version;
		[XmlElement("Objects")] public RMX_Objects RMXObjects;
		[XmlElement("Meshes")] public RMX_Meshes Meshes;
		[XmlElement("Materials")] public RMX_Materials Materials;

		public void Init()
		{
			if (RMXObjects != null) RMXObjects.Init();
			if (Meshes != null) Meshes.Init();
			if (Materials != null) Materials.Init();
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