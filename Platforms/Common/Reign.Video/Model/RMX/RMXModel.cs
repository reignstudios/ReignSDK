using System.Xml.Serialization;
using System;

namespace Reign.Video
{
	public class RMX_MeshVertexChannel
	{
		[XmlAttribute("ID")] public string ID;
		[XmlAttribute("Index")] public int Index;
		[XmlAttribute("Step")] public int Step;
		[XmlText] public string Content;
		public float[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToFloatArray(Content);
		}
	}

	public class RMX_MeshVerticies
	{
		[XmlElement("Channel")] public RMX_MeshVertexChannel[] Channels;

		public void Init()
		{
			foreach (var channel in Channels)
			{
				channel.Init();
			}
		}
	}

	public class RMX_MeshFaceIndices
	{
		[XmlAttribute("ID")] public string ID;
		[XmlText] public string Content;
		public int[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToIntArray(Content);
		}
	}

	public class RMX_MeshFaceSteps
	{
		[XmlText] public string Content;
		public int[] Values;

		public void Init()
		{
			Values = RMXModel.ConvertToIntArray(Content);
		}
	}

	public class RMX_MeshFaces
	{
		[XmlElement("Steps")] public RMX_MeshFaceSteps Steps;
		[XmlElement("Indices")] public RMX_MeshFaceIndices[] Indices;

		public void Init()
		{
			Steps.Init();

			if (Indices != null)
			{
				foreach (var i in Indices)
				{
					i.Init();
				}
			}
		}
	}

	public class RMX_Mesh
	{
		[XmlAttribute("Material")] public string Material;
		[XmlElement("Vertices")] public RMX_MeshVerticies Verticies;
		[XmlElement("Faces")] public RMX_MeshFaces Faces;
		public RMX_Object RMXObject;

		public void Init(RMX_Object o)
		{
			RMXObject = o;
			if (Verticies != null) Verticies.Init();
			if (Faces != null) Faces.Init();
		}
	}

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

	public class RMX_Object
	{
		[XmlAttribute("Name")] public string Name;
		[XmlElement("Transform")] public RMX_ObjectTransform Transform;
		[XmlElement("Mesh")] public RMX_Mesh Mesh;

		public void Init()
		{
			if (Transform != null) Transform.Init();
			if (Mesh != null) Mesh.Init(this);
		}
	}

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

	[XmlRoot("Scene")]
	public class RMXModel
	{
		[XmlAttribute("Version")] public float Version;
		[XmlElement("Materials")] public RMX_Materials Materials;
		[XmlElement("Object")] public RMX_Object[] RMXObjects;

		public void Init()
		{
			if (Materials != null) Materials.Init();
			if (RMXObjects != null)
			{
				foreach (var o in RMXObjects)
				{
					o.Init();
				}
			}
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