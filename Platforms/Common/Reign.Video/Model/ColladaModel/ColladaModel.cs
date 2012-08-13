using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_ExtraTechnique
	{
		[XmlAttribute("profile")] public string Profile;
		[XmlElement("double_sided")] public int DoubleSided;
	}

	public class ColladaModel_Extra
	{
		[XmlElement("technique")] public ColladaModel_ExtraTechnique Technique;
	}

	[XmlRoot("COLLADA")]
	public class ColladaModel
	{
		[XmlAttribute("version")] public string Version;
		[XmlElement("asset")] public ColladaModel_Asset Asset;
		[XmlElement("library_images")] public ColladaModel_LibraryImage LibraryImage;
		[XmlElement("library_effects")] public ColladaModel_LibraryEffect LibraryEffect;
		[XmlElement("library_materials")] public ColladaModel_LibraryMaterial LibraryMaterial;
		[XmlElement("library_geometries")] public ColladaModel_LibraryGeometry LibraryGeometry;
		[XmlElement("library_visual_scenes")] public ColladaModel_LibraryVisualScene LibraryVisualScene;
		[XmlElement("scene")] public ColladaModel_Scene Scene;

		public void Init()
		{
			LibraryGeometry.Init();
			if (LibraryImage != null) LibraryImage.Init();
			if (LibraryEffect != null) LibraryEffect.Init();
			LibraryVisualScene.Init();
		}

		public static float[] ConvertToFloatArray(string content)
		{
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