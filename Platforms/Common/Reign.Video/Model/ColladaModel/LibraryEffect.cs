using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_Surface
	{
		[XmlAttribute("type")] public string Type;
		[XmlElement("init_from")] public string InitFrom;
	}

	public class ColladaModel_Sampler2D
	{
		[XmlElement("source")] public string Source;
	}

	public class ColladaModel_NewParam
	{
		[XmlAttribute("sid")] public string SID;
		[XmlElement("surface")] public ColladaModel_Surface Surface;
		[XmlElement("sampler2D")] public ColladaModel_Sampler2D Sampler2D;
	}

	public class ColladaModel_Color
	{
		[XmlAttribute("sid")] public string SID;
		[XmlText] public string Content;
		public float[] Colors;

		public void Init()
		{
			var colors = Content.Split(' ');
			Colors = new float[colors.Length];
			for (int i = 0; i != colors.Length; ++i)
			{
				Colors[i] = float.Parse(colors[i]);
			}
		}
	}

	public class ColladaModel_Float
	{
		[XmlAttribute("sid")] public string SID;
		[XmlText] public string Content;
		public float Value;

		public void Init()
		{
			Value = float.Parse(Content);
		}
	}

	public class ColladaModel_Texture
	{
		[XmlAttribute("texture")] public string Texture;
		[XmlAttribute("texcoord")] public string TexCoord;
	}

	public class ColladaModel_Emission
	{
		[XmlElement("color")] public ColladaModel_Color Color;
		[XmlElement("texture")] public ColladaModel_Texture[] Textures;

		public void Init()
		{
			if (Color != null) Color.Init();
		}
	}

	public class ColladaModel_Ambient
	{
		[XmlElement("color")] public ColladaModel_Color Color;
		[XmlElement("texture")] public ColladaModel_Texture[] Textures;

		public void Init()
		{
			if (Color != null) Color.Init();
		}
	}

	public class ColladaModel_Diffuse
	{
		[XmlElement("color")] public ColladaModel_Color Color;
		[XmlElement("texture")] public ColladaModel_Texture[] Textures;

		public void Init()
		{
			if (Color != null) Color.Init();
		}
	}

	public class ColladaModel_Specular
	{
		[XmlElement("color")] public ColladaModel_Color Color;
		[XmlElement("texture")] public ColladaModel_Texture[] Textures;

		public void Init()
		{
			if (Color != null) Color.Init();
		}
	}

	public class ColladaModel_Shininess
	{
		[XmlElement("float")] public ColladaModel_Float Float;

		public void Init()
		{
			Float.Init();
		}
	}

	public class ColladaModel_IndexOfRefraction
	{
		[XmlElement("float")] public ColladaModel_Float Float;

		public void Init()
		{
			Float.Init();
		}
	}

	public class ColladaModel_Phong
	{
		[XmlElement("emission")] public ColladaModel_Emission Emission;
		[XmlElement("ambient")] public ColladaModel_Ambient Ambient;
		[XmlElement("diffuse")] public ColladaModel_Diffuse Diffuse;
		[XmlElement("specular")] public ColladaModel_Specular Specular;
		[XmlElement("shininess")] public ColladaModel_Shininess Shininess;
		[XmlElement("index_of_refraction")] public ColladaModel_IndexOfRefraction IndexOfRefraction;

		public void Init()
		{
			Emission.Init();
			Ambient.Init();
			Diffuse.Init();
			Specular.Init();
			Shininess.Init();
			IndexOfRefraction.Init();
		}
	}

	public class ColladaModel_Technique
	{
		[XmlAttribute("sid")] public string SID;
		[XmlElement("phong")] public ColladaModel_Phong Phong;

		public void Init()
		{
			Phong.Init();
		}
	}

	public class ColladaModel_ProfileCommon
	{
		[XmlElement("newparam")] public ColladaModel_NewParam[] NewParams;
		[XmlElement("technique")] public ColladaModel_Technique Technique;
		[XmlElement("extra")] public ColladaModel_Extra Extra;

		public void Init()
		{
			Technique.Init();
		}

		public ColladaModel_NewParam FindNewParam(string sid)
		{
			foreach (var newParam in NewParams)
			{
				if (newParam.SID == sid) return newParam;
			}

			return null;
		}
	}

	public class ColladaModel_Effect
	{
		[XmlAttribute("id")] public string ID;
		[XmlElement("profile_COMMON")] public ColladaModel_ProfileCommon ProfileCommon;
		[XmlElement("extra")] public ColladaModel_Extra Extra;

		public void Init()
		{
			ProfileCommon.Init();
		}
	}

	public class ColladaModel_LibraryEffect
	{
		[XmlElement("effect")] public ColladaModel_Effect[] Effects;

		public void Init()
		{
			foreach (var effect in Effects)
			{
				effect.Init();
			}
		}

		public ColladaModel_Effect FindEffect(string id)
		{
			id = id.Replace("#", "");
			foreach (var effect in Effects)
			{
				if (effect.ID == id) return effect;
			}

			return null;
		}
	}
}