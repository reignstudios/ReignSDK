using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class ColladaModel_NodeElement
	{
		[XmlAttribute("sid")] public string SID;
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			var values = Content.Split(' ');
			Values = new float[values.Length];
			for (int i = 0; i != values.Length; ++i)
			{
				Values[i] = float.Parse(values[i]);
			}
		}
	}

	public class ColladaModel_BindVertexInput
	{
		[XmlAttribute("semantic")] public string Semantic;
		[XmlAttribute("input_semantic")] public string InputSemantic;
		[XmlAttribute("input_set")] public int InputSet;
	}

	public class ColladaModel_InstanceMatrial
	{
		[XmlAttribute("symbol")] public string Symbol;
		[XmlAttribute("target")] public string Target;
		[XmlElement("bind_vertex_input")] public ColladaModel_BindVertexInput BindVertexInput;
	}

	public class ColladaModel_BindMaterial_TechniqueCommon
	{
		[XmlElement("instance_material")] public ColladaModel_InstanceMatrial InstanceMatrial;
	}

	public class ColladaModel_BindMaterial
	{
		[XmlElement("technique_common")] public ColladaModel_BindMaterial_TechniqueCommon TechniqueCommon;
	}

	public class ColladaModel_InstanceGeometry
	{
		[XmlAttribute("url")] public string URL;
		[XmlElement("bind_material")] public ColladaModel_BindMaterial BindMaterial;
	}

	public class ColladaModel_Node
	{
		[XmlAttribute("id")] public string ID;
		[XmlAttribute("type")] public string Type;
		[XmlElement("translate")] public ColladaModel_NodeElement Translate;
		[XmlElement("rotate")] public ColladaModel_NodeElement[] Rotations;
		[XmlElement("scale")] public ColladaModel_NodeElement Scale;
		[XmlElement("instance_geometry")] public ColladaModel_InstanceGeometry InstanceGeometry;

		public void Init()
		{
			Translate.Init();
			Scale.Init();

			foreach (var rotation in Rotations)
			{
				rotation.Init();
			}
		}
	}

	public class ColladaModel_VisualScene
	{
		[XmlAttribute("id")] public string ID;
		[XmlAttribute("name")] public string Name;
		[XmlElement("node")] public ColladaModel_Node[] Nodes;

		public void Init()
		{
			foreach (var node in Nodes)
			{
				node.Init();
			}
		}
	}

	public class ColladaModel_LibraryVisualScene
	{
		[XmlElement("visual_scene")] public ColladaModel_VisualScene[] VisualScenes;

		public void Init()
		{
			foreach (var visualScene in VisualScenes)
			{
				visualScene.Init();
			}
		}

		public ColladaModel_VisualScene FindVisualScene(string id)
		{
			id = id.Replace("#", "");
			foreach (var visualScene in VisualScenes)
			{
				if (visualScene.ID == id) return visualScene;
			}

			return null;
		}
	}
}