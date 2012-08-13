using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_Image
	{
		[XmlAttribute("id")] public string ID;
		[XmlAttribute("name")] public string Name;
		[XmlElement("init_from")] public string InitFrom;

		public void Init()
		{
			if (!string.IsNullOrEmpty(InitFrom) && InitFrom.Length != 0 && InitFrom[0] == '/') InitFrom = InitFrom.Substring(1, InitFrom.Length-1);
		}
	}

	public class ColladaModel_LibraryImage
	{
		[XmlElement("image")] public ColladaModel_Image[] Images;

		public void Init()
		{
			if (Images == null) return;
			foreach (var image in Images)
			{
				image.Init();
			}
		}

		public ColladaModel_Image FindImage(string id)
		{
			foreach (var image in Images)
			{
				if (image.ID == id) return image;
			}

			return null;
		}
	}
}